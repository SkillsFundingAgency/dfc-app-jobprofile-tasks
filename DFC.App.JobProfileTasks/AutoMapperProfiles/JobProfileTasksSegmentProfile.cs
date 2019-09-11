using AutoMapper;
using DFC.App.JobProfileTasks.Data.Models;
using DFC.App.JobProfileTasks.ViewModels;

namespace DFC.App.JobProfileTasks.AutoMapperProfiles
{
    public class JobProfileTasksSegmentProfile : Profile
    {
        public JobProfileTasksSegmentProfile()
        {
            CreateMap<JobProfileTasksSegmentModel, IndexDocumentViewModel>();

            CreateMap<JobProfileTasksSegmentModel, DocumentViewModel>();

            CreateMap<JobProfileTasksSegmentModel, BodyViewModel>();
        }
    }
}
