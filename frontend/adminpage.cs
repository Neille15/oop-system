using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frontend
{
    public partial class AdminPage : Form
    {
        public AdminPage()
        {
            InitializeComponent();
            this.passwordadmin.PasswordChar = '*';
        }
        private void loginadmin_Click(object? sender, EventArgs e)
        {
            if (usernameadmin.Text == "admin" && passwordadmin.Text == "admin")
            {
                MessageBox.Show("Login Successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                using dashboard dashboard = new dashboard();
                dashboard.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("Invalid Username or Password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
