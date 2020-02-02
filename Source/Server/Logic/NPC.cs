using System;

class NPC
{
    public enum Behaviour
    {
        Friendly,
        AttackOnSight,
        AttackWhenAttacked
    }

    public static short Regeneration(short Map_Num, byte Index, byte Vital)
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

            //////////////////
            // Aparecimento //
            //////////////////
            if (Data.Index == 0)
            {
                if (Environment.TickCount > Data.Spawn_Timer + (NPC_Data.SpawnTime * 1000)) Spawn(i, Map_Num);
            }
            else
            {
                byte TargetX = 0, TargetY = 0;
                bool[] CanMove = new bool[(byte)Game.Directions.Count];
                short Distance_X, Distance_Y;
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

                ///////////////
                // Movimento //
                ///////////////
                // Atacar ao ver
                if (Lists.Temp_Map[Map_Num].NPC[i].Target_Index == 0 && NPC_Data.Behaviour == (byte)Behaviour.AttackOnSight)
                    for (byte Player_Index = 1; Player_Index <= Game.HigherIndex; Player_Index++)
                    {
                        // Verifica se o jogador está jogando e no mesmo mapa que o NPC
                        if (!Player.IsPlaying(Player_Index)) continue;
                        if (Player.Character(Player_Index).Map != Map_Num) continue;

                        // Distância entre o NPC e o jogador
                        Distance_X = (short)(Data.X - Player.Character(Player_Index).X);
                        Distance_Y = (short)(Data.Y - Player.Character(Player_Index).Y);
                        if (Distance_X < 0) Distance_X *= -1;
                        if (Distance_Y < 0) Distance_Y *= -1;

                        // Se estiver no alcance, ir atrás do jogador
                        if (Distance_X <= NPC_Data.Sight && Distance_Y <= NPC_Data.Sight)
                        {
                            Lists.Temp_Map[Map_Num].NPC[i].Target_Type = (byte)Game.Target.Player;
                            Lists.Temp_Map[Map_Num].NPC[i].Target_Index = Player_Index;
                            Data = Lists.Temp_Map[Map_Num].NPC[i];

                            // Mensagem
                            Send.Message(Player_Index, Lists.NPC[Lists.Temp_Map[Map_Num].NPC[i].Index].Name + ": " + Lists.NPC[Lists.Temp_Map[Map_Num].NPC[i].Index].SayMsg, System.Drawing.Color.White);
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
                    TargetX = Player.Character(Lists.Temp_Map[Map_Num].NPC[i].Target_Index).X;
                    TargetY = Player.Character(Lists.Temp_Map[Map_Num].NPC[i].Target_Index).Y;
                }

                // Distância entre o NPC e o alvo
                Distance_X = (short)(Data.X - TargetX);
                Distance_Y = (short)(Data.Y - TargetY);
                if (Distance_X < 0) Distance_X *= -1;
                if (Distance_Y < 0) Distance_Y *= -1;

                // Verifica se o alvo saiu do alcance
                if (Distance_X > NPC_Data.Sight || Distance_Y > NPC_Data.Sight)
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
                    // Define o alvo até a zona do NPC
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

                // Movimenta o NPC até mais perto do alvo
                if (Move)
                {
                    // Verifica como pode se mover até o alvo
                    if (Data.Y > TargetY) CanMove[(byte)Game.Directions.Up] = true;
                    if (Data.Y < TargetY) CanMove[(byte)Game.Directions.Down] = true;
                    if (Data.X > TargetX) CanMove[(byte)Game.Directions.Left] = true;
                    if (Data.X < TargetX) CanMove[(byte)Game.Directions.Right] = true;

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
                        NPC.Move(Map_Num, i, (Game.Directions)Game.Random.Next(0, 4), 1, true);
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
                if (!global::Map.Tile_Blocked(Map_Num, x2, y2))
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

    public static void Spawn(byte Index, short Map_Num, byte x, byte y, Game.Directions Direction = 0)
    {
        Lists.Structures.NPC Data = Lists.NPC[Lists.Map[Map_Num].NPC[Index].Index];

        // Define os dados
        Lists.Temp_Map[Map_Num].NPC[Index].Index = Lists.Map[Map_Num].NPC[Index].Index;
        Lists.Temp_Map[Map_Num].NPC[Index].X = x;
        Lists.Temp_Map[Map_Num].NPC[Index].Y = y;
        Lists.Temp_Map[Map_Num].NPC[Index].Direction = Direction;
        Lists.Temp_Map[Map_Num].NPC[Index].Vital = new short[(byte)Game.Vitals.Count];
        for (byte i = 0; i < (byte)Game.Vitals.Count; i++) Lists.Temp_Map[Map_Num].NPC[Index].Vital[i] = Data.Vital[i];

        // Envia os dados aos jogadores
        if (Socket.Device != null) Send.Map_NPC(Map_Num, Index);
    }

    public static bool Move(short Map_Num, byte Index, Game.Directions Direction, byte Movement = 1, bool CountZone = false)
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
        if (CountZone)
            if (Lists.Map[Map_Num].Tile[Next_X, Next_Y].Zone != Lists.Map[Map_Num].NPC[Index].Zone)
                return false;

        // Movimenta o NPC
        Lists.Temp_Map[Map_Num].NPC[Index].X = (byte)Next_X;
        Lists.Temp_Map[Map_Num].NPC[Index].Y = (byte)Next_Y;
        Send.Map_NPC_Movement(Map_Num, Index, Movement);
        return true;
    }

    public static void Attack_Player(short Map_Num, byte Index, byte Victim)
    {
        Lists.Structures.Map_NPCs Data = Lists.Temp_Map[Map_Num].NPC[Index];
        short x = Data.X, y = Data.Y;

        // Define o azujelo a frente do NPC
        Map.NextTile(Data.Direction, ref x, ref y);

        // Verifica se a vítima pode ser atacada
        if (Data.Index == 0) return;
        if (Environment.TickCount < Data.Attack_Timer + 750) return;
        if (!Player.IsPlaying(Victim)) return;
        if (Lists.TempPlayer[Victim].GettingMap) return;
        if (Map_Num != Player.Character(Victim).Map) return;
        if (Player.Character(Victim).X != x || Player.Character(Victim).Y != y) return;
        if (Map.Tile_Blocked(Map_Num, Data.X, Data.Y, Data.Direction, false)) return;

        // Tempo de ataque 
        Lists.Temp_Map[Map_Num].NPC[Index].Attack_Timer = Environment.TickCount;

        // Demonstra o ataque aos outros jogadores
        Send.Map_NPC_Attack(Map_Num, Index, Victim, (byte)Game.Target.Player);

        // Cálculo de dano
        short Damage = (short)(Lists.NPC[Data.Index].Attribute[(byte)Game.Attributes.Strength] - Player.Character(Victim).Player_Defense);

        // Dano não fatal
        if (Damage > 0)
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
}