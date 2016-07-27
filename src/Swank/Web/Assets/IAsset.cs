namespace Swank.Web.Assets
{
    public interface IAsset
    {
        byte[] ReadBytes();
        string ReadString();
    }

    public interface IFileAsset : IAsset
    {
        string Path { get; }
        string RelativePath { get; }
    }
}
