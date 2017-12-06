using System;
using Swank.Extensions;

namespace Swank.Description
{
    public class EndpointConvention : IDescriptionConvention<IApiDescription, EndpointDescription>
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

        public virtual EndpointDescription GetDescription(IApiDescription endpoint)
        {
            var attribute = endpoint.GetActionAttribute<DescriptionAttribute>();
            var xmlComments = _xmlComments.GetMethod(endpoint.ActionMethod);
            
            return new EndpointDescription
            {
                Name = endpoint.GetActionAttribute<NameAttribute>()?.Name ??
                    attribute?.Name ?? xmlComments?.Summary ?? endpoint.Name,
                Comments = GetEndpointComments(endpoint, attribute, xmlComments),
                Namespace = _configuration.ActionNamespace(endpoint),
                MethodName = _configuration.ActionName(endpoint),
                Secure = endpoint.HasControllerOrActionAttribute<SecureAttribute>(),
                BinaryRequest = endpoint.HasControllerOrActionAttribute<BinaryRequestAttribute>(),
                BinaryResponse = endpoint.HasControllerOrActionAttribute<BinaryResponseAttribute>(),
                RequestComments = GetDataComments<RequestCommentsAttribute>(
                    endpoint, x => x.Comments, RequestCommentsExtension) ??
                    endpoint.RequestParameter?.Documentation ??
                    (endpoint.RequestParameter != null
                        ? xmlComments?.Parameters.TryGetValue(endpoint.RequestParameter.Name)
                        : null),
                ResponseComments = GetDataComments<ResponseCommentsAttribute>(
                    endpoint, x => x.Comments, ResponseCommentsExtension) ?? 
                    endpoint.ResponseDocumentation ?? 
                    xmlComments?.Returns
            };
        }

        protected virtual string GetEndpointComments(IApiDescription endpoint, 
            DescriptionAttribute description, XmlComments.Comments xmlComments)
        {
            return description?.Comments ??
                endpoint.GetActionAttribute<CommentsAttribute>()?.Comments ??

                endpoint.ControllerType.Assembly.FindResourceNamed(
                    endpoint.ControllerType.FullName + "." +
                    endpoint.ActionMethod.Name.AddMarkdownExtension()) ?? 

                endpoint.Documentation ?? xmlComments?.Remarks ??
                
                (!endpoint.HasControllerAttribute<ResourceAttribute>()
                    ? endpoint.ControllerType.Assembly.FindResourceNamed(
                        endpoint.ControllerType.FullName.AddMarkdownExtension())
                    : null);
        }

        protected virtual string GetDataComments<TAttribute>(IApiDescription endpoint, 
            Func<TAttribute, string> attributeComments, string resourcePostfix = "")
            where TAttribute : Attribute
        {
            var attribute = endpoint.GetActionAttribute<TAttribute>();
            return (attribute != null ? attributeComments(attribute) : null) ??

                endpoint.ControllerType.Assembly.FindResourceNamed(
                        endpoint.ControllerType.FullName + "." +
                        endpoint.ActionMethod.Name + 
                        resourcePostfix.AddMarkdownExtension());
        }
    }
}