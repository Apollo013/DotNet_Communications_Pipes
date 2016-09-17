using System;
using System.IO.Pipes;
using System.Text;

namespace BasicSendReceive.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client:");

            using (NamedPipeClientStream client = new NamedPipeClientStream("test-pipe"))
            {
                // Connect to server
                client.Connect();

                // Receive message
                var messageBytes = new Byte[256];
                int size = client.Read(messageBytes, 0, messageBytes.Length);
                var message = Encoding.UTF8.GetString(messageBytes, 0, size);
                Console.WriteLine(message);

                // Respond
                var response = Encoding.UTF8.GetBytes("Thanks");
                client.Write(response, 0, response.Length);
            }

            Console.ReadKey();
        }
    }
}
