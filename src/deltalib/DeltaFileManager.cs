using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

// DeltaPresentation namespace
namespace DeltaPresentation
{
    /// <summary>
    /// This class holds the methods for reading and writing a presentation from/to a .dp (Delta Presentation) file
    /// </summary>
    public abstract class DeltaFileManager
    {
        const string MAGIC_WORD_PRESENTATION = "DeP";
        const string MAGIC_WORD_SLIDE = "DeS";
        /// <summary>
        /// This method writes the presentation to a file
        /// </summary>
        /// <param name="sources">The HTML source list of the slides</param>
        /// <param name="filename">The file name</param>
        public static void Save(Dictionary<string, string> sources, string filename)
        {
            string header = makeHeader();
            string body = makeBody(sources);

            FileStream stream = File.OpenWrite(filename);
            DeflateStream deflate = new DeflateStream(stream, CompressionMode.Compress);
            UTF8Encoding utf8 = new UTF8Encoding();
            StreamWriter writer = new StreamWriter(deflate, utf8);
            writer.Write(header);
            writer.Write(body);
            writer.Close();
            stream.Close();
        }

        /// <summary>
        /// This method read the presentation from a file
        /// </summary>
        /// <param name="filename">The file name</param>
        /// <returns>Returns a list of HTML sources</returns>
        public static Dictionary<string, string> Load(string filename)
        {
            Dictionary<string, string> sources = new Dictionary<string, string>();
            FileStream stream = File.Open(filename, FileMode.Open);
            DeflateStream deflate = new DeflateStream(stream, CompressionMode.Decompress);
            UTF8Encoding utf8 = new UTF8Encoding();
            StreamReader reader = new StreamReader(deflate, utf8);

            string content = reader.ReadToEnd();

            reader.Close();
            deflate.Close();
            stream.Close();

            // Splitting the content
            string[] split = content.Split('\0');

            // Gets the header
            string header = split[0];

            if (header.Substring(0, 3) != MAGIC_WORD_PRESENTATION) throw new ArgumentException();

            // Gets the content
            for (int i = 1; i < (split.Length - 1); i++)
            {
                if (split[i].Substring(0, 3) != MAGIC_WORD_SLIDE) continue;
                string name = split[i].Substring(3, split[i].IndexOf((char)1) - 3);
                string source = split[i].Substring(split[i].IndexOf((char)1) + 1, split[i].Length - 1 - split[i].IndexOf((char)1));
                sources.Add(name, source);
            }
            
            return sources;
        }

        private static string makeHeader()
        {
            string header = "";
            
            // Magic word
            header += MAGIC_WORD_PRESENTATION;

            // Date
            header += (char)DateTime.Now.Day;
            header += (char)DateTime.Now.Month;
            header += (char)(DateTime.Now.Year / 100);
            header += (char)(DateTime.Now.Year - (DateTime.Now.Year / 100 * 100));

            // Author
            header += Environment.UserName;
            header += '\0';

            return header;
        }

        private static string makeBody(Dictionary<string, string> sources) {
            string body = "";
            foreach (KeyValuePair<string, string> pair in sources) {
                body += makeSlideChunk(pair.Key, pair.Value);
            }
            return body;
        }

        private static string makeSlideChunk(string name, string source)
        {
            return MAGIC_WORD_SLIDE + name + (char)1 + source + '\0';
        }
    }
}
