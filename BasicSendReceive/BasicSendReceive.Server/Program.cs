using System;
using System.IO.Pipes;
using System.Text;

namespace BasicSendReceive.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server: Waiting for client to connect");

            using (NamedPipeServerStream server = new NamedPipeServerStream("test-pipe"))
            {
                // The message to transmit
                var message = Encoding.UTF8.GetBytes("Hello from the server");

                // Wait for client to connect and then send message
                server.WaitForConnection();
                server.Write(message, 0, message.Length);

                // Get response
                var responseBytes = new Byte[256];
                int size = server.Read(responseBytes, 0, responseBytes.Length);
                var response = Encoding.UTF8.GetString(responseBytes, 0, size);
                Console.WriteLine(response);
            }

            Console.ReadKey();
        }
    }
}
