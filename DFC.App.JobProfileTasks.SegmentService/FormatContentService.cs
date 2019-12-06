using DFC.HtmlToDataTranslator.Contracts;
using DFC.HtmlToDataTranslator.Services;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfileTasks.SegmentService
{
    public class FormatContentService : IFormatContentService
    {
        private readonly IHtmlToDataTranslator htmlTranslator;

        public FormatContentService()
        {
            htmlTranslator = new HtmlAgilityPackDataTranslator();
        }

        public string GetParagraphText(string openingText, IEnumerable<string> dataItems, string separator)
        {
            if (dataItems == null || !dataItems.Any())
            {
                return string.Empty;
            }

            var translatedDataItems = TranslateItems(dataItems);

            switch (translatedDataItems.Count())
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

        private IEnumerable<string> TranslateItems(IEnumerable<string> sourceItems)
        {
            var result = new List<string>();

            foreach (var sourceItem in sourceItems)
            {
                result.AddRange(htmlTranslator.Translate(sourceItem));
            }

            return result;
        }
    }
}