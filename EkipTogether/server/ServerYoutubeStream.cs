using System;
using System.Windows.Forms;

namespace EkipTogether.server
{
    public partial class ServerYoutubeStream : Form
    {
        public ServerYoutubeStream()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var url = this.textBox1.Text;
            if (url != null)
            {
                
              //  this.browserView1. =  "<html><body><iframe width=\"1584\" height=\"616\" src=\"https://www.youtube.com/embed/r54ORSRH8T8\" title=\"YouTube video player\" frameborder=\"\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe></body></html>";
            }
           else
            {
                MessageBox.Show("L'url n'est pas valide", "Ekip | Error");
            }
        }

        private void ServerYoutubeStream_Load(object sender, EventArgs e)
        {
        }
    }
}