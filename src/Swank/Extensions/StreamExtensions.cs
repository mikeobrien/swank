using System.IO;

namespace Swank.Extensions
{
    internal static class StreamExtensions
    {
        public static string ReadAllText(this Stream stream)
        {
            using (stream)
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static byte[] ReadAllBytes(this Stream stream)
        {
            using (stream)
            using (MemoryStream buffer = new MemoryStream())
            {
                stream.CopyTo(buffer);
                return buffer.ToArray();
            }
        }
    }
}
