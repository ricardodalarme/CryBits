using System;
using System.Collections.Generic;
using System.Drawing;
using CryBits.Client.Library;
using CryBits.Client.Logic;
using CryBits;
using CryBits.Client.Media.Audio;
using static CryBits.Client.Logic.Game;
using static CryBits.Client.Logic.Utils;
using Graphics = CryBits.Client.Media.Graphics;

namespace CryBits.Client.UI
{
    internal class CheckBoxes : Tools.Structure
    {
        // Armazenamento dos dados da ferramenta
        public static Dictionary<string, CheckBoxes> List = new Dictionary<string, CheckBoxes>();

        // Margem da textura até o texto
        public const byte Margin = 4;

        // Dados
        public string Text;
        public bool Checked;

        // Eventos
        public void MouseUp()
        {
            // Tamanho do marcador
            Size textureSize = Graphics.Size(Graphics.Tex_CheckBox);
            Size box = new Size(textureSize.Width / 2 + MeasureString(Text) + Margin, textureSize.Height);

            // Somente se estiver sobrepondo a ferramenta
            if (!IsAbove(new Rectangle(Position, box))) return;

            // Altera o estado do marcador
            Checked = !Checked;

            // Executa o evento
            Execute(Name);
            Sound.Play(CryBits.Sounds.Click);
        }

        private static void Execute(string name)
        {
            // Executa o evento do marcador
            switch (name)
            {
                case "Sounds": Sounds(); break;
                case "Musics": Musics(); break;
                case "Connect_Save_Username": SaveUsername(); break;
                case "GenderMale": GenreName(); break;
                case "GenderFemale": GenreFemale(); break;
                case "Options_Sounds": Sounds(); break;
                case "Options_Musics": Musics(); break;
                case "Options_Chat": Chat(); break;
                case "Options_FPS": FPS(); break;
                case "Options_Latency": Latency(); break;
                case "Options_Party": Party(); break;
                case "Options_Trade": Trade(); break;
            }
        }

        private static void Sounds()
        {
            // Salva os dados
            Option.Sounds = !Option.Sounds;
            if (!Option.Sounds) Sound.Stop_All();
            Write.Options();
        }

        private static void Musics()
        {
            // Salva os dados
            Option.Musics = !Option.Musics;
            Write.Options();

            // Para ou reproduz a música dependendo do estado do marcador
            if (!Option.Musics)
                Music.Stop();
            else if (Windows.Current == WindowsTypes.Menu)
                Music.Play(CryBits.Musics.Menu);
            else if (Windows.Current == WindowsTypes.Game)
                Music.Play((Musics)Mapper.Current.Data.Music);
        }

        private static void SaveUsername()
        {
            // Salva os dados
            Option.SaveUsername = List["Connect_Save_Username"].Checked;
            Write.Options();
        }

        private static void GenreName()
        {
            // Altera o estado do marcador de outro gênero
            List["GenderFemale"].Checked = !List["GenderMale"].Checked;
            Panels.CreateCharacter_Tex = 0;
        }

        private static void GenreFemale()
        {
            // Altera o estado do marcador de outro gênero
            List["GenderMale"].Checked = !List["GenderFemale"].Checked;
            Panels.CreateCharacter_Tex = 0;
        }

        private static void Chat()
        {
            // Salva os dado
            Option.Chat = List["Options_Chat"].Checked;
            Write.Options();
            if (Option.Chat) Loop.Chat_Timer = Environment.TickCount + UI.Chat.SleepTimer;
        }

        private static void FPS()
        {
            // Salva os dado
            Option.FPS = List["Options_FPS"].Checked;
            Write.Options();
        }

        private static void Latency()
        {
            // Desabilita a prévia do chat
            Option.Latency = List["Options_Latency"].Checked;
            Write.Options();
        }

        private static void Party()
        {
            // Salva os dado
            Option.Party = List["Options_Party"].Checked;
            Write.Options();
        }

        private static void Trade()
        {
            // Salva os dado
            Option.Trade = List["Options_Trade"].Checked;
            Write.Options();
        }
    }
}