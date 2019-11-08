using AutoMapper;
using DFC.App.JobProfileTasks.SegmentService;
using System.Collections.Generic;

namespace DFC.App.JobProfileTasks.AutoMapperProfiles.ValueConverters
{
    public class UniformFormatter : FormatContentService, IValueConverter<IEnumerable<string>, string>
    {
        public string Convert(IEnumerable<string> sourceMember, ResolutionContext context)
        {
            const string UniformLeadingText = "You may need to wear";
            const string UniformConjunction = "and";

            return GetParagraphText(UniformLeadingText, sourceMember, UniformConjunction);
        }
    }
}