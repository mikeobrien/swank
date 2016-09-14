using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bender;
using NUnit.Framework;
using Should;
using Swank.Extensions;
using Swank.Specification;

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
                !x.StartsWith("Tests.Unit.Description.CodeExamples.") &&
                !x.StartsWith("Tests.Unit.Web.Assets."));
        }
    }
}