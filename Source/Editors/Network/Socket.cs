using CryBits.Editors.Forms;
using Lidgren.Network;
using System;
using System.Windows.Forms;

namespace CryBits.Editors.Network
{
    class Socket
    {
        // Protocolo do controle de transmissão
        public static NetClient Device;

        // Manuseamento dos dados
        private static NetIncomingMessage _data;

        // Dados para a conexão com o servidor
        public const string Ip = "localhost";
        public const short Port = 7001;

        public static void Init()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("CryBits");

            // Cria o dispositivo com as devidas configurações
            Device = new NetClient(config);
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
            while ((_data = Device.ReadMessage()) != null)
            {
                switch (_data.MessageType)
                {
                    // Recebe e manuseia os dados
                    case NetIncomingMessageType.Data:
                        Receive.Handle(_data);
                        break;
                    // Desconectar o jogador caso o servidor seja desligado
                    case NetIncomingMessageType.StatusChanged:
                        if ((NetConnectionStatus)_data.ReadByte() == NetConnectionStatus.Disconnected)
                            Leave();

                        break;
                }

                Device.Recycle(_data);
            }
        }

        // Retorna um valor de acordo com o estado da conexão do jogador
        public static bool IsConnected() => Device.ConnectionStatus == NetConnectionStatus.Connected;

        public static bool TryConnect()
        {
            int waitTimer = Environment.TickCount;

            // Se o jogador já estiver conectado, então isso não é mais necessário
            if (IsConnected()) return true;

            // Tenta se conectar
            Device.Connect(Ip, Port);

            // Espere até que o jogador se conecte
            while (!IsConnected() && Environment.TickCount <= waitTimer + 1000)
                HandleData();

            return IsConnected();
        }

        private static void Leave()
        {
            // Fecha todas as janelas abertar e abre o menu de login
            for (int i = 0; i < Application.OpenForms.Count; i++)
                if (Application.OpenForms[i] != Login.Form)
                    Application.OpenForms[i].Close();
            Login.Form.Visible = true;
        }
    }
}