using NUnit.Framework;

namespace Tests.Unit
{
    [TestFixture]
    public class EmbeddedCommentsTests
    {
        [Test]
        public void embedded_comments_should_match_types()
        {
            Swank.Description.Assert.AllEmbeddedCommentsMatchTypes(x => 
                !x.StartsWith("Tests.Unit.Configuration.ConfigurationDslTests") &&
                !x.StartsWith("Tests.Unit.Extensions.") && 
                !x.EndsWith(".Comments.md") &&
                !x.EndsWith(".Resource.md") &&
                !x.StartsWith("Tests.Unit.Description.CodeExamples.") &&
                !x.StartsWith("Tests.Unit.Web.Assets."));
        }
    }
}