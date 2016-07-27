using System.Collections.Generic;
using Swank.Extensions;

namespace Swank.Specification
{
    public static class Markdown
    {
        public static void Apply(List<Module> modules)
        {
            modules?.ForEach(m =>
            {
                m.Comments = m.Comments.TransformMarkdownBlock();
                m.Resources?.ForEach(r =>
                {
                    r.Comments = r.Comments.TransformMarkdownBlock();
                    r.Endpoints?.ForEach(e =>
                    {
                        e.Comments = e.Comments.TransformMarkdownBlock();
                        if (e.Request != null)
                        {
                            e.Request.Comments = e.Request.Comments.TransformMarkdownBlock();
                            e.Request.Headers.ForEach(h => h.Comments =
                                h.Comments.TransformMarkdownInline());
                            Apply(e.Request.Type);
                        }

                        if (e.Response != null)
                        {
                            e.Response.Comments = e.Response.Comments.TransformMarkdownBlock();
                            e.Response.Headers.ForEach(h => h.Comments =
                                h.Comments.TransformMarkdownInline());
                            Apply(e.Response.Type);
                        }

                        e.UrlParameters?.ForEach(u => {
                            u.Comments = u.Comments.TransformMarkdownInline();
                            Apply(u.Options);
                        });
                        e.QuerystringParameters?.ForEach(q => {
                            q.Comments = q.Comments.TransformMarkdownInline();
                            Apply(q.Options);
                        });
                        e.StatusCodes?.ForEach(s => s.Comments =
                            s.Comments.TransformMarkdownInline());
                    });
                });
            });
        }

        private static void Apply(DataType type)
        {
            if (type == null) return;

            type.Comments = type.Comments.TransformMarkdownInline();
            if (type.ArrayItem != null)
            {
                type.ArrayItem.Comments = type.ArrayItem.Comments.TransformMarkdownInline();
                Apply(type.ArrayItem.Type);
            }
            if (type.DictionaryEntry != null)
            {
                type.DictionaryEntry.KeyComments = type.DictionaryEntry
                    .KeyComments.TransformMarkdownInline();
                type.DictionaryEntry.ValueComments = type.DictionaryEntry
                    .ValueComments.TransformMarkdownInline();
                Apply(type.DictionaryEntry.KeyType);
                Apply(type.DictionaryEntry.ValueType);
            }
            type.Members?.ForEach(m =>
            {
                m.Comments = m.Comments.TransformMarkdownInline();
                m.DeprecationMessage = m.DeprecationMessage.TransformMarkdownInline();
                Apply(m.Type);
            });
            Apply(type.Options);
        }

        private static void Apply(Enumeration enumeration)
        {
            if (enumeration == null) return;
            enumeration.Comments = enumeration.Comments.TransformMarkdownInline();
            enumeration.Options?.ForEach(o =>
            {
                o.Comments = o.Comments.TransformMarkdownInline();
            });
        }
    }
}
