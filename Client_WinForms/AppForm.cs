using IO_TCPClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppForm
{
    public partial class AppForm : Form
    {
        SimpleTCPClient client;

        public AppForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Create client object with default port
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginForm_Load(object sender, EventArgs e)
        {
            Random r = new Random();
            int localPort = r.Next() % 55535 + 10000;
            client = new SimpleTCPClient((ushort)localPort);
        }

        private void buttonSignIn_Click(object sender, EventArgs e)
        {
            labLoginError.Visible = false;
            if (txtLoginLogin.Text.Length == 0 || txtLoginPass.Text == null)
            {
                labLoginError.Text = "No login provided!";
                labLoginError.Visible = true;
                return;
            }
            if (txtLoginPass.Text.Length == 0 || txtLoginPass.Text == null)
            {
                labLoginError.Text = "No password provided!";
                labLoginError.Visible = true;
                return;
            }
            //TODO: opcjonalna wstępna walidacja po stronie klienta (długość loginu/hasła), jeśli tak to serwer potrzebuje to uwzględnić przy rejestracji
            //...
            //TODO: request o login do serwera
            if (client.Connect("127.0.0.1", 8010))
            {
                //TODO: send login request
                if(!client.RequestSignIn(txtLoginLogin.Text, txtLoginPass.Text))
                {
                    labLoginError.Text = "Unexpected error while logging in.";
                    labLoginError.Visible = true;
                    return;
                }
            }
            else
            {
                labLoginError.Text = "Failed to connect to the server!";
                labLoginError.Visible = true;
                return;
            }
            string jsonResponse = Helper.ReadIntoJson(client.tcpClient);
            Tuple<string, bool> response = client.ProcessSignInResponse(jsonResponse);

            if (response.Item2) panelChat.Visible = true;
            else
            {
                labLoginError.Text = response.Item1;
                labLoginError.Visible = true;
            }
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            panelRegister.Visible = true;
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            //TODO
            chat.Text += chatInput.Text;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            chat.Text = "";
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            panelRegister.Visible = false;
        }

        private void buttonSignUp_Click(object sender, EventArgs e)
        {
            labRegError.Visible = false;
            labRegError.ForeColor = Color.Red;
            if (txtRegLogin.Text.Length == 0 || txtRegPass.Text == null)
            {
                labRegError.Text = "No login provided!";
                labRegError.Visible = true;
                return;
            }
            if (txtRegPass.Text.Length == 0 || txtRegPass.Text == null)
            {
                labRegError.Text = "No password provided!";
                labRegError.Visible = true;
                return;
            }
            if (txtRegConfirm.Text.Length == 0 || txtRegConfirm.Text == null)
            {
                labRegError.Text = "Please confirm your password!";
                labRegError.Visible = true;
                return;
            }
            //TODO: opcjonalna wstępna walidacja po stronie klienta (długość loginu/hasła), jeśli tak to serwer potrzebuje to uwzględnić przy rejestracji
            if (txtRegPass.Text != txtRegConfirm.Text)
            {
                labRegError.Text = "Passwords don't match!";
                labRegError.Visible = true;
                return;
            }
            //TODO: request o rejestrację do serwera
            if (client.Connect("127.0.0.1", 8010))
            {
                //send login request
                if (!client.RequestSignUp(txtRegLogin.Text, txtRegPass.Text))
                {
                    labRegError.Text = "Unexpected error while logging in.";
                    labRegError.Visible = true;
                    return;
                }
            }
            else
            {
                labRegError.Text = "Failed to connect to the server!";
                labRegError.Visible = true;
                return;
            }

            string jsonResponse = Helper.ReadIntoJson(client.tcpClient);
            Tuple<string, bool> response = client.ProcessSignUpResponse(jsonResponse);
            if (response.Item2)
            {
                labRegError.ForeColor = Color.Green;
                labRegError.Text = "You have been signed up";
                labRegError.Visible = true;
            }
            else
            {
                labRegError.Text = response.Item1;
                labRegError.Visible = true;
            }
        }
    }
}
