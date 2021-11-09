using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EkipServer
{
    public class Room
    {

        private int id;
        private List<EClient> viewers;
        private string type;
        public RoomConnection RoomConnection;

        public Room(int id, string type)
        {
            this.id = id;
            this.viewers = new List<EClient>();
            this.type = type;
            CreateSocket();
        }

        private void CreateSocket()
        {
            int port = RoomManager.instance.generateNewPort();
            RoomConnection = new RoomConnection(port, this);
            RoomConnection.Start();
        }

        public int Id
        {
            get => id;
            set => id = value;
        }

        public List<EClient> Viewers => viewers;

        public string Type
        {
            get => type;
            set => type = value;
        }

        public void HandleLeave(EClient eClient)
        {
            this.Viewers.Remove(eClient);
            foreach (var client in viewers)
            {
                dynamic data = new Object();
                data.eventData = "MEMBER_LEAVE";
                data.name = eClient.Name;
                client.Send(JsonConvert.SerializeObject(data));
            }
        }
    }

    public class RoomConnection
    {

        private int port;
        public bool running = true;
        private List<RoomClient> connectedClients = new List<RoomClient>();
        private Room Room;
        public TcpListener listener;
        
        public RoomConnection(int port, Room Room)
        {
            this.port = port;
            this.Room = Room;
        }

        public void Start()
        {
            listener = new TcpListener(IPAddress.Any, port);
            Console.WriteLine("Room Connection #"+Room.Id+" - Server listening >> IP:"+IPAddress.Any+":" + port);
            Console.WriteLine("Room Connection #"+Room.Id+" - Watting connection...");
            Task.Run(() =>
            {
                while (running)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    RoomClient ytbClient = new RoomClient(client);
                    connectedClients.Add(ytbClient); 
                    Console.WriteLine("Room Connection #"+Room.Id+" - New connection ("+client.Client.RemoteEndPoint+")");
                }
            });
        }
        
    }

    public class RoomClient
    {
        
        private TcpClient Client;

        private bool running;

        private NetworkStream Stream;

        public string Name;

        public RoomClient(TcpClient client)
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
                    if (!IsConnected())
                    {
                        Console.WriteLine(Client.Client.RemoteEndPoint+ " has disconnected");
                    }
                    byte[] buf = new byte[1024];
                    Stream.Read(buf, 0, buf.Length);
                    string data = Encoding.UTF8.GetString(buf);
                    handleData(data);
                }
            });
        }

        public void Send(String data)
        {
            Client.Client.Send(Encoding.UTF8.GetBytes(data));
        }
        
        public bool IsConnected()
        {
            try
            {
                return !(Client.Client.Poll(1, SelectMode.SelectRead) && Client.Available == 0);
            }
            catch (SocketException) { return false; }
        }

        private void handleData(string data)
        {
            dynamic stuff = JsonConvert.DeserializeObject(data);
            if (stuff.type == "AUTH")
            {
                this.Name = stuff.name;
            }
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