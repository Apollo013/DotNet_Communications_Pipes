using System;
using System.IO;
using System.IO.Pipes;

namespace AnonymousPipe.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                using (PipeStream client = new AnonymousPipeClientStream(PipeDirection.In, args[0]))
                {
                    // Display trans mode
                    Console.WriteLine($"[CLIENT] Current TransmissionMode: {client.TransmissionMode}");

                    using (StreamReader reader = new StreamReader(client))
                    {
                        // Display the read text to the console
                        string temp;

                        // Wait for 'sync message' from the server.
                        do
                        {
                            Console.WriteLine("[CLIENT] Wait for sync...");
                            temp = reader.ReadLine();
                        }
                        while (!temp.StartsWith("SYNC"));

                        Console.WriteLine($"[CLIENT] Echo: {temp}");
                    }
                }
            }

            Console.Write("[CLIENT] Press Enter to continue...");
            Console.ReadLine();
        }
    }
}
