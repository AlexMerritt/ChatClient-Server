using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CSChat
{
    class ChatServer : Server
    {
        int m_id;
        string m_ipAddress;
        int m_port;

        List<HandleClientRequest> m_clients;
        uint m_currentClientId;

        public int ID
        {
            get { return m_id; }
        }

        public string IpAddress
        {
            get { return m_ipAddress; }
        }

        public int Port
        {
            get { return m_port; }
        }

        public ChatServer(int serverId)
        {
            m_id = serverId;
            m_ipAddress = Defines.ServerIpAddress;
            m_port = Defines.ServerPortStart + serverId;

            Initialize(serverId);
        }

        void Initialize(int serverId)
        {
            try
            {
                m_currentClientId = 0;
                m_clients = new List<HandleClientRequest>();
                IPAddress ipAd = IPAddress.Parse(m_ipAddress);

                m_listener = new TcpListener(ipAd, m_port);

                m_listener.Start();

                
            }
            catch (Exception e)
            {
                Console.WriteLine("Error...." + e.StackTrace);
            }
        }

        public void CloseServer()
        {
        }

        public void Run()
        {
            WaitForClientConnect();
        }

        protected override void ClientConnected(TcpClient client)
        {
            HandleClientRequest clRequest = new HandleClientRequest(m_currentClientId, client, this);
            clRequest.StartClient();

            m_clients.Add(clRequest);

            m_currentClientId++;

            clRequest.SendMessage(new DataPacket(0, "Welcome to server " + m_id.ToString()));
        }

        public void MessageRecieved(HandleClientRequest client, DataPacket packet)
        {
            string fixedString = client.ID + ": " + packet.Message;

            Console.WriteLine(fixedString);

            foreach (HandleClientRequest cl in m_clients)
            {
                if (client.ID != cl.ID)
                    cl.SendMessage(packet);
            }
        }

        public void BroadcastMessage(DataPacket packet)
        {
            Console.WriteLine(m_id + ": " + packet.Message);

            foreach (HandleClientRequest cl in m_clients)
            {
                cl.SendMessage(packet);
            }
        }
    }
}
