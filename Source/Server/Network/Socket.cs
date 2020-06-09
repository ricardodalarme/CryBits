using Lidgren.Network;

namespace Network
{
    static class Socket
    {
        // Protocolo do controle de transmissão
        public static NetServer Device;

        public static void Init()
        {
            NetPeerConfiguration Config;

            // Define algumas configurações da rede
            Config = new NetPeerConfiguration("CryBits")
            {
                Port = Utils.Port,
                AcceptIncomingConnections = true,
                MaximumConnections = Utils.Max_Players
            };

            // Cria o dispositivo com as devidas configurações
            Device = new NetServer(Config);
            Device.Start();
        }

        public static void HandleData()
        {
            NetIncomingMessage Data;
            Objects.Account Account;

            // Lê e direciona todos os dados recebidos
            while ((Data = Device.ReadMessage()) != null)
            {
                // Jogador que está a enviar os dados
                Account = FindConnection(Data.SenderConnection);

                switch (Data.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus Status = (NetConnectionStatus)Data.ReadByte();

                        // Nova conexão - Conecta o jogador ao jogo
                        if (Status == NetConnectionStatus.Connected)
                            Lists.Account.Add(new Objects.Account(Data.SenderConnection));
                        // Conexão perdida, disconecta o jogador do jogo
                        else if (Status == NetConnectionStatus.Disconnected)
                            Account.Leave();

                        break;
                    // Recebe e manuseia os dados recebidos
                    case NetIncomingMessageType.Data:
                        Receive.Handle(Account, Data);
                        break;
                }

                Device.Recycle(Data);
            }
        }

        private static Objects.Account FindConnection(NetConnection Data)
        {
            // Encontra uma determinada conexão
            for (byte i = 0; i < Lists.Account.Count; i++)
                if (Lists.Account[i].Connection == Data)
                    return Lists.Account[i];

            return null;
        }
    }
}