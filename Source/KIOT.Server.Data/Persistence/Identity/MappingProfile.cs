using AutoMapper;

using KIOT.Server.Core.Models.Application;

namespace KIOT.Server.Data.Persistence.Identity
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, ApplicationUser>()
                .ForMember(iu => iu.Id, x => x.MapFrom(u => u.IdentityId))
                .ForMember(iu => iu.UserName, x => x.MapFrom(u => u.Username))
                .ForMember(iu => iu.Email, x => x.MapFrom(u => u.Email))
                .ForMember(iu => iu.Claims, x => x.MapFrom(u => u.Claims))
                .ForMember(iu => iu.PasswordHash, x => x.MapFrom(u => u.HashedPassword))
                .ForAllOtherMembers(x => x.Ignore());

            SetUserMappingExpression(CreateMap<ApplicationUser, Customer>());
            SetUserMappingExpression(CreateMap<ApplicationUser, Caretaker>());
        }

        private static void SetUserMappingExpression<TUser>(IMappingExpression<ApplicationUser, TUser> expression) where TUser : User
        {
            expression
                .ForMember(u => u.Claims, x => x.MapFrom(iu => iu.Claims))
                .ForMember(u => u.HashedPassword, x => x.MapFrom(iu => iu.PasswordHash))
                .ForMember(u => u.Email, x => x.MapFrom(iu => iu.Email))
                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
