using AutoMapper;
using WebApp.ViewModels;
using TM.BL.Models;

namespace TaskManager.App.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TM.BL.Models.Task, TaskVm>()
                
               .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager.User.Username))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());


        }
    }
}
