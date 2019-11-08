using AutoMapper;
using DFC.App.JobProfileTasks.SegmentService;
using System.Collections.Generic;

namespace DFC.App.JobProfileTasks.AutoMapperProfiles.ValueConverters
{
    public class EnvironmentFormatter : FormatContentService, IValueConverter<IEnumerable<string>, string>
    {
        public string Convert(IEnumerable<string> sourceMember, ResolutionContext context)
        {
            const string EnvironmentLeadingText = "Your working environment may be";
            const string EnvironmentConjunction = "and";

            return GetParagraphText(EnvironmentLeadingText, sourceMember, EnvironmentConjunction);
        }
    }
}