using Lidgren.Network;

class Socket
{
    // Protocolo do controle de transmissão
    public static NetServer Device;

    // Lista dos jogadores conectados
    public static NetConnection[] Connection;

    public static void Init()
    {
        NetPeerConfiguration Config;
        Connection = new NetConnection[Lists.Server_Data.Max_Players + 1];

        // Define algumas configurações da rede
        Config = new NetPeerConfiguration("CryBits")
        {
            Port = Lists.Server_Data.Port,
            AcceptIncomingConnections = true,
            MaximumConnections = Lists.Server_Data.Max_Players
        };

        // Cria o dispositivo com as devidas configurações
        Device = new NetServer(Config);
        Device.Start();
    }

    public static void HandleData()
    {
        NetIncomingMessage Data;
        byte Index;

        // Lê e direciona todos os dados recebidos
        while ((Data = Device.ReadMessage()) != null)
        {
            // Jogador que está a enviar os dados
            Index = FindConnection(Data.SenderConnection);

            switch (Data.MessageType)
            {
                case NetIncomingMessageType.StatusChanged:
                    NetConnectionStatus Status = (NetConnectionStatus)Data.ReadByte();

                    // Nova conexão - Conecta o jogador ao jogo
                    if (Status == NetConnectionStatus.Connected)
                        Connect(Data);
                    // Conexão perdida, disconecta o jogador do jogo
                    else if (Status == NetConnectionStatus.Disconnected)
                        Disconnect(Index);

                    break;
                // Recebe e manuseia os dados recebidos
                case NetIncomingMessageType.Data:
                    Receive.Handle(Index, Data);
                    break;
            }

            Device.Recycle(Data);
        }
    }

    private static void Connect(NetIncomingMessage IncomingMsg)
    {
        // Define a conexão do jogador
        Connection[FindConnection(null)] = IncomingMsg.SenderConnection;

        // Redefine o índice máximo de jogadores
        Game.SetHigherIndex();
    }

    private static void Disconnect(byte Index)
    {
        // Redefine o maior índice dos jogadores
        Game.SetHigherIndex();

        // Acaba com a conexão e restabelece os dados do jogador
        Connection[Index] = null;
        Player.Leave(Index);
    }

    public static bool IsConnected(byte Index)
    {
        // Previne sobrecarga
        if (Connection[Index] == null) return false;

        // Retorna um valor de acordo com a conexão do jogador
        return Connection[Index].Status == NetConnectionStatus.Connected;
    }

    private static byte FindConnection(NetConnection Data)
    {
        // Encontra uma determinada conexão
        for (byte i = 1; i <= Lists.Server_Data.Max_Players; i++)
            if (Connection[i] == Data)
                return i;

        return 0;
    }
}