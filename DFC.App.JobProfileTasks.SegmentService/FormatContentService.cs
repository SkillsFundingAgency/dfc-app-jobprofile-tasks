using DFC.HtmlToDataTranslator.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfileTasks.SegmentService
{
    public class FormatContentService : IFormatContentService
    {
        public string GetParagraphText(string openingText, IEnumerable<string> dataItems, string separator, IHtmlToDataTranslator htmlToDataTranslator)
        {
            if ((!dataItems?.Any()).GetValueOrDefault())
            {
                return string.Empty;
            }

            var translatedDataItems = TranslateItems(htmlToDataTranslator, dataItems);

            switch (translatedDataItems.Count)
            {
                case 1:
                    return $"{openingText} {translatedDataItems.FirstOrDefault()}.";

                case 2:
                    return $"{openingText} {string.Join($" {separator} ", translatedDataItems)}.";

                default:
                    return
                        $"{openingText} {string.Join(", ", translatedDataItems.Take(translatedDataItems.Count() - 1))} {separator} {translatedDataItems.Last()}.";
            }
        }

        private IList<string> TranslateItems(IHtmlToDataTranslator htmlToDataTranslator, IEnumerable<string> sourceItems)
        {
            var result = new List<string>();

            if (htmlToDataTranslator != null)
            {
                foreach (var sourceItem in sourceItems)
                {
                    result.AddRange(htmlToDataTranslator.Translate(sourceItem));
                }
            }
            else
            {
                result.AddRange(sourceItems);
            }

            return result.ToList();
        }
    }
}