using System;
using System.IO;
using System.Text;

namespace NamedPipe.Common
{
    /// <summary>
    /// Defines the data protocol for reading and writing strings on our stream
    /// </summary>
    public class StringPipeStream
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StringPipeStream(Stream ioStream)
        {
            this.ioStream = ioStream;
            streamEncoding = new UnicodeEncoding();
        }

        /// <summary>
        /// Read from stream
        /// </summary>
        /// <returns></returns>
        public string ReadString()
        {
            int len = 0;

            len = ioStream.ReadByte() * 256;
            len += ioStream.ReadByte();
            byte[] inBuffer = new byte[len];
            ioStream.Read(inBuffer, 0, len);

            return streamEncoding.GetString(inBuffer);
        }

        /// <summary>
        /// Write to stream
        /// </summary>
        /// <param name="outString"></param>
        /// <returns></returns>
        public int WriteString(string outString)
        {
            byte[] outBuffer = streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }
            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }
}
