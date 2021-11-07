using System;
using DiscordRPC;
using DiscordRPC.Logging;

namespace EkipTogether
{
    public class DiscordRichPresence
    {
            public static DiscordRpcClient client;

            public static DiscordState currentState;
            
            public static void Initialize() 
            {
                /*
                Create a Discord client
                NOTE: 	If you are using Unity3D, you must use the full constructor and define
                         the pipe connection.
                */
                client = new DiscordRpcClient("906512122282901555");			
	
                //Set the logger
                client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

                //Subscribe to events
                client.OnReady += (sender, e) =>
                {
                    Console.WriteLine("Received Ready from user {0}", e.User.Username);
                };
		
                client.OnPresenceUpdate += (sender, e) =>
                {
                    Console.WriteLine("Received Update! {0}", e.Presence);
                };
	
                //Connect to the RPC
                client.Initialize();

                //Set the rich presence
                //Call this as many times as you want and anywhere in your code.
                client.SetPresence(new RichPresence()
                {
                    Details = "Ekip Together",
                    State = "Idle",
                    Timestamps = Timestamps.Now,
                    Assets = new Assets()
                    {
                        LargeImageKey = "icon_512",
                        LargeImageText = "Ekip Together",
                    }
                });	
            }

            public static void updateState(DiscordState state, string data)
            {
                if (currentState == state) return;
                currentState = state;
                client.UpdateStartTime(DateTime.Now);
                switch (state)
                {
                    case DiscordState.IDLE:
                        client.UpdateState("Idle");
                        break;
                    case DiscordState.PLAY:
                        client.UpdateState("Regarde le stream de " + data);
                        break;
                    case DiscordState.CREATE_ROOM:
                        client.UpdateState("Créer une Room...");
                        break;
                }
            }
            
            
    }

    public enum DiscordState
    {
        IDLE,
        PLAY,
        CREATE_ROOM,
    }
}