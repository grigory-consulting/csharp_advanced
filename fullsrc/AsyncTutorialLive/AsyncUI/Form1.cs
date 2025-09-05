namespace AsyncUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "Downloading Sync ..."; 
            using var http = new HttpClient();
            string html = http.GetStringAsync("https://raw.githubusercontent.com/zemirco/sf-city-lots-json/master/citylots.json").Result;
            label1.Text = $"Downloaded {html.Length} chars (Sync)";
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            label1.Text = "Downloading (async) ...";
            using var http = new HttpClient();
            string html = await http.GetStringAsync("https://raw.githubusercontent.com/zemirco/sf-city-lots-json/master/citylots.json");
            label1.Text = $"Downloaded {html.Length} chars (Async)";
        }
    }
}
