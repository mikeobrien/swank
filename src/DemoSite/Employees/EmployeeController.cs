using System;
using System.Collections.Generic;
using System.Web.Http;
using Swank.Description;

namespace DemoSite.Employees
{
    public class EmployeeController : ApiController
    {
        public enum Options
        {
            Option1,
            Option2
        }

        public class InputModel
        {
            public string Value { get; set; }
            public string OptionalValue { get; set; }
            public string DefaultValue { get; set; }
            public Options OptionValue { get; set; }
            public string DepricatedValue { get; set; }
            public ChildModel ComplexValue { get; set; }
            public List<int> List { get; set; }
            public Dictionary<string, Options> Dictionary { get; set; }
        }

        public class OutputModel
        {
            public string Value { get; set; }
            public Options OptionValue { get; set; }
            public string DepricatedValue { get; set; }
            public ChildModel ComplexValue { get; set; }
            public List<int> List { get; set; }
            public Dictionary<string, Options> Dictionary { get; set; }
        }

        public class ChildModel
        {
            public string Value { get; set; }
            public int NumericValue { get; set; }
            public bool BooleanValue { get; set; }
            public DateTime DateValue { get; set; }
            public TimeSpan DurationValue { get; set; }
            public Guid GuidValue { get; set; }
        }

        [Route("employees")]
        [Description("Get All Departments")]
        public List<OutputModel> GetAll(string sortBy = null)
        {
            return null;
        }

        [Route("employees/{id}")]
        [Description("Get a Department")]
        public OutputModel Get(Guid id)
        {
            return null;
        }

        [Route("employees")]
        [Description("Adds a Department")]
        public OutputModel Post(InputModel model)
        {
            return null;
        }

        [Route("employees/{id}")]
        [Description("Updates a Department")]
        public OutputModel Put(InputModel model, Guid id)
        {
            return null;
        }

        [Route("employees/{id}")]
        [Description("Deletes a Department")]
        public OutputModel Delete(InputModel model, Guid id)
        {
            return null;
        }
    }
}