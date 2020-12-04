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
        public AppForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            //useless
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
            //TODO: request o login do serwera
            //...
            //TODO: if(zalogowany) panelChat.Visible = true;
            if (true) panelChat.Visible = true;
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
            //...
            //TODO: if(zarejestrowany)
            if(true)
            {
                labRegError.ForeColor = Color.Green;
                labRegError.Text = "You have been signed up";
                labRegError.Visible = true;
            }
            else
            {
                labRegError.Text = "Unexpected error occured, please try again";
                labRegError.Visible = true;
            }
        }
    }
}
