using NamedPipe.Common;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace NamedPipe.Server
{
    class Program
    {
        // Number of server threads to run
        private static int numThreads = 4;

        static void Main(string[] args)
        {
            int i; // Thread watch count

            Console.WriteLine("Waiting for client connect...\n");

            // Container for servers
            Thread[] servers = new Thread[numThreads];

            // Create & start servers
            for (i = 0; i < numThreads; i++)
            {
                servers[i] = new Thread(ServerThread);
                servers[i].Start();
            }

            // Delay main thread
            Thread.Sleep(250);

            // Join our threads (Blocks the calling thread until all are finished)
            while (i > 0)
            {
                for (int j = 0; j < numThreads; j++)
                {
                    if (servers[j].Join(250))
                    {
                        Console.WriteLine($"Server thread[{servers[j].ManagedThreadId}] finished.");
                        servers[j] = null;
                        i--;
                    }
                }
            }

            // Finish up
            Console.WriteLine("\nServer threads exhausted, exiting.");
        }

        private static void ServerThread(object data)
        {
            NamedPipeServerStream pipeServer = new NamedPipeServerStream("test-pipe", PipeDirection.InOut, numThreads);

            // Get thread id of current server
            int threadId = Thread.CurrentThread.ManagedThreadId;

            // Wait for a client to connect
            pipeServer.WaitForConnection();

            // Notify when a client connects
            Console.WriteLine($"Client connected on thread[{threadId}].");

            try
            {
                // Read the request from the client. Once the client has written to the pipe, its security token will be available.
                StringPipeStream sps = new StringPipeStream(pipeServer);

                // Verify our identity to the connected client using a string that the client anticipates.
                sps.WriteString("I am the one true server!");

                // Get name of file to read (from client)
                string filename = sps.ReadString();

                // Read in the contents of the file while impersonating the client.
                FilePipeStreamReader fileReader = new FilePipeStreamReader(sps, filename);

                // Display the name of the user we are impersonating.
                Console.WriteLine($"Reading file: {filename} on thread[{threadId}] as user: {pipeServer.GetImpersonationUserName()}.");
                pipeServer.RunAsClient(fileReader.Start);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }

            pipeServer.Close(); // close this server instance
        }
    }
}
