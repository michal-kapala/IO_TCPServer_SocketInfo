using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.Net.Sockets;

namespace AppForm
{
    public partial class AppForm : Form
    {
        TcpClient client;
        NetworkStream stream;
        byte[] request;
        byte[] response;
        int bytesRead;
        public AppForm()
        {

            InitializeComponent();
            //Klient narazie w tej klasie żeby działało a potem się pomyśli nad podziałem jakoś frontu i api
            client = new TcpClient("localhost", 8010);
            stream = client.GetStream();
            response = new byte[1024];
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
            // ^ chyba zrobione
            //...
            Debug.WriteLine("Server login");
            JsonMessage loginMessage = new JsonMessage("signin", username: txtLoginLogin.Text, password: txtLoginPass.Text);
            request = JsonSerializer.SerializeToUtf8Bytes(loginMessage);
            stream.Write(request, 0, request.Length);
            bytesRead = stream.Read(response, 0, response.Length);
            Debug.WriteLine(System.Text.Encoding.UTF8.GetString(response, 0, bytesRead));
            JsonMessage responseJson = JsonSerializer.Deserialize<JsonMessage>(System.Text.Encoding.UTF8.GetString(response, 0, bytesRead));
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
