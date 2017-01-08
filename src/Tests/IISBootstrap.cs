using IISExpressBootstrapper;
using NUnit.Framework;

namespace Tests
{
    [SetUpFixture]
    public class IISBootstrap
    {
        private IISExpressHost _host;

        [OneTimeSetUp]
        public void StartIIS()
        {
            _host = new IISExpressHost("TestHarness", 61960);
        }

        [OneTimeTearDown]
        public void StopIIS()
        {
            _host.Dispose();
        }
    }
}
