using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BingLibrary.Net.net
{
    public class UDPClient
    {
        public UdpClient udp;

        //public string IPAddress = "LocalHost";
        public IPEndPoint RemoteIpEndPoint;

        public bool Connect(int localPort, int targetPort)
        {
            try
            {
                udp = new UdpClient(localPort);
                udp.Client.SendTimeout = 1000;
                udp.Client.ReceiveTimeout = 1000;
                RemoteIpEndPoint = new IPEndPoint(System.Net.IPAddress.Loopback, targetPort);
                return true;
            }
            catch { return false; }
        }

        public bool Connect(int localPort, int targetPort, string targetIP)
        {
            try
            {
                udp = new UdpClient(localPort);
                udp.Client.SendTimeout = 1000;
                udp.Client.ReceiveTimeout = 1000;
                RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(targetIP), targetPort);
                return true;
            }
            catch { return false; }
        }

        public async Task<string> ReceiveAsync()
        {
            string tempS = "error";
            try
            {
                UdpReceiveResult x = await udp.ReceiveAsync();
                tempS = Encoding.UTF8.GetString(x.Buffer);
            }
            catch { tempS = "error"; }

            return tempS;
        }

        public async Task<bool> SendAsync(string mStrToSend, bool wait = true)
        {
            try
            {
                byte[] ByteToSend = System.Text.Encoding.UTF8.GetBytes(mStrToSend);
                await udp.SendAsync(ByteToSend, ByteToSend.Length, RemoteIpEndPoint);
                return true;
            }
            catch { return false; }
        }
    }
}