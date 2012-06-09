using System.IO;
using System.Reflection;

namespace Torshify.Core
{
    internal class SpotifyLibExtractor
    {
        internal static void ExtractResourceToFile(string resourceName, string filename)
        {
            var baseDirectory = Path.GetDirectoryName(typeof(SpotifyLibExtractor).Assembly.Location);
            var libspotifyLocation = Path.Combine(baseDirectory, filename);
            if (!File.Exists(libspotifyLocation))
            {
                using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    using (FileStream fs = new FileStream(libspotifyLocation, FileMode.Create))
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