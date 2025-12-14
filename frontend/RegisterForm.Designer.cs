namespace frontend
{
    partial class RegisterForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cameraView = new System.Windows.Forms.PictureBox();
            this.firstNameLabel = new System.Windows.Forms.Label();
            this.firstNameTextBox = new System.Windows.Forms.TextBox();
            this.lastNameLabel = new System.Windows.Forms.Label();
            this.lastNameTextBox = new System.Windows.Forms.TextBox();
            this.emailLabel = new System.Windows.Forms.Label();
            this.emailTextBox = new System.Windows.Forms.TextBox();
            this.birthDateLabel = new System.Windows.Forms.Label();
            this.birthDatePicker = new System.Windows.Forms.DateTimePicker();
            this.studentNumberLabel = new System.Windows.Forms.Label();
            this.studentNumberTextBox = new System.Windows.Forms.TextBox();
            this.captureButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.frameTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.cameraView)).BeginInit();
            this.SuspendLayout();
            // 
            // cameraView
            // 
            this.cameraView.BackColor = System.Drawing.Color.Black;
            this.cameraView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cameraView.Location = new System.Drawing.Point(12, 12);
            this.cameraView.Name = "cameraView";
            this.cameraView.Size = new System.Drawing.Size(400, 300);
            this.cameraView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.cameraView.TabIndex = 0;
            this.cameraView.TabStop = false;
            // 
            // firstNameLabel
            // 
            this.firstNameLabel.AutoSize = true;
            this.firstNameLabel.Location = new System.Drawing.Point(12, 325);
            this.firstNameLabel.Name = "firstNameLabel";
            this.firstNameLabel.Size = new System.Drawing.Size(67, 15);
            this.firstNameLabel.TabIndex = 1;
            this.firstNameLabel.Text = "First Name:";
            // 
            // firstNameTextBox
            // 
            this.firstNameTextBox.Location = new System.Drawing.Point(85, 322);
            this.firstNameTextBox.Name = "firstNameTextBox";
            this.firstNameTextBox.Size = new System.Drawing.Size(200, 23);
            this.firstNameTextBox.TabIndex = 2;
            // 
            // lastNameLabel
            // 
            this.lastNameLabel.AutoSize = true;
            this.lastNameLabel.Location = new System.Drawing.Point(12, 354);
            this.lastNameLabel.Name = "lastNameLabel";
            this.lastNameLabel.Size = new System.Drawing.Size(66, 15);
            this.lastNameLabel.TabIndex = 3;
            this.lastNameLabel.Text = "Last Name:";
            // 
            // lastNameTextBox
            // 
            this.lastNameTextBox.Location = new System.Drawing.Point(85, 351);
            this.lastNameTextBox.Name = "lastNameTextBox";
            this.lastNameTextBox.Size = new System.Drawing.Size(200, 23);
            this.lastNameTextBox.TabIndex = 4;
            // 
            // emailLabel
            // 
            this.emailLabel.AutoSize = true;
            this.emailLabel.Location = new System.Drawing.Point(12, 383);
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size(39, 15);
            this.emailLabel.TabIndex = 5;
            this.emailLabel.Text = "Email:";
            // 
            // emailTextBox
            // 
            this.emailTextBox.Location = new System.Drawing.Point(85, 380);
            this.emailTextBox.Name = "emailTextBox";
            this.emailTextBox.Size = new System.Drawing.Size(200, 23);
            this.emailTextBox.TabIndex = 6;
            // 
            // birthDateLabel
            // 
            this.birthDateLabel.AutoSize = true;
            this.birthDateLabel.Location = new System.Drawing.Point(12, 412);
            this.birthDateLabel.Name = "birthDateLabel";
            this.birthDateLabel.Size = new System.Drawing.Size(64, 15);
            this.birthDateLabel.TabIndex = 7;
            this.birthDateLabel.Text = "Birth Date:";
            // 
            // birthDatePicker
            // 
            this.birthDatePicker.Location = new System.Drawing.Point(85, 409);
            this.birthDatePicker.Name = "birthDatePicker";
            this.birthDatePicker.Size = new System.Drawing.Size(200, 23);
            this.birthDatePicker.TabIndex = 8;
            this.birthDatePicker.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            // 
            // studentNumberLabel
            // 
            this.studentNumberLabel.AutoSize = true;
            this.studentNumberLabel.Location = new System.Drawing.Point(12, 441);
            this.studentNumberLabel.Name = "studentNumberLabel";
            this.studentNumberLabel.Size = new System.Drawing.Size(95, 15);
            this.studentNumberLabel.TabIndex = 9;
            this.studentNumberLabel.Text = "Student Number:";
            // 
            // studentNumberTextBox
            // 
            this.studentNumberTextBox.Location = new System.Drawing.Point(113, 438);
            this.studentNumberTextBox.Name = "studentNumberTextBox";
            this.studentNumberTextBox.Size = new System.Drawing.Size(200, 23);
            this.studentNumberTextBox.TabIndex = 10;
            // 
            // captureButton
            // 
            this.captureButton.Enabled = false;
            this.captureButton.Location = new System.Drawing.Point(12, 474);
            this.captureButton.Name = "captureButton";
            this.captureButton.Size = new System.Drawing.Size(150, 35);
            this.captureButton.TabIndex = 9;
            this.captureButton.Text = "Capture & Register";
            this.captureButton.UseVisualStyleBackColor = true;
            this.captureButton.Click += new System.EventHandler(this.CaptureButton_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(12, 519);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(42, 15);
            this.statusLabel.TabIndex = 10;
            this.statusLabel.Text = "Ready";
            // 
            // frameTimer
            // 
            this.frameTimer.Interval = 33;
            this.frameTimer.Tick += new System.EventHandler(this.FrameTimer_Tick);
            // 
            // RegisterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 544);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.captureButton);
            this.Controls.Add(this.studentNumberTextBox);
            this.Controls.Add(this.studentNumberLabel);
            this.Controls.Add(this.birthDatePicker);
            this.Controls.Add(this.birthDateLabel);
            this.Controls.Add(this.emailTextBox);
            this.Controls.Add(this.emailLabel);
            this.Controls.Add(this.lastNameTextBox);
            this.Controls.Add(this.lastNameLabel);
            this.Controls.Add(this.firstNameTextBox);
            this.Controls.Add(this.firstNameLabel);
            this.Controls.Add(this.cameraView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "RegisterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Register New User";
            this.Load += new System.EventHandler(this.RegisterForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cameraView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox cameraView;
        private System.Windows.Forms.Label firstNameLabel;
        private System.Windows.Forms.TextBox firstNameTextBox;
        private System.Windows.Forms.Label lastNameLabel;
        private System.Windows.Forms.TextBox lastNameTextBox;
        private System.Windows.Forms.Label emailLabel;
        private System.Windows.Forms.TextBox emailTextBox;
        private System.Windows.Forms.Label birthDateLabel;
        private System.Windows.Forms.DateTimePicker birthDatePicker;
        private System.Windows.Forms.Label studentNumberLabel;
        private System.Windows.Forms.TextBox studentNumberTextBox;
        private System.Windows.Forms.Button captureButton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Timer frameTimer;
    }
}
