﻿using AutoMapper;
using DFC.App.JobProfileTasks.ApiModels;
using DFC.App.JobProfileTasks.AutoMapperProfiles.ValueConverters;
using DFC.App.JobProfileTasks.Data.Models.SegmentModels;
using DFC.HtmlToDataTranslator.Services;
using DFC.HtmlToDataTranslator.ValueConverters;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfileTasks.AutoMapperProfiles
{
    public class ApiModelProfile : Profile
    {
        public ApiModelProfile()
        {
            var htmlDataTranslator = new HtmlAgilityPackDataTranslator();
            var htmlToStringValueConverter = new HtmlToStringValueConverter(htmlDataTranslator);

            CreateMap<JobProfileTasksDataSegmentModel, WhatYouWillDoApiModel>()
                .ForMember(d => d.WYDDayToDayTasks, opt => opt.MapFrom((source, destination, member) =>
                {
                    var result = new List<string>();
                    var introTranslated = new List<string>();
                    var tasksTranslated = new List<string>();

                    if (!string.IsNullOrWhiteSpace(source.Introduction))
                    {
                        introTranslated = htmlDataTranslator.Translate(source.Introduction);
                    }

                    if (!string.IsNullOrWhiteSpace(source.Tasks))
                    {
                        tasksTranslated = htmlDataTranslator.Translate(source.Tasks);
                    }

                    result.AddRange(introTranslated);

                    if (introTranslated.Any() && tasksTranslated.Any())
                    {
                        result.Add(" ");
                    }

                    result.AddRange(tasksTranslated);

                    return result;
                }))
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