using System.Collections.Generic;
using System.Linq;
using Swank.Extensions;
using Swank.Specification;

namespace Swank.Web.Handlers.Templates
{
    public class NamespaceModel
    {
        public List<NamespaceModel> Children { get; set; }
        public string Name { get; set; }
        public List<string> FullName { get; set; }
        public List<Endpoint> Endpoints { get; set; }
    }

    public class NamespaceDescriptionService
    {
        private readonly Configuration.Configuration _configuration;

        public NamespaceDescriptionService(Configuration.Configuration configuration)
        {
            _configuration = configuration;
        }

        public List<NamespaceModel> Create(List<Module> specification)
        {
            if (_configuration.TemplateNamespaceIncludesModule)
            {
                return specification.Select(x =>
                {
                    var @namespace = new NamespaceModel
                    {
                        Name = x.Name,
                        FullName = new List<string> { x.Name },
                        Children = new List<NamespaceModel>(),
                        Endpoints = new List<Endpoint>()
                    };
                    AddEndpoints(@namespace,
                        x.Resources.SelectMany(r => r.Endpoints).ToList());
                    return @namespace;
                }).ToList(); 
            }
            else
            {
                var @namespace = new NamespaceModel
                {
                    Children = new List<NamespaceModel>(),
                    Endpoints = new List<Endpoint>()
                };
                AddEndpoints(@namespace,
                    specification.SelectMany(m => m.Resources)
                    .SelectMany(r => r.Endpoints).ToList());
                return @namespace.Children;
            }
        }

        private void AddEndpoints(NamespaceModel root, List<Endpoint> endpoints)
        {
            endpoints.ForEach(endpoint =>
            {
                var previous = root;
                endpoint.Namespace.ForEach(n =>
                {
                    if (n == previous.Name) return;
                    var next = previous.Children.FirstOrDefault(c => c.Name == n);
                    if (next != null)
                    {
                        previous = next;
                        return;
                    }
                    next = new NamespaceModel
                    {
                        Name = n,
                        FullName = previous.FullName.Concat(n).ToList(),
                        Children = new List<NamespaceModel>(),
                        Endpoints = new List<Endpoint>()
                    };
                    previous.Children.Add(next);
                    previous = next;
                });
                previous.Endpoints.Add(endpoint);
            });
        }
    }
}
