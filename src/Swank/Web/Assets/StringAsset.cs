using System.Text;

namespace Swank.Web.Assets
{
    public class StringAsset : IAsset
    {
        private readonly string _source;

        public StringAsset(string source)
        {
            _source = source;
        }

        public byte[] ReadBytes()
        {
            return Encoding.UTF8.GetBytes(_source);
        }

        public string ReadString()
        {
            return _source;
        }

        public static explicit operator StringAsset(string value)
        {
            return new StringAsset(value);
        }
    }
}