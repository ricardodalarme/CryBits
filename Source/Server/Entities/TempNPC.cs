using System;
using CryBits.Entities;
using CryBits.Server.Logic;
using CryBits.Server.Network;
using static CryBits.Utils;

namespace CryBits.Server.Entities
{
    internal class TempNPC : Character
    {
        // Dados básicos
        public byte Index;
        public NPC Data;
        public bool Alive;
        public Character Target;
        public int SpawnTimer;
        public int AttackTimer;

        private short Regeneration(byte vital)
        {
            // Cálcula o máximo de vital que o NPC possui
            switch ((Vitals)vital)
            {
                case Vitals.HP: return (short)(Data.Vital[vital] * 0.05 + Data.Attribute[(byte)Attributes.Vitality] * 0.3);
                case Vitals.MP: return (short)(Data.Vital[vital] * 0.05 + Data.Attribute[(byte)Attributes.Intelligence] * 0.1);
            }

            return 0;
        }

        // Construtor
        public TempNPC(byte index, TempMap map, NPC data)
        {
            Index = index;
            Map = map;
            Data = data;
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

            byte targetX = 0, targetY = 0;
            bool[] canMove = new bool[(byte) Directions.Count];
            short distance;
            bool moved = false;
            bool move = false;

            /////////////////
            // Regeneração //
            /////////////////
            if (Environment.TickCount > Loop.Timer_Regen + 5000)
                for (byte v = 0; v < (byte) Vitals.Count; v++)
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
            if (Data.Behaviour == NPCBehaviour.AttackOnSight)
            {
                // Jogador
                if (Target == null)
                    foreach (var player in Account.List)
                    {
                        // Verifica se o jogador está jogando e no mesmo mapa que o NPC
                        if (!player.IsPlaying) continue;
                        if (player.Character.Map != Map) continue;

                        // Se o jogador estiver no alcance do NPC, ir atrás dele
                        distance = (short) Math.Sqrt(Math.Pow(X - player.Character.X, 2) +
                                                     Math.Pow(Y - player.Character.Y, 2));
                        if (distance <= Data.Sight)
                        {
                            Target = player.Character;

                            // Mensagem
                            if (!string.IsNullOrEmpty(Data.SayMsg))
                                Send.Message(player.Character, Data.Name + ": " + Data.SayMsg,
                                    System.Drawing.Color.White);
                            break;
                        }
                    }

                // NPC
                if (Data.AttackNPC && Target == null)
                    for (byte npcIndex = 0; npcIndex < Map.NPC.Length; npcIndex++)
                    {
                        // Verifica se pode atacar
                        if (npcIndex == Index) continue;
                        if (!Map.NPC[npcIndex].Alive) continue;
                        if (Data.IsAlied(Map.NPC[npcIndex].Data)) continue;

                        // Se o NPC estiver no alcance do NPC, ir atrás dele
                        distance = (short) Math.Sqrt(Math.Pow(X - Map.NPC[npcIndex].X, 2) +
                                                     Math.Pow(Y - Map.NPC[npcIndex].Y, 2));
                        if (distance <= Data.Sight)
                        {
                            Target = Map.NPC[npcIndex];
                            break;
                        }
                    }
            }

            // Verifica se o alvo ainda está disponível
            if (Target != null)
                if (Target is Player && !((Player) Target).Account.IsPlaying || Target.Map != Map)
                    Target = null;
                else if (Target is TempNPC && !((TempNPC) Target).Alive)
                    Target = null;

            // Evita que ele se movimente sem sentido
            if (Target != null)
            {
                targetX = Target.X;
                targetY = Target.Y;

                // Verifica se o alvo saiu do alcance do NPC
                if (Data.Sight < Math.Sqrt(Math.Pow(X - targetX, 2) + Math.Pow(Y - targetY, 2)))
                    Target = null;
                else
                    move = true;
            }
            else
            {
                // Define o alvo a zona do NPC
                if (Map.Data.NPC[Index].Zone > 0)
                    if (Map.Data.Attribute[X, Y].Zone != Map.Data.NPC[Index].Zone)
                        for (byte x2 = 0; x2 < CryBits.Entities.Map.Width; x2++)
                        for (byte y2 = 0; y2 < CryBits.Entities.Map.Height; y2++)
                            if (Map.Data.Attribute[x2, y2].Zone == Map.Data.NPC[Index].Zone)
                                if (!Map.Data.Tile_Blocked(x2, y2))
                                {
                                    targetX = x2;
                                    targetY = y2;
                                    move = true;
                                    break;
                                }
            }

            // Movimenta o NPC
            if (move)
            {
                // Verifica como o NPC pode se mover
                if (Vital[(byte) Vitals.HP] > Data.Vital[(byte) Vitals.HP] * (Data.Flee_Helth / 100.0))
                {
                    // Para perto do alvo
                    canMove[(byte) Directions.Up] = Y > targetY;
                    canMove[(byte) Directions.Down] = Y < targetY;
                    canMove[(byte) Directions.Left] = X > targetX;
                    canMove[(byte) Directions.Right] = X < targetX;
                }
                else
                {
                    // Para longe do alvo
                    canMove[(byte) Directions.Up] = Y < targetY;
                    canMove[(byte) Directions.Down] = Y > targetY;
                    canMove[(byte) Directions.Left] = X < targetX;
                    canMove[(byte) Directions.Right] = X > targetX;
                }

                // Aleatoriza a forma que ele vai se movimentar até o alvo
                if (MyRandom.Next(0, 2) == 0)
                {
                    for (byte d = 0; d < (byte) Directions.Count; d++)
                        if (!moved && canMove[d] && Move((Directions) d))
                            moved = true;
                }
                else
                    for (short d = (byte) Directions.Count - 1; d >= 0; d--)
                        if (!moved && canMove[d] && Move((Directions) d))
                            moved = true;
            }

            // Move-se aleatoriamente
            if (Data.Behaviour == (byte) NPCBehaviour.Friendly || Target == null)
                if (MyRandom.Next(0, 3) == 0 && !moved)
                    if (Data.Movement == NPCMovements.MoveRandomly)
                        Move((Directions) MyRandom.Next(0, 4), 1, true);
                    else if (Data.Movement == NPCMovements.TurnRandomly)
                    {
                        Direction = (Directions) MyRandom.Next(0, 4);
                        Send.Map_NPC_Direction(this);
                    }

            ////////////
            // Ataque //
            ////////////
            Attack();
        }

        private void Spawn(byte x, byte y, Directions direction = 0)
        {
            // Faz o NPC surgir no mapa
            Alive = true;
            X = x;
            Y = y;
            Direction = direction;
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
                x = (byte)MyRandom.Next(0, CryBits.Entities.Map.Width - 1);
                y = (byte)MyRandom.Next(0, CryBits.Entities.Map.Height - 1);

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
            for (byte x2 = 0; x2 < CryBits.Entities.Map.Width; x2++)
                for (byte y2 = 0; y2 < CryBits.Entities.Map.Height; y2++)
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

        private bool Move(Directions direction, byte movement = 1, bool checkZone = false)
        {
            byte nextX = X, nextY = Y;

            // Define a direção do NPC
            Direction = direction;
            Send.Map_NPC_Direction(this);

            // Próximo azulejo
            NextTile(direction, ref nextX, ref nextY);

            // Próximo azulejo bloqueado ou fora do limite
            if (Map.Data.OutLimit(nextX, nextY)) return false;
            if (Map.Tile_Blocked(X, Y, direction)) return false;

            // Verifica se está dentro da zona
            if (checkZone)
                if (Map.Data.Attribute[nextX, nextY].Zone != Map.Data.NPC[Index].Zone)
                    return false;

            // Movimenta o NPC
            X = nextX;
            Y = nextY;
            Send.Map_NPC_Movement(this, movement);
            return true;
        }

        private void Attack()
        {
            byte nextX = X, nextY = Y;
            NextTile(Direction, ref nextX, ref nextY);

            // Apenas se necessário
            if (!Alive) return;
            if (Environment.TickCount < AttackTimer + 750) return;
            if (Map.Tile_Blocked(X, Y, Direction, false)) return;

            // Verifica se o jogador está na frente do NPC
            if (Target is Player)
                Attack_Player(Map.HasPlayer(nextX, nextY));
            // Verifica se o NPC alvo está na frente do NPC
            else if (Target is TempNPC)
                Attack_NPC(Map.HasNPC(nextX, nextY));
        }

        private void Attack_Player(Player victim)
        {
            // Verifica se a vítima pode ser atacada
            if (victim == null) return;
            if (victim.GettingMap) return;

            // Tempo de ataque 
            AttackTimer = Environment.TickCount;

            // Cálculo de dano
            short attackDamage = (short)(Data.Attribute[(byte)Attributes.Strength] - victim.Player_Defense);

            // Dano não fatal
            if (attackDamage > 0)
            {
                // Demonstra o ataque aos outros jogadores
                Send.Map_NPC_Attack(this, victim.Name, Targets.Player);

                if (attackDamage < victim.Vital[(byte)Vitals.HP])
                {
                    victim.Vital[(byte)Vitals.HP] -= attackDamage;
                    Send.Player_Vitals(victim);
                }
                // FATALITY
                else
                {
                    // Reseta o alvo do NPC
                    Target = null;

                    // Mata o jogador
                    victim.Died();
                }
            }
            // Demonstra o ataque aos outros jogadores
            else
                Send.Map_NPC_Attack(this);
        }

        private void Attack_NPC(TempNPC victim)
        {
            // Verifica se a vítima pode ser atacada
            if (victim == null) return;
            if (!victim.Alive) return;

            // Tempo de ataque 
            AttackTimer = Environment.TickCount;

            // Define o alvo do NPC
            victim.Target = this;

            // Cálculo de dano
            short attackDamage = (short)(Data.Attribute[(byte)Attributes.Strength] - victim.Data.Attribute[(byte)Attributes.Resistance]);

            // Dano não fatal
            if (attackDamage > 0)
            {
                // Demonstra o ataque aos outros jogadores
                Send.Map_NPC_Attack(this, victim.Index.ToString(), Targets.NPC);

                if (attackDamage < victim.Vital[(byte)Vitals.HP])
                {
                    victim.Vital[(byte)Vitals.HP] -= attackDamage;
                    Send.Map_NPC_Vitals(victim);
                }
                // FATALITY
                else
                {
                    // Reseta o alvo do NPC
                    Target = null;

                    // Mata o NPC
                    victim.Died();
                }
            }
            // Demonstra o ataque aos outros jogadores
            else
                Send.Map_NPC_Attack(this);
        }

        public void Died()
        {
            // Solta os itens
            for (byte i = 0; i < Data.Drop.Count; i++)
                if (Data.Drop[i].Item != null)
                    if (MyRandom.Next(1, 99) <= Data.Drop[i].Chance)
                        // Solta o item
                        Map.Item.Add(new MapItems
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