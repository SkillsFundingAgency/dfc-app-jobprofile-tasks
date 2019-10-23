using AutoMapper;
using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.AutoMapperProfiles
{
    public class TasksProfile : Profile
    {
        public TasksProfile()
        {
            CreateMap<JobProfileTasksDataEnvironmentServiceBusModel, JobProfileTasksDataEnvironmentSegmentModel>();

            CreateMap<JobProfileServiceBusModel, JobProfileTasksSegmentModel>()
                .ForMember(d => d.Data, s => s.MapFrom(a => a.WhatYouWillDoData));

            CreateMap<JobProfileTasksDataEnvironmentServiceBusModel, JobProfileTasksDataEnvironmentSegmentModel>();

            CreateMap<JobProfileTasksDataLocationServiceBusModel, JobProfileTasksDataLocationSegmentModel>();

            CreateMap<JobProfileTasksDataServiceBusModel, JobProfileTasksDataSegmentModel>();

            CreateMap<JobProfileTasksDataUniformServiceBusModel, JobProfileTasksDataUniformSegmentModel>();

            CreateMap<PatchUniformModel, JobProfileTasksDataUniformSegmentModel>();

            CreateMap<JobProfileUniformPatchServiceBusModel, PatchUniformModel>();

            CreateMap<JobProfileLocationPatchServiceBusModel, PatchLocationModel>();

            CreateMap<JobProfileEnvironmentPatchServiceBusModel, PatchEnvironmentsModel>();
        }
    }
}