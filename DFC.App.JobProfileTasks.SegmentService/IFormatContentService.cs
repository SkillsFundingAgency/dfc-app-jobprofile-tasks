using DFC.HtmlToDataTranslator.Contracts;
using System.Collections.Generic;

namespace DFC.App.JobProfileTasks.SegmentService
{
    public interface IFormatContentService
    {
        string GetParagraphText(string openingText, IEnumerable<string> dataItems, string separator, IHtmlToDataTranslator htmlToDataTranslator);
    }
}