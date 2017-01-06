using System.Collections.Generic;

namespace Swank.Configuration
{
    public class AuthenticationScheme
    {
        public string Name { get; set; }
        public List<AuthenticationComponent> Components { get; set; }
    }

    public enum AuthenticationLocation { Header, Querystring, UrlParameter }

    public class AuthenticationComponent
    {
        public string Name { get; set; }
        public AuthenticationLocation Location { get; set; }
        public string ClientSideGenerator { get; set; }
        public List<AuthenticationParameter> Parameters { get; set; }
    }

    public class AuthenticationParameter
    {
        public string Name { get; set; }
        public bool Hide { get; set; }
    }
}
