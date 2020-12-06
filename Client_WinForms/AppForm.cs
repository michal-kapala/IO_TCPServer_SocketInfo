using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Text.Json;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Linq.Expressions;

namespace AppForm
{
    public static class ControlHelper {
        private delegate void SetPropertyThreadSafeDelegate<TResult>(
                Control @this,
                Expression<Func<TResult>> property,
                TResult value);

        public static void SetPropertyThreadSafe<TResult>(
            this Control @this,
            Expression<Func<TResult>> property,
            TResult value)
        {
            var propertyInfo = (property.Body as MemberExpression).Member
                as PropertyInfo;

            if (propertyInfo == null ||
                !@this.GetType().IsSubclassOf(propertyInfo.ReflectedType) ||
                @this.GetType().GetProperty(
                    propertyInfo.Name,
                    propertyInfo.PropertyType) == null)
            {
                throw new ArgumentException("The lambda expression 'property' must reference a valid property on this Control.");
            }

            if (@this.InvokeRequired)
            {
                @this.Invoke(new SetPropertyThreadSafeDelegate<TResult>
                (SetPropertyThreadSafe),
                new object[] { @this, property, value });
            }
            else
            {
                @this.GetType().InvokeMember(
                    propertyInfo.Name,
                    BindingFlags.SetProperty,
                    null,
                    @this,
                    new object[] { value });
            }
        }
    }

    public class StateObject {  
        // Client socket.  
        public Socket workSocket = null;  
        // Size of receive buffer.  
        public const int BufferSize = 1024;  
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];  
        // Received data string.  
        public StringBuilder sb = new StringBuilder();

        public Control chat;
        
        public StateObject(Control control)
        {
            chat = control;
        }
    }  

    public partial class AppForm : Form
    {
        TcpClient client;
        NetworkStream stream;
        byte[] request;
        byte[] response;
        int bytesRead;
        string username;
        static DateTime timeStart;
        static StateObject chatState;

        private static ManualResetEvent receiveDone =
        new ManualResetEvent(false);
        private static string asyncResponse = String.Empty;

        public AppForm()
        {

            InitializeComponent();
            //Klient narazie w tej klasie żeby działało a potem się pomyśli nad podziałem jakoś frontu i api
            client = new TcpClient("localhost", 8010);
            stream = client.GetStream();
            response = new byte[1024];
            timeStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        //Metody do asynchronicznego odbierania wiadomosci i dopisywania ich do czatu
        //Ratunku...

        private static void Receive(Socket client, Control control)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject(control);
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                chatState = (StateObject)ar.AsyncState;
                Socket client = chatState.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);
                // There might be more data, so store the data received so far.
                string text = string.Empty;
                chatState.chat.BeginInvoke(new MethodInvoker(delegate {
                    text = chatState.chat.Text;
                }));
                chatState.sb.Append(Encoding.UTF8.GetString(chatState.buffer, 0, bytesRead));
                Debug.WriteLine(text);
                JsonMessage responseJson = JsonSerializer.Deserialize<JsonMessage>(chatState.sb.ToString());
                Debug.WriteLine((responseJson.type == "message").ToString() + "  :  " + (responseJson.status == JsonMessageStatus.Ok).ToString());
                if (responseJson.type == "message" && responseJson.status == JsonMessageStatus.Ok)
                {
                    DateTime time = timeStart.AddMilliseconds(long.Parse(responseJson.chatMsg.time)).ToLocalTime();
                    chatState.chat.SetPropertyThreadSafe(() => chatState.chat.Text, text + "[" + time.TimeOfDay + "]  (" + responseJson.chatMsg.username + ")  { " + responseJson.chatMsg.msg + " }\n");
                    chatState.sb.Clear();
                }
                client.BeginReceive(chatState.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), chatState);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
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
            JsonMessage msg = new JsonMessage("signin", username: txtLoginLogin.Text, password: txtLoginPass.Text);
            request = JsonSerializer.SerializeToUtf8Bytes(msg);
            stream.Write(request, 0, request.Length);
            bytesRead = stream.Read(response, 0, response.Length);
            Debug.WriteLine(System.Text.Encoding.UTF8.GetString(response, 0, bytesRead));
            JsonMessage responseJson = JsonSerializer.Deserialize<JsonMessage>(System.Text.Encoding.UTF8.GetString(response, 0, bytesRead));
            if (responseJson.status == JsonMessageStatus.Ok)
            {
                username = responseJson.username;
                panelChat.Visible = true;
                Receive(client.Client, chat);
            }
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            panelRegister.Visible = true;
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            JsonMessage msg = new JsonMessage("message", username: this.username, chatMsg: new ChatMessage(username, chatInput.Text, DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()));
            Debug.WriteLine(chatInput.Text);
            string t = JsonSerializer.Serialize(msg);
            Debug.WriteLine(t);
            request = JsonSerializer.SerializeToUtf8Bytes(msg);
            stream.Write(request, 0, request.Length);
            //bytesRead = stream.Read(response, 0, response.Length);
            //Debug.WriteLine(System.Text.Encoding.UTF8.GetString(response, 0, bytesRead));
            //JsonMessage responseJson = JsonSerializer.Deserialize<JsonMessage>(System.Text.Encoding.UTF8.GetString(response, 0, bytesRead));
            //if (responseJson.status == JsonMessageStatus.Ok)
            //{
            //    DateTime time = timeStart.AddMilliseconds(long.Parse(responseJson.chatMsg.time)).ToLocalTime();
            //    chat.Text += "[" + time.TimeOfDay + "](" + responseJson.chatMsg.username + "){ " + responseJson.chatMsg.msg + "}\n";
            //}
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
