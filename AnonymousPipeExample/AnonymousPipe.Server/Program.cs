using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace AnonymousPipe.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // Process for running the client
            Process client = new Process();
            client.StartInfo.FileName = "AnonymousPipe.Client.exe"; // path for our client executable

            using (AnonymousPipeServerStream server = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
            {
                // Display trans mode
                Console.WriteLine($"[SERVER] Current TransmissionMode: {server.TransmissionMode}");

                // Pass the client process a handle to the server
                client.StartInfo.Arguments = server.GetClientHandleAsString();
                client.StartInfo.UseShellExecute = false;
                client.Start();

                // free any resources
                /*
                 * The DisposeLocalCopyOfClientHandle method should be called after the client handle has been passed to the client. 
                 * If this method is not called, the AnonymousPipeServerStream object will not receive notice when the client disposes 
                 * of its PipeStream object.
                 */
                server.DisposeLocalCopyOfClientHandle();

                try
                {
                    using (StreamWriter writer = new StreamWriter(server))
                    {
                        writer.AutoFlush = true;

                        // send message
                        writer.Write("SYNC");

                        // wait for the other end of the pipe to read the message 
                        server.WaitForPipeDrain();

                        // prompt for some input to send to client
                        Console.Write("[SERVER] Enter text: ");
                        writer.WriteLine(Console.ReadLine());
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                Thread.Sleep(60000);
                client.WaitForExit();
                client.Close();
                Console.WriteLine("[SERVER] Done");
            }
        }
    }
}
