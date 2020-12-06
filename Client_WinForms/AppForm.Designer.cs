namespace AppForm
{
    partial class AppForm
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelLogin = new System.Windows.Forms.Panel();
            this.panelRegister = new System.Windows.Forms.Panel();
            this.labRegError = new System.Windows.Forms.Label();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonSignUp = new System.Windows.Forms.Button();
            this.txtRegConfirm = new System.Windows.Forms.TextBox();
            this.labRegConfirm = new System.Windows.Forms.Label();
            this.txtRegPass = new System.Windows.Forms.TextBox();
            this.txtRegLogin = new System.Windows.Forms.TextBox();
            this.labRegPass = new System.Windows.Forms.Label();
            this.labRegLogin = new System.Windows.Forms.Label();
            this.panelChat = new System.Windows.Forms.Panel();
            this.buttonClear = new System.Windows.Forms.Button();
            this.chatInput = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.chat = new System.Windows.Forms.RichTextBox();
            this.labLoginError = new System.Windows.Forms.Label();
            this.buttonRegister = new System.Windows.Forms.Button();
            this.buttonSignIn = new System.Windows.Forms.Button();
            this.txtLoginPass = new System.Windows.Forms.TextBox();
            this.txtLoginLogin = new System.Windows.Forms.TextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.labelLogin = new System.Windows.Forms.Label();
            this.panelLogin.SuspendLayout();
            this.panelRegister.SuspendLayout();
            this.panelChat.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLogin
            // 
            this.panelLogin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panelLogin.Controls.Add(this.panelRegister);
            this.panelLogin.Controls.Add(this.panelChat);
            this.panelLogin.Controls.Add(this.labLoginError);
            this.panelLogin.Controls.Add(this.buttonRegister);
            this.panelLogin.Controls.Add(this.buttonSignIn);
            this.panelLogin.Controls.Add(this.txtLoginPass);
            this.panelLogin.Controls.Add(this.txtLoginLogin);
            this.panelLogin.Controls.Add(this.labelPassword);
            this.panelLogin.Controls.Add(this.labelLogin);
            this.panelLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLogin.Location = new System.Drawing.Point(0, 0);
            this.panelLogin.Margin = new System.Windows.Forms.Padding(2);
            this.panelLogin.Name = "panelLogin";
            this.panelLogin.Size = new System.Drawing.Size(584, 361);
            this.panelLogin.TabIndex = 0;
            // 
            // panelRegister
            // 
            this.panelRegister.Controls.Add(this.labRegError);
            this.panelRegister.Controls.Add(this.buttonBack);
            this.panelRegister.Controls.Add(this.buttonSignUp);
            this.panelRegister.Controls.Add(this.txtRegConfirm);
            this.panelRegister.Controls.Add(this.labRegConfirm);
            this.panelRegister.Controls.Add(this.txtRegPass);
            this.panelRegister.Controls.Add(this.txtRegLogin);
            this.panelRegister.Controls.Add(this.labRegPass);
            this.panelRegister.Controls.Add(this.labRegLogin);
            this.panelRegister.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRegister.Location = new System.Drawing.Point(0, 0);
            this.panelRegister.Name = "panelRegister";
            this.panelRegister.Size = new System.Drawing.Size(584, 361);
            this.panelRegister.TabIndex = 10;
            this.panelRegister.Visible = false;
            // 
            // labRegError
            // 
            this.labRegError.AutoSize = true;
            this.labRegError.ForeColor = System.Drawing.Color.Red;
            this.labRegError.Location = new System.Drawing.Point(37, 322);
            this.labRegError.Name = "labRegError";
            this.labRegError.Size = new System.Drawing.Size(85, 13);
            this.labRegError.TabIndex = 14;
            this.labRegError.Text = "Default error text";
            this.labRegError.Visible = false;
            // 
            // buttonBack
            // 
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonBack.Location = new System.Drawing.Point(28, 24);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(95, 46);
            this.buttonBack.TabIndex = 13;
            this.buttonBack.Text = "Back";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonSignUp
            // 
            this.buttonSignUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonSignUp.Location = new System.Drawing.Point(248, 209);
            this.buttonSignUp.Name = "buttonSignUp";
            this.buttonSignUp.Size = new System.Drawing.Size(150, 62);
            this.buttonSignUp.TabIndex = 12;
            this.buttonSignUp.Text = "Sign up";
            this.buttonSignUp.UseVisualStyleBackColor = true;
            this.buttonSignUp.Click += new System.EventHandler(this.buttonSignUp_Click);
            // 
            // txtRegConfirm
            // 
            this.txtRegConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtRegConfirm.Location = new System.Drawing.Point(248, 165);
            this.txtRegConfirm.Name = "txtRegConfirm";
            this.txtRegConfirm.PasswordChar = '●';
            this.txtRegConfirm.Size = new System.Drawing.Size(150, 29);
            this.txtRegConfirm.TabIndex = 11;
            // 
            // labRegConfirm
            // 
            this.labRegConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labRegConfirm.Location = new System.Drawing.Point(70, 165);
            this.labRegConfirm.Name = "labRegConfirm";
            this.labRegConfirm.Size = new System.Drawing.Size(172, 35);
            this.labRegConfirm.TabIndex = 10;
            this.labRegConfirm.Text = "Confirm password:";
            this.labRegConfirm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtRegPass
            // 
            this.txtRegPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtRegPass.Location = new System.Drawing.Point(248, 124);
            this.txtRegPass.Name = "txtRegPass";
            this.txtRegPass.PasswordChar = '●';
            this.txtRegPass.Size = new System.Drawing.Size(150, 29);
            this.txtRegPass.TabIndex = 9;
            // 
            // txtRegLogin
            // 
            this.txtRegLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtRegLogin.Location = new System.Drawing.Point(248, 83);
            this.txtRegLogin.Name = "txtRegLogin";
            this.txtRegLogin.Size = new System.Drawing.Size(150, 29);
            this.txtRegLogin.TabIndex = 8;
            // 
            // labRegPass
            // 
            this.labRegPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labRegPass.Location = new System.Drawing.Point(142, 124);
            this.labRegPass.Name = "labRegPass";
            this.labRegPass.Size = new System.Drawing.Size(100, 35);
            this.labRegPass.TabIndex = 7;
            this.labRegPass.Text = "Password:";
            this.labRegPass.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labRegLogin
            // 
            this.labRegLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labRegLogin.Location = new System.Drawing.Point(176, 83);
            this.labRegLogin.Name = "labRegLogin";
            this.labRegLogin.Size = new System.Drawing.Size(66, 35);
            this.labRegLogin.TabIndex = 6;
            this.labRegLogin.Text = "Login:";
            this.labRegLogin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelChat
            // 
            this.panelChat.Controls.Add(this.buttonClear);
            this.panelChat.Controls.Add(this.chatInput);
            this.panelChat.Controls.Add(this.buttonSend);
            this.panelChat.Controls.Add(this.chat);
            this.panelChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelChat.Location = new System.Drawing.Point(0, 0);
            this.panelChat.Name = "panelChat";
            this.panelChat.Size = new System.Drawing.Size(584, 361);
            this.panelChat.TabIndex = 9;
            this.panelChat.Visible = false;
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(453, 311);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 35);
            this.buttonClear.TabIndex = 3;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // chatInput
            // 
            this.chatInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.chatInput.Location = new System.Drawing.Point(28, 311);
            this.chatInput.Multiline = true;
            this.chatInput.Name = "chatInput";
            this.chatInput.Size = new System.Drawing.Size(338, 35);
            this.chatInput.TabIndex = 2;
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(372, 311);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(75, 35);
            this.buttonSend.TabIndex = 1;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // chat
            // 
            //usuniete this
            this.chat.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.chat.Location = new System.Drawing.Point(13, 13);
            this.chat.Name = "chat";
            this.chat.Size = new System.Drawing.Size(515, 280);
            this.chat.TabIndex = 0;
            this.chat.Text = "";
            // 
            // labLoginError
            // 
            this.labLoginError.AutoSize = true;
            this.labLoginError.ForeColor = System.Drawing.Color.Red;
            this.labLoginError.Location = new System.Drawing.Point(37, 321);
            this.labLoginError.Name = "labLoginError";
            this.labLoginError.Size = new System.Drawing.Size(85, 13);
            this.labLoginError.TabIndex = 8;
            this.labLoginError.Text = "Default error text";
            this.labLoginError.Visible = false;
            // 
            // buttonRegister
            // 
            this.buttonRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonRegister.Location = new System.Drawing.Point(28, 24);
            this.buttonRegister.Name = "buttonRegister";
            this.buttonRegister.Size = new System.Drawing.Size(95, 46);
            this.buttonRegister.TabIndex = 7;
            this.buttonRegister.Text = "Register";
            this.buttonRegister.UseVisualStyleBackColor = true;
            this.buttonRegister.Click += new System.EventHandler(this.buttonRegister_Click);
            // 
            // buttonSignIn
            // 
            this.buttonSignIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonSignIn.Location = new System.Drawing.Point(248, 209);
            this.buttonSignIn.Name = "buttonSignIn";
            this.buttonSignIn.Size = new System.Drawing.Size(150, 62);
            this.buttonSignIn.TabIndex = 6;
            this.buttonSignIn.Text = "Sign In";
            this.buttonSignIn.UseVisualStyleBackColor = true;
            this.buttonSignIn.Click += new System.EventHandler(this.buttonSignIn_Click);
            // 
            // txtLoginPass
            // 
            this.txtLoginPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtLoginPass.Location = new System.Drawing.Point(248, 159);
            this.txtLoginPass.Name = "txtLoginPass";
            this.txtLoginPass.PasswordChar = '●';
            this.txtLoginPass.Size = new System.Drawing.Size(150, 29);
            this.txtLoginPass.TabIndex = 5;
            // 
            // txtLoginLogin
            // 
            this.txtLoginLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtLoginLogin.Location = new System.Drawing.Point(248, 118);
            this.txtLoginLogin.Name = "txtLoginLogin";
            this.txtLoginLogin.Size = new System.Drawing.Size(150, 29);
            this.txtLoginLogin.TabIndex = 4;
            // 
            // labelPassword
            // 
            this.labelPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelPassword.Location = new System.Drawing.Point(142, 159);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(100, 35);
            this.labelPassword.TabIndex = 3;
            this.labelPassword.Text = "Password:";
            this.labelPassword.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelLogin
            // 
            this.labelLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelLogin.Location = new System.Drawing.Point(176, 118);
            this.labelLogin.Name = "labelLogin";
            this.labelLogin.Size = new System.Drawing.Size(66, 35);
            this.labelLogin.TabIndex = 2;
            this.labelLogin.Text = "Login:";
            this.labelLogin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.panelLogin);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "AppForm";
            this.Text = "WinForms Client";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.panelLogin.ResumeLayout(false);
            this.panelLogin.PerformLayout();
            this.panelRegister.ResumeLayout(false);
            this.panelRegister.PerformLayout();
            this.panelChat.ResumeLayout(false);
            this.panelChat.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel panelLogin;
        private System.Windows.Forms.Label labelLogin;
        private System.Windows.Forms.Label labelPassword;
        public System.Windows.Forms.TextBox txtLoginLogin;
        public System.Windows.Forms.TextBox txtLoginPass;
        private System.Windows.Forms.Button buttonRegister;
        private System.Windows.Forms.Button buttonSignIn;
        public System.Windows.Forms.Label labLoginError;
        public System.Windows.Forms.Panel panelChat;
        private System.Windows.Forms.RichTextBox chat;
        private System.Windows.Forms.Button buttonClear;
        public System.Windows.Forms.TextBox chatInput;
        private System.Windows.Forms.Button buttonSend;
        public System.Windows.Forms.Panel panelRegister;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonSignUp;
        public System.Windows.Forms.TextBox txtRegConfirm;
        private System.Windows.Forms.Label labRegConfirm;
        public System.Windows.Forms.TextBox txtRegPass;
        public System.Windows.Forms.TextBox txtRegLogin;
        private System.Windows.Forms.Label labRegPass;
        private System.Windows.Forms.Label labRegLogin;
        public System.Windows.Forms.Label labRegError;
    }
}

