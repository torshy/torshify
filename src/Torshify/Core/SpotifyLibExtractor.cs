using System.IO;
using System.Reflection;

namespace Torshify.Core
{
    internal class SpotifyLibExtractor
    {
        internal static void ExtractResourceToFile(string resourceName, string filename)
        {
            if (!File.Exists(filename))
            {
                using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    using (FileStream fs = new FileStream(filename, FileMode.Create))
                    {
                        byte[] b = new byte[s.Length];
                        s.Read(b, 0, b.Length);
                        fs.Write(b, 0, b.Length);
                    }
                }
            }
        }
    }
}