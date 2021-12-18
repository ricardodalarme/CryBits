using CryBits.Server.Entities;
using Lidgren.Network;
using static CryBits.Globals;

namespace CryBits.Server.Network;

internal static class Socket
{
    // Protocolo do controle de transmissão
    public static NetServer Device;

    public static void Init()
    {
        // Define algumas configurações da rede
        NetPeerConfiguration config = new NetPeerConfiguration(GameName)
        {
            Port = Port,
            AcceptIncomingConnections = true,
            MaximumConnections = MaxPlayers
        };

        // Cria o dispositivo com as devidas configurações
        Device = new NetServer(config);
        Device.Start();
    }

    public static void HandleData()
    {
        NetIncomingMessage data;

        // Lê e direciona todos os dados recebidos
        while ((data = Device.ReadMessage()) != null)
        {
            // Jogador que está enviando os dados
            Account account = Account.List.Find(x => x.Connection == data.SenderConnection);

            switch (data.MessageType)
            {
                case NetIncomingMessageType.StatusChanged:
                    NetConnectionStatus status = (NetConnectionStatus)data.ReadByte();

                    // Nova conexão - Conecta o jogador ao jogo
                    if (status == NetConnectionStatus.Connected)
                        Account.List.Add(new Account(data.SenderConnection));
                    // Conexão perdida, disconecta o jogador do jogo
                    else if (status == NetConnectionStatus.Disconnected)
                        account?.Leave();

                    break;
                // Recebe e manuseia os dados recebidos
                case NetIncomingMessageType.Data:
                    Receive.Handle(account, data);
                    break;
            }

            Device.Recycle(data);
        }
    }
}