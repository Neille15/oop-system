using System;
using System.Data;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frontend
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private async void dashboard_Load(object sender, EventArgs e)
        {
            await LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://localhost:5001/api/users"; // CHANGE PORT IF NEEDED

                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();

                    DataTable table =
                        JsonSerializer.Deserialize<DataTable>(json);

                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = table;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load Error");
            }
        }
    }
}
