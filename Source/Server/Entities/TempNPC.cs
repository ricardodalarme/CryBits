using CryBits.Server.Logic;
using CryBits.Server.Network;
using System;
using static CryBits.Server.Logic.Utils;

namespace CryBits.Server.Entities
{
    class TempNPC : Character
    {
        // Dados básicos
        public byte Index;
        public NPC Data;
        public bool Alive;
        public Character Target;
        public int SpawnTimer;
        public int AttackTimer;

        private short Regeneration(byte Vital)
        {
            // Cálcula o máximo de vital que o NPC possui
            switch ((Vitals)Vital)
            {
                case Vitals.HP: return (short)(Data.Vital[Vital] * 0.05 + Data.Attribute[(byte)Attributes.Vitality] * 0.3);
                case Vitals.MP: return (short)(Data.Vital[Vital] * 0.05 + Data.Attribute[(byte)Attributes.Intelligence] * 0.1);
            }

            return 0;
        }

        // Construtor
        public TempNPC(byte Index, TempMap Map, NPC Data)
        {
            this.Index = Index;
            this.Map = Map;
            this.Data = Data;
        }

        /////////////
        // Funções //
        /////////////
        public void Logic()
        {
            ////////////////
            // Surgimento //
            ////////////////
            if (!Alive)
            {
                if (Environment.TickCount > SpawnTimer + (Data.SpawnTime * 1000)) Spawn();
                return;
            }
            else
            {
                byte TargetX = 0, TargetY = 0;
                bool[] CanMove = new bool[(byte)Directions.Count];
                short Distance;
                bool Moved = false;
                bool Move = false;

                /////////////////
                // Regeneração //
                /////////////////
                if (Environment.TickCount > Loop.Timer_Regen + 5000)
                    for (byte v = 0; v < (byte)Vitals.Count; v++)
                        if (Vital[v] < Data.Vital[v])
                        {
                            // Renera os vitais
                            Vital[v] += Regeneration(v);

                            // Impede que o valor passe do limite
                            if (Vital[v] > Data.Vital[v]) Vital[v] = Data.Vital[v];

                            // Envia os dados aos jogadores do mapa
                            Send.Map_NPC_Vitals(this);
                        }

                //////////////////
                // Movimentação //
                //////////////////
                // Atacar ao ver
                if (Data.Behaviour == (byte)NPCBehaviour.AttackOnSight)
                {
                    // Jogador
                    if (Target == null)
                        foreach (var Player in Account.List)
                        {
                            // Verifica se o jogador está jogando e no mesmo mapa que o NPC
                            if (!Player.IsPlaying) continue;
                            if (Player.Character.Map != Map) continue;

                            // Se o jogador estiver no alcance do NPC, ir atrás dele
                            Distance = (short)Math.Sqrt(Math.Pow(X - Player.Character.X, 2) + Math.Pow(Y - Player.Character.Y, 2));
                            if (Distance <= Data.Sight)
                            {
                                Target = Player.Character;

                                // Mensagem
                                if (!string.IsNullOrEmpty(Data.SayMsg)) Send.Message(Player.Character, Data.Name + ": " + Data.SayMsg, System.Drawing.Color.White);
                                break;
                            }
                        }

                    // NPC
                    if (Data.AttackNPC && Target == null)
                        for (byte NPC_Index = 0; NPC_Index < Map.NPC.Length; NPC_Index++)
                        {
                            // Verifica se pode atacar
                            if (NPC_Index == Index) continue;
                            if (!Map.NPC[NPC_Index].Alive) continue;
                            if (Data.IsAlied(Map.NPC[NPC_Index].Data)) continue;

                            // Se o NPC estiver no alcance do NPC, ir atrás dele
                            Distance = (short)Math.Sqrt(Math.Pow(X - Map.NPC[NPC_Index].X, 2) + Math.Pow(Y - Map.NPC[NPC_Index].Y, 2));
                            if (Distance <= Data.Sight)
                            {
                                Target = Map.NPC[NPC_Index];
                                break;
                            }
                        }
                }

                // Verifica se o alvo ainda está disponível
                if (Target != null)
                    if (Target is Player && !((Player)Target).Account.IsPlaying || Target.Map != Map)
                        Target = null;
                    else if (Target is TempNPC && !((TempNPC)Target).Alive)
                        Target = null;

                // Evita que ele se movimente sem sentido
                if (Target != null)
                {
                    TargetX = Target.X;
                    TargetY = Target.Y;

                    // Verifica se o alvo saiu do alcance do NPC
                    if (Data.Sight < Math.Sqrt(Math.Pow(X - TargetX, 2) + Math.Pow(Y - TargetY, 2)))
                        Target = null;
                    else
                        Move = true;
                }
                else
                {
                    // Define o alvo a zona do NPC
                    if (Map.Data.NPC[Index].Zone > 0)
                        if (Map.Data.Attribute[X, Y].Zone != Map.Data.NPC[Index].Zone)
                            for (byte x2 = 0; x2 < Entities.Map.Width; x2++)
                                for (byte y2 = 0; y2 < Entities.Map.Height; y2++)
                                    if (Map.Data.Attribute[x2, y2].Zone == Map.Data.NPC[Index].Zone)
                                        if (!Map.Data.Tile_Blocked(x2, y2))
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
                    if (Vital[(byte)Vitals.HP] > Data.Vital[(byte)Vitals.HP] * (Data.Flee_Helth / 100.0))
                    {
                        // Para perto do alvo
                        CanMove[(byte)Directions.Up] = Y > TargetY;
                        CanMove[(byte)Directions.Down] = Y < TargetY;
                        CanMove[(byte)Directions.Left] = X > TargetX;
                        CanMove[(byte)Directions.Right] = X < TargetX;
                    }
                    else
                    {
                        // Para longe do alvo
                        CanMove[(byte)Directions.Up] = Y < TargetY;
                        CanMove[(byte)Directions.Down] = Y > TargetY;
                        CanMove[(byte)Directions.Left] = X < TargetX;
                        CanMove[(byte)Directions.Right] = X > TargetX;
                    }

                    // Aleatoriza a forma que ele vai se movimentar até o alvo
                    if (MyRandom.Next(0, 2) == 0)
                    {
                        for (byte d = 0; d < (byte)Directions.Count; d++)
                            if (!Moved && CanMove[d] && this.Move((Directions)d))
                                Moved = true;
                    }
                    else
                        for (short d = (byte)Directions.Count - 1; d >= 0; d--)
                            if (!Moved && CanMove[d] && this.Move((Directions)d))
                                Moved = true;
                }

                // Move-se aleatoriamente
                if (Data.Behaviour == (byte)NPCBehaviour.Friendly || Target == null)
                    if (MyRandom.Next(0, 3) == 0 && !Moved)
                        if (Data.Movement == NPCMovements.MoveRandomly)
                            this.Move((Directions)MyRandom.Next(0, 4), 1, true);
                        else if (Data.Movement == NPCMovements.TurnRandomly)
                        {
                            Direction = (Directions)MyRandom.Next(0, 4);
                            Send.Map_NPC_Direction(this);
                        }

                ////////////
                // Ataque //
                ////////////
                Attack();
            }
        }

        private void Spawn(byte X, byte Y, Directions Direction = 0)
        {
            // Faz o NPC surgir no mapa
            Alive = true;
            this.X = X;
            this.Y = Y;
            this.Direction = Direction;
            for (byte i = 0; i < (byte)Vitals.Count; i++) Vital[i] = Data.Vital[i];

            // Envia os dados aos jogadores
            if (Socket.Device != null) Send.Map_NPC(Map.NPC[Index]);
        }

        public void Spawn()
        {
            byte x, y;

            // Antes verifica se tem algum local de aparecimento específico
            if (Map.Data.NPC[Index].Spawn)
            {
                Spawn(Map.Data.NPC[Index].X, Map.Data.NPC[Index].Y);
                return;
            }

            // Faz com que ele apareça em um local aleatório
            for (byte i = 0; i < 50; i++) // tenta 50 vezes com que ele apareça em um local aleatório
            {
                x = (byte)MyRandom.Next(0, Entities.Map.Width - 1);
                y = (byte)MyRandom.Next(0, Entities.Map.Height - 1);

                // Verifica se está dentro da zona
                if (Map.Data.NPC[Index].Zone > 0)
                    if (Map.Data.Attribute[x, y].Zone != Map.Data.NPC[Index].Zone)
                        continue;

                // Define os dados
                if (!Map.Data.Tile_Blocked(x, y))
                {
                    Spawn(x, y);
                    return;
                }
            }

            // Em último caso, tentar no primeiro lugar possível
            for (byte x2 = 0; x2 < Entities.Map.Width; x2++)
                for (byte y2 = 0; y2 < Entities.Map.Height; y2++)
                    if (!Map.Data.Tile_Blocked(x2, y2))
                    {
                        // Verifica se está dentro da zona
                        if (Map.Data.NPC[Index].Zone > 0)
                            if (Map.Data.Attribute[x2, y2].Zone != Map.Data.NPC[Index].Zone)
                                continue;

                        // Define os dados
                        Spawn(x2, y2);
                        return;
                    }
        }

        private bool Move(Directions Direction, byte Movement = 1, bool CheckZone = false)
        {
            byte Next_X = X, Next_Y = Y;

            // Define a direção do NPC
            this.Direction = Direction;
            Send.Map_NPC_Direction(this);

            // Próximo azulejo
            NextTile(Direction, ref Next_X, ref Next_Y);

            // Próximo azulejo bloqueado ou fora do limite
            if (Map.Data.OutLimit(Next_X, Next_Y)) return false;
            if (Map.Tile_Blocked(X, Y, Direction)) return false;

            // Verifica se está dentro da zona
            if (CheckZone)
                if (Map.Data.Attribute[Next_X, Next_Y].Zone != Map.Data.NPC[Index].Zone)
                    return false;

            // Movimenta o NPC
            X = Next_X;
            Y = Next_Y;
            Send.Map_NPC_Movement(this, Movement);
            return true;
        }

        private void Attack()
        {
            byte Next_X = X, Next_Y = Y;
            NextTile(Direction, ref Next_X, ref Next_Y);

            // Apenas se necessário
            if (!Alive) return;
            if (Environment.TickCount < AttackTimer + 750) return;
            if (Map.Tile_Blocked(X, Y, Direction, false)) return;

            // Verifica se o jogador está na frente do NPC
            if (Target is Player)
                Attack_Player(Map.HasPlayer(Next_X, Next_Y));
            // Verifica se o NPC alvo está na frente do NPC
            else if (Target is TempNPC)
                Attack_NPC(Map.HasNPC(Next_X, Next_Y));
        }

        private void Attack_Player(Player Victim)
        {
            // Verifica se a vítima pode ser atacada
            if (Victim == null) return;
            if (Victim.GettingMap) return;

            // Tempo de ataque 
            AttackTimer = Environment.TickCount;

            // Cálculo de dano
            short Attack_Damage = (short)(Data.Attribute[(byte)Attributes.Strength] - Victim.Player_Defense);

            // Dano não fatal
            if (Attack_Damage > 0)
            {
                // Demonstra o ataque aos outros jogadores
                Send.Map_NPC_Attack(this, Victim.Name, Targets.Player);

                if (Attack_Damage < Victim.Vital[(byte)Vitals.HP])
                {
                    Victim.Vital[(byte)Vitals.HP] -= Attack_Damage;
                    Send.Player_Vitals(Victim);
                }
                // FATALITY
                else
                {
                    // Reseta o alvo do NPC
                    Target = null;

                    // Mata o jogador
                    Victim.Died();
                }
            }
            // Demonstra o ataque aos outros jogadores
            else
                Send.Map_NPC_Attack(this);
        }

        private void Attack_NPC(TempNPC Victim)
        {
            // Verifica se a vítima pode ser atacada
            if (Victim == null) return;
            if (!Victim.Alive) return;

            // Tempo de ataque 
            AttackTimer = Environment.TickCount;

            // Define o alvo do NPC
            Victim.Target = this;

            // Cálculo de dano
            short Attack_Damage = (short)(Data.Attribute[(byte)Attributes.Strength] - Victim.Data.Attribute[(byte)Attributes.Resistance]);

            // Dano não fatal
            if (Attack_Damage > 0)
            {
                // Demonstra o ataque aos outros jogadores
                Send.Map_NPC_Attack(this, Victim.Index.ToString(), Targets.NPC);

                if (Attack_Damage < Victim.Vital[(byte)Vitals.HP])
                {
                    Victim.Vital[(byte)Vitals.HP] -= Attack_Damage;
                    Send.Map_NPC_Vitals(Victim);
                }
                // FATALITY
                else
                {
                    // Reseta o alvo do NPC
                    Target = null;

                    // Mata o NPC
                    Victim.Died();
                }
            }
            // Demonstra o ataque aos outros jogadores
            else
                Send.Map_NPC_Attack(this);
        }

        public void Died()
        {
            // Solta os itens
            for (byte i = 0; i < Data.Drop.Length; i++)
                if (Data.Drop[i].Item != null)
                    if (MyRandom.Next(1, 99) <= Data.Drop[i].Chance)
                        // Solta o item
                        Map.Item.Add(new TMap_Items
                        {
                            Item = Data.Drop[i].Item,
                            Amount = Data.Drop[i].Amount,
                            X = X,
                            Y = Y
                        });

            // Envia os dados dos itens no chão para o mapa
            Send.Map_Items(Map);

            // Reseta os dados do NPC 
            SpawnTimer = Environment.TickCount;
            Target = null;
            Alive = false;
            Send.Map_NPC_Died(this);
        }
    }
}