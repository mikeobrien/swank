namespace Swank.Web.Templates
{
    public interface ITemplate
    {
        byte[] RenderBytes<TModel>(TModel model);
        string RenderString<TModel>(TModel model);
    }
}
