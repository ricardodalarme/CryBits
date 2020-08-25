using Lidgren.Network;
using Entities;

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
                Port = Logic.Utils.Port,
                AcceptIncomingConnections = true,
                MaximumConnections = Logic.Utils.Max_Players
            };

            // Cria o dispositivo com as devidas configurações
            Device = new NetServer(Config);
            Device.Start();
        }

        public static void HandleData()
        {
            NetIncomingMessage Data;
            Account Account;

            // Lê e direciona todos os dados recebidos
            while ((Data = Device.ReadMessage()) != null)
            {
                // Jogador que está enviando os dados
                Account = Account.List.Find(x => x.Connection == Data.SenderConnection);

                switch (Data.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus Status = (NetConnectionStatus)Data.ReadByte();

                        // Nova conexão - Conecta o jogador ao jogo
                        if (Status == NetConnectionStatus.Connected)
                            Account.List.Add(new Account(Data.SenderConnection));
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
    }
}