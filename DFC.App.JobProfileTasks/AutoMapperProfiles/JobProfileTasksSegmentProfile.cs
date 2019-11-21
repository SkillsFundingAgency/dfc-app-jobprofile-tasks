using AutoMapper;
using DFC.App.JobProfileTasks.AutoMapperProfiles.ValueConverters;
using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels;
using DFC.App.JobProfileTasks.ViewModels;
using System.Linq;

namespace DFC.App.JobProfileTasks.AutoMapperProfiles
{
    public class JobProfileTasksSegmentProfile : Profile
    {
        public JobProfileTasksSegmentProfile()
        {
            CreateMap<JobProfileTasksSegmentModel, IndexDocumentViewModel>();

            CreateMap<JobProfileTasksSegmentModel, DocumentViewModel>();

            CreateMap<JobProfileTasksSegmentModel, BodyViewModel>();

            CreateMap<JobProfileTasksDataSegmentModel, BodyDataViewModel>()
                .ForMember(d => d.Environment, opt => opt.ConvertUsing(new EnvironmentFormatter(), s => s.Environments != null ? s.Environments.Select(x => x.Description) : null))
                .ForMember(d => d.Location, opt => opt.ConvertUsing(new LocationFormatter(), s => s.Locations != null ? s.Locations.Select(x => x.Description) : null))
                .ForMember(d => d.Uniform, opt => opt.ConvertUsing(new UniformFormatter(), s => s.Uniforms != null ? s.Uniforms.Select(x => x.Description) : null))
                ;

            CreateMap<JobProfileTasksSegmentModel, RefreshJobProfileSegmentServiceBusModel>()
                .ForMember(d => d.JobProfileId, s => s.MapFrom(a => a.DocumentId))
                .ForMember(d => d.Segment, s => s.MapFrom(a => JobProfileTasksSegmentModel.SegmentName));

            CreateMap<PatchUniformModel, JobProfileTasksDataUniformSegmentModel>();

            CreateMap<PatchLocationModel, JobProfileTasksDataLocationSegmentModel>();

            CreateMap<PatchEnvironmentsModel, JobProfileTasksDataEnvironmentSegmentModel>();
        }
    }
}
