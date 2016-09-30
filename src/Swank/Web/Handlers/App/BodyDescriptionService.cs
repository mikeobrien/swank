using System;
using System.Collections.Generic;
using System.Linq;
using Swank.Extensions;
using Swank.Specification;

namespace Swank.Web.Handlers.App
{
    public class BodyDescriptionService
    {
        public const string Whitespace = "    ";

        private readonly Configuration.Configuration _configuration;

        public BodyDescriptionService(Configuration.Configuration configuration)
        {
            _configuration = configuration;
        }

        public List<BodyDefinitionModel> Create(DataType type)
        {
            var data = new List<BodyDefinitionModel>();
            WalkGraph(data, type, 0);
            data.ForEach((x, i) => x.Index = i + 1);
            return data;
        }

        private void WalkGraph(List<BodyDefinitionModel> data, DataType type, int level, 
            Action<BodyDefinitionModel> opening = null, 
            Action<BodyDefinitionModel> closing = null)
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
            List<BodyDefinitionModel> description, 
            DataType type, int level,
            Action<BodyDefinitionModel> opening)
        {
            var data = new BodyDefinitionModel
            {
                Name = type.Name,
                TypeName = type.Name,
                Comments = type.Comments,
                Namespace = type.Namespace,
                FullNamespace = type.FullNamespace,
                Whitespace = Whitespace.Repeat(level),
                IsSimpleType = true,
                Nullable = type.IsNullable,
                SampleValue = type.SampleValue
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
                    break;
                case Xml.FloatType:
                case Xml.DoubleType:
                case Xml.DecimalType: 
                    data.IsNumeric = true;
                    break;
                case Xml.BooleanType: 
                    data.IsBoolean = true;
                    break;
                case Xml.DateTimeType: 
                    data.IsDateTime = true;
                    break;
                case Xml.DurationType: 
                    data.IsDuration = true;
                    break;
                case Xml.UuidType: 
                    data.IsGuid = true;
                    break;
                default: 
                    data.IsString = true;
                    break;
            }

            data.Enumeration = WalkOptions(type);

            opening?.Invoke(data);
            description.Add(data);
        }

        private static Enumeration WalkOptions(DataType type)
        {
            if (type.Enumeration == null) return null;
            var enumeration = new Enumeration
            {
                Name = type.Enumeration.Name,
                Comments = type.Enumeration.Comments,
                Options = new List<Option>(type.Enumeration.Options.Select(x => new Option
                {
                    Name = x.Name,
                    Value = x.Value,
                    Comments = x.Comments
                }))
            };
            return enumeration;
        }

        private void WalkArray(List<BodyDefinitionModel> data, DataType type, int level,
            Action<BodyDefinitionModel> opening = null,
            Action<BodyDefinitionModel> closing = null)
        {
            var arrayOpening = new BodyDefinitionModel
            {
                Name = type.Name,
                Namespace = type.ArrayItem.Type.Namespace,
                FullNamespace = type.ArrayItem.Type.FullNamespace,
                LogicalName = type.ArrayItem.Type.LogicalName,
                Comments = type.Comments,
                Whitespace = Whitespace.Repeat(level),
                IsOpening = true,
                IsArray = true,
                Enumeration = WalkOptions(type)
            };

            if (type.ArrayItem.Type.IsSimple)
                arrayOpening.TypeName = type.ArrayItem.Type.Name;

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

            var arrayClosing = new BodyDefinitionModel
            {
                Name = type.Name,
                Whitespace = Whitespace.Repeat(level),
                IsClosing = true,
                IsArray = true
            };

            closing?.Invoke(arrayClosing);

            data.Add(arrayClosing);
        }

        private void WalkDictionary(List<BodyDefinitionModel> data, DataType type, int level,
            Action<BodyDefinitionModel> opening = null,
            Action<BodyDefinitionModel> closing = null)
        {
            var dictionaryOpening = new BodyDefinitionModel
            {
                Name = type.Name,
                Namespace = type.DictionaryEntry.ValueType.Namespace,
                FullNamespace = type.DictionaryEntry.ValueType.FullNamespace,
                LogicalName = type.DictionaryEntry.ValueType.LogicalName,
                Comments = type.Comments,
                Whitespace = Whitespace.Repeat(level),
                IsOpening = true,
                IsDictionary = true,
                Enumeration = WalkOptions(type)
            };

            if (type.DictionaryEntry.ValueType.IsSimple)
                dictionaryOpening.TypeName = type.DictionaryEntry.ValueType.Name;

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
                    x.DictionaryKey = new KeyModel
                    {
                        TypeName = type.DictionaryEntry.KeyType.Name,
                        Enumeration = WalkOptions(type.DictionaryEntry.KeyType),
                        Comments = type.DictionaryEntry.KeyComments
                    };
                },
                x =>
                {
                    x.Name = _configuration.DefaultDictionaryKeyName;
                    x.IsDictionaryEntry = true;
                });

            var dictionaryClosing = new BodyDefinitionModel
            {
                Name = type.Name,
                Whitespace = Whitespace.Repeat(level),
                IsClosing = true,
                IsDictionary = true
            };

            closing?.Invoke(dictionaryClosing);

            data.Add(dictionaryClosing);
        }

        private void WalkComplexType(List<BodyDefinitionModel> data, 
            DataType type, int level,
            Action<BodyDefinitionModel> opening = null,
            Action<BodyDefinitionModel> closing = null)
        {
            var complexOpening = new BodyDefinitionModel
            {
                Name = type.Name,
                Namespace = type.Namespace,
                FullNamespace = type.FullNamespace,
                LogicalName = type.LogicalName,
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
                        x.Optional = member.Optional;

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

            var complexClosing = new BodyDefinitionModel
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