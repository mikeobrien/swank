using System;

namespace Swank.Web.Assets
{
    public class LazyAsset : IAsset
    {
        private readonly Lazy<IAsset> _asset;

        public LazyAsset(Func<IAsset> asset)
        {
            _asset = new Lazy<IAsset>(asset);
        }

        public byte[] ReadBytes()
        {
            return _asset.Value.ReadBytes();
        }

        public string ReadString()
        {
            return _asset.Value.ReadString();
        }
    }
}
