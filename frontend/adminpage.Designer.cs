namespace frontend
{
    partial class AdminPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdminPage));
            pictureBox1 = new PictureBox();
            usernameadmin = new TextBox();
            passwordadmin = new TextBox();
            loginadmin = new Button();
            textBox3 = new TextBox();
            textBox4 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(199, 47);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(216, 179);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // usernameadmin
            // 
            usernameadmin.BackColor = Color.FromArgb(169, 171, 169);
            usernameadmin.BorderStyle = BorderStyle.None;
            usernameadmin.Font = new Font("Bruno Ace SC", 12F);
            usernameadmin.Location = new Point(199, 260);
            usernameadmin.Name = "usernameadmin";
            usernameadmin.Size = new Size(208, 20);
            usernameadmin.TabIndex = 1;
            // 
            // passwordadmin
            // 
            passwordadmin.BackColor = Color.FromArgb(169, 171, 169);
            passwordadmin.BorderStyle = BorderStyle.None;
            passwordadmin.Font = new Font("Bruno Ace SC", 12F);
            passwordadmin.Location = new Point(199, 330);
            passwordadmin.Name = "passwordadmin";
            passwordadmin.Size = new Size(208, 20);
            passwordadmin.TabIndex = 2;
            // 
            // loginadmin
            // 
            loginadmin.BackColor = Color.White;
            loginadmin.Font = new Font("Bruno Ace SC", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            loginadmin.Location = new Point(224, 383);
            loginadmin.Name = "loginadmin";
            loginadmin.Size = new Size(136, 38);
            loginadmin.TabIndex = 3;
            loginadmin.Text = "LOGIN";
            loginadmin.UseVisualStyleBackColor = false;
            loginadmin.Click += loginadmin_Click;
            // 
            // textBox3
            // 
            textBox3.BackColor = Color.FromArgb(36, 38, 58);
            textBox3.BorderStyle = BorderStyle.None;
            textBox3.Font = new Font("Bruno Ace SC", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox3.ForeColor = Color.White;
            textBox3.Location = new Point(199, 234);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(100, 20);
            textBox3.TabIndex = 4;
            textBox3.Text = "EMAIL:";
            // 
            // textBox4
            // 
            textBox4.BackColor = Color.FromArgb(36, 38, 58);
            textBox4.BorderStyle = BorderStyle.None;
            textBox4.Font = new Font("Bruno Ace SC", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox4.ForeColor = Color.White;
            textBox4.Location = new Point(199, 304);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(143, 20);
            textBox4.TabIndex = 5;
            textBox4.Text = "PASSWORD:";
            // 
            // adminpage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(36, 38, 58);
            ClientSize = new Size(636, 476);
            Controls.Add(textBox4);
            Controls.Add(textBox3);
            Controls.Add(loginadmin);
            Controls.Add(passwordadmin);
            Controls.Add(usernameadmin);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Name = "adminpage";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "adminpage";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private TextBox usernameadmin;
        private TextBox passwordadmin;
        private Button loginadmin;
        private TextBox textBox3;
        private TextBox textBox4;
    }
}