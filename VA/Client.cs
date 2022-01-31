using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows;
using Newtonsoft.Json;

namespace VA
{
    // State object for receiving data from remote device.  
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }
    public class Client
    {
        private const int port = 11000;

        private static Socket client;
        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        // The response from the remote device.  
        private static String response = String.Empty;

        private static async void StartClient()
        {
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // The name of the
                // remote device is "host.contoso.com".  
                //IPHostEntry ipHostInfo = Dns.GetHostEntry("host.contoso.com");
                //IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("192.168.1.209"), port);

                // Create a TCP/IP socket.  
                client = new Socket(IPAddress.Parse("192.168.1.209").AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Send test data to the remote device.  
                Send("");

                // Receive the response from the remote device.  
                Receive();
                receiveDone.WaitOne();

                // Write the response to the console.  
              

                // Release the socket.  
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void TurnOff()
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive()
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    if(IsReport)
                        state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
                    else
                    {
                        state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    }
                    if (state.sb.Length > 1 && !IsReport)
                    {
                       
                        vm.ServerAnswer = state.sb.ToString();
                        state.sb.Clear();
                    }
                    else
                    {

                        DeserializeData(state.sb.ToString());
                        state.sb.Clear();

                    }
                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                    
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1|| !IsReport)
                    {
                        vm.ServerAnswer = state.sb.ToString();
                    }
                   
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            }
            
            catch (Exception e)
            {
               MessageBox.Show(e.ToString());
              
            }
        }
        static string GetIPAddress()
        {
            String address = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                address = stream.ReadToEnd();
            }

            int first = address.IndexOf("Address: ") + 9;
            int last = address.LastIndexOf("</body>");
            address = address.Substring(first, last - first);

            return address;
        }
        private static void Send( String data)
        {
            var mac = GetMac().ToArray();
            var z = new char[mac.Length + data.Length];
            mac.CopyTo(z, 0);
            data.ToArray().CopyTo(z, mac.Length);
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(z);

            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
               

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        public Client()
        {
           Task.Run(StartClient);
        }

        




        public static bool IsEnter { get; private set; }
        public static bool IsReport { get; private set; } 
        public static bool IsSault { get; private set; } 
        private string _serverResponce;


        public static Client сlient(MainWindowVM Vm)
        {
            vm = Vm;
                if (_сlient == null)
                {
                    _сlient = new Client();
                }

                return _сlient;
           
        }


        public static MainWindowVM vm { get; set; }
        private static Client _сlient;

        public void Login(string login, string pass)
        {
            //var Hesh = sha256_hash(pass);
            var reqString = $"LOGIN login {login} password {pass}";
            ReqestTypeSwither(2);
            Send(reqString);
        }
        public void Register(string login, string password, string role)
        {
            //var Hesh = sha256_hash(password);
            var reqString = $"REGISTR login {login} password {password} role u";
            ReqestTypeSwither(2);
            Send(reqString);
        }

        public void GetReport(string type, string param)
        {
            var reqString = $"REPORT type {type} param {param}";
            ReqestTypeSwither(3);
            Send(reqString);
        }
        public void GetSault(string login = "nan")
        {
            var reqString = $"SAULT login {login}";
            ReqestTypeSwither(1);
            Send(reqString);
        }
        /// <summary>
        /// это не работает
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public String sha256_hash(String value)
        {
            using (SHA256 hash = SHA256Managed.Create())
            {
                return String.Concat(hash
                    .ComputeHash(Encoding.UTF8.GetBytes(value))
                    .Select(item => item.ToString("x2")));
            }
        }


        public static void DeserializeData(string json)
        {
            ReqestTypeSwither();
            var a = JsonConvert.DeserializeObject<DataSet>(json);
            vm.Db = a.Tables[0];
        }

        public static void ReqestTypeSwither(int i = 0)
        {
            switch (i)
            {
                case 1:
                    IsEnter = false;
                    IsReport = false;
                    IsSault = true;
                    break;
                case 2:
                    IsEnter = true;
                    IsReport = false;
                    IsSault = false;
                    break;
                case 3:
                    IsEnter = false;
                    IsReport = true;
                    IsSault = false;
                    break;
                case 0:
                    IsEnter = false;
                    IsReport = false;
                    IsSault = false;
                    break;
            }
        }

        public static string GetMac()
        {
            const int MIN_MAC_ADDR_LENGTH = 12;
            string macAddress = string.Empty;
            long maxSpeed = -1;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                Console.WriteLine(
                    "Found MAC Address: " + nic.GetPhysicalAddress() +
                    " Type: " + nic.NetworkInterfaceType);

                string tempMac = nic.GetPhysicalAddress().ToString();
                if (nic.Speed > maxSpeed &&
                    !string.IsNullOrEmpty(tempMac) &&
                    tempMac.Length >= MIN_MAC_ADDR_LENGTH)
                {
                  
                    maxSpeed = nic.Speed;
                    macAddress = tempMac;
                }
            }

            return macAddress+" ";
        }

        
    }
}

