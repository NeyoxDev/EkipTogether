using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EkipServer
{
    public class RoomManager
    {

        public static RoomManager instance;
        
        private List<EClient> connectedClients = new List<EClient>();
        public List<Room> rooms = new List<Room>();
        public bool running;

        public RoomManager()
        {
            instance = this;
        }
        
        public void StartServer()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 6969);
            Console.WriteLine("Server listening >> IP:"+IPAddress.Any+":" + 6969);
            Console.WriteLine("Watting connection...");
            Task.Run(() =>
            {
                while (running)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    EClient ytbClient = new EClient(client);
                    connectedClients.Add(ytbClient); 
                    Console.WriteLine("New connection ("+client.Client.RemoteEndPoint+")");
                }
            });
        }

        public bool AuthorHasRoom(EClient client)
        {
            return getAuthorRoom(client) != null;
        }

        public Room getAuthorRoom(EClient client)
        {
            return (from room in rooms where room.Viewers.Contains(client) select room).First();
        }

        public int generateNewPort()
        {
            int port = new Random().Next(1500) + 1000;
            while ((from room in rooms where room.RoomConnection != null && ((IPEndPoint) room.RoomConnection.listener.Server.RemoteEndPoint).Port != port select room).Any())
            {
                port = new Random().Next(1500) + 1000;
            }
            return port;
        }
    }

    public class EClient
    {
        private TcpClient Client;

        private bool running;

        private NetworkStream Stream;

        public string Name;

        public EClient(TcpClient client)
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
            if (stuff.type == "CREATE_ROOM")
            {
                RoomManager instance = RoomManager.instance;
                Room room = new Room(instance.rooms.Count + 1, stuff.appType);
                if (instance.AuthorHasRoom(this))
                {
                    instance.getAuthorRoom(this).HandleLeave(this);
                }
                instance.rooms.Add(room);
                dynamic d = new Object();
                d.type = "ROOM_CREATED";
                d.port = ((IPEndPoint) room.RoomConnection.listener.Server.RemoteEndPoint).Port;
                Send(JsonConvert.SerializeObject(d));
            }else if (stuff.type == "AUTH")
            {
                this.Name = stuff.name;
            }
        }
    }
}