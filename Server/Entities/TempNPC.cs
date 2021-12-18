using System;
using System.Drawing;
using CryBits.Entities;
using CryBits.Enums;
using CryBits.Server.Logic;
using CryBits.Server.Network;
using static CryBits.Globals;
using static CryBits.Utils;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Server.Entities;

internal class TempNpc : Character
{
    // Dados básicos
    public readonly byte Index;
    public readonly Npc Data;
    public bool Alive;
    public Character Target;
    private int _spawnTimer;
    private int _attackTimer;

    private short Regeneration(byte vital)
    {
        // Cálcula o máximo de vital que o Npc possui
        switch ((Vital)vital)
        {
            case Enums.Vital.HP: return (short)((Data.Vital[vital] * 0.05) + (Data.Attribute[(byte)Attribute.Vitality] * 0.3));
            case Enums.Vital.MP: return (short)((Data.Vital[vital] * 0.05) + (Data.Attribute[(byte)Attribute.Intelligence] * 0.1));
        }

        return 0;
    }

    // Construtor
    public TempNpc(byte index, TempMap map, Npc data)
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
            if (Environment.TickCount > _spawnTimer + (Data.SpawnTime * 1000)) Spawn();
            return;
        }

        byte targetX = 0, targetY = 0;
        var canMove = new bool[(byte)Direction.Count];
        short distance;
        var moved = false;
        var move = false;

        /////////////////
        // Regeneração //
        /////////////////
        if (Environment.TickCount > Loop.TimerRegeneration + 5000)
            for (byte v = 0; v < (byte)Enums.Vital.Count; v++)
                if (Vital[v] < Data.Vital[v])
                {
                    // Renera os vitais
                    Vital[v] += Regeneration(v);

                    // Impede que o valor passe do limite
                    if (Vital[v] > Data.Vital[v]) Vital[v] = Data.Vital[v];

                    // Envia os dados aos jogadores do mapa
                    Send.MapNpcVitals(this);
                }

        //////////////////
        // Movimentação //
        //////////////////
        // Atacar ao ver
        if (Data.Behaviour == Behaviour.AttackOnSight)
        {
            // Jogador
            if (Target == null)
                foreach (var player in Account.List)
                {
                    // Verifica se o jogador está jogando e no mesmo mapa que o Npc
                    if (!player.IsPlaying) continue;
                    if (player.Character.Map != Map) continue;

                    // Se o jogador estiver no alcance do Npc, ir atrás dele
                    distance = (short)Math.Sqrt(Math.Pow(X - player.Character.X, 2) +
                                                Math.Pow(Y - player.Character.Y, 2));
                    if (distance <= Data.Sight)
                    {
                        Target = player.Character;

                        // Mensagem
                        if (!string.IsNullOrEmpty(Data.SayMsg))
                            Send.Message(player.Character, Data.Name + ": " + Data.SayMsg,
                                Color.White);
                        break;
                    }
                }

            // Npc
            if (Data.AttackNpc && Target == null)
                for (byte npcIndex = 0; npcIndex < Map.Npc.Length; npcIndex++)
                {
                    // Verifica se pode atacar
                    if (npcIndex == Index) continue;
                    if (!Map.Npc[npcIndex].Alive) continue;
                    if (Data.IsAllied(Map.Npc[npcIndex].Data)) continue;

                    // Se o Npc estiver no alcance do Npc, ir atrás dele
                    distance = (short)Math.Sqrt(Math.Pow(X - Map.Npc[npcIndex].X, 2) +
                                                Math.Pow(Y - Map.Npc[npcIndex].Y, 2));
                    if (distance <= Data.Sight)
                    {
                        Target = Map.Npc[npcIndex];
                        break;
                    }
                }
        }

        // Verifica se o alvo ainda está disponível
        if (Target != null)
            if ((Target is Player player && !player.Account.IsPlaying) || Target.Map != Map)
                Target = null;
            else if (Target is TempNpc {Alive: false})
                Target = null;

        // Evita que ele se movimente sem sentido
        if (Target != null)
        {
            targetX = Target.X;
            targetY = Target.Y;

            // Verifica se o alvo saiu do alcance do Npc
            if (Data.Sight < Math.Sqrt(Math.Pow(X - targetX, 2) + Math.Pow(Y - targetY, 2)))
                Target = null;
            else
                move = true;
        }
        else
        {
            // Define o alvo a zona do Npc
            if (Map.Data.Npc[Index].Zone > 0)
                if (Map.Data.Attribute[X, Y].Zone != Map.Data.Npc[Index].Zone)
                    for (byte x2 = 0; x2 < CryBits.Entities.Map.Width; x2++)
                    for (byte y2 = 0; y2 < CryBits.Entities.Map.Height; y2++)
                        if (Map.Data.Attribute[x2, y2].Zone == Map.Data.Npc[Index].Zone)
                            if (!Map.Data.TileBlocked(x2, y2))
                            {
                                targetX = x2;
                                targetY = y2;
                                move = true;
                                break;
                            }
        }

        // Movimenta o Npc
        if (move)
        {
            // Verifica como o Npc pode se mover
            if (Vital[(byte)Enums.Vital.HP] > Data.Vital[(byte)Enums.Vital.HP] * (Data.FleeHealth / 100.0))
            {
                // Para perto do alvo
                canMove[(byte)Direction.Up] = Y > targetY;
                canMove[(byte)Direction.Down] = Y < targetY;
                canMove[(byte)Direction.Left] = X > targetX;
                canMove[(byte)Direction.Right] = X < targetX;
            }
            else
            {
                // Para longe do alvo
                canMove[(byte)Direction.Up] = Y < targetY;
                canMove[(byte)Direction.Down] = Y > targetY;
                canMove[(byte)Direction.Left] = X < targetX;
                canMove[(byte)Direction.Right] = X > targetX;
            }

            // Aleatoriza a forma que ele vai se movimentar até o alvo
            if (MyRandom.Next(0, 2) == 0)
            {
                for (byte d = 0; d < (byte)Direction.Count; d++)
                    if (!moved && canMove[d] && Move((Direction)d))
                        moved = true;
            }
            else
                for (short d = (byte)Direction.Count - 1; d >= 0; d--)
                    if (!moved && canMove[d] && Move((Direction)d))
                        moved = true;
        }

        // Move-se aleatoriamente
        if (Data.Behaviour == (byte)Behaviour.Friendly || Target == null)
            if (MyRandom.Next(0, 3) == 0 && !moved)
                if (Data.Movement == MovementStyle.MoveRandomly)
                    Move((Direction)MyRandom.Next(0, 4), 1, true);
                else if (Data.Movement == MovementStyle.TurnRandomly)
                {
                    Direction = (Direction)MyRandom.Next(0, 4);
                    Send.MapNpcDirection(this);
                }

        ////////////
        // Ataque //
        ////////////
        Attack();
    }

    private void Spawn(byte x, byte y, Direction direction = 0)
    {
        // Faz o Npc surgir no mapa
        Alive = true;
        X = x;
        Y = y;
        Direction = direction;
        for (byte i = 0; i < (byte)Enums.Vital.Count; i++) Vital[i] = Data.Vital[i];

        // Envia os dados aos jogadores
        if (Socket.Device != null) Send.MapNpc(Map.Npc[Index]);
    }

    public void Spawn()
    {
        // Antes verifica se tem algum local de aparecimento específico
        if (Map.Data.Npc[Index].Spawn)
        {
            Spawn(Map.Data.Npc[Index].X, Map.Data.Npc[Index].Y);
            return;
        }

        // Faz com que ele apareça em um local aleatório
        for (byte i = 0; i < 50; i++) // tenta 50 vezes com que ele apareça em um local aleatório
        {
            var x = (byte)MyRandom.Next(0, CryBits.Entities.Map.Width - 1);
            var y = (byte)MyRandom.Next(0, CryBits.Entities.Map.Height - 1);

            // Verifica se está dentro da zona
            if (Map.Data.Npc[Index].Zone > 0)
                if (Map.Data.Attribute[x, y].Zone != Map.Data.Npc[Index].Zone)
                    continue;

            // Define os dados
            if (!Map.Data.TileBlocked(x, y))
            {
                Spawn(x, y);
                return;
            }
        }

        // Em último caso, tentar no primeiro lugar possível
        for (byte x2 = 0; x2 < CryBits.Entities.Map.Width; x2++)
        for (byte y2 = 0; y2 < CryBits.Entities.Map.Height; y2++)
            if (!Map.Data.TileBlocked(x2, y2))
            {
                // Verifica se está dentro da zona
                if (Map.Data.Npc[Index].Zone > 0)
                    if (Map.Data.Attribute[x2, y2].Zone != Map.Data.Npc[Index].Zone)
                        continue;

                // Define os dados
                Spawn(x2, y2);
                return;
            }
    }

    private bool Move(Direction direction, byte movement = 1, bool checkZone = false)
    {
        byte nextX = X, nextY = Y;

        // Define a direção do Npc
        Direction = direction;
        Send.MapNpcDirection(this);

        // Próximo azulejo
        NextTile(direction, ref nextX, ref nextY);

        // Próximo azulejo bloqueado ou fora do limite
        if (CryBits.Entities.Map.OutLimit(nextX, nextY)) return false;
        if (Map.TileBlocked(X, Y, direction)) return false;

        // Verifica se está dentro da zona
        if (checkZone)
            if (Map.Data.Attribute[nextX, nextY].Zone != Map.Data.Npc[Index].Zone)
                return false;

        // Movimenta o Npc
        X = nextX;
        Y = nextY;
        Send.MapNpcMovement(this, movement);
        return true;
    }

    private void Attack()
    {
        byte nextX = X, nextY = Y;
        NextTile(Direction, ref nextX, ref nextY);

        // Apenas se necessário
        if (!Alive) return;
        if (Environment.TickCount < _attackTimer + AttackSpeed) return;
        if (Map.TileBlocked(X, Y, Direction, false)) return;

        // Verifica se o jogador está na frente do Npc
        if (Target is Player)
            AttackPlayer(Map.HasPlayer(nextX, nextY));
        // Verifica se o Npc alvo está na frente do Npc
        else if (Target is TempNpc)
            AttackNpc(Map.HasNpc(nextX, nextY));
    }

    private void AttackPlayer(Player victim)
    {
        // Verifica se a vítima pode ser atacada
        if (victim == null) return;
        if (victim.GettingMap) return;

        // Tempo de ataque 
        _attackTimer = Environment.TickCount;

        // Cálculo de dano
        var attackDamage = (short)(Data.Attribute[(byte)Attribute.Strength] - victim.PlayerDefense);

        // Dano não fatal
        if (attackDamage > 0)
        {
            // Demonstra o ataque aos outros jogadores
            Send.MapNpcAttack(this, victim.Name, Enums.Target.Player);

            if (attackDamage < victim.Vital[(byte)Enums.Vital.HP])
            {
                victim.Vital[(byte)Enums.Vital.HP] -= attackDamage;
                Send.PlayerVitals(victim);
            }
            // FATALITY
            else
            {
                // Reseta o alvo do Npc
                Target = null;

                // Mata o jogador
                victim.Died();
            }
        }
        // Demonstra o ataque aos outros jogadores
        else
            Send.MapNpcAttack(this);
    }

    private void AttackNpc(TempNpc victim)
    {
        // Verifica se a vítima pode ser atacada
        if (victim == null) return;
        if (!victim.Alive) return;

        // Tempo de ataque 
        _attackTimer = Environment.TickCount;

        // Define o alvo do Npc
        victim.Target = this;

        // Cálculo de dano
        var attackDamage = (short)(Data.Attribute[(byte)Attribute.Strength] - victim.Data.Attribute[(byte)Attribute.Resistance]);

        // Dano não fatal
        if (attackDamage > 0)
        {
            // Demonstra o ataque aos outros jogadores
            Send.MapNpcAttack(this, victim.Index.ToString(), Enums.Target.Npc);

            if (attackDamage < victim.Vital[(byte)Enums.Vital.HP])
            {
                victim.Vital[(byte)Enums.Vital.HP] -= attackDamage;
                Send.MapNpcVitals(victim);
            }
            // FATALITY
            else
            {
                // Reseta o alvo do Npc
                Target = null;

                // Mata o Npc
                victim.Died();
            }
        }
        // Demonstra o ataque aos outros jogadores
        else
            Send.MapNpcAttack(this);
    }

    public void Died()
    {
        // Solta os itens
        for (byte i = 0; i < Data.Drop.Count; i++)
            if (Data.Drop[i].Item != null)
                if (MyRandom.Next(1, 99) <= Data.Drop[i].Chance)
                    // Solta o item
                    Map.Item.Add(new MapItems(Data.Drop[i].Item, Data.Drop[i].Amount, X, Y));

        // Envia os dados dos itens no chão para o mapa
        Send.MapItems(Map);

        // Reseta os dados do Npc 
        _spawnTimer = Environment.TickCount;
        Target = null;
        Alive = false;
        Send.MapNpcDied(this);
    }
}