using System.Collections.Generic;

namespace DFC.App.JobProfileTasks.Data.Contracts
{
    public interface IFormatContentService
    {
        string GetParagraphText(string openingText, IEnumerable<string> dataItems, string separator);
    }
}
