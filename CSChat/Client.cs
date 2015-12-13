using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace CSChat
{
    class Client
    {
        List<TcpClient> m_clients;
        bool m_doneWithMainServer = false;
        TcpClient m_mainServerClient;

        Dictionary<int, ServerAddress> m_chatServers;

        public Client()
        {
            Initialize();
        }

        void Initialize()
        {
            try
            {
                Console.WriteLine("Connecting");

                //ConnectToMainServer();

                //while (!m_doneWithMainServer)
                //{
                //    System.Threading.Thread.Sleep(500);
                //}

                //ConnectToStream(0);

                //bool done = false;

                //while (!done)
                //{
                //    Console.WriteLine("Enter the server you would like to connect");
                //    foreach (KeyValuePair<int, ServerAddress> servers in m_chatServers)
                //    {
                //        Console.WriteLine(servers.Key);
                //    }
                //    string result = Console.ReadLine();
                //    int id;

                //    ServerAddress serverAddress;

                //    if (int.TryParse(result, out id))
                //    {
                //        m_chatServers.TryGetValue(id, out serverAddress);

                //        // If this exists in the dict then load up the server
                //        if (serverAddress != null)
                //        {
                //            TcpClient client = new TcpClient();
                //            client.Connect(serverAddress.IpAddress, serverAddress.Port);

                //            m_mainServerClient = client;

                //            m_stream = m_mainServerClient.GetStream();

                //            done = true;
                //        }
                //    }
                //}

                //WaitForRequest();

                //Console.WriteLine("Connected to the server");

                m_clients = new List<TcpClient>();

                TcpClient client = new TcpClient();
                client.Connect(Defines.ServerIpAddress, Defines.ServerPortStart);

                m_clients.Add(client);

                WaitForRequest(client.GetStream());

                while (true)
                {
                    string msg = Console.ReadLine();

                    if (msg.Substring(0, 2) == "/c")
                    {
                        ConnectToStream(msg.Substring(2).Trim());
                    }
                    else
                    {
                        SendMessage(msg);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error...." + e.StackTrace);
            }
        }

        void ConnectToMainServer()
        {
            m_chatServers = new Dictionary<int, ServerAddress>();

            m_mainServerClient = new TcpClient();
            m_mainServerClient.Connect(Defines.ServerIpAddress, Defines.ServerPortStart);

            NetworkStream stream = m_mainServerClient.GetStream();

            byte[] buffer = new byte[m_mainServerClient.ReceiveBufferSize];

            stream.BeginRead(buffer, 0, buffer.Length, MainServerReadCallback, buffer);
        }

        void ConnectToStream(string server)
        {
            int id;
            ServerAddress serverAddress;

            if (int.TryParse(server, out id))
            {
                m_chatServers.TryGetValue(id, out serverAddress);

                // If this exists in the dict then load up the server
                if (serverAddress != null)
                {
                    TcpClient client = new TcpClient();
                    client.Connect(serverAddress.IpAddress, serverAddress.Port);

                    m_clients.Add(client);

                    WaitForRequest(client.GetStream());
                }
            }
        }


        void MainServerReadCallback(IAsyncResult ar)
        {
            NetworkStream stream = m_mainServerClient.GetStream();
            
            try
            {
                int read = stream.EndRead(ar);
                if (read == 0)
                {
                    stream.Close();
                    m_mainServerClient.Close();
                    return;
                }

                byte[] buffer = ar.AsyncState as byte[];
                DataPacket packet = DataPacket.ConvertFromBytes(buffer);

                string[] servers = packet.Message.Split(' ');

                foreach (string s in servers)
                {
                    

                    int idDelim = s.IndexOf(',');
                    int ipDelim = s.IndexOf(':');

                    
                    if (idDelim >= 0 || ipDelim >= 0)
                    {
                        int id;
                        string ipAddress;
                        int port;

                        id = Convert.ToInt32(s.Substring(0, idDelim));
                        ipAddress = s.Substring(idDelim + 1, ipDelim - idDelim - 1);
                        port = Convert.ToInt32(s.Substring(ipDelim + 1));


                        m_chatServers.Add(id, new ServerAddress(ipAddress, port));
                    }
                }

                m_mainServerClient.Close();

                m_doneWithMainServer = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error....." + e.StackTrace);
            }
        }

        public void Close()
        {
        }

        void SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            

            foreach (TcpClient client in m_clients)
            {
                NetworkStream stream = client.GetStream();

                byte[] buffer = DataPacket.ConvertToBytes(new DataPacket(0, message));
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }
        }

        private void WaitForRequest(NetworkStream stream)
        {
            byte[] buffer = new byte[65532];// m_client.ReceiveBufferSize];

            stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, stream);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            NetworkStream stream = (NetworkStream)ar.AsyncState;

            try
            {
                int read = stream.EndRead(ar);
                if (read == 0)
                {
                    return;
                }

                byte[] buffer = new byte[65532]; ;// = ar.AsyncState as byte[];
                //string data = Encoding.Default.GetString(buffer, 0, read);

                read = stream.Read(buffer, 0, 65532);

                DataPacket p = DataPacket.ConvertFromBytes(buffer);

                Console.WriteLine(p.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error...." + e.StackTrace);
            }

            WaitForRequest(stream);
        }
    }
}
