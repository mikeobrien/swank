using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.Http;
using Swank.Description;
using Swank.Extensions;

namespace TestHarness.TestDrive
{
    [Resource("testdrive")]
    public class TestDriveController : ApiController
    {
        public class Model
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; } = "Fark";
            public string LastName { get; set; } = "Farker";
            public string Address1 { get; set; } = "100 Fark Lane";
            public string Address2 { get; set; } = "Suite 100";
            public string City { get; set; } = "Farkadelphia";
            public string State { get; set; } = "FA";
            public string Zip { get; set; } = "44663";
        }

        [Route("testdrive/error/400")]
        public IHttpActionResult Get400Error()
        {
            return BadRequest();
        }

        [Route("testdrive/error/500")]
        public IHttpActionResult Get500Error()
        {
            return InternalServerError();
        }

        [Secure]
        [Route("testdrive/secure")]
        public IHttpActionResult GetSecure()
        {
            var auth = Request.Headers.Authorization.Parameter;
            if (auth.IsNullOrEmpty() || System.Text.Encoding.UTF8
                .GetString(Convert.FromBase64String(auth)) != "fark:farker")
                return StatusCode(HttpStatusCode.Unauthorized);
            return Ok(new Model { Id = Guid.NewGuid() });
        }

        [Route("testdrive")]
        public List<Model> GetAll(string sort = null)
        {
            return new List<Model>
            {
                new Model { Id = Guid.NewGuid(), LastName = "Pauli" },
                new Model { Id = Guid.NewGuid(), LastName = "Bohr" },
                new Model { Id = Guid.NewGuid(), LastName = "Heisenberg" },
                new Model { Id = Guid.NewGuid(), LastName = "Born" },
                new Model { Id = Guid.NewGuid(), LastName = "Dirac" }
            }.OrderBy(x => (string)typeof(Model)
                .GetProperty(sort ?? "FirstName").GetValue(x)).ToList();
        }

        [Route("testdrive/{id}")]
        public Model Get(Guid id)
        {
            return new Model { Id = id };
        }

        [Route("testdrive")]
        public Model Post(Model model)
        {
            model.Id = Guid.NewGuid();
            return model;
        }

        [Route("testdrive/{id}")]
        public Model Put(Model model, Guid id)
        {
            model.Id = id;
            return model;
        }

        [Route("testdrive/{id}")]
        public Model Delete(Guid id)
        {
            return new Model { Id = Guid.NewGuid() };
        }

        [Route("testdrive/slow")]
        public Model GetSlow()
        {
            Thread.Sleep(3000);
            return new Model { Id = Guid.NewGuid() };
        }

        [Route("testdrive/binary")]
        public HttpResponseMessage PostBinary()
        {
            var bytes = Request.Content.ReadAsByteArrayAsync().Result;
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(bytes)
            };
            result.Content.Headers.ContentType = 
                new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }
    }
}