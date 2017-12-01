---
layout: site
---

Swank is an ASP.NET Web API 2 library that documents RESTful services. You can take a look at a sample of the documentation it generates [here](http://www.mikeobrien.net/swank/sample). 

## Features

- Document every detail of your API with [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) and [emojis](http://www.webpagefx.com/tools/emoji-cheat-sheet/).
- Highly configurable [resource grouping](#resources).
- Optionally group resources into [modules](#modules).
- Optional [custom [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) main page](#main-page).
- Supports [XML comments](#xml-comments-1).
- Custom [code examples](#code-examples) from Razor or Mustache templates.
- [Overrides](#overrides) and [conventions](#conventions) allow you to customize every aspect of spec generation.
- Custom [templates](#templates) that allow generation of code or documentation.

## Getting Started

### Installation

Swank can be found on nuget:

    PM> Install-Package Swank
    
### Basic Configuration

Swank can be enabled by calling the `Swank` extension method on an `HttpConfiguration` instance. There are a handful of options you you'll probably want to fill in, although these are not required.

```csharpusing Swank;using Swank.Configuration;using Swank.Extensions;var configuration = GlobalConfiguration.Configuration;
configuration.Swank(x => x    .WithFavIconAt("/img/favicon.png")    .WithPageTitle("Setec Astronomy API")    .WithLogoAt("/img/logo.png")    .WithHeader("Setec Astronomy")    .WithCopyright("Copyright &copy; {year} Setec Astronomy"));
```  

At this point you should see documentation under `http://yoursite/api`. The url of the docs can be changed to whatever you want, even the root of the website.

```csharpconfiguration.Swank(x => x.WithAppAt("my/custom/path")...);
```  
<div class="bs-callout bs-callout-danger">
<p>Note: If your url matches a physical folder path, IIS will try to list directory contents (and you'll likely get a 403). The reason is that the <code>DirectoryListingModule</code> takes precedence over the <code>UrlRoutingModule</code>. Swank can override this behavior with the <code>IgnoreFolders()</code> option.</p>

<div class="language-csharp highlighter-rouge"><pre class="highlight"><code><span class="n">configuration</span><span class="p">.</span><span class="nf">Swank</span><span class="p">(</span><span class="n">x</span> <span class="p">=&gt;</span> <span class="n">x</span><span class="p">.</span><span class="nf"> IgnoreFolders</span><span class="p">()...);</span>
</code></pre>
</div>

<p>This option overrides the configured virtual path provider to selectively ignore physical paths that match Swank urls.</p>

</div>

By default the request authority is used throughout the docs. Although you can override it as follows.

```csharpconfiguration.Swank(x => x.WithApiAt("https://www.setecastronomy.com")...);
```  

If you plan to document your API via [XML comments](https://msdn.microsoft.com/en-us/library/b2s063f7.aspx) you'll want to reference those. This overload assumes the Visual Studio convention and expects them to be in the `\bin` folder along side the calling assembly and named as `[Assembly name].xml`.

```csharp
configuration.Swank(x => x.AddXmlComments()...);
``` 

If they are located somewhere else you can reference them directly. You can add as many XML documentation files as you want.

```csharp
configuration.Swank(x => x
    .AddXmlComments("~/assets/docs1.xml")
    .AddXmlComments("~/assets/docs2.xml")...);
``` 

Thats all there is to it! Now its time to organize your API.

## Organize

### Resources

Out of the box, Swank tries to group your endpoints into resources. The way it does this is by removing all url/querystring parameters and grouping by the resulting value. So for example this:

```
some/{param1}/path/{param2}?param3={param3}
```

Would be grouped by:

```
some/path
```

There are number of ways to override this behavior as described below. 

#### Resource Id

First you can replace the function used to generate the resource grouping id. For example, lets say you wanted to group resources by controller, regardless of the url:

```configuration.Swank(x => x
    .WithResourceIdentifier(r => r.ActionDescriptor
        .ControllerDescriptor.ControllerType.FullName)...);
```

#### Markers

Swank also supports the concept of "markers" to group endpoints under namespaces into resources. A resource marker will apply to all endpoints in its namespace and below. If another resource marker is defined in a deeper namespace, this marker will group endpoints in and under it and so on. A resource marker is simply a class that inherits from `ResourceDescription` where you specify a name and optional comments (and all comments support [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) and [emojis](http://www.webpagefx.com/tools/emoji-cheat-sheet/)):

```csharp
public class Resource : ResourceDescription{    public Resource()    {        Name = "some/path";        Comments = "Some *description*. :trollface:";    }}
```

As an example, the following layout:

```
MyApp
    .Api
        Resource.cs --> some/path
        Controller1.cs
        .FooBar
            Controller2.cs
        .Widgets
            Resource.cs --> some/widgets
            Controller3.cs
```

Would result in *two* resources, `some/path` and `some/widgets`. Endpoints in `Controller1` and `Controller2` would be grouped into the `some/path` resource and `Controller3` in the `some/widgets` resource.

If the resource comments are lengthy you can put them in a [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) file as an embedded resource. The filename must match the name of the resource marker class with a `.md` extension and be in the same namespace. So in the example above:

```
MyApp
    .Api
        Resource.cs --> some/path
        Resource.md
        ...
```

#### Attributes

Swank also supports marking a controller as a resource with the `ResourceAttribute` as follows:

```csharp
[Resource("some/path", "Some *description*. :trollface:")]public class SomeController : ApiController { ... }
```
    
If the comments are lengthy you can also put them in a [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) file as an embedded resource, just as with the resource marker. The only difference is that the filename must match the name of the *controller* class with a `.md` extension and be in the same namespace. So in the example above:

```
MyApp
    .Api
        Controller1.cs
        Controller1.md
        ...
```

#### XML Comments

And finally using XML comments as follows:

```csharp
/// <summary>some/path</summary>/// <remarks>Some *description*. :trollface:</remarks>public class SomeController : ApiController { ... }
```
    
The summary is the resource name and the remarks are the comments. If the remarks are lengthy you can also put them in a [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) file as you would with the resource attribute (see the previous section).

### Modules

As noted above, by default endpoints are organized into resources. Swank allows you to optionally organize your resources into modules. These can be used to group resources by functionality or different versions of your API (or whatever you want).

Just as with resources, Swank also supports the concept of "markers" to group resources under namespaces into modules. A module marker will apply to all resources in its namespace and below. If another module marker is defined in a deeper namespace, this marker will group resources in and under it and so on. A module marker is simply a class that inherits from `ModuleDescription` where you specify a name and optional comments (and all comments support [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) and [emojis](http://www.webpagefx.com/tools/emoji-cheat-sheet/)):

```csharp
public class Module : ModuleDescription{    public Module()    {        Name = "Module";        Comments = "Some *description*. :trollface:";    }}
```

As an example the following layout:

```
MyApp
    .Api
        Module.cs --> Some Module
        Resource.cs --> some/path
        Controller1.cs
        .FooBar
            Resource.cs --> another/path
            Controller2.cs
        .Widgets
            Module.cs --> Widget Module
            Resource.cs --> some/widgets
            Controller3.cs
```

Would result in two modules, `Some Module` and `Widget Module`. Resources `some/path` and `another/path` would be grouped into the `Some Module` module and `some/widgets` in the `Widget Module` module.

If the module comments are lengthy you can put them in a [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) file as an embedded resource. The filename must match the name of the module marker class with a `.md` extension and be in the same namespace. So in the example above:

```
MyApp
    .Api
        Module.cs --> Some Module
        Module.md
        ...
```

## Describe

We covered a few places where you can describe your API in the previous section (Modules and resources). This section will cover how to describe the rest of your API.

### Main Page

Swank allows you to set content on the main page. All documentation supports [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) and [emojis](http://www.webpagefx.com/tools/emoji-cheat-sheet/). By default Swank looks for a [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) file at the root of the website called `Overview.md`. You can also load a file from a virtual path, embedded resource or by passing the document in directly.

```csharp
// Embedded resource, only the file name is required.configuration.Swank(x => x.WithOverviewResource("Overview.md")...);

// Virtual pathconfiguration.Swank(x => x.WithOverviewFromVirtualPath("~/docs/Overview.md")...);

// Directly passedconfiguration.Swank(x => x.WithOverview("*Sweet* docs! :trollface:")...);
```  

If your document contains headers, you can add links to these in the UI.

```csharp
// If the fragment id is not passed, one will automatically be 
// generated by snake casing the link name (e.g. `sweet-section`).  configuration.Swank(x => x.WithOverviewLink("Sweet Section")...);

// Or you can pass in a specific fragment id.configuration.Swank(x => x.WithOverviewLink("Sweet Section", "my-sweet-section")...);
```  

### Endpoints

There are a number of different ways to document your API. You will probably use a combination of methods to do this. If these out-of-the-box methods don't meet your needs or you just don't like them, Swank allows you to add your own custom conventions, more on this under [Conventions](#conventions).

#### XML Comments

You can use XML comments to document most things although you will have to use attributes or custom conventions to do the rest. Documenting modules and resources with XML comments is covered in the [Organize](#Organize) section. All XML comments support [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) and [emojis](http://www.webpagefx.com/tools/emoji-cheat-sheet/).

First you can describe types, members, enums and enum options with either the `summary` tag or the `remarks` tag (but not both). These show up under the request and response sections to the right of the XML and json descriptions. 

```csharp
/// <summary>   /// *Type* summary... :trollface:/// </summary>public class Model 
{    /// <remarks>    /// *Member* remarks... :trollface:    /// </remarks>    public string Value1 { get; set; }

    /// <summary>
    /// *Member* summary... :trollface:    /// </summary>    public string Value2 { get; set; }
}
/// <remarks>/// *Enum* remarks... :trollface:/// </remarks>public enum Options{    /// <summary>    /// *Enum* option summary... :trollface:    /// </summary>    Option1,    /// <remarks>    /// *Enum* option remarks... :trollface:    /// </remarks>    Option2}
```

Next you can describe actions as follows.

|----|----|
| Element | Description |
|----|----|
| `summary` | This is displayed as the endpoint name in the endpoint header bar. |
| `remarks` | This is displayed as the endpoint description at the top of the endpoint section. |
| Request body `param` | This is displayed at the top of the request section. |
| Url parameter & querystring `param` | These are displayed as the descriptions of individual url parameters and querystrings in their respective sections. |
| `returns` | This is displayed at the top of the response section. |
|----|----|

```csharp
/// <summary>/// Action name... :trollface:/// </summary>/// <remarks>
/// Action remarks... :trollface:
/// </remarks>/// <param name="model">Request comments. :trollface:</param>/// <param name="param1">Parameter 1 comments. :trollface:</param>/// <param name="param2">Parameter 2 comments. :trollface:</param>/// <returns>
/// Response comments... :trollface:
/// </returns>public Model Get(Model model, string param1, string param1) { ... }
```

#### Attributes

Yo dawg I heard you like attributes... so you can use attributes to describe everything in your API. Attributes can be very tedious and people can forget to add them so [overrides](#overrides) or [conventions](#conventions) may be better option for some things. Documenting resources with attributes is covered in the [Organize](#organize) section. All attribute comments support [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) and [emojis](http://www.webpagefx.com/tools/emoji-cheat-sheet/).

The following are all the built in attributes.

|----|----|
| Attribute | Description |
|----|----|
| `Comments` | This can be used to describe action methods, action parameters, types, members, enums and enum options. |
| `Description` | Sets the name and comments on action methods, action parameters, types, members, enums and enum options. |
| `Hide` | Excludes controllers, action methods, members, enum options and action parameters |
| `Secure` | Indicates that an endpoint is secure, used on action methods. |
| `ActionName` |  Specifies the action name and optionally the action namespace, by default this is the action method name. |
| `ActionNamespace` |  Specifies the action namespace, by default this is derived from the url template. |
| `RequestHeader` |  Adds request headers, used on action methods. |
| `ResponseHeader` | Adds response headers, used on action methods. |
| `RequestComments` | Describes the request, used on action methods. |
| `ResponseComments` | Describes the response, used on action methods. |
| `BinaryRequest` | Indicates that request is binary, used on action methods. |
| `BinaryResponse` | Indicates that response is binary, used on action methods. |
| `StatusCode` | Adds status codes, used on action methods. |
| `SampleValue` | Sets the sample value which is displayed in the XML and json samples. Used on members and action parameters. |
| `DefaultValue` | Specifies a default value, used on members and querystring action parameters. |
| `MaxLength` | Specifies max length of a string, used on members. |
| `UnicodeEncoding`, `AsciiEncoding`, `ISO8601Encoding` | Specifies the encoding, used on members. Yeah, I know unicode itself isn't an encoding, just indicates that the field supports unicode characters as opposed to ASCII. |
| `Optional` | Indicates that something is optional, used on members and querystring action parameters. |
| `OptionalForPost` | Indicates that a member is optional for `POST` otherwise required. |
| `OptionalForPut` | Indicates that a member is optional for `PUT` otherwise required. |
| `Required` | Indicates that a member or querystring action parameter is required. |
| `RequiredForPost` | Indicates that a member is required for `POST` otherwise optional. |
| `RequiredForPut` | Indicates that a member is required for `PUT` otherwise optional. |
| `Multiple` | Indicates that multiple querystring action parameters are allowed. |
| `ArrayDescription` | Describes list types and members. You can set the name and comments as well as the item name and comments. |
| `DictionaryDescription` | Describes dictionary types and members. You can set the name and comments as well as the key name and, key and value comments. |
|----|----|

Swank also recognizes the following framework attributes.

|----|----|
| Attribute | Description |
|----|----|
| `Obsolete` | Indicates that a member is depricated. This is displayed, along with the comments in the request and response member descriptions. |
| `XmlIgnore` | Excludes members and action parameters. |
| `XmlElement` | Sets the name of members. |
| `XmlArrayItem` | Sets the name of array items, used on members. |
| `XmlType` | Sets the name of types and enums. |
| `XmlRootAttribute` | Sets the name of types and enums. |
| `DataMember` | Sets the name of members. |
| `DataContract` | Sets the name of types and enums. |
| `CollectionDataContract` | Sets the name of list types. |
|----|----|

The following demonstrates how to use many of the above attributes.

```csharp
[Comments("Enum comments. :trollface:")]public enum Options{    [Comments("Enum option comments. :trollface:")]    Option1,    [Comments("Enum option comments. :trollface:")]    Option2} 

[Description("NewModelName", "Type comments. :trollface:")]public class Model{    [Comments("Member comments. :trollface:")]    public string Member { get; set; }
    
    [SampleValue("fark")]    [Comments("Member comments. :trollface:")]    public string SampleValueMember { get; set; }
    
    [Hide]    public string HiddenMember { get; set; }
        [Optional, DefaultValue("fark")]    [Comments("Member comments. :trollface:")]    public string OptionalMember { get; set; }
        [Description("NewMemberName", "Member comments. :trollface:")]    public string DescriptionMember { get; set; }
        [Comments("Member comments. :trollface:")]    [Obsolete("Obsolete comments. :trollface:")]    public string DepricatedMember { get; set; }
    
    [ArrayDescription(comments: "List comments.", itemName: "item-name", 
        itemComments: "Item comments. :trollface:")]    public List<int> ListMember { get; set; }
        [DictionaryDescription(comments: "Dictionary comments. :trollface:",        keyName: "key-name", keyComments: "Key comments. :trollface:",         valueComments: "Value comments. :trollface:")]    public Dictionary<string, int> DictionaryMember { get; set; }}

[Secure][Description("Endpoint Name", "Endpoint comments. :trollface:")][RequestHeader("request-header-1", "Request header comments. :trollface:")][RequestHeader("request-header-2", "Request header comments. :trollface:", optional: true)][ResponseHeader("response-header-1", "Response header comments. :trollface:")][ResponseHeader("response-header-2", "Response header comments. :trollface:")][StatusCode(HttpStatusCode.Created, "Status code comments. :trollface:")][StatusCode(HttpStatusCode.BadGateway, "Status code comments. :trollface:")][BinaryResponse][ResponseComments("Response comments. :trollface:")]

[Route("some/path/{id}")]public OutputModel Post(    [Comments("Request comments. :trollface:")] InputModel model,    [Comments("Url parameter comments. :trollface:")] Guid id,    [Comments("Querystring comments. :trollface:"), Required] Options requiredQuerystring,    [Comments("Querystring comments. :trollface:"), Multiple, DefaultValue(5)] 
    int? optionalQuerystring = null){    return null;}
```

As you can see attributes get ugly fast. [Overrides](#overrides) and [conventions](#conventions) can reduce or eliminate attribute descriptions.

#### Embedded [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet)

You can use embedded [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) to document a few things. Documenting modules and resources with embedded [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) is covered in the [Organize](#organize) section. All [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) documents support [emojis](http://www.webpagefx.com/tools/emoji-cheat-sheet/).

Lets say we have the following controller.

```csharp
namespace MyApp.Api{    public class MyController : ApiController    {        public OutputModel Post(InputModel model) { ... }    }}
```

You can document the endpoint, request and response by adding embedded resources named as `[controller name].md`, `[controller name].[method name].md` (if there are multiple actions), `[controller name].[method name].Request.md`, `[controller name].[method name].Response.md` respectively. So the mardown files for the above controller would be as follows:

```
MyApp
    .Api
        MyController.cs
        MyController.md
        MyController.Post.md
        MyController.Post.Request.md
        MyController.Post.Response.md
```

Currently there is not support for creating different [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) descriptions for overloads so overloads will all share the same comments. 

### Code Examples

Swank supports code examples that are generated from the specification. This can be done with Razor or Mustache templates. Two examples ship with Swank, [curl](https://github.com/mikeobrien/swank/blob/master/src/Swank/Description/CodeExamples/curl.cshtml) and [Node.js](https://github.com/mikeobrien/swank/blob/master/src/Swank/Description/CodeExamples/node.cshtml). You can use these as a starting point for creating your own custom examples. Code example templates are passed a [model](https://github.com/mikeobrien/swank/blob/master/src/Swank/Description/CodeExamples/TemplateModel.cs) that contains a complete description of the endpoint. You may also want to include [Swank extension methods](#overrides) in your template.

```
@model Swank.Description.CodeExamples.CodeExampleModel
@using Swank.Extensions;

Name: @name
```

Swank uses [Highlight.js](https://highlightjs.org/) to highlight code examples. Swank ships with all the available themes and defaults to GitHub [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet). You can change the theme as follows.

```csharpconfiguration.Swank(x => x.WithCodeExampleTheme(CodeExampleTheme.SolarizedDark)...);
``` 

If you are adding your own templates you may want to clear out the stock ones, you can do this in the configuration DSL as follows.

```csharpconfiguration.Swank(x => x.ClearCodeExamples()...);
``` 

Code examples can be loaded from a file, a virtual path, embedded resource or by passing the template in directly.

```csharp
// Embedded resource, only the file name is required.configuration.Swank(x => x
    .WithCodeExampleResource("Fsharp.cshtml",
        "F#", CodeExampleLanguage.Fsharp)...);
        
// Virtual pathconfiguration.Swank(x => x
    .WithCodeExampleFromVirtualPath(
        "~/Code Samples/Fsharp.cshtml",
        "F#", CodeExampleLanguage.Fsharp)...);

// Directly passed mustache templateconfiguration.Swank(x => x
    .WithMustacheCodeExample(
        "F#", 
        CodeExampleLanguage.Fsharp, 
        "Code example comments. :trollface:", 
        "let model = new...")...);

// Directly passed Razor templateconfiguration.Swank(x => x
    .WithRazorCodeExample(
        "F#", 
        CodeExampleLanguage.Fsharp, 
        "Code example comments. :trollface:", 
        "@model Swank.Web.Handlers.AppModel...")...);
```  

You can also load multiple templates and comments from embedded resources or a virtual directory. In both cases the filename (without the extension) will be used as the name and [Highligh.js language](http://highlightjs.readthedocs.io/en/latest/css-classes-reference.html#language-names-and-aliases). Comments can be added by including a [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) file with the same name as follows.

```bash
Curl.mustache
Curl.md
Node.js.cshtml
Node.js.md
FSharp.cshtml
```

All files under the resource namespace or virtual path with a `.cshtml`, `.mustache` or `.md` will be loaded using the following configuration.

```csharp
// Embedded resourcesconfiguration.Swank(x => x.WithCodeExampleResources("MyApp.Assets.CodeSamples")...);
        
// Virtual pathconfiguration.Swank(x => x.WithCodeExamplesInVirtualPath("~/Code Samples")...);
``` 
You can turn on debugging, where errors are displayed instead of the rendered template, with the following configuration.

```csharp
// Enable debug modeconfiguration.Swank(x => x.IsInDebugMode()...);
        
// Enable debug mode when calling assembly is in debug mode
configuration.Swank(x => x.IsInDebugModeWhenAppIsInDebugMode()...);
``` 

Swank ships with a utility that will allow you to render your code example templates during development, using the specification from your API. More on this under [Template Development](#template-development).

## Customize

### User Interface

There are a number of ways you can customize the user interface, some simple and one extreme.

#### Display Options

There are a few options that control what is displayed. 

|----|----|
| Option | Description |
|----|----|
| `WithFavIconAt(string url)` | Sets the favicon url. || `WithPageTitle(string title)` | Sets the page title. If not set, defaults to the header text. || `WithHeader(string name)` | Sets the header text. || `WithLogoAt(string url)` | Sets the logo url. || `WithApiAt(string url)` | Sets the API authority used throughout the docs. |
| `WithCopyright(string copyright)` | Sets the copyright text in the footer, the `{year}` token is replaced with the current year. |
| `HideJsonData()`  | Hides the json data request and response samples. |
| `HideXmlData()`  | Hides the XML data request and response samples. |
| `WithDefaultDictionaryKeyName(...)` | Allows you to set the default dictionary key name (`key`). |
| `WithSample[data type]Value(...)`  | Sets the sample data displayed for a particular data type. |
| `WithSample[data type]Format(...)`  | Sets the sample data format string for a particular data type. |
|----|----|

#### Scripts and Stylesheets

The user interface was designed so that you can easily override any part of the layout. You can see all the css classes listed in [the Swank stylesheet](https://github.com/mikeobrien/swank/blob/master/src/Swank/Web/Content/css/swank.css). The configuration allows you to add your own stylesheets and scripts.

```csharp
configuration.Swank(x => x
    .WithStylesheets("/css/my.css", "/css/theme.css")
    .WithScripts("/js/some.js", "/js/another.js")...);
```

#### UI Template

If overriding styles and scripts isn't enough, you can provide your own UI template. You can use [the existing one](https://github.com/mikeobrien/swank/blob/master/src/Swank/Web/Content/App.cshtml) as a starting point and customize from there. The UI is a Razor template rendered by [RazorEngine](https://github.com/Antaris/RazorEngine). To set your own you can load it from an embedded resource, a virtual path or pass it in directly.

```csharp
// Embedded resource, only the file name is required.configuration.Swank(x => x.WithAppTemplateResource("App.cshtml")...);

// Virtual pathconfiguration.Swank(x => x.WithAppTemplateFromVirtualPath("~/docs/App.cshtml")...);

// Directly passedconfiguration.Swank(x => x.WithAppTemplate("@model Swank.Web.Handlers.AppModel...")...);
```  

### Specification

Swank offers a few ways to customize the specification. First through options, then by modifying the specification after it has been generated (overrides) and finally generating the specification (conventions).

#### Options

There are a few options to control how the specification is generated. 

|----|----|
| Option | Description |
|----|----|
| `AppliesTo(*)` | Sets the assemblies Swank scans for embedded resources and other artifacts. |
| `Where(...)` | Allows you to filter the endpoints included in the specification. |
| `WithNumericEnum()` | Indicates that enum values are numeric. |
| `WithDefaultModuleName(...)` | The name of the default module. |
| `WithDefaultResource(...)` | The default resource factory. |
| `WhenModuleOrphaned(...)` | Determines what should happen when there are orphaned modules. |
| `WhenResourceOrphaned(...)` | Determines what should happen when there are orphaned resources. |
|----|----|

#### Overrides

If you want to modify the spec *after* it's been generated you can override it. Lets say for example you wanted to generate all the resource comments instead of manually filling them in. 

```csharpconfiguration.Swank(x => x.OverrideResources(
    r => r.Resource.Comments = $"This is the {r.Resource.Name.ToLower()} resource.")...);
```

This will set the comments for every resource. But what if you only wanted to do it for resources that don't have comments, you can pass a predicate.

```csharpconfiguration.Swank(x => x.OverrideResourcesWhen(
    r => r.Resource.Comments = $"This is the {r.Resource.Name.ToLower()} resource.", 
    r => r.Resource.Comments.IsNullOrEmpty())...);
```

Another example is adding common status codes to every `POST` and `PUT` endpoint.

```csharpconfiguration.Swank(x => x.OverrideEndpointsWhen(
    e => e.Endpoint.StatusCodes.Add(new StatusCode { Code = 201, Comments = "..." }), 
    e => e.Endpoint.HttpMethod == "POST" || e.Endpoint.HttpMethod == "PUT")...);
```

Override functions are passed a context that includes the final specification, the original description (`Description` property) and other information that describes that specific part of the specification. You will want to update the specification object (see below) as this is what will be used to generate documentation or returned from the specification endpoint.

Overrides are available for every part of the spec. The configuration DSL methods follow the convention of `Override[thing]` and `Override[thing]When` for conditional overrides.

|----|----|
| Configuration DSL | Specification Property | Description Properties | Comments |
|----|----|----|
| `OverrideModules` | `Module` |  `Description` | |
| `OverrideResources` | `Resource` |  `Description` | |
| `OverrideEndpoints` | `Endpoint` |  `Description`, `ApiDescription` | |
| `OverrideParameters` | `Parameter` |  `Description`, `ApiDescription` | Both url and querystring parameters. |
| `OverrideUrlParameters` | `UrlParameter` |  `Description`, `ApiDescription` | |
| `OverrideQuerystring` | `Querystring` |  `Description`, `ApiDescription` | |
| `OverrideStatusCodes` | `StatusCode` |  `Description`, `ApiDescription` | |
| `OverrideHeaders` | `Header` |  `Description`, `ApiDescription` | Both request and response headers. |
| `OverrideRequestHeaders` | `Header` |  `Description`, `ApiDescription` |  |
| `OverrideResponseHeaders` | `Header` |  `Description`, `ApiDescription` |  |
| `OverrideMessage` | `Message` |  `Description`, `ApiDescription` | Both request and response messages. |
| `OverrideRequest` | `Message` |  `Description`, `ApiDescription` |  |
| `OverrideResponse` | `Message` |  `Description`, `ApiDescription` |  |
| `OverrideTypes` | `DataType` |  `Description`, `ApiDescription`, `Type`, `IsRequest` | |
| `OverrideMembers` | `Member` |  `Description`, `ApiDescription`, `Property`, `IsRequest` | |
| `OverrideOptions` | `Option` |  `Description`, `ApiDescription`, `Field`, `IsRequest` | |
|----|----|

Swank also ships with a number of convenience extension methods under `Swank.Extensions` that you can use in your overrides.

- `bool HasParameter<T>(ApiDescription description)`
- `Type GetRequestType(ApiDescription endpoint)`
- `ApiParameterDescription GetRequestDescription(ApiDescription endpoint)`
- `Type GetControllerType(ApiDescription description)`
- `Assembly GetControllerAssembly(ApiDescription description)`
- `ApiParameterDescription GetBodyParameter(ApiDescription endpoint)`
- `MethodInfo GetMethodInfo(ApiDescription description)`
- `MethodInfo GetMethodInfo(ApiParameterDescription description)`
- `MethodInfo GetMethodInfo(HttpActionDescriptor descriptor)`
- `bool HasAttribute<T>(ApiParameterDescription description)`
- `bool HasControllerAttribute<T>(ApiDescription description)`
- `bool HasControllerOrActionAttribute<T>(ApiDescription description)`
- `T GetAttribute<T>(ApiParameterDescription description)`
- `T GetActionAttribute<T>(ApiDescription description)`
- `T GetControllerAttribute<T>(ApiDescription description)`
- `IEnumerable<T> GetControllerAndActionAttributes<T>(ApiDescription description)`
- `bool IsUrlParameter(ApiParameterDescription parameter, ApiDescription endpoint)`
- `bool IsQuerystring(ApiParameterDescription parameter, ApiDescription endpoint)`
- `bool IsGet(HttpMethod method)`
- `bool IsPost(HttpMethod method)`
- `bool IsPut(HttpMethod method)`
- `bool IsPostOrPut(HttpMethod method)`
- `bool IsDelete(HttpMethod method)`
- `Is[Type](this Type type)` Indicates if the type matches the specified type or its nullable form. Supports `String`, `Boolean`, `Decimal`, `Double`, `Single`, `ByteArray`, `Byte`, `SByte`, `Int16`, `UInt16`, `Int32`, `UInt32`, `Int64`, `UInt64`, `DateTime`, `TimeSpan`, `Guid`, `Char`.
- `bool Is[Type](this PropertyInfo property)` Indicates if the property type matches the specified type or its nullable form. Supports `String`, `Boolean`, `Decimal`, `Double`, `Single`, `ByteArray`, `Byte`, `SByte`, `Int16`, `UInt16`, `Int32`, `UInt32`, `Int64`, `UInt64`, `DateTime`, `TimeSpan`, `Guid`, `Char`.

#### Conventions

If you want to modify how Swank actually generates the spec you can override the default conventions. Conventions are passed various objects they are related to and return a description. For example the default convention for status codes is passed an `ApiDescription`. It looks for any status code attributes on the controller and action method and then returns status code descriptions.

```csharp
public class StatusCodeConvention : IDescriptionConvention<ApiDescription, List<StatusCodeDescription>>{    public virtual List<StatusCodeDescription> GetDescription(ApiDescription endpoint)    {        return endpoint.GetControllerAndActionAttributes<StatusCodeAttribute>()            .Select(x => new StatusCodeDescription            {                Code = x.Code,                Name = x.Name,                Comments = x.Comments            }).OrderBy(x => x.Code).ToList();    }}
```

Lets say for example we stored all endpoint status codes in a file and you wanted to include them in your documentation (Obviously this is a *very* contrived example).

```
GETsome/path/{id},200,OK,Indicates widgets have been successfully returned.
POSTsome/path/{id},201,Created,Indicates a widget has been created.
...
```
You could create a status code convention that pulls these from the file, matches them up to the endpoint id, then plugs them into the description.

```csharp
public class FileStatusCodeConvention : StatusCodeConvention 
{    public override List<StatusCodeDescription> GetDescription(ApiDescription endpoint)    {        return File.ReadAllLines("path/to/statuscodes.csv")
            .Select(x => x.Split(','))
            .Where(x => x[0] == endpoint.ID)            .Select(x => new StatusCodeDescription            {                Code = x[1],                Name = x[2],                Comments = x[3]
            })
            .Concat(base.GetDescription(endpoint))
            .OrderBy(x => x.Code).ToList();    }}
```

This convention would then be set in the configuration.

```csharpconfiguration.Swank(x => x.WithStatusCodeConvention<FileStatusCodeConvention>()...);
```

You can also pass configuration which is then injected into the constructor. The config must have a parameterless constructor.

```csharp
public class SomeConfig 
{
    public bool Flag { get; set; }
}

public class SomeConvention : StatusCodeConvention 
{
    private SomeConfig _config;
    
    public SomeSonvention(SomeConfig config) 
    {
        _config = config;
    }
        public override List<StatusCodeDescription> GetDescription(ApiDescription endpoint)    {        if (_config.Flag)...;    }}
configuration.Swank(x => x
    .WithStatusCodeConvention<SomeConvention, SomeConfig>(x => x.Flag == true)...);
```

Conventions are available for everything in the spec, the configuration DSL methods follow the convention of `With[thing]Convention` and each has an additional overload to pass configuration. You can inherit from existing conventions to extend them or create conventions from scratch by implementing the following interfaces.

|----|----|
| Convention | Default Implementation | Interface |
|----|----|
| Module | `ModuleConvention` |  `IDescriptionConvention<ApiDescription, ModuleDescription>` |
| Resource | `ResourceConvention` | `IDescriptionConvention<ApiDescription, ResourceDescription>` |
| Endpoint | `EndpointConvention` | `IDescriptionConvention<ApiDescription, EndpointDescription>` |
| Header | `HeaderConvention` |  `IDescriptionConvention<ApiDescription, List<HeaderDescription>>` |
| Parameter | `ParameterConvention` |  `IDescriptionConvention<ApiParameterDescription, ParameterDescription>` |
| StatusCode | `StatusCodeConvention` |  `IDescriptionConvention<ApiDescription, List<StatusCodeDescription>>` |
| Type | `TypeConvention`  | `IDescriptionConvention<Type, TypeDescription>` |
| Member | `MemberConvention` |  `IDescriptionConvention<PropertyInfo, MemberDescription>` |
| Enum | `EnumConvention` |  `IDescriptionConvention<Type, EnumDescription>` |
| Enum Options | `OptionConvention` | `IDescriptionConvention<FieldInfo, EnumOptionDescription>` |
|----|----|

## Integrate

There are a couple of ways to integrate Swank with other processes. 

### Specification

Swank exposes an endpoint that returns the specification as json which can be consumed by other process to generate additional documentation or bindings. The default url for this is `api/spec`. You can change this url with the following configuration.

```csharpconfiguration.Swank(x => WithSpecificationAtUrl("myapi/spec")...);
```

The specification model returned is `List<Swank.Specification.Module>` found [here](https://github.com/mikeobrien/swank/blob/master/src/Swank/Specification/Specification.cs).

### Templates

Swank also allows you to generate your own content with templates. For example you could generate html documentation or bindings in different languages that could be consumed by other processes. Swank supports Razor and mustache templates which can be loaded from a file, a virtual path, embedded resource or by passing the template in directly. These are then exposed at a url you specify. Templates are passed a model as illustrated in the Razor template below. This model contains the spec (`Specification`), a code generation optimized model (`Namespaces`) and querystring values (`Values`).

```
@model Swank.Web.Handlers.Templates.TemplateModel
@using Swank.Web.Handlers.Templates;
@using Swank.Specification;
@using Swank.Extensions;
...
```
You can add templates as follows.

```csharp
// Embedded resource, only the file name is required.configuration.Swank(x => x
    .WithTemplateResource(
        "Fsharp.cshtml",
        "bindings/fsharp", 
        "text/plain")...);
        
// Virtual pathconfiguration.Swank(x => x
    .WithTemplateFromVirtualPath(
        "~/Templates/FSharp.cshtml", 
        "bindings/fsharp", 
        "text/plain")...);

// Directly passed mustache templateconfiguration.Swank(x => x
    .WithMustacheTemplate(
        "bindings/fsharp", 
        "text/plain", 
        "let model = new...")...);

// Directly passed Razor templateconfiguration.Swank(x => x
    .WithRazorTemplate(
        "bindings/fsharp", 
        "text/plain", 
        "@model Swank.Web.Handlers.AppModel...")...);
```  

You can also load multiple templates from embedded resources or a virtual directory. In both cases the relative path will be used as the url. So for example the following files...

```bash
/Templates
    /Bindings
        Node.js.mustache
        FSharp.cshtml
```

With the base url `code` and under the `/Templates` virtual path would be at the following urls:

```bash
/code/bindings/node_js
/code/bindings/fsharp
```

Periods are converted to underscores to prevent IIS from loading them as static files. All files under the resource namespace or virtual path with a `.cshtml` or `.mustache` extension will be loaded using the following configuration.

```csharp
// Embedded resourcesconfiguration.Swank(x => x.WithTemplateResources("MyApp.Assets.Templates")...);
        
// Virtual pathconfiguration.Swank(x => x.WithTemplatesInVirtualPath("~/Templates")...);
``` 

The specification includes namespaces for endpoints and types that can be used for generating wrappers. The namespace is a list of strings that can be joined by the appropriate character in your wrapper. By default the namespace of the type is simply the actual namespace and the namespace of the action is the namespace of the controller plus the name of the controller minus "Controller". There is also a convention for the action name which defaults to the method name. These can be easily overridden with a custom implementation as follows.

```csharp
configuration.Swank(x => x
    .WithTypeNamespaceConvention(x => Configuration
        .DefaultTypeNamespace(x).Skip(2).ToList())
    .WithActionNamespaceConvention(x => Configuration
        .DefaultActionNamespace(x).Skip(2).ToList())...)
    .WithActionNameConvention(x => Configuration
        .DefaultActionName(x) + "Endpoint")...);
``` 
The example above uses the stock convention but also removes the first two levels of the namespace and concats some text to the action name. So you can tweak the stock convention or replace it entirely.

You can also turn on debugging, where errors are displayed instead of the rendered template, with the following configuration.

```csharp
// Enable debug modeconfiguration.Swank(x => x.IsInDebugMode()...);
        
// Enable debug mode when calling assembly is in debug mode
configuration.Swank(x => x.IsInDebugModeWhenAppIsInDebugMode()...);
``` 

Swank ships with a utility that will allow you to render your templates during development, using the specification from your API. More on this under [Template Development](#template-development).

## Tools

Swank ships with some tools to help you build out your documentation. These are contained in the command line app `SwankUtil.exe`. This app is distributed with the Swank library and is in the `packages\Swank.[version]\tools` Nuget folder.

### Template Development

To make developing templates easier, `SwankUtil` will render templates and code examples using your API specification. This is much faster than making a change, starting your app, browsing to an endpoint, rinse, repeat... The utility will try to determine the type of file by looking at its extension (either `.cshtml` for Razor or `.mustache` for Mustache). You can also override this value if desired. 

First you'll need to grab your specification. Assuming Swank is configured, start your app and browse to the Swank page. There will be a little cloud download icon in the upper left hand corner to the right of the logo and/or title. Right-click and save this file, this is your specification, a JSON representation of your API. Additionally you may want test your template against the [sample specification](http://www.mikeobrien.net/swank/sample/spec.json) as it contains every built-in variation of the specification.

Next, open a command prompt and you can run `SwankUtil` to render templates. *Note: The caret's in the command below allow multiple lines in the Windows command prompt and are not part of the SwankUtil arguments.*

```bash
D:\Dev\MyApp\packages\Swank.1.0.71.0\tools\SwankUtil ^
    -c Template ^
    -s D:\Temp\Spec.json ^
    -t D:\Dev\MyApp\Templates\SomeTemplate.cshtml ^
    -o D:\Temp\SomeTemplate.txt
``` 

Code examples are rendered pretty much the same way except you have to pass the id of the endpoint you want to use as the model. To do this, open your specification file and find the endpoint you want to use, you will see it has an `id` field as illustrated below.

```json
[
  {
    "name": "Candidates",
    "resources": [
      {
        "name": "/api/candidates",
        "endpoints": [
          {
            "id": "10e848fd4b5df7728b259d69de40e25d",
            "name": "Queries candidates",
```

Once you have the id you can pass that in and your code example will be rendered for that particular endpoint.

```bash
D:\Dev\MyApp\packages\Swank.1.0.71.0\tools\SwankUtil ^
    -c CodeExample ^
    -s D:\Temp\Spec.json ^
    -e 10e848fd4b5df7728b259d69de40e25d ^
    -t D:\Dev\MyApp\CodeExamples\Haskell.cshtml ^
    -o D:\Temp\Example.hs
```

You will probably want to render your code examples for all your verbs and any edge cases.