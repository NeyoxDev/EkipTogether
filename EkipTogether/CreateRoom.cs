using System;
using System.Windows.Forms;
using EkipTogether.server;

namespace EkipTogether
{
    public partial class CreateRoom : Form
    {
        public CreateRoom()
        {
            InitializeComponent();
            DiscordRichPresence.updateState(DiscordState.CREATE_ROOM, null);
            this.textBox2.Text = Saver.getValue("user", "");
            this.comboBox1.Items.AddRange(ApplicationType.values());
            this.comboBox1.SelectedItem = ApplicationType.YOUTUBE;

        }

        private void CreateRoom_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var currentApp = (string) this.comboBox1.SelectedItem;
            if (currentApp != null)
            {
                switch (currentApp)
                {
                    case ApplicationType.YOUTUBE:
                        Hide();
                        var youtube = new ServerYoutubeStream();
                        youtube.Show();
                        
                        break;
                    default:
                        MessageBox.Show("Désolé mais l'application séléctionnée n'est pas disponible pour le moment",
                            "EKIP | Error");
                        break;
                }
            }
            else
            {
                MessageBox.Show("Impossible de trouver l'application séléctionnée", "EKIP | Error");

            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.gg/ffTcfvdHGd");
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Saver.saveOptions();
            base.OnFormClosing(e);
        }
    }

    public class ApplicationType
    {
        public const string YOUTUBE = "Youtube";
        public const string SCREEN = "Mon écran";
        public const string NETFLIX = "Netflix";
        public const string DISNEYPLUS = "Disney+";
        public const string AMAZON_VIDEO = "Amazon Vidéo";
        public const string MP4STREAM = "Flux personnalisé";
        
        public static string[] values()
        {
            return new[] {YOUTUBE, SCREEN, NETFLIX, DISNEYPLUS, AMAZON_VIDEO, MP4STREAM};
        }
    }
}