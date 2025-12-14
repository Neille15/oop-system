namespace frontend;

partial class Form1
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        button1 = new Button();
        button2 = new Button();
        pictureBox1 = new PictureBox();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        SuspendLayout();
        // 
        // button1
        // 
        button1.BackColor = Color.FromArgb(177, 179, 177);
        button1.Font = new Font("Bruno Ace SC", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
        button1.Location = new Point(242, 205);
        button1.Margin = new Padding(0);
        button1.Name = "button1";
        button1.Size = new Size(158, 38);
        button1.TabIndex = 1;
        button1.Text = "LOGIN STUDENT";
        button1.UseVisualStyleBackColor = false;
        // 
        // button2
        // 
        button2.Font = new Font("Bruno Ace SC", 9.749999F, FontStyle.Regular, GraphicsUnit.Point, 0);
        button2.Location = new Point(242, 264);
        button2.Margin = new Padding(0);
        button2.Name = "button2";
        button2.Size = new Size(158, 37);
        button2.TabIndex = 2;
        button2.Text = "ADMIN LOGIN";
        button2.UseVisualStyleBackColor = true;
        // 
        // pictureBox1
        // 
        pictureBox1.Anchor = AnchorStyles.None;
        pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
        pictureBox1.Location = new Point(253, 64);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(127, 120);
        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBox1.TabIndex = 3;
        pictureBox1.TabStop = false;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(36, 38, 58);
        ClientSize = new Size(640, 480);
        Controls.Add(button2);
        Controls.Add(button1);
        Controls.Add(pictureBox1);
        MaximizeBox = false;
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Form1";
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Button button1;
    private Button button2;
    private PictureBox pictureBox1;
}
