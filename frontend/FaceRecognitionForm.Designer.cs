namespace frontend
{
    partial class FaceRecognitionForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FaceRecognitionForm));
            cameraView = new PictureBox();
            capturedFaceView = new PictureBox();
            modeLabel = new Label();
            modeComboBox = new ComboBox();
            statusLabel = new Label();
            loadingLabel = new Label();
            startButton = new Button();
            stopButton = new Button();
            registerButton = new Button();
            frameTimer = new System.Windows.Forms.Timer(components);
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)cameraView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)capturedFaceView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // cameraView
            // 
            cameraView.BackColor = Color.Transparent;
            cameraView.BorderStyle = BorderStyle.FixedSingle;
            cameraView.Location = new Point(169, 109);
            cameraView.Name = "cameraView";
            cameraView.Size = new Size(303, 196);
            cameraView.SizeMode = PictureBoxSizeMode.Zoom;
            cameraView.TabIndex = 0;
            cameraView.TabStop = false;
            // 
            // capturedFaceView
            // 
            capturedFaceView.BackColor = Color.Black;
            capturedFaceView.BorderStyle = BorderStyle.FixedSingle;
            capturedFaceView.Location = new Point(432, 360);
            capturedFaceView.Name = "capturedFaceView";
            capturedFaceView.Size = new Size(138, 110);
            capturedFaceView.SizeMode = PictureBoxSizeMode.Zoom;
            capturedFaceView.TabIndex = 8;
            capturedFaceView.TabStop = false;
            // 
            // modeLabel
            // 
            modeLabel.AutoSize = true;
            modeLabel.Font = new Font("Bruno Ace SC", 9.749999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            modeLabel.Location = new Point(10, 350);
            modeLabel.Name = "modeLabel";
            modeLabel.Size = new Size(54, 15);
            modeLabel.TabIndex = 1;
            modeLabel.Text = "Mode:";
            // 
            // modeComboBox
            // 
            modeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            modeComboBox.FormattingEnabled = true;
            modeComboBox.Items.AddRange(new object[] { "Time In", "Time Out" });
            modeComboBox.Location = new Point(70, 347);
            modeComboBox.Name = "modeComboBox";
            modeComboBox.Size = new Size(150, 23);
            modeComboBox.TabIndex = 2;
            modeComboBox.SelectedIndexChanged += modeComboBox_SelectedIndexChanged;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Font = new Font("Bruno Ace SC", 9.749999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            statusLabel.Location = new Point(10, 380);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(123, 15);
            statusLabel.TabIndex = 3;
            statusLabel.Text = "Status: Ready";
            // 
            // loadingLabel
            // 
            loadingLabel.AutoSize = true;
            loadingLabel.Font = new Font("Arial", 9F, FontStyle.Bold);
            loadingLabel.ForeColor = Color.Blue;
            loadingLabel.Location = new Point(10, 410);
            loadingLabel.Name = "loadingLabel";
            loadingLabel.Size = new Size(0, 15);
            loadingLabel.TabIndex = 4;
            // 
            // startButton
            // 
            startButton.Anchor = AnchorStyles.Top;
            startButton.Font = new Font("Bruno Ace SC", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            startButton.Location = new Point(10, 440);
            startButton.Name = "startButton";
            startButton.Size = new Size(100, 30);
            startButton.TabIndex = 5;
            startButton.Text = "Start Camera";
            startButton.UseVisualStyleBackColor = true;
            startButton.Click += StartButton_Click;
            // 
            // stopButton
            // 
            stopButton.Enabled = false;
            stopButton.Location = new Point(120, 440);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(100, 30);
            stopButton.TabIndex = 6;
            stopButton.Text = "Stop Camera";
            stopButton.UseVisualStyleBackColor = true;
            stopButton.Click += StopButton_Click;
            // 
            // registerButton
            // 
            registerButton.Location = new Point(225, 440);
            registerButton.Name = "registerButton";
            registerButton.Size = new Size(100, 30);
            registerButton.TabIndex = 7;
            registerButton.Text = "Register";
            registerButton.UseVisualStyleBackColor = true;
            registerButton.Click += RegisterButton_Click;
            // 
            // frameTimer
            // 
            frameTimer.Interval = 33;
            frameTimer.Tick += FrameTimer_Tick;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(-3, -27);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(700, 520);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 9;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = Color.Transparent;
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(95, 36);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(462, 334);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 10;
            pictureBox2.TabStop = false;
            // 
            // FaceRecognitionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(640, 480);
            Controls.Add(capturedFaceView);
            Controls.Add(registerButton);
            Controls.Add(stopButton);
            Controls.Add(startButton);
            Controls.Add(loadingLabel);
            Controls.Add(statusLabel);
            Controls.Add(modeComboBox);
            Controls.Add(modeLabel);
            Controls.Add(cameraView);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FaceRecognitionForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Face Recognition Attendance";
            ((System.ComponentModel.ISupportInitialize)cameraView).EndInit();
            ((System.ComponentModel.ISupportInitialize)capturedFaceView).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox cameraView;
        private System.Windows.Forms.PictureBox capturedFaceView;
        private System.Windows.Forms.Label modeLabel;
        private System.Windows.Forms.ComboBox modeComboBox;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label loadingLabel;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button registerButton;
        private System.Windows.Forms.Timer frameTimer;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
    }
}

