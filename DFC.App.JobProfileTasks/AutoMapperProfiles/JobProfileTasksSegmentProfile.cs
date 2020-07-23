using AutoMapper;
using DFC.App.JobProfileTasks.AutoMapperProfiles.ValueConverters;
using DFC.App.JobProfileTasks.Data.Models.PatchModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.Data.Models.ServiceBusModels;
using DFC.App.JobProfileTasks.ViewModels;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Environment = DFC.App.JobProfileTasks.Data.Models.SegmentModels.Environment;
using Location = DFC.App.JobProfileTasks.Data.Models.SegmentModels.Location;
using Uniform = DFC.App.JobProfileTasks.Data.Models.SegmentModels.Uniform;

namespace DFC.App.JobProfileTasks.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class JobProfileTasksSegmentProfile : Profile
    {
        public JobProfileTasksSegmentProfile()
        {
            CreateMap<JobProfileTasksSegmentModel, IndexDocumentViewModel>();

            CreateMap<JobProfileTasksSegmentModel, BodyViewModel>();

            CreateMap<JobProfileTasksSegmentModel, DocumentViewModel>()
                .ForMember(d => d.Data, s => s.MapFrom(a => a.Data));

            CreateMap<JobProfileTasksDataSegmentModel, DocumentDataViewModel>()
                .ForMember(d => d.Environment, opt => opt.ConvertUsing(new EnvironmentFormatter(null), s => GetEnvironmentsDescription(s)))
                .ForMember(d => d.Location, opt => opt.ConvertUsing(new LocationFormatter(null), s => GetLocationsDescription(s)))
                .ForMember(d => d.Uniform, opt => opt.ConvertUsing(new UniformFormatter(null), s => GetUniformsDescription(s)));

            CreateMap<JobProfileTasksDataSegmentModel, BodyDataViewModel>()
                .ForMember(d => d.Environment, opt => opt.ConvertUsing(new EnvironmentFormatter(null), s => GetEnvironmentsDescription(s)))
                .ForMember(d => d.Location, opt => opt.ConvertUsing(new LocationFormatter(null), s => GetLocationsDescription(s)))
                .ForMember(d => d.Uniform, opt => opt.ConvertUsing(new UniformFormatter(null), s => GetUniformsDescription(s)));

            CreateMap<JobProfileTasksSegmentModel, RefreshJobProfileSegmentServiceBusModel>()
                .ForMember(d => d.JobProfileId, s => s.MapFrom(a => a.DocumentId))
                .ForMember(d => d.Segment, s => s.MapFrom(a => JobProfileTasksDataSegmentModel.SegmentName));

            CreateMap<PatchUniformModel, Uniform>();

            CreateMap<PatchLocationModel, Location>();

            CreateMap<PatchEnvironmentsModel, Environment>();
        }

        private static IEnumerable<string> GetEnvironmentsDescription(JobProfileTasksDataSegmentModel segmentModel)
        {
            return segmentModel.Environments?.Select(environment => environment.Description);
        }

        private static IEnumerable<string> GetLocationsDescription(JobProfileTasksDataSegmentModel segmentModel)
        {
            return segmentModel.Locations?.Select(location => location.Description);
        }

        private static IEnumerable<string> GetUniformsDescription(JobProfileTasksDataSegmentModel segmentModel)
        {
            return segmentModel.Uniforms?.Select(uniform => uniform.Description);
        }
    }
}