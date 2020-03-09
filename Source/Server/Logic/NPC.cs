using System;

class NPC
{
    public enum Behaviour
    {
        Friendly,
        AttackOnSight,
        AttackWhenAttacked
    }

    public enum Movements
    {
        MoveRandomly,
        TurnRandomly,
        StandStill
    }

    private static short Regeneration(short Map_Num, byte Index, byte Vital)
    {
        Lists.Structures.NPC Data = Lists.NPC[Lists.Temp_Map[Map_Num].NPC[Index].Index];

        // Cálcula o máximo de vital que o NPC possui
        switch ((Game.Vitals)Vital)
        {
            case Game.Vitals.HP: return (short)(Data.Vital[Vital] * 0.05 + Data.Attribute[(byte)Game.Attributes.Vitality] * 0.3);
            case Game.Vitals.MP: return (short)(Data.Vital[Vital] * 0.05 + Data.Attribute[(byte)Game.Attributes.Intelligence] * 0.1);
        }

        return 0;
    }

    public static void Logic(short Map_Num)
    {
        // Lógica dos NPCs
        for (byte i = 1; i < Lists.Temp_Map[Map_Num].NPC.Length; i++)
        {
            Lists.Structures.Map_NPCs Data = Lists.Temp_Map[Map_Num].NPC[i];
            Lists.Structures.NPC NPC_Data = Lists.NPC[Lists.Map[Map_Num].NPC[i].Index];

            ////////////////
            // Surgimento //
            ////////////////
            if (Data.Index == 0)
            {
                if (Environment.TickCount > Data.Spawn_Timer + (NPC_Data.SpawnTime * 1000)) Spawn(i, Map_Num);
                continue;
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
                        if (Data.Vital[v] < NPC_Data.Vital[v])
                        {
                            // Renera os vitais
                            Lists.Temp_Map[Map_Num].NPC[i].Vital[v] += Regeneration(Map_Num, i, v);

                            // Impede que o valor passe do limite
                            if (Lists.Temp_Map[Map_Num].NPC[i].Vital[v] > NPC_Data.Vital[v])
                                Lists.Temp_Map[Map_Num].NPC[i].Vital[v] = NPC_Data.Vital[v];

                            // Envia os dados aos jogadores do mapa
                            Send.Map_NPC_Vitals(Map_Num, i);
                        }

                //////////////////
                // Movimentação //
                //////////////////
                // Atacar ao ver
                if (NPC_Data.Behaviour == (byte)Behaviour.AttackOnSight)
                {
                    // Jogador
                    if (Lists.Temp_Map[Map_Num].NPC[i].Target_Index == 0)
                        for (byte Player_Index = 1; Player_Index <= Game.HigherIndex; Player_Index++)
                        {
                            // Verifica se o jogador está jogando e no mesmo mapa que o NPC
                            if (!Player.IsPlaying(Player_Index)) continue;
                            if (Player.Character(Player_Index).Map != Map_Num) continue;

                            // Se o jogador estiver no alcance do NPC, ir atrás do jogador
                            Distance = (short)Math.Sqrt(Math.Pow(Data.X - Player.Character(Player_Index).X, 2) + Math.Pow(Data.Y - Player.Character(Player_Index).Y, 2));
                            if (Distance <= NPC_Data.Sight)
                            {
                                Lists.Temp_Map[Map_Num].NPC[i].Target_Type = (byte)Game.Target.Player;
                                Lists.Temp_Map[Map_Num].NPC[i].Target_Index = Player_Index;
                                Data = Lists.Temp_Map[Map_Num].NPC[i];

                                // Mensagem
                                if (!string.IsNullOrEmpty(Lists.NPC[Lists.Temp_Map[Map_Num].NPC[i].Index].SayMsg))
                                    Send.Message(Player_Index, Lists.NPC[Lists.Temp_Map[Map_Num].NPC[i].Index].Name + ": " + Lists.NPC[Lists.Temp_Map[Map_Num].NPC[i].Index].SayMsg, System.Drawing.Color.White);
                                break;
                            }
                        }

                    // NPC
                    if (NPC_Data.AttackNPC && Lists.Temp_Map[Map_Num].NPC[i].Target_Index == 0)
                        for (byte NPC_Index = 1; NPC_Index < Lists.Temp_Map[Map_Num].NPC.Length; NPC_Index++)
                        {
                            // Verifica se pode atacar
                            if (NPC_Index == i) continue;
                            if (Lists.Temp_Map[Map_Num].NPC[NPC_Index].Index == 0) continue;

                            // Se o jogador estiver no alcance do NPC, ir atrás do jogador
                            Distance = (short)Math.Sqrt(Math.Pow(Data.X - Lists.Temp_Map[Map_Num].NPC[NPC_Index].X, 2) + Math.Pow(Data.Y - Lists.Temp_Map[Map_Num].NPC[NPC_Index].Y, 2));
                            if (Distance <= NPC_Data.Sight)
                            {
                                Lists.Temp_Map[Map_Num].NPC[i].Target_Type = (byte)Game.Target.NPC;
                                Lists.Temp_Map[Map_Num].NPC[i].Target_Index = NPC_Index;
                                Data = Lists.Temp_Map[Map_Num].NPC[i];
                                break;
                            }
                        }
                }

                if (Data.Target_Type == (byte)Game.Target.Player)
                {
                    // Verifica se o jogador ainda está disponível
                    if (!Player.IsPlaying(Data.Target_Index) || Player.Character(Data.Target_Index).Map != Map_Num)
                    {
                        Lists.Temp_Map[Map_Num].NPC[i].Target_Type = 0;
                        Lists.Temp_Map[Map_Num].NPC[i].Target_Index = 0;
                        Data = Lists.Temp_Map[Map_Num].NPC[i];
                    }
                    // Posição do alvo
                    else
                    {
                        TargetX = Player.Character(Data.Target_Index).X;
                        TargetY = Player.Character(Data.Target_Index).Y;
                    }
                }
                else if (Data.Target_Type == (byte)Game.Target.NPC)
                {
                    // Verifica se o NPC ainda está disponível
                    if (Lists.Temp_Map[Map_Num].NPC[Data.Target_Index].Index == 0)
                    {
                        Lists.Temp_Map[Map_Num].NPC[i].Target_Type = 0;
                        Lists.Temp_Map[Map_Num].NPC[i].Target_Index = 0;
                        Data = Lists.Temp_Map[Map_Num].NPC[i];
                    }
                    // Posição do alvo
                    else
                    {
                        TargetX = Lists.Temp_Map[Map_Num].NPC[Data.Target_Index].X;
                        TargetY = Lists.Temp_Map[Map_Num].NPC[Data.Target_Index].Y;
                    }
                }

                // Verifica se o alvo saiu do alcance do NPC
                Distance = (short)Math.Sqrt(Math.Pow(Data.X - TargetX, 2) + Math.Pow(Data.Y - TargetY, 2));
                if (Distance > NPC_Data.Sight)
                {
                    Lists.Temp_Map[Map_Num].NPC[i].Target_Type = 0;
                    Lists.Temp_Map[Map_Num].NPC[i].Target_Index = 0;
                    Data = Lists.Temp_Map[Map_Num].NPC[i];
                }

                // Evita que ele se movimente sem sentido
                if (Data.Target_Index > 0)
                    Move = true;
                else
                {
                    // Define o alvo a zona do NPC
                    if (Lists.Map[Map_Num].NPC[i].Zone > 0)
                        if (Lists.Map[Map_Num].Tile[Data.X, Data.Y].Zone != Lists.Map[Map_Num].NPC[i].Zone)
                            for (byte x2 = 0; x2 <= Lists.Map[Map_Num].Width; x2++)
                                for (byte y2 = 0; y2 <= Lists.Map[Map_Num].Height; y2++)
                                    if (Lists.Map[Map_Num].Tile[x2, y2].Zone == Lists.Map[Map_Num].NPC[i].Zone)
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
                    if (NPC_Data.Flee_Helth == 0 || Data.Vital[(byte)Game.Vitals.HP] > NPC_Data.Vital[(byte)Game.Vitals.HP] * (NPC_Data.Flee_Helth / 100.0))
                    {
                        // Para perto do alvo
                        CanMove[(byte)Game.Directions.Up] = Data.Y > TargetY;
                        CanMove[(byte)Game.Directions.Down] = Data.Y < TargetY;
                        CanMove[(byte)Game.Directions.Left] = Data.X > TargetX;
                        CanMove[(byte)Game.Directions.Right] = Data.X < TargetX;
                    }
                    else
                    {
                        // Para longe do alvo
                        CanMove[(byte)Game.Directions.Up] = Data.Y < TargetY;
                        CanMove[(byte)Game.Directions.Down] = Data.Y > TargetY;
                        CanMove[(byte)Game.Directions.Left] = Data.X < TargetX;
                        CanMove[(byte)Game.Directions.Right] = Data.X > TargetX;
                    }

                    // Aleatoriza a forma que ele vai se movimentar até o alvo
                    if (Game.Random.Next(0, 2) == 0)
                    {
                        for (byte d = 0; d < (byte)Game.Directions.Count; d++)
                            if (!Moved && CanMove[d] && NPC.Move(Map_Num, i, (Game.Directions)d))
                                Moved = true;
                    }
                    else
                        for (short d = (byte)Game.Directions.Count - 1; d >= 0; d--)
                            if (!Moved && CanMove[d] && NPC.Move(Map_Num, i, (Game.Directions)d))
                                Moved = true;
                }

                // Move-se aleatoriamente
                if (NPC_Data.Behaviour == (byte)Behaviour.Friendly || Data.Target_Index == 0)
                    if (Game.Random.Next(0, 3) == 0 && !Moved)
                        if (NPC_Data.Movement == Movements.MoveRandomly)
                            NPC.Move(Map_Num, i, (Game.Directions)Game.Random.Next(0, 4), 1, true);
                        else if (NPC_Data.Movement == Movements.TurnRandomly)
                        {
                            Lists.Temp_Map[Map_Num].NPC[i].Direction = (Game.Directions)Game.Random.Next(0, 4);
                            Send.Map_NPC_Direction(Map_Num, i);
                        }

                ////////////
                // Ataque //
                ////////////
                short Next_X = Data.X, Next_Y = Data.Y;
                Map.NextTile(Data.Direction, ref Next_X, ref Next_Y);
                if (Data.Target_Type == (byte)Game.Target.Player)
                {
                    // Verifica se o jogador está na frente do NPC
                    if (Map.HasPlayer(Map_Num, Next_X, Next_Y) == Data.Target_Index) Attack_Player(Map_Num, i, Data.Target_Index);
                }
                else if (Data.Target_Type == (byte)Game.Target.NPC)
                {
                    // Verifica se o NPC alvo está na frente do NPC
                    if (Map.HasNPC(Map_Num, Next_X, Next_Y) == Data.Target_Index) Attack_NPC(Map_Num, i, Data.Target_Index);
                }
            }
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
        Lists.Temp_Map[Map_Num].NPC[Index].Index = NPC_Index;
        Lists.Temp_Map[Map_Num].NPC[Index].X = x;
        Lists.Temp_Map[Map_Num].NPC[Index].Y = y;
        Lists.Temp_Map[Map_Num].NPC[Index].Direction = Direction;
        Lists.Temp_Map[Map_Num].NPC[Index].Vital = new short[(byte)Game.Vitals.Count];
        for (byte i = 0; i < (byte)Game.Vitals.Count; i++) Lists.Temp_Map[Map_Num].NPC[Index].Vital[i] = Lists.NPC[NPC_Index].Vital[i];

        // Envia os dados aos jogadores
        if (Socket.Device != null) Send.Map_NPC(Map_Num, Index);
    }

    private static bool Move(short Map_Num, byte Index, Game.Directions Direction, byte Movement = 1, bool CheckZone = false)
    {
        Lists.Structures.Map_NPCs Data = Lists.Temp_Map[Map_Num].NPC[Index];
        byte x = Data.X, y = Data.Y;
        short Next_X = x, Next_Y = y;

        // Define a direção do NPC
        Lists.Temp_Map[Map_Num].NPC[Index].Direction = Direction;
        Send.Map_NPC_Direction(Map_Num, Index);

        // Próximo azulejo
        Map.NextTile(Direction, ref Next_X, ref Next_Y);

        // Próximo azulejo bloqueado ou fora do limite
        if (Map.OutLimit(Map_Num, Next_X, Next_Y)) return false;
        if (Map.Tile_Blocked(Map_Num, x, y, Direction)) return false;

        // Verifica se está dentro da zona
        if (CheckZone)
            if (Lists.Map[Map_Num].Tile[Next_X, Next_Y].Zone != Lists.Map[Map_Num].NPC[Index].Zone)
                return false;

        // Movimenta o NPC
        Lists.Temp_Map[Map_Num].NPC[Index].X = (byte)Next_X;
        Lists.Temp_Map[Map_Num].NPC[Index].Y = (byte)Next_Y;
        Send.Map_NPC_Movement(Map_Num, Index, Movement);
        return true;
    }

    private static void Attack_Player(short Map_Num, byte Index, byte Victim)
    {
        Lists.Structures.Map_NPCs Data = Lists.Temp_Map[Map_Num].NPC[Index];
        short x = Data.X, y = Data.Y;

        // Define o azujelo a frente do NPC
        Map.NextTile(Data.Direction, ref x, ref y);

        // Verifica se a vítima pode ser atacada
        if (Data.Index == 0) return;
        if (Environment.TickCount < Data.Attack_Timer + 750) return;
        if (!Player.IsPlaying(Victim)) return;
        if (Lists.Temp_Player[Victim].GettingMap) return;
        if (Map_Num != Player.Character(Victim).Map) return;
        if (Player.Character(Victim).X != x || Player.Character(Victim).Y != y) return;
        if (Map.Tile_Blocked(Map_Num, Data.X, Data.Y, Data.Direction, false)) return;

        // Tempo de ataque 
        Lists.Temp_Map[Map_Num].NPC[Index].Attack_Timer = Environment.TickCount;

        // Cálculo de dano
        short Damage = (short)(Lists.NPC[Data.Index].Attribute[(byte)Game.Attributes.Strength] - Player.Character(Victim).Player_Defense);

        // Dano não fatal
        if (Damage > 0)
        {
            // Demonstra o ataque aos outros jogadores
            Send.Map_NPC_Attack(Map_Num, Index, Victim, (byte)Game.Target.Player);

            if (Damage < Player.Character(Victim).Vital[(byte)Game.Vitals.HP])
            {
                Player.Character(Victim).Vital[(byte)Game.Vitals.HP] -= Damage;
                Send.Player_Vitals(Victim);
            }
            // FATALITY
            else
            {
                // Reseta o alvo do NPC
                Lists.Temp_Map[Map_Num].NPC[Index].Target_Type = 0;
                Lists.Temp_Map[Map_Num].NPC[Index].Target_Index = 0;

                // Mata o jogador
                Player.Died(Victim);
            }
        }
        // Demonstra o ataque aos outros jogadores
        else
            Send.Map_NPC_Attack(Map_Num, Index);
    }

    private static void Attack_NPC(short Map_Num, byte Index, byte Victim)
    {
        Lists.Structures.Map_NPCs Data = Lists.Temp_Map[Map_Num].NPC[Index], Victim_Data = Lists.Temp_Map[Map_Num].NPC[Victim];
        short x = Data.X, y = Data.Y;

        // Define o azujelo a frente do NPC
        Map.NextTile(Data.Direction, ref x, ref y);

        // Verifica se a vítima pode ser atacada
        if (Data.Index == 0) return;
        if (Victim_Data.Index == 0) return;
        if (Environment.TickCount < Data.Attack_Timer + 750) return;
        if (Victim_Data.X != x || Victim_Data.Y != y) return;
        if (Map.Tile_Blocked(Map_Num, Data.X, Data.Y, Data.Direction, false)) return;
        if (IsAlied(Data.Index, Victim_Data.Index)) return;

        // Tempo de ataque 
        Lists.Temp_Map[Map_Num].NPC[Index].Attack_Timer = Environment.TickCount;

        // Define o alvo do NPC
        Lists.Temp_Map[Map_Num].NPC[Victim].Target_Index = Index;
        Lists.Temp_Map[Map_Num].NPC[Victim].Target_Type = (byte)Game.Target.NPC;

        // Cálculo de dano
        short Damage = (short)(Lists.NPC[Data.Index].Attribute[(byte)Game.Attributes.Strength] - Lists.NPC[Victim_Data.Index].Attribute[(byte)Game.Attributes.Resistance]);

        // Dano não fatal
        if (Damage > 0)
        {
            // Demonstra o ataque aos outros jogadores
            Send.Map_NPC_Attack(Map_Num, Index, Victim, (byte)Game.Target.NPC);

            if (Damage < Victim_Data.Vital[(byte)Game.Vitals.HP])
            {
                Lists.Temp_Map[Map_Num].NPC[Victim].Vital[(byte)Game.Vitals.HP] -= Damage;
                Send.Map_NPC_Vitals(Map_Num, Victim);
            }
            // FATALITY
            else
            {
                // Reseta o alvo do NPC
                Lists.Temp_Map[Map_Num].NPC[Index].Target_Type = 0;
                Lists.Temp_Map[Map_Num].NPC[Index].Target_Index = 0;

                // Mata o NPC
                Died(Map_Num, Victim);
            }
        }
        // Demonstra o ataque aos outros jogadores
        else
            Send.Map_NPC_Attack(Map_Num, Index);
    }

    public static void Died(short Map_Num, byte Index)
    {
        Lists.Structures.NPC NPC = Lists.NPC[Lists.Temp_Map[Map_Num].NPC[Index].Index];

        // Solta os itens
        for (byte i = 0; i < NPC.Drop.Length; i++)
            if (NPC.Drop[i].Item_Num > 0)
                if (Game.Random.Next(NPC.Drop[i].Chance, 101) == 100)
                {
                    // Dados do item
                    Lists.Structures.Map_Items Item = new Lists.Structures.Map_Items();
                    Item.Index = NPC.Drop[i].Item_Num;
                    Item.Amount = NPC.Drop[i].Amount;
                    Item.X = Lists.Temp_Map[Map_Num].NPC[Index].X;
                    Item.Y = Lists.Temp_Map[Map_Num].NPC[Index].Y;

                    // Solta
                    Lists.Temp_Map[Map_Num].Item.Add(Item);
                }

        // Envia os dados dos itens no chão para o mapa
        Send.Map_Items(Map_Num);

        // Reseta os dados do NPC 
        Lists.Temp_Map[Map_Num].NPC[Index].Vital[(byte)Game.Vitals.HP] = 0;
        Lists.Temp_Map[Map_Num].NPC[Index].Spawn_Timer = Environment.TickCount;
        Lists.Temp_Map[Map_Num].NPC[Index].Index = 0;
        Lists.Temp_Map[Map_Num].NPC[Index].Target_Type = 0;
        Lists.Temp_Map[Map_Num].NPC[Index].Target_Index = 0;
        Send.Map_NPC_Died(Map_Num, Index);
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