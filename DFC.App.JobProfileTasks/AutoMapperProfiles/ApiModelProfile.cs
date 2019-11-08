using AutoMapper;
using DFC.App.JobProfileTasks.ApiModels;
using DFC.App.JobProfileTasks.AutoMapperProfiles.ValueConverters;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfileTasks.AutoMapperProfiles
{
    public class HtmlStringFormatter : IValueConverter<string, List<string>>
    {
        public List<string> Convert(string sourceMember, ResolutionContext context)
        {
            return new List<string> { sourceMember };
        }
    }

    public class ApiModelProfile : Profile
    {
        public ApiModelProfile()
        {
            CreateMap<JobProfileTasksDataSegmentModel, WhatYouWillDoApiModel>()
                .ForMember(d => d.WYDDayToDayTasks, opt => opt.ConvertUsing(new HtmlStringFormatter(), s => $"{s.Introduction} {s.Tasks}"))
                .ForMember(d => d.WorkingEnvironment, s => s.MapFrom(a => a))
                ;

            CreateMap<JobProfileTasksDataSegmentModel, WorkingEnvironmentApiModel>()
                .ForMember(d => d.Environment, opt => opt.ConvertUsing(new EnvironmentFormatter(), s => s.Environments != null ? s.Environments.Select(x => x.Description) : null))
                .ForMember(d => d.Location, opt => opt.ConvertUsing(new LocationFormatter(), s => s.Locations != null ? s.Locations.Select(x => x.Description) : null))
                .ForMember(d => d.Uniform, opt => opt.ConvertUsing(new UniformFormatter(), s => s.Uniforms != null ? s.Uniforms.Select(x => x.Description) : null))
               ;
        }
    }
}