using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CSChat
{
    class ServerManager : Server
    {
        List<ChatServer> m_chatServers;

        public ServerManager()
        {
            IPAddress ipAd = IPAddress.Parse(Defines.ServerIpAddress);

            m_listener = new TcpListener(ipAd, Defines.ServerPortStart);

            m_listener.Start();

            CreateChannels();

            WaitForClientConnect();


            Random rand = new Random();

            //while (true)
            //{
            //    System.Threading.Thread.Sleep(500);

            //    int rndSvr = rand.Next(m_chatServers.Count());

            //    m_chatServers[rndSvr].BroadcastMessage(new DataPacket(0, "Channel " + m_chatServers[rndSvr].ID + ": " + rand.NextDouble().ToString()));
            //}
        }

        private void CreateChannels()
        {
            m_chatServers = new List<ChatServer>();

            for (int i = 0; i < 10; ++i)
            {
                // Create a new thread for each server
                ChatServer s = new ChatServer(i + 1);
                m_chatServers.Add(s);

                Thread t = new Thread(s.Run);
                t.Start();
            }
        }


        protected override void ClientConnected(TcpClient client)
        {
            // A client just connected
            // Ask the client what channels they would like to connect
            byte[] sendBytes = DataPacket.ConvertToBytes(new DataPacket(0, GetServerAddresses()));

            Console.WriteLine("Sending Client channel addresses");
            client.Client.Send(sendBytes);

            //client.Close();
        }

        string GetServerAddresses()
        {
            string output = "";

            foreach (ChatServer server in m_chatServers)
            {
                int id = server.ID;

                output += server.ID.ToString() + ',' + server.IpAddress + ':' + server.Port.ToString() + ' ';
            }

            return output;
        }
    }
}
