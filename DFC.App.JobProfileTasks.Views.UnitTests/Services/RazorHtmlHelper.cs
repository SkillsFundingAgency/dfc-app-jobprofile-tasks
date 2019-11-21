using RazorEngine.Text;

namespace DFC.App.JobProfileTasks.Views.UnitTests.Services
{
    public class RazorHtmlHelper
    {
        public IEncodedString Raw(string rawString)
        {
            return new RawString(rawString);
        }
    }
}
