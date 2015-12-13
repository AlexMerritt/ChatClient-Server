using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CSChat
{
    class Server
    {
        protected TcpListener m_listener;

        protected void WaitForClientConnect()
        {
            m_listener.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), null);
        }

        virtual protected void OnClientConnect(IAsyncResult ar)
        {
            try
            {
                TcpClient client = default(TcpClient);
                client = m_listener.EndAcceptTcpClient(ar);

                ClientConnected(client);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error...." + e.StackTrace);
            }

            WaitForClientConnect();
        }

        protected virtual void ClientConnected(TcpClient client)
        {

        }
    }
}
