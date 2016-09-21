﻿using System.Collections.Generic;
using Swank.Extensions;

namespace Swank.Specification
{
    public interface IDescription
    {
        string Name { get; set; }
        string Comments { get; set; }
    }

    public class Module : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public List<Resource> Resources { get; set; }
    }

    public class Resource : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public List<Endpoint> Endpoints { get; set; }
    }

    public class Endpoint : IDescription
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public string MethodName { get; set; }
        public List<string> Namespace { get; set; }
        public string UrlTemplate { get; set; }
        public string Method { get; set; }
        public List<UrlParameter> UrlParameters { get; set; }
        public List<QuerystringParameter> QuerystringParameters { get; set; }
        public bool Secure { get; set; }
        public List<StatusCode> StatusCodes { get; set; }
        public Message Request { get; set; }
        public Message Response { get; set; }
    }

    public class UrlParameter : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public DataType Type { get; set; }
        public string SampleValue { get; set; }
    }

    public class QuerystringParameter : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public DataType Type { get; set; }
        public string SampleValue { get; set; }
        public string DefaultValue { get; set; }
        public bool MultipleAllowed { get; set; }
        public bool Required { get; set; }
    }

    public class StatusCode : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public int Code { get; set; }
    }

    public class Header : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public bool Optional { get; set; }
        public bool Required { get; set; }
        public bool IsContentType => Name.EqualsIgnoreCase("content-type");
        public bool IsAccept => Name.EqualsIgnoreCase("accept");
    }

    public class Message
    {
        public string Comments { get; set; }
        public List<Header> Headers { get; set; }
        public bool IsBinary { get; set; }
        public DataType Type { get; set; }
        public bool HasBody => Type != null;
    }

    public class DataType : IDescription
    {
        public string Name { get; set; }
        public string LogicalName { get; set; }
        public string Namespace { get; set; }
        public List<string> FullNamespace { get; set; }
        public string Comments { get; set; }

        public bool IsSimple { get; set; }
        public Enumeration Enumeration { get; set; }

        public bool IsComplex { get; set; }
        public List<Member> Members { get; set; }

        public bool IsArray { get; set; }
        public ArrayItem ArrayItem { get; set; }

        public bool IsDictionary { get; set; }
        public DictionaryEntry DictionaryEntry { get; set; }
    }

    public class ArrayItem : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public DataType Type { get; set; }
    }

    public class DictionaryEntry
    {
        public string KeyName { get; set; }
        public DataType KeyType { get; set; }
        public string KeyComments { get; set; }

        public DataType ValueType { get; set; }
        public string ValueComments { get; set; }
    }

    public class Member : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public bool Required { get; set; }
        public bool Optional { get; set; }
        public string DefaultValue { get; set; }
        public string SampleValue { get; set; }
        public bool Deprecated { get; set; }
        public string DeprecationMessage { get; set; }
        public DataType Type { get; set; }
    }

    public class Enumeration
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public List<Option> Options { get; set; }
    }

    public class Option
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Comments { get; set; }
    }
}