using AutoMapper;
using Wellmeet.Core.Enums;
using Wellmeet.Data;
using Wellmeet.DTO;

namespace Wellmeet.Configuration
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            // USER MAPPINGS !!!! ReverseMap() enables mapping from DTO → Entity, which is dangerous so i avoid it but i will check if needed later
            // Registration → User
            // (Password is included because we hash it later in the service)
            CreateMap<UserRegisterDTO, User>();

            // Update → User (DO NOT touch password)
            CreateMap<UserUpdateDTO, User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());

            // User → UserReadOnlyDTO
            CreateMap<User, UserReadOnlyDTO>()
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.UserRole.ToString()));


            // ACTIVITY MAPPINGS
            CreateMap<ActivityCreateDTO, Activity>();
            CreateMap<ActivityUpdateDTO, Activity>();

            CreateMap<Activity, ActivityReadOnlyDTO>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator))
                .ForMember(dest => dest.CurrentParticipants, opt => opt.MapFrom(src => src.Participants!.Count));

            // ACTIVITY PARTICIPANT
            CreateMap<ActivityParticipant, ActivityParticipantReadOnlyDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.ActivityId, opt => opt.MapFrom(src => src.ActivityId))
                .ForMember(dest => dest.ActivityTitle, opt => opt.MapFrom(src => src.Activity.Title))
                .ForMember(dest => dest.ActivityStartDateTime, opt => opt.MapFrom(src => src.Activity.StartDateTime));

            // DASHBOARD DTO - no mapping needed (constructed manually)


            CreateMap<Activity, ActivityHasJoinedReadOnlyDTO>()
                    .IncludeBase<Activity, ActivityReadOnlyDTO>();  //add explicit mapping to include base properties just in case
            //used in new user-aware activity 

        }
    }
}
