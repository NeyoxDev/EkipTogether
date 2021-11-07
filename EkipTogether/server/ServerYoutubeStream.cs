using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using VideoLibrary;

namespace EkipTogether.server
{
    public partial class ServerYoutubeStream : Form
    {
        
        private const string YoutubeLinkRegex = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";
        private static Regex regexExtractId = new Regex(YoutubeLinkRegex, RegexOptions.Compiled);
        private static string[] validAuthorities = { "youtube.com", "www.youtube.com", "youtu.be", "www.youtu.be" };

        private ChromiumWebBrowser Browser;

        private YoutubeStream Stream;
        
        public static ServerYoutubeStream instance;

        public ServerYoutubeStream()
        {
            instance = this;
            InitializeComponent();
            initStream();
            initBrowser();

        }

        private void initBrowser()
        {
            CefSettings settings = new CefSettings();
            settings.Locale = "fr";
            if (!Cef.IsInitialized)
            {
                Cef.Initialize(settings);
            }

            Browser = new ChromiumWebBrowser("");
            this.panel1.Controls.Add(Browser);
        }

        private void initStream()
        {
            Stream = new YoutubeStream();
            Stream.start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var url = this.textBox1.Text;
            if (url != null)
            {
                var youTube = YouTube.Default;
                var video = youTube.GetVideo(url);
                string title = video.Title;
                this.Name = this.Name + " - " + title;
                //Browser.LoadHtml("<html><body><div id=\"popupVid\"><iframe width=\"1584\" height=\"616\" src=\"https://www.youtube.com/embed/"+getIdVideo(url)+"?enablejsapi=1\" title=\"YouTube video player\" frameborder=\"\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe></div></body></html>", "http://www.example.com/");
                Browser.LoadHtml( "<html><head>" +
                                  "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=Edge,chrome=1\"/>" +
                                  "<script>"+
                                  "function stop() {{"+
                                  "var i = document.getElementsByTagName(\"iframe\")[0].contentWindow;" +
                                  "i.postMessage('{{\"event\":\"command\",\"func\":\"pauseVideo\",\"args\":\"\"}}', '*');" +
                                  "}}</script>"+
                                  "</head><body scroll=\"no\" style=\"padding:0px;margin:0px;\">" +
                                  "<iframe id=\"youtube\"style=\"border: 1px solid #fff;\" width=\"1584\" height=\"616\" src=\"https://www.youtube.com/embed/"+getIdVideo(url)+"?enablejsapi=1&rel=0&amp;showinfo=0\"" +
                                  "allow =\"autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen ></iframe>" +
                                  "</body></html>", "https://www.youtube.com");
                Task.Run(() =>
                {
                    Thread.Sleep(5000);

                    Browser.ExecuteScriptAsync("var player = new YT.Player('youtube', { events: { 'onStateChange': onPlayerStateChange } });  function onPlayerStateChange() { alert('Sonaah est homo');");
                });
            }
           else
            {
                MessageBox.Show("L'url n'est pas valide", "Ekip | Error");
            }
        }

        public void PauseVideo()
        {
            Browser.ExecuteScriptAsync("var i = document.getElementsByTagName(\"iframe\")[0].contentWindow;" +
                                       "i.postMessage(JSON.stringify({ event: 'command', func: 'pauseVideo' }), '*');");
        }
        
        public void PlayVideo()
        {
            Browser.ExecuteScriptAsync("var i = document.getElementsByTagName(\"iframe\")[0].contentWindow;" +
                                       "i.postMessage(JSON.stringify({ event: 'command', func: 'playVideo' }), '*');");
        }

        

        private string getIdVideo(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                string authority = new UriBuilder(uri).Uri.Authority.ToLower();

                //check if the url is a youtube url
                if (((IList) validAuthorities).Contains(authority))
                {
                    //and extract the id
                    var regRes = regexExtractId.Match(uri.ToString());
                    if (regRes.Success)
                    {
                        return regRes.Groups[1].Value;
                    }
                }
            }catch{}
            return null;
        }

        private void ServerYoutubeStream_Load_1(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
            new ConnectForm().Show();
        }
    }

    public class YoutubeStream
    {

        private List<YoutubeClient> connectedClients = new List<YoutubeClient>(); 
        public void start()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 6969);
            Task.Run(() =>
            {
                while (ServerYoutubeStream.instance.Visible)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    YoutubeClient ytbClient = new YoutubeClient(client);
                    connectedClients.Add(ytbClient); 
                }
            });
        }
    }

    public class YoutubeClient
    {
        private TcpClient Client;

        private bool running;

        private NetworkStream Stream;

        public YoutubeClient(TcpClient client)
        {
            this.Client = client;
            this.Stream = client.GetStream();
            running = true;
            start();
        }

        private void start()
        {
            Task.Run(() =>
            {
                while (running)
                {
                    byte[] buf = new byte[1024];
                    Stream.Read(buf, 0, buf.Length);
                    string data = Encoding.UTF8.GetString(buf);
                    handleData(data);
                }
            });
        }

        private void handleData(string data)
        {
            
        }
    }
}