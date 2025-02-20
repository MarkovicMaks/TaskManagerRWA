﻿using AutoMapper;
using WebApp.ViewModels;
using TM.BL.Models;

namespace TaskManager.App.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TM.BL.Models.Task, TaskVM>()
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager.User.Username))
            .ForMember(dest => dest.SelectedSkillIds, opt => opt.MapFrom(src => src.TaskSkills.Select(ts => ts.SkillId)))
            .ForMember(dest => dest.AssignedSkills, opt => opt.MapFrom(src => src.TaskSkills.Select(ts => ts.Skill.Name)))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<Manager, TaskVM>();

            CreateMap<Skill, SkillVM>()
            .ForMember(dest => dest.TaskSkillIds, opt => opt.MapFrom(src => src.TaskSkills.Select(ts => ts.Id)))
            .ForMember(dest => dest.UserSkillIds, opt => opt.MapFrom(src => src.UserSkills.Select(us => us.Id)));

            CreateMap<TaskAssignment, TaskAssignmentVM>()
            .ForMember(dest => dest.TaskName, opt => opt.MapFrom(src => src.Task.Title))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));

        }
    }
}
