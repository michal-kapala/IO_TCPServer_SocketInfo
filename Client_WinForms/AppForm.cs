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
using IO_TCPClient;
using JsonProtocol;

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
        SimpleTCPClient simpleClient;

        TcpClient client;
        byte[] response;
        string username;
        static DateTime timeStart;
        static StateObject chatState;

        private static ManualResetEvent receiveDone =
        new ManualResetEvent(false);
        private static string asyncResponse = String.Empty;

        public AppForm()
        {

            InitializeComponent();
            simpleClient = new SimpleTCPClient(54321);
            //Klient narazie w tej klasie żeby działało a potem się pomyśli nad podziałem jakoś frontu i api
            client = simpleClient.tcpClient;
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
                ChatMessageResponse chatMsgResp = JsonSerializer.Deserialize<ChatMessageResponse>(chatState.sb.ToString());
                //JsonMessage responseJson = JsonSerializer.Deserialize<JsonMessage>(chatState.sb.ToString());
                Debug.WriteLine((chatMsgResp.Id == ResponseId.ChatMessage).ToString() + "  :  " + (chatMsgResp.Status == ChatMessageResponse.StatusId.Ok).ToString());
                if (chatMsgResp.Id == ResponseId.ChatMessage && chatMsgResp.Status == ChatMessageResponse.StatusId.Ok)
                {
                    DateTime time = timeStart.AddMilliseconds(long.Parse(chatMsgResp.ChatMsg.time)).ToLocalTime();
                    chatState.chat.SetPropertyThreadSafe(() => chatState.chat.Text, text + "[" + time.TimeOfDay + "]  (" + chatMsgResp.ChatMsg.username + ")  { " + chatMsgResp.ChatMsg.msg + " }\n");
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
            if (txtLoginLogin.Text.Length == 0 || txtLoginLogin.Text == null)
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
            if (simpleClient.Connect("127.0.0.1", 8010))
            {
                //TODO: send login request
                if (!simpleClient.RequestSignIn(txtLoginLogin.Text, txtLoginPass.Text))
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
            string jsonResponse = Helper.ReadIntoJson(simpleClient.tcpClient);
            Tuple<string, bool> response = simpleClient.ProcessSignInResponse(jsonResponse);
            
            if (response.Item2)
            {
                username = txtLoginLogin.Text;
                panelChat.Visible = true;
                Receive(client.Client, chat);
            }
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
            if (chatInput.Text != "")
            {
                simpleClient.RequestChatMessage(username, chatInput.Text);
            }
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
            if (simpleClient.Connect("127.0.0.1", 8010))
            {
                //send login request
                if (!simpleClient.RequestSignUp(txtRegLogin.Text, txtRegPass.Text))
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

            string jsonResponse = Helper.ReadIntoJson(simpleClient.tcpClient);
            Tuple<string, bool> response = simpleClient.ProcessSignUpResponse(jsonResponse);
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
