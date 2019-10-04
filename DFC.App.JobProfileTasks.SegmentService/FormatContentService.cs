using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfileTasks.SegmentService
{
    public class FormatContentService : IFormatContentService
    {
        public string GetParagraphText(string openingText, IEnumerable<string> dataItems, string separator)
        {
            if (dataItems == null || !dataItems.Any())
            {
                return string.Empty;
            }

            switch (dataItems.Count())
            {
                case 1:
                    return $"{openingText} {dataItems.FirstOrDefault()}.";

                case 2:
                    return $"{openingText} {string.Join($" {separator} ", dataItems)}.";

                default:
                    return
                        $"{openingText} {string.Join(", ", dataItems.Take(dataItems.Count() - 1))} {separator} {dataItems.Last()}.";
            }
        }
    }
}