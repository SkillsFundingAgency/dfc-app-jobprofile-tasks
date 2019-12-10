using AutoMapper;
using DFC.App.JobProfileTasks.SegmentService;
using DFC.HtmlToDataTranslator.Contracts;
using System.Collections.Generic;

namespace DFC.App.JobProfileTasks.AutoMapperProfiles.ValueConverters
{
    public class LocationFormatter : FormatContentService, IValueConverter<IEnumerable<string>, string>
    {
        private readonly IHtmlToDataTranslator htmlToDataTranslator;

        public LocationFormatter(IHtmlToDataTranslator htmlToDataTranslator)
        {
            this.htmlToDataTranslator = htmlToDataTranslator;
        }

        public string Convert(IEnumerable<string> sourceMember, ResolutionContext context)
        {
            const string LocationLeadingText = "You could work";
            const string LocationConjunction = "or";

            return GetParagraphText(LocationLeadingText, sourceMember, LocationConjunction, htmlToDataTranslator);
        }
    }
}