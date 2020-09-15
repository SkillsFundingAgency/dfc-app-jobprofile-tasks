using System;

namespace DFC.App.JobProfileOverview.Tests.IntegrationTests.API.Model.API
{
    public class JobProfileOverviewApiResponse
    {
        public string Title { get; set; }

        public DateTime LastUpdatedDate { get; set; }

        public Uri URL { get; set; }

        public string SOC { get; set; }

        public string ONetOccupationalCode { get; set; }

        public string AlternativeTitle { get; set; }

        public string Overview { get; set; }

        public string SalaryStarter { get; set; }

        public string SalaryExperienced { get; set; }

        public double MinimumHours { get; set; }

        public double MaximumHours { get; set; }

        public string WorkingHoursDetails { get; set; }

        public string WorkingPattern { get; set; }

        public string WorkingPatternDetails { get; set; }
    }
}
