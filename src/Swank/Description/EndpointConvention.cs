using System;
using System.Web.Http.Description;
using Swank.Extensions;

namespace Swank.Description
{
    public class EndpointConvention : IDescriptionConvention<ApiDescription, EndpointDescription>
    {
        public const string RequestCommentsExtension = ".request";
        public const string ResponseCommentsExtension = ".response";

        private readonly XmlComments _xmlComments;
        private readonly Configuration.Configuration _configuration;

        public EndpointConvention(XmlComments xmlComments, Configuration.Configuration configuration)
        {
            _xmlComments = xmlComments;
            _configuration = configuration;
        }

        public virtual EndpointDescription GetDescription(ApiDescription endpoint)
        {
            var attribute = endpoint.GetActionAttribute<DescriptionAttribute>();
            var xmlComments = _xmlComments.GetMethod(endpoint.GetMethodInfo());
            
            return new EndpointDescription
            {
                Name = attribute?.Name ?? xmlComments?.Summary ?? 
                    endpoint.ActionDescriptor.ActionName,
                Comments = GetEndpointComments(endpoint, attribute, xmlComments),
                Namespace = _configuration.ActionNamespace(endpoint),
                MethodName = _configuration.ActionName(endpoint),
                Secure = endpoint.HasControllerOrActionAttribute<SecureAttribute>(),
                BinaryRequest = endpoint.HasControllerOrActionAttribute<BinaryRequestAttribute>(),
                BinaryResponse = endpoint.HasControllerOrActionAttribute<BinaryResponseAttribute>(),
                RequestComments = GetDataComments<RequestCommentsAttribute>(
                    endpoint, x => x.Comments, RequestCommentsExtension) ??
                    endpoint.GetRequestDescription()?.Documentation ??
                    endpoint.GetBodyParameter().WhenNotNull(x => xmlComments?
                        .Parameters.TryGetValue(x.Name)).OtherwiseDefault(),
                ResponseComments = GetDataComments<ResponseCommentsAttribute>(
                    endpoint, x => x.Comments, ResponseCommentsExtension) ?? 
                    endpoint.ResponseDescription?.Documentation ?? 
                    xmlComments?.Returns
            };
        }

        protected virtual string GetEndpointComments(ApiDescription endpoint, 
            DescriptionAttribute description, XmlComments.Comments xmlComments)
        {
            return description
                .WhenNotNull(x => x.Comments)
                .Otherwise(endpoint.GetActionAttribute<CommentsAttribute>()
                .WhenNotNull(x => x.Comments)
                .OtherwiseDefault()) ??

                endpoint.GetControllerAssembly().FindResourceNamed(
                    endpoint.GetControllerType().FullName + "." +
                    endpoint.GetMethodInfo().Name.AddMarkdownExtension()) ?? 

                endpoint.Documentation ??

                xmlComments?.Remarks;
        }

        protected virtual string GetDataComments<TAttribute>(ApiDescription endpoint, 
            Func<TAttribute, string> attributeComments, string resourcePostfix = "")
            where TAttribute : Attribute
        {
            return endpoint.GetActionAttribute<TAttribute>()
                .WhenNotNull(attributeComments)
                .OtherwiseDefault() ??

                endpoint.GetControllerAssembly().FindResourceNamed(
                        endpoint.GetControllerType().FullName + "." +
                        endpoint.GetMethodInfo().Name + 
                        resourcePostfix.AddMarkdownExtension());
        }
    }
}