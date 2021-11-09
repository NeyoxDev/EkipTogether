using System;
using System.Threading;

namespace EkipServer
{
    class Program
    {
        public static RoomManager RoomManager = new RoomManager();
        
        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            RoomManager.StartServer();
            Thread.Sleep(-1);
        }
    }
}
