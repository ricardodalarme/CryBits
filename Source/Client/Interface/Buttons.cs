﻿using System.Drawing;
using System.Windows.Forms;

public class Buttons
{
    // Aramazenamento de dados da ferramenta
    public static Structure[] List = new Structure[1];

    // Estrutura das ferramentas
    public class Structure
    {
        public byte Texture;
        public States State;
        public Tools.Structure General;
    }

    // Estados dos botões
    public enum States
    {
        Normal,
        Click,
        Above,
    }

    public static byte FindIndex(string Name)
    {
        // Lista os nomes das ferramentas
        for (byte i = 1; i <= List.GetUpperBound(0); i++)
            if (List[i].General.Name == Name)
                return i;

        return 0;
    }

    public static Structure Find(string Name)
    {
        // Lista os nomes das ferramentas
        for (byte i = 1; i <= List.GetUpperBound(0); i++)
            if (List[i].General.Name == Name)
                return List[i];

        return null;
    }

    public class Events
    {
        public static void MouseUp(MouseEventArgs e, byte Index)
        {
            SFML.Graphics.Texture Texture = Graphics.Tex_Button[List[Index].Texture];

            // Somente se necessário
            if (!List[Index].General.Able) return;
            if (!Tools.IsAbove(new Rectangle(List[Index].General.Position, Graphics.TSize(Texture)))) return;

            // Altera o estado do botão
            Audio.Sound.Play(Audio.Sounds.Click);
            List[Index].State = States.Above;

            // Executa o evento
            Execute(List[Index].General.Name);
        }

        public static void MouseDown(MouseEventArgs e, byte Index)
        {
            SFML.Graphics.Texture Texture = Graphics.Tex_Button[List[Index].Texture];

            // Somente se necessário
            if (e.Button == MouseButtons.Right) return;
            if (!List[Index].General.Able) return;

            // Se o mouse não estiver sobre a ferramenta, então não executar o evento
            if (!Tools.IsAbove(new Rectangle(List[Index].General.Position, Graphics.TSize(Texture))))
                return;

            // Altera o estado do botão
            List[Index].State = States.Click;
        }

        public static void MouseMove(MouseEventArgs e, byte i)
        {
            SFML.Graphics.Texture Texture = Graphics.Tex_Button[List[i].Texture];

            // Somente se necessário
            if (e.Button == MouseButtons.Right) return;
            if (!List[i].General.Able) return;

            // Se o mouse não estiver sobre a ferramenta, então não executar o evento
            if (!Tools.IsAbove(new Rectangle(List[i].General.Position, Graphics.TSize(Texture))))
            {
                List[i].State = States.Normal;
                return;
            }

            // Se o botão já estiver no estado normal, isso não é necessário
            if (List[i].State != States.Normal)
                return;

            // Altera o estado do botão
            List[i].State = States.Above;
            Audio.Sound.Play(Audio.Sounds.Above);
        }

        public static void Execute(string Name)
        {
            // Executa o evento do botão
            switch (Name)
            {
                case "Conectar": Connect(); break;
                case "Registrar": Register(); break;
                case "Opções": Options(); break;
                case "Opções_Retornar": Menu_Return(); break;
                case "Conectar_Pronto": Connect_Ok(); break;
                case "Registrar_Pronto": Register_Ok(); break;
                case "CriarPersonagem": CreateCharacter(); break;
                case "CriarPersonagem_TrocarDireita": CreateCharacter_Change_Right(); break;
                case "CriarPersonagem_TrocarEsquerda": CreateCharacter_Change_Left(); break;
                case "CriarPersonagem_Retornar": CreateCharacter_Return(); break;
                case "Personagem_Usar": Character_Use(); break;
                case "Personagem_Criar": Character_Create(); break;
                case "Personagem_Deletar": Character_Delete(); break;
                case "Personagem_TrocarDireita": Character_Change_Right(); break;
                case "Personagem_TrocarEsquerda": Character_Change_Left(); break;
                case "Chat_Subir": Chat_Up(); break;
                case "Chat_Descer": Chat_Down(); break;
                case "Menu_Personagem": Menu_Character(); break;
                case "Atributos_Força": Attribute_Strenght(); break;
                case "Atributos_Resistência": Attribute_Resistance(); break;
                case "Atributos_Inteligência": Attribute_Intelligence(); break;
                case "Atributos_Agilidade": Attribute_Agility(); break;
                case "Atributos_Vitalidade": Attribute_Vitality(); break;
                case "Menu_Inventário": Menu_Inventory(); break;
            }
        }

        public static void Characters_Change_Buttons()
        {
            bool Visibility = false;

            // Verifica apenas se o painel for visível
            if (!Panels.Find("SelecionarPersonagem").General.Visible)
                return;

            if (Lists.Characters[Game.SelectCharacter].Class != 0)
                Visibility = true;

            // Altera os botões visíveis
            Find("Personagem_Criar").General.Visible = !Visibility;
            Find("Personagem_Deletar").General.Visible = Visibility;
            Find("Personagem_Usar").General.Visible = Visibility;
        }

        public static void Connect()
        {
            // Termina a conexão
            Socket.Disconnect();

            // Abre o painel
            Panels.Menu_Close();
            Panels.Find("Conectar").General.Visible = true;
        }

        public static void Register()
        {
            // Termina a conexão
            Socket.Disconnect();

            // Abre o painel
            Panels.Menu_Close();
            Panels.Find("Registrar").General.Visible = true;
        }

        public static void Options()
        {
            // Termina a conexão
            Socket.Disconnect();

            // Abre o painel
            Panels.Menu_Close();
            Panels.Find("Opções").General.Visible = true;
        }

        public static void Menu_Return()
        {
            // Termina a conexão
            Socket.Disconnect();

            // Abre o painel
            Panels.Menu_Close();
            Panels.Find("Conectar").General.Visible = true;
        }

        public static void Connect_Ok()
        {
            // Salva o nome do usuário
            Lists.Options.Username = TextBoxes.Find("Conectar_Usuário").Text;
            Write.Options();

            // Conecta-se ao jogo
            Game.SetSituation(Game.Situations.Connect);
        }

        public static void Register_Ok()
        {
            // Regras de segurança
            if (TextBoxes.Find("Registrar_Senha").Text != TextBoxes.Find("Registrar_RepetirSenha").Text)
            {
                MessageBox.Show("As senhas digitadas não são iquais.");
                return;
            }

            // Registra o jogador, se estiver tudo certo
            Game.SetSituation(Game.Situations.Registrar);
        }

        public static void CreateCharacter()
        {
            // Abre a criação de personagem
            Game.SetSituation(Game.Situations.CreateCharacter);
        }

        public static void CreateCharacter_Change_Right()
        {
            // Altera a classe selecionada pelo jogador
            if (Game.CreateCharacter_Class == Lists.Class.GetUpperBound(0))
                Game.CreateCharacter_Class = 1;
            else
                Game.CreateCharacter_Class += 1;
        }

        public static void CreateCharacter_Change_Left()
        {
            // Altera a classe selecionada pelo jogador
            if (Game.CreateCharacter_Class == 1)
                Game.CreateCharacter_Class = (byte)Lists.Class.GetUpperBound(0);
            else
                Game.CreateCharacter_Class -= 1;
        }

        public static void CreateCharacter_Return()
        {
            // Abre o painel de personagens
            Panels.Menu_Close();
            Panels.Find("SelecionarPersonagem").General.Visible = true;
        }

        public static void Character_Use()
        {
            // Usa o personagem selecionado
            Send.Character_Use();
        }

        public static void Character_Delete()
        {
            // Deleta o personagem selecionado
            Send.Character_Delete();
        }

        public static void Character_Create()
        {
            // Abre a criação de personagem
            Send.Character_Create();
        }

        public static void Character_Change_Right()
        {
            // Altera o personagem selecionado pelo jogador
            if (Game.SelectCharacter == Lists.Server_Data.Max_Characters)
                Game.SelectCharacter = 1;
            else
                Game.SelectCharacter += 1;
        }

        public static void Character_Change_Left()
        {
            // Altera o personagem selecionado pelo jogador
            if (Game.SelectCharacter == 1)
                Game.SelectCharacter = Lists.Server_Data.Max_Characters;
            else
                Game.SelectCharacter -= 1;
        }

        public static void Chat_Up()
        {
            // Sobe as linhas do chat
            if (Tools.Chat_Line > 0)
                Tools.Chat_Line -= 1;
        }

        public static void Chat_Down()
        {
            // Sobe as linhas do chat
            if (Tools.Chat.Count - 1 - Tools.Chat_Line - Tools.Chat_Lines_Visible > 0)
                Tools.Chat_Line += 1;
        }

        public static void Menu_Character()
        {
            // Altera a visibilidade do painel e fecha os outros
            Panels.Find("Menu_Personagem").General.Visible = !Panels.Find("Menu_Personagem").General.Visible;
            Panels.Find("Menu_Inventário").General.Visible = false;
        }

        public static void Attribute_Strenght()
        {
            Send.AddPoint(Game.Atributos.Strength);
        }

        public static void Attribute_Resistance()
        {
            Send.AddPoint(Game.Atributos.Resistance);
        }

        public static void Attribute_Intelligence()
        {
            Send.AddPoint(Game.Atributos.Intelligence);
        }

        public static void Attribute_Agility()
        {
            Send.AddPoint(Game.Atributos.Agility);
        }

        public static void Attribute_Vitality()
        {
            Send.AddPoint(Game.Atributos.Vitality);
        }

        public static void Menu_Inventory()
        {
            // Altera a visibilidade do painel e fecha os outros
            Panels.Find("Menu_Inventário").General.Visible = !Panels.Find("Menu_Inventário").General.Visible;
            Panels.Find("Menu_Personagem").General.Visible = false;
        }
    }
}