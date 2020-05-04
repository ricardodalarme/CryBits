using System;

class NPC
{
    public enum Behaviour
    {
        Friendly,
        AttackOnSight,
        AttackWhenAttacked,
        ShopKeeper
    }

    public enum Movements
    {
        MoveRandomly,
        TurnRandomly,
        StandStill
    }

    public class Structure
    {
        // Dados básicos
        public byte Index;
        public short Map_Num;
        public short Data_Index;
        public bool Alive;
        public byte X;
        public byte Y;
        public Game.Directions Direction;
        public byte Target_Type;
        public byte Target_Index;
        public short[] Vital;
        public int Spawn_Timer;
        public int Attack_Timer;

        private short Regeneration(byte Vital)
        {
            Lists.Structures.NPC Data = Lists.NPC[Data_Index];

            // Cálcula o máximo de vital que o NPC possui
            switch ((Game.Vitals)Vital)
            {
                case Game.Vitals.HP: return (short)(Data.Vital[Vital] * 0.05 + Data.Attribute[(byte)Game.Attributes.Vitality] * 0.3);
                case Game.Vitals.MP: return (short)(Data.Vital[Vital] * 0.05 + Data.Attribute[(byte)Game.Attributes.Intelligence] * 0.1);
            }

            return 0;
        }

        /////////////
        // Funções //
        /////////////
        public void Logic()
        {
            Lists.Structures.NPC NPC_Data = Lists.NPC[Data_Index];

            ////////////////
            // Surgimento //
            ////////////////
            if (!Alive)
            {
                if (Environment.TickCount > Spawn_Timer + (NPC_Data.SpawnTime * 1000)) Spawn(Index, Map_Num);
                return;
            }
            else
            {
                byte TargetX = 0, TargetY = 0;
                bool[] CanMove = new bool[(byte)Game.Directions.Count];
                short Distance;
                bool Moved = false;
                bool Move = false;

                /////////////////
                // Regeneração //
                /////////////////
                if (Environment.TickCount > Loop.Timer_NPC_Regen + 5000)
                    for (byte v = 0; v < (byte)Game.Vitals.Count; v++)
                        if (Vital[v] < NPC_Data.Vital[v])
                        {
                            // Renera os vitais
                            Vital[v] += Regeneration(v);

                            // Impede que o valor passe do limite
                            if (Vital[v] > NPC_Data.Vital[v]) Vital[v] = NPC_Data.Vital[v];

                            // Envia os dados aos jogadores do mapa
                            Send.Map_NPC_Vitals(Map_Num, Index);
                        }

                //////////////////
                // Movimentação //
                //////////////////
                // Atacar ao ver
                if (NPC_Data.Behaviour == (byte)Behaviour.AttackOnSight)
                {
                    // Jogador
                    if (Target_Index == 0)
                        for (byte Player_Index = 1; Player_Index <= Game.HigherIndex; Player_Index++)
                        {
                            // Verifica se o jogador está jogando e no mesmo mapa que o NPC
                            if (!Account.IsPlaying(Player_Index)) continue;
                            if (Account.Character(Player_Index).Map_Num != Map_Num) continue;

                            // Se o jogador estiver no alcance do NPC, ir atrás dele
                            Distance = (short)Math.Sqrt(Math.Pow(X - Account.Character(Player_Index).X, 2) + Math.Pow(Y - Account.Character(Player_Index).Y, 2));
                            if (Distance <= NPC_Data.Sight)
                            {
                                Target_Type = (byte)Game.Target.Player;
                                Target_Index = Player_Index;

                                // Mensagem
                                if (!string.IsNullOrEmpty(NPC_Data.SayMsg)) Send.Message(Account.Character(Player_Index), NPC_Data.Name + ": " + NPC_Data.SayMsg, System.Drawing.Color.White);
                                break;
                            }
                        }

                    // NPC
                    if (NPC_Data.AttackNPC && Target_Index == 0)
                        for (byte NPC_Index = 1; NPC_Index < Lists.Temp_Map[Map_Num].NPC.Length; NPC_Index++)
                        {
                            // Verifica se pode atacar
                            if (NPC_Index == Index) continue;
                            if (!Lists.Temp_Map[Map_Num].NPC[NPC_Index].Alive) continue;
                            if (IsAlied(Data_Index, Lists.Temp_Map[Map_Num].NPC[NPC_Index].Data_Index)) continue;

                            // Se o NPC estiver no alcance do NPC, ir atrás dele
                            Distance = (short)Math.Sqrt(Math.Pow(X - Lists.Temp_Map[Map_Num].NPC[NPC_Index].X, 2) + Math.Pow(Y - Lists.Temp_Map[Map_Num].NPC[NPC_Index].Y, 2));
                            if (Distance <= NPC_Data.Sight)
                            {
                                Target_Type = (byte)Game.Target.NPC;
                                Target_Index = NPC_Index;
                                break;
                            }
                        }
                }

                if (Target_Type == (byte)Game.Target.Player)
                {
                    // Verifica se o jogador ainda está disponível
                    if (!Account.IsPlaying(Target_Index) || Account.Character(Target_Index).Map_Num != Map_Num)
                    {
                        Target_Type = 0;
                        Target_Index = 0;
                    }
                    // Posição do alvo
                    else
                    {
                        TargetX = Account.Character(Target_Index).X;
                        TargetY = Account.Character(Target_Index).Y;
                    }
                }
                else if (Target_Type == (byte)Game.Target.NPC)
                {
                    // Verifica se o NPC ainda está disponível
                    if (!Lists.Temp_Map[Map_Num].NPC[Target_Index].Alive )
                    {
                        Target_Type = 0;
                        Target_Index = 0;
                    }
                    // Posição do alvo
                    else
                    {
                        TargetX = Lists.Temp_Map[Map_Num].NPC[Target_Index].X;
                        TargetY = Lists.Temp_Map[Map_Num].NPC[Target_Index].Y;
                    }
                }

                // Verifica se o alvo saiu do alcance do NPC
                Distance = (short)Math.Sqrt(Math.Pow(X - TargetX, 2) + Math.Pow(Y - TargetY, 2));
                if (Distance > NPC_Data.Sight)
                {
                    Target_Type = 0;
                    Target_Index = 0;
                }

                // Evita que ele se movimente sem sentido
                if (Target_Index > 0)
                    Move = true;
                else
                {
                    // Define o alvo a zona do NPC
                    if (Lists.Map[Map_Num].NPC[Index].Zone > 0)
                        if (Lists.Map[Map_Num].Tile[X, Y].Zone != Lists.Map[Map_Num].NPC[Index].Zone)
                            for (byte x2 = 0; x2 <= Lists.Map[Map_Num].Width; x2++)
                                for (byte y2 = 0; y2 <= Lists.Map[Map_Num].Height; y2++)
                                    if (Lists.Map[Map_Num].Tile[x2, y2].Zone == Lists.Map[Map_Num].NPC[Index].Zone)
                                        if (!Map.Tile_Blocked(Map_Num, x2, y2))
                                        {
                                            TargetX = x2;
                                            TargetY = y2;
                                            Move = true;
                                            break;
                                        }
                }

                // Movimenta o NPC
                if (Move)
                {
                    // Verifica como o NPC pode se mover
                    if (Vital[(byte)Game.Vitals.HP] > NPC_Data.Vital[(byte)Game.Vitals.HP] * (NPC_Data.Flee_Helth / 100.0))
                    {
                        // Para perto do alvo
                        CanMove[(byte)Game.Directions.Up] = Y > TargetY;
                        CanMove[(byte)Game.Directions.Down] = Y < TargetY;
                        CanMove[(byte)Game.Directions.Left] = X > TargetX;
                        CanMove[(byte)Game.Directions.Right] = X < TargetX;
                    }
                    else
                    {
                        // Para longe do alvo
                        CanMove[(byte)Game.Directions.Up] = Y < TargetY;
                        CanMove[(byte)Game.Directions.Down] = Y > TargetY;
                        CanMove[(byte)Game.Directions.Left] = X < TargetX;
                        CanMove[(byte)Game.Directions.Right] = X > TargetX;
                    }

                    // Aleatoriza a forma que ele vai se movimentar até o alvo
                    if (Game.Random.Next(0, 2) == 0)
                    {
                        for (byte d = 0; d < (byte)Game.Directions.Count; d++)
                            if (!Moved && CanMove[d] && this.Move((Game.Directions)d))
                                Moved = true;
                    }
                    else
                        for (short d = (byte)Game.Directions.Count - 1; d >= 0; d--)
                            if (!Moved && CanMove[d] && this.Move((Game.Directions)d))
                                Moved = true;
                }

                // Move-se aleatoriamente
                if (NPC_Data.Behaviour == (byte)Behaviour.Friendly || Target_Index == 0)
                    if (Game.Random.Next(0, 3) == 0 && !Moved)
                        if (NPC_Data.Movement == Movements.MoveRandomly)
                            this.Move((Game.Directions)Game.Random.Next(0, 4), 1, true);
                        else if (NPC_Data.Movement == Movements.TurnRandomly)
                        {
                            Direction = (Game.Directions)Game.Random.Next(0, 4);
                            Send.Map_NPC_Direction(Map_Num, Index);
                        }

                ////////////
                // Ataque //
                ////////////
                short Next_X = X, Next_Y = Y;
                Map.NextTile(Direction, ref Next_X, ref Next_Y);
                if (Target_Type == (byte)Game.Target.Player)
                {
                    // Verifica se o jogador está na frente do NPC
                    if (Map.HasPlayer(Map_Num, Next_X, Next_Y) == Target_Index) Attack_Player(Account.Character(Target_Index));
                }
                else if (Target_Type == (byte)Game.Target.NPC)
                {
                    // Verifica se o NPC alvo está na frente do NPC
                    if (Map.HasNPC(Map_Num, Next_X, Next_Y) == Target_Index) Attack_NPC(Lists.Temp_Map[Map_Num].NPC[Target_Index]);
                }
            }
        }

        private bool Move(Game.Directions Direction, byte Movement = 1, bool CheckZone = false)
        {
            short Next_X = X, Next_Y = Y;

            // Define a direção do NPC
            this.Direction = Direction;
            Send.Map_NPC_Direction(Map_Num, Index);

            // Próximo azulejo
            Map.NextTile(Direction, ref Next_X, ref Next_Y);

            // Próximo azulejo bloqueado ou fora do limite
            if (Map.OutLimit(Map_Num, Next_X, Next_Y)) return false;
            if (Map.Tile_Blocked(Map_Num, X, Y, Direction)) return false;

            // Verifica se está dentro da zona
            if (CheckZone)
                if (Lists.Map[Map_Num].Tile[Next_X, Next_Y].Zone != Lists.Map[Map_Num].NPC[Index].Zone)
                    return false;

            // Movimenta o NPC
            X = (byte)Next_X;
            Y = (byte)Next_Y;
            Send.Map_NPC_Movement(Map_Num, Index, Movement);
            return true;
        }

        private void Attack_Player(Player Victim)
        {
            short Next_X = X, Next_Y = Y;

            // Define o azujelo a frente do NPC
            Map.NextTile(Direction, ref Next_X, ref Next_Y);

            // Verifica se a vítima pode ser atacada
            if (!Alive) return;
            if (Environment.TickCount < Attack_Timer + 750) return;
            if (!Account.IsPlaying(Victim.Index)) return;
            if (Victim.GettingMap) return;
            if (Map_Num != Victim.Map_Num) return;
            if (Victim.X != Next_X || Victim.Y != Next_Y) return;
            if (Map.Tile_Blocked(Map_Num, X, Y, Direction, false)) return;

            // Tempo de ataque 
            Attack_Timer = Environment.TickCount;

            // Cálculo de dano
            short Attack_Damage = (short)(Lists.NPC[Data_Index].Attribute[(byte)Game.Attributes.Strength] - Victim.Player_Defense);

            // Dano não fatal
            if (Attack_Damage > 0)
            {
                // Demonstra o ataque aos outros jogadores
                Send.Map_NPC_Attack(Map_Num, Index, Victim.Index, (byte)Game.Target.Player);

                if (Attack_Damage < Victim.Vital[(byte)Game.Vitals.HP])
                {
                    Victim.Vital[(byte)Game.Vitals.HP] -= Attack_Damage;
                    Send.Player_Vitals(Victim);
                }
                // FATALITY
                else
                {
                    // Reseta o alvo do NPC
                    Target_Type = 0;
                    Target_Index = 0;

                    // Mata o jogador
                    Victim.Died();
                }
            }
            // Demonstra o ataque aos outros jogadores
            else
                Send.Map_NPC_Attack(Map_Num, Index);
        }

        private void Attack_NPC(Structure Victim)
        {
            short Next_X = X, Next_Y = Y;

            // Define o azujelo a frente do NPC
            Map.NextTile(Direction, ref Next_X, ref Next_Y);

            // Verifica se a vítima pode ser atacada
            if (!Alive) return;
            if (!Victim.Alive) return;
            if (Environment.TickCount < Attack_Timer + 750) return;
            if (Victim.X != Next_X || Victim.Y != Next_Y) return;
            if (Map.Tile_Blocked(Map_Num, X, Y, Direction, false)) return;

            // Tempo de ataque 
            Attack_Timer = Environment.TickCount;

            // Define o alvo do NPC
            Victim.Target_Index = Index;
            Victim.Target_Type = (byte)Game.Target.NPC;

            // Cálculo de dano
            short Attack_Damage = (short)(Lists.NPC[Data_Index].Attribute[(byte)Game.Attributes.Strength] - Lists.NPC[Victim.Data_Index].Attribute[(byte)Game.Attributes.Resistance]);

            // Dano não fatal
            if (Attack_Damage > 0)
            {
                // Demonstra o ataque aos outros jogadores
                Send.Map_NPC_Attack(Map_Num, Index, Victim.Index, (byte)Game.Target.NPC);

                if (Attack_Damage < Victim.Vital[(byte)Game.Vitals.HP])
                {
                    Victim.Vital[(byte)Game.Vitals.HP] -= Attack_Damage;
                    Send.Map_NPC_Vitals(Map_Num, Victim.Index);
                }
                // FATALITY
                else
                {
                    // Reseta o alvo do NPC
                    Target_Type = 0;
                    Target_Index = 0;

                    // Mata o NPC
                    Victim.Died();
                }
            }
            // Demonstra o ataque aos outros jogadores
            else
                Send.Map_NPC_Attack(Map_Num, Index);
        }

        public void Died()
        {
            Lists.Structures.NPC NPC = Lists.NPC[Data_Index];

            // Solta os itens
            for (byte i = 0; i < NPC.Drop.Length; i++)
                if (NPC.Drop[i].Item_Num > 0)
                    if (Game.Random.Next(1, 99) <= NPC.Drop[i].Chance)
                    {
                        // Dados do item
                        Lists.Structures.Map_Items Item = new Lists.Structures.Map_Items();
                        Item.Index = NPC.Drop[i].Item_Num;
                        Item.Amount = NPC.Drop[i].Amount;
                        Item.X = X;
                        Item.Y = Y;

                        // Solta o item
                        Lists.Temp_Map[Map_Num].Item.Add(Item);
                    }

            // Envia os dados dos itens no chão para o mapa
            Send.Map_Items(Map_Num);

            // Reseta os dados do NPC 
            Spawn_Timer = Environment.TickCount;
            Target_Type = 0;
            Target_Index = 0;
            Alive = false;
            Send.Map_NPC_Died(Map_Num, Index);
        }
    }

    public static void Spawn(byte Index, short Map_Num)
    {
        byte x, y;

        // Antes verifica se tem algum local de aparecimento específico
        if (Lists.Map[Map_Num].NPC[Index].Spawn)
        {
            Spawn(Index, Map_Num, Lists.Map[Map_Num].NPC[Index].X, Lists.Map[Map_Num].NPC[Index].Y);
            return;
        }

        // Faz com que ele apareça em um local aleatório
        for (byte i = 0; i <= 50; i++) // tenta 50 vezes com que ele apareça em um local aleatório
        {
            x = (byte)Game.Random.Next(0, Lists.Map[Map_Num].Width);
            y = (byte)Game.Random.Next(0, Lists.Map[Map_Num].Height);

            // Verifica se está dentro da zona
            if (Lists.Map[Map_Num].NPC[Index].Zone > 0)
                if (Lists.Map[Map_Num].Tile[x, y].Zone != Lists.Map[Map_Num].NPC[Index].Zone)
                    continue;

            // Define os dados
            if (!Map.Tile_Blocked(Map_Num, x, y))
            {
                Spawn(Index, Map_Num, x, y);
                return;
            }
        }

        // Em último caso, tentar no primeiro lugar possível
        for (byte x2 = 0; x2 <= Lists.Map[Map_Num].Width; x2++)
            for (byte y2 = 0; y2 <= Lists.Map[Map_Num].Height; y2++)
                if (!Map.Tile_Blocked(Map_Num, x2, y2))
                {
                    // Verifica se está dentro da zona
                    if (Lists.Map[Map_Num].NPC[Index].Zone > 0)
                        if (Lists.Map[Map_Num].Tile[x2, y2].Zone != Lists.Map[Map_Num].NPC[Index].Zone)
                            continue;

                    // Define os dados
                    Spawn(Index, Map_Num, x2, y2);
                    return;
                }
    }

    private static void Spawn(byte Index, short Map_Num, byte x, byte y, Game.Directions Direction = 0)
    {
        short NPC_Index = Lists.Map[Map_Num].NPC[Index].Index;

        // Previne erros
        if (NPC_Index >= Lists.NPC.Length) return;

        // Define os dados
        Lists.Temp_Map[Map_Num].NPC[Index] = new Structure();
        Lists.Temp_Map[Map_Num].NPC[Index].Index = Index;
        Lists.Temp_Map[Map_Num].NPC[Index].Map_Num = Map_Num;
        Lists.Temp_Map[Map_Num].NPC[Index].Data_Index = NPC_Index;
        Lists.Temp_Map[Map_Num].NPC[Index].Alive = true;
        Lists.Temp_Map[Map_Num].NPC[Index].X = x;
        Lists.Temp_Map[Map_Num].NPC[Index].Y = y;
        Lists.Temp_Map[Map_Num].NPC[Index].Direction = Direction;
        Lists.Temp_Map[Map_Num].NPC[Index].Vital = new short[(byte)Game.Vitals.Count];
        for (byte i = 0; i < (byte)Game.Vitals.Count; i++) Lists.Temp_Map[Map_Num].NPC[Index].Vital[i] = Lists.NPC[NPC_Index].Vital[i];

        // Envia os dados aos jogadores
        if (Socket.Device != null) Send.Map_NPC(Map_Num, Index);
    }

    public static bool IsAlied(short Index, short Allie)
    {
        // Verifica se o NPC é aliado do outro
        for (byte i = 0; i < Lists.NPC[Index].Allie.Length; i++)
            if (Lists.NPC[Index].Allie[i] == Allie)
                return true;

        return false;
    }
}