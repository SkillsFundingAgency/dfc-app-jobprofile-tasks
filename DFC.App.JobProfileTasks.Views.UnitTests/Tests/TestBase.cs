using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net;

namespace DFC.App.JobProfileTasks.Views.UnitTests.Tests
{
    public class TestBase
    {
        private readonly IConfigurationRoot configuration;

        protected string CurrencySymbol => CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;

        protected string ViewRootPath => "..\\..\\..\\..\\DFC.App.JobProfileTasks\\";

        protected string HtmlEncode(string value)
        {
            return WebUtility.HtmlEncode(value);
        }
    }
}