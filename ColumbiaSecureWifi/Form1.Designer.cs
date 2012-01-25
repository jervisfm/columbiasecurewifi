namespace ColumbiaSecureProxy
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnConnect = new System.Windows.Forms.Button();
            this.textBox_status = new System.Windows.Forms.TextBox();
            this.textBox_Server = new System.Windows.Forms.TextBox();
            this.label_status = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Password = new System.Windows.Forms.TextBox();
            this.label_Password = new System.Windows.Forms.Label();
            this.textBox_Username = new System.Windows.Forms.TextBox();
            this.label_Username = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(327, 256);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Credentials";
            
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnConnect);
            this.panel1.Controls.Add(this.textBox_status);
            this.panel1.Controls.Add(this.textBox_Server);
            this.panel1.Controls.Add(this.label_status);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBox_Password);
            this.panel1.Controls.Add(this.label_Password);
            this.panel1.Controls.Add(this.textBox_Username);
            this.panel1.Controls.Add(this.label_Username);
            this.panel1.Location = new System.Drawing.Point(17, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(285, 231);
            this.panel1.TabIndex = 0;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(70, 191);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(134, 29);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // textBox_status
            // 
            this.textBox_status.Location = new System.Drawing.Point(96, 130);
            this.textBox_status.Multiline = true;
            this.textBox_status.Name = "textBox_status";
            this.textBox_status.ReadOnly = true;
            this.textBox_status.Size = new System.Drawing.Size(148, 55);
            this.textBox_status.TabIndex = 4;
            
            // 
            // textBox_Server
            // 
            this.textBox_Server.Location = new System.Drawing.Point(96, 91);
            this.textBox_Server.Name = "textBox_Server";
            this.textBox_Server.ReadOnly = true;
            this.textBox_Server.Size = new System.Drawing.Size(148, 23);
            this.textBox_Server.TabIndex = 5;
            
            // 
            // label_status
            // 
            this.label_status.AutoSize = true;
            this.label_status.Location = new System.Drawing.Point(7, 133);
            this.label_status.Name = "label_status";
            this.label_status.Size = new System.Drawing.Size(39, 15);
            this.label_status.TabIndex = 0;
            this.label_status.Text = "Status";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "SOCKS Server";
            // 
            // textBox_Password
            // 
            this.textBox_Password.Location = new System.Drawing.Point(96, 54);
            this.textBox_Password.Name = "textBox_Password";
            this.textBox_Password.Size = new System.Drawing.Size(148, 23);
            this.textBox_Password.TabIndex = 2;
            this.textBox_Password.UseSystemPasswordChar = true;
            this.textBox_Password.WordWrap = false;
            
            // 
            // label_Password
            // 
            this.label_Password.AutoSize = true;
            this.label_Password.Location = new System.Drawing.Point(7, 56);
            this.label_Password.Name = "label_Password";
            this.label_Password.Size = new System.Drawing.Size(57, 15);
            this.label_Password.TabIndex = 0;
            this.label_Password.Text = "Password";
            // 
            // textBox_Username
            // 
            this.textBox_Username.Location = new System.Drawing.Point(96, 16);
            this.textBox_Username.Name = "textBox_Username";
            this.textBox_Username.Size = new System.Drawing.Size(148, 23);
            this.textBox_Username.TabIndex = 1;
            // 
            // label_Username
            // 
            this.label_Username.AutoSize = true;
            this.label_Username.Location = new System.Drawing.Point(7, 18);
            this.label_Username.Name = "label_Username";
            this.label_Username.Size = new System.Drawing.Size(82, 15);
            this.label_Username.TabIndex = 0;
            this.label_Username.Text = "Columbia UNI";
            // 
            // Form1
            // 
            this.AcceptButton = this.btnConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 280);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Columbia Secure Proxy";
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label_Username;
        private System.Windows.Forms.Label label_Password;
        private System.Windows.Forms.TextBox textBox_Username;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox textBox_Password;
        private System.Windows.Forms.TextBox textBox_Server;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_status;
        private System.Windows.Forms.Label label_status;
    }
}

