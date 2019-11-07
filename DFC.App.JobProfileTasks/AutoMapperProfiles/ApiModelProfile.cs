using AutoMapper;
using DFC.App.JobProfileTasks.ApiModels;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.App.JobProfileTasks.ViewModels;
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
                .ForMember(d => d.WorkingEnvironment, s => s.Ignore())
                ;

            CreateMap<BodyDataViewModel, WorkingEnvironmentApiModel>();
        }
    }
}