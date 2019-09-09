using AutoMapper;
using DFC.App.JobProfileTasks.Data.Models;
using DFC.App.JobProfileTasks.ViewModels;
using Microsoft.AspNetCore.Html;

namespace DFC.App.JobProfileTasks.AutoMapperProfiles
{
    public class JobProfileTasksSegmentProfile : Profile
    {
        public JobProfileTasksSegmentProfile()
        {
            CreateMap<JobProfileTasksSegmentModel, IndexDocumentViewModel>();

            CreateMap<JobProfileTasksSegmentModel, DocumentViewModel>()
                .ForMember(d => d.Content, s => s.MapFrom(x => new HtmlString(x.Content)));

            CreateMap<JobProfileTasksSegmentModel, BodyViewModel>()
                    .ForMember(d => d.Content, s => s.MapFrom(x => new HtmlString(x.Content)));
        }
    }
}
