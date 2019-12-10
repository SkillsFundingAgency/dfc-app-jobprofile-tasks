using AutoMapper;
using DFC.App.JobProfileTasks.SegmentService;
using DFC.HtmlToDataTranslator.Contracts;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfileTasks.AutoMapperProfiles.ValueConverters
{
    [ExcludeFromCodeCoverage]
    public class UniformFormatter : FormatContentService, IValueConverter<IEnumerable<string>, string>
    {
        private readonly IHtmlToDataTranslator htmlToDataTranslator;

        public UniformFormatter(IHtmlToDataTranslator htmlToDataTranslator)
        {
            this.htmlToDataTranslator = htmlToDataTranslator;
        }

        public string Convert(IEnumerable<string> sourceMember, ResolutionContext context)
        {
            const string UniformLeadingText = "You may need to wear";
            const string UniformConjunction = "and";

            return GetParagraphText(UniformLeadingText, sourceMember, UniformConjunction, htmlToDataTranslator);
        }
    }
}