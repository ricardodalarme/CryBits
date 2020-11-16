using Lidgren.Network;
using System;
using System.Windows.Forms;

class Socket
{
    // Protocolo do controle de transmissão
    public static NetClient Device;

    // Manuseamento dos dados
    private static NetIncomingMessage Data;

    // Dados para a conexão com o servidor
    public const string IP = "localhost";
    public const short Port = 7001;

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
                        Leave();

                    break;
            }

            Device.Recycle(Data);
        }
    }

    public static bool IsConnected()
    {
        // Retorna um valor de acordo com o estado da conexão do jogador
        return Device.ConnectionStatus == NetConnectionStatus.Connected;
    }

    public static bool TryConnect()
    {
        int Wait_Timer = Environment.TickCount;

        // Se o jogador já estiver conectado, então isso não é mais necessário
        if (IsConnected()) return true;

        // Tenta se conectar
        Device.Connect(IP, Port);

        // Espere até que o jogador se conecte
        while (!IsConnected() && Environment.TickCount <= Wait_Timer + 1000)
            HandleData();

        return IsConnected();
    }

    private static void Leave()
    {
        // Fecha todas as janelas abertar e abre o menu de login
        for (int i = 0; i < Application.OpenForms.Count; i++) Application.OpenForms[i].Visible = false;
        Login.Objects.Visible = true;
    }
}