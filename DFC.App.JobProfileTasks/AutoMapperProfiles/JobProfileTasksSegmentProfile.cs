﻿using AutoMapper;
using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
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

            CreateMap<JobProfileTasksDataSegmentModel, BodyDataViewModel>().ForAllOtherMembers(x => x.Ignore());

            CreateMap<PatchUniformModel, JobProfileTasksDataUniformSegmentModel>();

            CreateMap<PatchLocationModel, JobProfileTasksDataLocationSegmentModel>();

            CreateMap<PatchEnvironmentsModel, JobProfileTasksDataEnvironmentSegmentModel>();
        }
    }
}
