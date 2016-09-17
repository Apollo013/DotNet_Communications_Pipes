using System.IO;

namespace NamedPipe.Common
{
    public class FilePipeStreamReader
    {
        private string _filename;
        private StringPipeStream _sps;

        public FilePipeStreamReader(StringPipeStream sps, string filename)
        {
            _filename = filename;
            _sps = sps;
        }

        public void Start()
        {
            string contents = File.ReadAllText(_filename);
            _sps.WriteString(contents);
        }
    }
}
