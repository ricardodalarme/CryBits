using Lidgren.Network;
using System;
using System.Windows.Forms;

namespace Network
{
    static class Socket
    {
        // Protocolo do controle de transmissão
        public static NetClient Device;

        // Manuseamento dos dados
        private static NetIncomingMessage Data;

        // Dados para a conexão com o servidor
        public const string IP = "localhost";
        public const short Port = 7001;

        // Latência
        public static int Latency;
        public static int Latency_Send;

        public static void Init()
        {
            NetPeerConfiguration Config = new NetPeerConfiguration("CryBits");

            // Cria o dispositivo com as devidas configurações
            Device = new NetClient(Config);
            Device.Start();
        }

        public static void Disconnect()
        {
            // Acaba com a conexão
            if (Device != null)
                Device.Disconnect(string.Empty);
        }

        public static void HandleData()
        {
            // Lê e direciona todos os dados recebidos
            while ((Data = Device.ReadMessage()) != null)
            {
                switch (Data.MessageType)
                {
                    // Recebe e manuseia os dados
                    case NetIncomingMessageType.Data:
                        Receive.Handle(Data);
                        break;
                    // Desconectar o jogador caso o servidor seja desligado
                    case NetIncomingMessageType.StatusChanged:
                        if ((NetConnectionStatus)Data.ReadByte() == NetConnectionStatus.Disconnected)
                        {
                            // Apaga os dados e volta ao menu
                            if (Player.Me != null) Player.Me.Leave();
                            Window.OpenMenu();
                        }
                        break;
                }

                Device.Recycle(Data);
            }
        }

        // Retorna um valor de acordo com o estado da conexão do jogador
        public static bool IsConnected() => Device.ConnectionStatus == NetConnectionStatus.Connected;

        public static bool TryConnect()
        {
            // Se o jogador já estiver conectado, então isso não é mais necessário
            if (IsConnected()) return true;

            // Tenta se conectar
            Device.Connect(IP, Port);

            // Espere até que o jogador se conecte
            int Wait_Timer = Environment.TickCount;
            while (!IsConnected() && Environment.TickCount <= Wait_Timer + 1000)
                HandleData();

            // Retorna uma mensagem caso não conseguir se conectar
            if (!IsConnected())
            {
                MessageBox.Show("The server is currently unavailable.");
                return false;
            }

            return true;
        }
    }
}