﻿using System;
using System.Windows.Forms;
using CryBits.Client.UI;
using Lidgren.Network;

namespace CryBits.Client.Network
{
    static class Socket
    {
        // Protocolo do controle de transmissão
        public static NetClient Device;

        // Manuseamento dos dados
        private static NetIncomingMessage _data;

        // Dados para a conexão com o servidor
        public const string Ip = "localhost";
        public const short Port = 7001;

        // Latência
        public static int Latency;
        public static int Latency_Send;

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
            Device?.Disconnect(string.Empty);
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
                        {
                            // Apaga os dados e volta ao menu
                            if (Entities.Player.Me != null) Entities.Player.Me.Leave();
                            Windows.OpenMenu();
                        }
                        break;
                }

                Device.Recycle(_data);
            }
        }

        // Retorna um valor de acordo com o estado da conexão do jogador
        public static bool IsConnected() => Device.ConnectionStatus == NetConnectionStatus.Connected;

        public static bool TryConnect()
        {
            // Se o jogador já estiver conectado, então isso não é mais necessário
            if (IsConnected()) return true;

            // Tenta se conectar
            Device.Connect(Ip, Port);

            // Espere até que o jogador se conecte
            int waitTimer = Environment.TickCount;
            while (!IsConnected() && Environment.TickCount <= waitTimer + 1000)
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