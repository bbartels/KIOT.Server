using AutoMapper;

using KIOT.Server.Core.Models.Application;
using KIOT.Server.Dto.Application.Appliances;
using KIOT.Server.Dto.Application.Caretakers;
using KIOT.Server.Dto.Application.Caretakers.Customers;
using KIOT.Server.Dto.Application.Customers;
using KIOT.Server.Dto.Application.Tasks;

namespace KIOT.Server.Business.Handler
{
    public class HandlerMappingProfile : Profile
    {
        public HandlerMappingProfile()
        {
            CreateMap<Customer, CustomerInfoDto>()
                .ForMember(d => d.CustomerId, x => x.MapFrom(s => s.Guid))
                .ForMember(d => d.Username, x => x.MapFrom(s => s.Username))
                .ForMember(d => d.FirstName, x => x.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, x => x.MapFrom(s => s.LastName))
                .ForMember(d => d.PhoneNumber, x => x.MapFrom(s => s.PhoneNumber));

            CreateMap<Caretaker, CaretakerInfoDto>()
                .ForMember(d => d.CustomerId, x => x.MapFrom(s => s.Guid))
                .ForMember(d => d.Username, x => x.MapFrom(s => s.Username))
                .ForMember(d => d.FirstName, x => x.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, x => x.MapFrom(s => s.LastName))
                .ForMember(d => d.PhoneNumber, x => x.MapFrom(s => s.PhoneNumber));

            CreateMap<CaretakerForCustomerRequest, CaretakerForCustomerRequestDto>()
                .ForMember(d => d.RequestId, x => x.MapFrom(s => s.Guid))
                .ForMember(d => d.Timestamp, x => x.MapFrom(s => s.Timestamp));

            CreateMap<CustomerTask, CustomerTaskDto>()
                .ForMember(d => d.Guid, x => x.MapFrom(s => s.Guid))
                .ForMember(d => d.Title, x => x.MapFrom(s => s.Title))
                .ForMember(d => d.Description, x => x.MapFrom(s => s.Description))
                .ForMember(d => d.ExpiresAt, x => x.MapFrom(s => s.ExpiresAt))
                .ForMember(d => d.AssignedBy, x => x.MapFrom(s => s.Caretaker.Username))
                .ForMember(d => d.Finished, x => x.MapFrom(s => s.TaskFinished))
                .ForMember(d => d.StartedAt, x => x.MapFrom(s => s.StartedAt));

            CreateMap<ApplianceCategory, ApplianceCategoryDto>()
                .ForMember(d => d.CategoryName, x => x.MapFrom(s => s.Name))
                .ForMember(d => d.Guid, x => x.MapFrom(s => s.Guid));
        }
    }
}
