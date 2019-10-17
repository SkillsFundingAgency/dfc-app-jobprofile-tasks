using AutoMapper;
using DFC.App.JobProfileTasks.Data.Models;
using DFC.App.JobProfileTasks.Data.ServiceBusModels;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.AutoMapperProfiles
{
    public class TasksProfile : Profile
    {
        public TasksProfile()
        {
            CreateMap<JobProfileTasksServiceBusModel, JobProfileTasksSegmentModel>()
                .ForMember(s => s.DocumentId, d => d.MapFrom(a => a.Id));

            CreateMap<JobProfileTasksDataEnvironmentServiceBusModel, JobProfileTasksDataEnvironmentSegmentModel>();
            CreateMap<JobProfileTasksDataLocationServiceBusModel, JobProfileTasksDataLocationSegmentModel>();
            CreateMap<JobProfileTasksDataServiceBusModel, JobProfileTasksDataSegmentModel>();
            CreateMap<JobProfileTasksDataUniformServiceBusModel, JobProfileTasksDataUniformSegmentModel>();
        }
    }
}