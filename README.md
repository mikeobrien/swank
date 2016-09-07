Swank
=============

[![Nuget](http://img.shields.io/nuget/v/Swank.svg?style=flat)](http://www.nuget.org/packages/Swank/) [![TeamCity Build Status](https://img.shields.io/teamcity/http/build.mikeobrien.net/s/swank.svg?style=flat)](http://build.mikeobrien.net/viewType.html?buildTypeId=swank&guest=1)

Swank is an ASP.NET Web API 2 library that documents RESTful services. You can take a look at a sample of the documentation it generates [here](http://www.mikeobrien.net/swank/sample) and Swank documentation [here](http://www.mikeobrien.net/swank/). 

## Features

- Document every detail of your API with [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) and [emojis](http://www.webpagefx.com/tools/emoji-cheat-sheet/).
- Highly configurable [resource grouping](#resources).
- Optionally group resources into [modules](#modules).
- Optional [custom [markdown](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) main page](#main-page).
- Supports [XML comments](#xml-comments-1).
- Custom [code examples](#code-examples) from Razor or Mustache templates.
- [Overrides](#overrides) and [conventions](#conventions) allow you to customize every aspect of spec generation.
- Custom [templates](#templates) that allow generation of code or documentation.


Install
------------

Swank can be found on nuget:

    PM> Install-Package Swank

Documentation
------------

Documentation can be found [here](http://www.mikeobrien.net/swank/).

Props
------------

Thanks to [JetBrains](http://www.jetbrains.com/) for providing OSS licenses! 

Thanks to [Swagger](http://swagger.wordnik.com/) for some design elements which Swank shamelessly ripped off.