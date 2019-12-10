using AutoMapper;
using DFC.App.JobProfileTasks.SegmentService;
using DFC.HtmlToDataTranslator.Contracts;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfileTasks.AutoMapperProfiles.ValueConverters
{
    [ExcludeFromCodeCoverage]
    public class EnvironmentFormatter : FormatContentService, IValueConverter<IEnumerable<string>, string>
    {
        private readonly IHtmlToDataTranslator htmlToDataTranslator;

        public EnvironmentFormatter(IHtmlToDataTranslator htmlToDataTranslator)
        {
            this.htmlToDataTranslator = htmlToDataTranslator;
        }

        public string Convert(IEnumerable<string> sourceMember, ResolutionContext context)
        {
            const string EnvironmentLeadingText = "Your working environment may be";
            const string EnvironmentConjunction = "and";

            return GetParagraphText(EnvironmentLeadingText, sourceMember, EnvironmentConjunction, htmlToDataTranslator);
        }
    }
}