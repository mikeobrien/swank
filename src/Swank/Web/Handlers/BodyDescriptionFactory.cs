using System;
using System.Collections.Generic;
using System.Linq;
using Swank.Extensions;
using Swank.Specification;

namespace Swank.Web.Handlers
{
    public class BodyDescriptionFactory
    {
        public const string Whitespace = "    ";

        private readonly Configuration.Configuration _configuration;

        public BodyDescriptionFactory(Configuration.Configuration configuration)
        {
            _configuration = configuration;
        }

        public List<BodyDefinition> Create(DataType type)
        {
            var data = new List<BodyDefinition>();
            WalkGraph(data, type, 0);
            data.ForEach((x, i) => x.Index = i + 1);
            return data;
        }

        private void WalkGraph(List<BodyDefinition> data, DataType type, int level, 
            Action<BodyDefinition> opening = null, 
            Action<BodyDefinition> closing = null)
        {
            if (type.IsSimple) WalkSimpleType(data, type, level, opening);
            else if (type.IsArray) WalkArray(data, type, level, opening, closing);
            else if (type.IsDictionary) WalkDictionary(data, type, level, opening, closing);
            else if (type.IsComplex) WalkComplexType(data, type, level, opening, closing);
            if (level == 0)
            {
                data.First().IsFirst = true;
                data.Last().IsLast = true;
            }
        }

        private void WalkSimpleType(
            List<BodyDefinition> description, 
            DataType type, int level,
            Action<BodyDefinition> opening)
        {
            var data = new BodyDefinition
            {
                Name = type.Name,
                TypeName = type.Name,
                Comments = type.Comments,
                Whitespace = Whitespace.Repeat(level),
                IsSimpleType = true
            };
            
            switch (type.Name)
            {
                case Xml.UnsignedLongType:
                case Xml.LongType:
                case Xml.UnsignedIntType:
                case Xml.IntType:
                case Xml.UnsignedShortType:
                case Xml.ShortType:
                case Xml.ByteType:
                case Xml.UnsignedByteType:
                    data.IsNumeric = true;
                    data.SampleValue = typeof(int).GetSampleValue(_configuration);
                    break;
                case Xml.FloatType:
                case Xml.DoubleType:
                case Xml.DecimalType: 
                    data.IsNumeric = true;
                    data.SampleValue = typeof(decimal).GetSampleValue(_configuration);
                    break;
                case Xml.BooleanType: 
                    data.IsBoolean = true;
                    data.SampleValue = typeof(bool).GetSampleValue(_configuration);
                    break;
                case Xml.DateTimeType: 
                    data.IsDateTime = true;
                    data.SampleValue = typeof(DateTime).GetSampleValue(_configuration);
                    break;
                case Xml.DurationType: 
                    data.IsDuration = true;
                    data.SampleValue = typeof(TimeSpan).GetSampleValue(_configuration);
                    break;
                case Xml.UuidType: 
                    data.IsGuid = true;
                    data.SampleValue = typeof(Guid).GetSampleValue(_configuration);
                    break;
                default: 
                    data.IsString = true;
                    data.SampleValue = _configuration.SampleStringValue; 
                    break;
            }

            data.Options = WalkOptions(type, x => data.SampleValue = x.Options.First().Value);

            opening?.Invoke(data);
            description.Add(data);
        }

        private static Enumeration WalkOptions(DataType type, 
            Action<Enumeration> whenOptions = null)
        {
            if (type.Options == null) return null;
            var enumeration = new Enumeration
            {
                Name = type.Options.Name,
                Comments = type.Options.Comments,
                Options = new List<Option>(type.Options.Options.Select(x => new Option
                {
                    Name = x.Name,
                    Value = x.Value,
                    Comments = x.Comments
                }))
            };
            whenOptions?.Invoke(enumeration);
            return enumeration;
        }

        private void WalkArray(List<BodyDefinition> data, DataType type, int level,
            Action<BodyDefinition> opening = null,
            Action<BodyDefinition> closing = null)
        {
            var arrayOpening = new BodyDefinition
            {
                Name = type.Name,
                Namespace = type.Namespace,
                Comments = type.Comments,
                Whitespace = Whitespace.Repeat(level),
                IsOpening = true,
                IsArray = true
            };

            opening?.Invoke(arrayOpening);

            data.Add(arrayOpening);

            WalkGraph(data, type.ArrayItem.Type, level + 1,
                x =>
                {
                    if (type.ArrayItem == null) return;
                    if (type.ArrayItem.Name != null)
                        x.Name = type.ArrayItem.Name;
                    if (type.ArrayItem.Comments != null)
                        x.Comments = type.ArrayItem.Comments;
                },
                x =>
                {
                    if (type.ArrayItem?.Name != null) x.Name = type.ArrayItem.Name;
                });

            var arrayClosing = new BodyDefinition
            {
                Name = type.Name,
                Whitespace = Whitespace.Repeat(level),
                IsClosing = true,
                IsArray = true
            };

            closing?.Invoke(arrayClosing);

            data.Add(arrayClosing);
        }

        private void WalkDictionary(List<BodyDefinition> data, DataType type, int level,
            Action<BodyDefinition> opening = null,
            Action<BodyDefinition> closing = null)
        {
            var dictionaryOpening = new BodyDefinition
            {
                Name = type.Name,
                Namespace = type.Namespace,
                Comments = type.Comments,
                Whitespace = Whitespace.Repeat(level),
                IsOpening = true,
                IsDictionary = true
            };

            opening?.Invoke(dictionaryOpening);

            data.Add(dictionaryOpening);

            WalkGraph(data, type.DictionaryEntry.ValueType, level + 1,
                x =>
                {
                    x.Name = type.DictionaryEntry.KeyName ?? 
                        _configuration.DefaultDictionaryKeyName;
                    x.IsDictionaryEntry = true;
                    if (type.DictionaryEntry.ValueComments != null)
                        x.Comments = type.DictionaryEntry.ValueComments;
                    x.DictionaryKey = new Key
                    {
                        TypeName = type.DictionaryEntry.KeyType.Name,
                        Options = WalkOptions(type.DictionaryEntry.KeyType),
                        Comments = type.DictionaryEntry.KeyComments
                    };
                },
                x =>
                {
                    x.Name = _configuration.DefaultDictionaryKeyName;
                    x.IsDictionaryEntry = true;
                });

            var dictionaryClosing = new BodyDefinition
            {
                Name = type.Name,
                Whitespace = Whitespace.Repeat(level),
                IsClosing = true,
                IsDictionary = true
            };

            closing?.Invoke(dictionaryClosing);

            data.Add(dictionaryClosing);
        }

        private void WalkComplexType(List<BodyDefinition> data, 
            DataType type, int level,
            Action<BodyDefinition> opening = null,
            Action<BodyDefinition> closing = null)
        {
            var complexOpening = new BodyDefinition
            {
                Name = type.Name,
                Namespace = type.Namespace,
                Comments = type.Comments,
                Whitespace = Whitespace.Repeat(level),
                IsOpening = true,
                IsComplexType = true
            };

            opening?.Invoke(complexOpening);

            data.Add(complexOpening);

            foreach (var member in type.Members)
            {
                var lastMember = member == type.Members.Last();

                WalkGraph(data, member.Type, level + 1, 
                    x => {
                        x.Name = member.Name;
                        x.Comments = member.Comments;
                        x.DefaultValue = member.DefaultValue;
                        if (member.SampleValue != null) x.SampleValue = 
                            member.SampleValue.ToSampleValueString(_configuration);
                        x.IsMember = true;
                        if (lastMember) x.IsLastMember = true;
                        if (!member.Type.IsSimple) x.IsOpening = true;
                        if (member.Required) x.Required = true;
                        if (member.Optional) x.Optional = true;

                        if (member.Deprecated)
                        {
                            x.IsDeprecated = true;
                            x.DeprecationMessage = member.DeprecationMessage;
                        }
                    }, 
                    x => {
                        x.Name = member.Name;
                        x.IsMember = true;
                        if (lastMember) x.IsLastMember = true;
                    });
            }

            var complexClosing = new BodyDefinition
            {
                Name = type.Name,
                Whitespace = Whitespace.Repeat(level),
                IsClosing = true,
                IsComplexType = true
            };

            closing?.Invoke(complexClosing);

            data.Add(complexClosing);
        }
    }
}