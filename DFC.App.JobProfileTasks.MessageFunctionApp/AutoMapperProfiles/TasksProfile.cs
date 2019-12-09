using AutoMapper;
using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels.PatchModels;
using Environment = DFC.App.JobProfileTasks.Data.Models.SegmentModels.Environment;
using Location = DFC.App.JobProfileTasks.Data.Models.SegmentModels.Location;
using Uniform = DFC.App.JobProfileTasks.Data.Models.SegmentModels.Uniform;

namespace DFC.App.JobProfileTasks.MessageFunctionApp.AutoMapperProfiles
{
    public class TasksProfile : Profile
    {
        public TasksProfile()
        {
            CreateMap<Data.Models.ServiceBusModels.Environment, Environment>();

            CreateMap<JobProfileMessage, JobProfileTasksSegmentModel>()
                .ForPath(d => d.Data.LastReviewed, s => s.MapFrom(a => a.LastModified))
                .ForMember(d => d.DocumentId, s => s.MapFrom(a => a.JobProfileId))
                .ForMember(d => d.Data, s => s.MapFrom(a => a.WhatYouWillDoData));

            CreateMap<Data.Models.ServiceBusModels.Environment, Environment>();

            CreateMap<Data.Models.ServiceBusModels.Location, Location>();

            CreateMap<JobProfileTasksDataServiceBusModel, JobProfileTasksDataSegmentModel>()
                .ForMember(d => d.Tasks, s => s.MapFrom(a => a.DailyTasks));

            CreateMap<Data.Models.ServiceBusModels.Uniform, Uniform>();

            CreateMap<PatchUniformModel, Uniform>();

            CreateMap<PatchUniformServiceBusModel, PatchUniformModel>();

            CreateMap<PatchLocationServiceBusModel, PatchLocationModel>();

            CreateMap<PatchEnvironmentServiceBusModel, PatchEnvironmentsModel>();
        }
    }
}