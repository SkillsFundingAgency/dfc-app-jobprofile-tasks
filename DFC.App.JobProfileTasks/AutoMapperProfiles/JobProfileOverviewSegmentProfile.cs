using AutoMapper;
using DFC.App.JobProfileTasks.Data.Models;
using DFC.App.JobProfileTasks.ViewModels;
using Microsoft.AspNetCore.Html;

namespace DFC.App.JobProfileTasks.AutoMapperProfiles
{
    public class JobProfileOverviewSegmentProfile : Profile
    {
        public JobProfileOverviewSegmentProfile()
        {
            CreateMap<JobProfileTasksSegmentModel, IndexDocumentViewModel>()
                .ForMember(d => d.CanonicalName, s => s.MapFrom(x => x.CanonicalName));

            CreateMap<JobProfileTasksSegmentModel, DocumentViewModel>()
                .ForMember(d => d.Content, s => s.MapFrom(x => new HtmlString(x.Content)));
        }
    }
}
