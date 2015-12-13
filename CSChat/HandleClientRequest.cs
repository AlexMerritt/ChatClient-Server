using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CSChat
{
    class HandleClientRequest
    {
        TcpClient m_client;
        ChatServer m_server;
        NetworkStream m_stream = null;
        uint m_id;

        public uint ID
        {
            get { return m_id; }
        }

        public HandleClientRequest(uint id, TcpClient client, ChatServer server)
        {
            m_id = id;
            m_client = client;
            m_server = server;
        }

        public void StartClient()
        {
            m_stream = m_client.GetStream();

            WaitForRequest();
        }

        private void WaitForRequest()
        {
            byte[] buffer = new byte[m_client.ReceiveBufferSize];

            m_stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            NetworkStream stream = m_client.GetStream();

            try
            {
                int read = stream.EndRead(ar);
                if (read == 0)
                {
                    m_stream.Close();
                    m_client.Close();
                    return;
                }

                byte[] buffer = ar.AsyncState as byte[];
                DataPacket packet = DataPacket.ConvertFromBytes(buffer);

                m_server.MessageRecieved(this, packet);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error....." + e.StackTrace);
            }

            WaitForRequest();
        }

        public void SendMessage(DataPacket packet)
        {
            NetworkStream stream = m_client.GetStream();

            byte[] sendBytes = DataPacket.ConvertToBytes(packet);
            stream.Write(sendBytes, 0, sendBytes.Length);
            stream.Flush();
        }
    }
}
