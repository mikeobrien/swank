﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Swank.Configuration;
using Swank.Extensions;
using Swank.Specification;

namespace Swank.Web.Handlers.App
{
    public class LinkModel
    {
        public string Name { get; set; }
        public string FragmentId { get; set; }
    }

    public class ModuleModel
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Overview { get; set; }
        public bool HasOverview { get; set; }
        public bool Collapsed { get; set; }
        public List<string> Resources { get; set; }
    }

    public class AppModel
    {
        public string AppUrl { get; set; }
        public string SpecificationUrl { get; set; }
        public string FavIconUrl { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string Overview { get; set; }
        public List<LinkModel> OverviewLinks { get; set; }
        public List<ModuleModel> Modules { get; set; }
        public string Copyright { get; set; }
        public List<string> Scripts { get; set; }
        public List<string> Stylesheets { get; set; }
        public List<string> IEPolyfills { get; set; }
        public bool DisplayJsonData { get; set; }
        public bool DisplayXmlData { get; set; }
        public bool HideTestDrive { get; set; }
        public List<AuthenticationScheme> AuthenticationSchemes { get; set; }
    }

    public class AppHandler : CachingHandlerBase
    {
        private readonly Configuration.Configuration _configuration;
        private readonly SpecificationService _specification;

        public AppHandler(Configuration.Configuration configuration, 
            SpecificationService specification) : 
            base(configuration, Mime.TextHtml, true)
        {
            _configuration = configuration;
            _specification = specification;
        }

        protected override byte[] CreateResponse(HttpRequestMessage request)
        {
            var app = new AppModel
            {
                AppUrl = _configuration.AppUrl,
                SpecificationUrl = _configuration.SpecificationUrl,
                FavIconUrl = _configuration.FavIconUrl,
                Title = _configuration.Title ?? _configuration.Name,
                Name = _configuration.Name,
                LogoUrl = _configuration.LogoUrl,
                Overview = _configuration.Overview.ReadString(),
                OverviewLinks = new List<LinkModel>(
                    _configuration.OverviewLinks
                        .Select(x => new LinkModel
                        {
                            Name = x.Name,
                            FragmentId = x.FragmentId
                        })),
                Copyright = _configuration.Copyright,
                Scripts = _configuration.Scripts.Select(x => x.GetUrl()).ToList(),
                Stylesheets = _configuration.Stylesheets.Select(x => x.GetUrl()).ToList(),
                IEPolyfills = _configuration.IEPolyfills.Select(x => x.GetUrl()).ToList(),
                DisplayJsonData = _configuration.DisplayJsonData,
                DisplayXmlData = _configuration.DisplayXmlData,
                HideTestDrive = _configuration.HideTestDriveSection,
                AuthenticationSchemes = _configuration.AuthenticationSchemes,
                Modules = _specification.Generate().Select((m, i) => new ModuleModel
                {
                    Index = i,
                    Name = m.Name,
                    Overview = m.Comments,
                    Collapsed = _configuration.CollapseModules,
                    Resources = m.Resources.Select(r => r.Name.TrimStart('/')).ToList()
                }).ToList()
            };

            _configuration.AppPreRender?.Invoke(request, app);

            return _configuration.AppTemplate.RenderBytes(app);
        }
    }
}
