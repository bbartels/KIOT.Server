using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using KIOT.Server.Dto;
using KIOT.Server.Dto.Application.Appliances;
using KIOT.Server.Dto.Application.Authentication;
using KIOT.Server.Dto.Application.Customers.Appliances;
using KIOT.Server.Dto.Application.Customers.Caretakers;
using KIOT.Server.Dto.Application.Customers.Data;
using KIOT.Server.Dto.Application.Customers.Tasks;
using KIOT.Server.Dto.Application.Caretakers.Customers;
using KIOT.Server.Dto.Application.Caretakers.Data;
using KIOT.Server.Dto.Application.Caretakers.Tasks;

namespace KIOT.Server.Api.Tests.Integration
{
    public interface IClient
    {
        Task<SuccessfulRequestDto> Accounts_RegisterCaretakerAsync(RegisterCaretakerDto caretakerDto);
        Task<SuccessfulRequestDto> Accounts_RegisterCaretakerAsync(RegisterCaretakerDto caretakerDto, CancellationToken cancellationToken);

        Task<SuccessfulRequestDto> Accounts_RegisterCustomerAsync(RegisterCustomerDto customerDto);
        Task<SuccessfulRequestDto> Accounts_RegisterCustomerAsync(RegisterCustomerDto customerDto, CancellationToken cancellationToken);

        Task<SuccessfulRequestDto> Appliance_RegisterApplianceCategoryAsync(string categoryName);
        Task<SuccessfulRequestDto> Appliance_RegisterApplianceCategoryAsync(string categoryName, CancellationToken cancellationToken);

        Task<SuccessfulRequestDto> Appliance_DeleteApplianceCategoryAsync(Guid categoryGuid);
        Task<SuccessfulRequestDto> Appliance_DeleteApplianceCategoryAsync(Guid categoryGuid, CancellationToken cancellationToken);

        Task<SuccessfulRequestDto> Appliance_SetApplianceCategoryAsync(int applianceId, string categoryName);
        Task<SuccessfulRequestDto> Appliance_SetApplianceCategoryAsync(int applianceId, string categoryName, CancellationToken cancellationToken);

        Task<GetApplianceCategoriesDto> Appliance_GetApplianceCategoriesAsync();
        Task<GetApplianceCategoriesDto> Appliance_GetApplianceCategoriesAsync(CancellationToken cancellationToken);

        Task<ICollection<ApplianceActivityDto>> Appliance_GetApplianceActivityAsync();
        Task<ICollection<ApplianceActivityDto>> Appliance_GetApplianceActivityAsync(CancellationToken cancellationToken);

        Task<ICollection<ApplianceActivityDto>> Appliance_GetCustomerApplianceActivityAsync(Guid customerGuid);
        Task<ICollection<ApplianceActivityDto>> Appliance_GetCustomerApplianceActivityAsync(Guid customerGuid, CancellationToken cancellationToken);

        Task<AccessTokenDto> Authentication_AuthenticateAsync(LoginUserDto loginDto);
        Task<AccessTokenDto> Authentication_AuthenticateAsync(LoginUserDto loginDto, CancellationToken cancellationToken);

        Task<AccessTokenDto> Authentication_ExchangeRefreshTokenAsync(ExchangeRefreshTokenDto dto);
        Task<AccessTokenDto> Authentication_ExchangeRefreshTokenAsync(ExchangeRefreshTokenDto dto, CancellationToken cancellationToken);

        Task<SuccessfulRequestDto> Authentication_RegisterPushTokenAsync(RegisterPushTokenDto dto);
        Task<SuccessfulRequestDto> Authentication_RegisterPushTokenAsync(RegisterPushTokenDto dto, CancellationToken cancellationToken);

        Task<PendingCaretakerRequestsDto> Caretaker_GetPendingCaretakerRequestsAsync();
        Task<PendingCaretakerRequestsDto> Caretaker_GetPendingCaretakerRequestsAsync(CancellationToken cancellationToken);

        Task<SuccessfulRequestDto> Caretaker_HandleCaretakerRequestAsync(Guid requestId, HandleCaretakerForCustomerRequestDto dto);
        Task<SuccessfulRequestDto> Caretaker_HandleCaretakerRequestAsync(Guid requestId, HandleCaretakerForCustomerRequestDto dto, CancellationToken cancellationToken);

        Task<AssignedCustomersForCaretakerDto> Caretaker_GetAssignedCustomersAsync();
        Task<AssignedCustomersForCaretakerDto> Caretaker_GetAssignedCustomersAsync(CancellationToken cancellationToken);

        Task<CustomerHomepageForCaretakerDto> Caretaker_GetCustomerHomepageAsync(Guid customerGuid);
        Task<CustomerHomepageForCaretakerDto> Caretaker_GetCustomerHomepageAsync(Guid customerGuid, CancellationToken cancellationToken);

        Task<SuccessfulRequestDto> Caretaker_DeleteAssignedCustomerAsync(Guid customerGuid);
        Task<SuccessfulRequestDto> Caretaker_DeleteAssignedCustomerAsync(Guid customerGuid, CancellationToken cancellationToken);

        Task<SuccessfulRequestDto> Caretaker_AlertCustomerAsync(AlertCustomerDto dto);
        Task<SuccessfulRequestDto> Caretaker_AlertCustomerAsync(AlertCustomerDto dto, CancellationToken cancellationToken);

        Task<SuccessfulRequestDto> Caretaker_RegisterCustomerTaskAsync(RegisterCustomerTaskDto dto);
        Task<SuccessfulRequestDto> Caretaker_RegisterCustomerTaskAsync(RegisterCustomerTaskDto dto, CancellationToken cancellationToken);

        Task<SuccessfulRequestDto> Caretaker_SetCustomerTaskStateAsync(SetCustomerTaskStateDto dto);
        Task<SuccessfulRequestDto> Caretaker_SetCustomerTaskStateAsync(SetCustomerTaskStateDto dto, CancellationToken cancellationToken);

        Task<CustomerTasksForCaretakerDto> Caretaker_GetCustomerTasksAsync(Guid userGuid);
        Task<CustomerTasksForCaretakerDto> Caretaker_GetCustomerTasksAsync(Guid userGuid, CancellationToken cancellationToken);

        Task<CustomerDetailedPageDto> Caretaker_GetCustomerApplianceHistoryAsync(Guid customerGuid,
            TimeInterval timeInterval, int? intervalOffset);
        Task<CustomerDetailedPageDto> Caretaker_GetCustomerApplianceHistoryAsync(Guid customerGuid,
            TimeInterval timeInterval, int? intervalOffset, CancellationToken cancellationToken);

        Task<SuccessfulRequestDto> Customer_AddCaretakerForCustomerAsync(AddCaretakerForCustomerDto dto);
        Task<SuccessfulRequestDto> Customer_AddCaretakerForCustomerAsync(
            AddCaretakerForCustomerDto dto, CancellationToken cancellationToken);

        Task<CustomerHomepageDto> Customer_GetHomepageAsync();
        Task<CustomerHomepageDto> Customer_GetHomepageAsync(CancellationToken cancellationToken);

        Task<AssignedCaretakersForCustomerDto> Customer_GetAssignedCaretakersAsync();
        Task<AssignedCaretakersForCustomerDto> Customer_GetAssignedCaretakersAsync(CancellationToken cancellationToken);

        Task<SuccessfulRequestDto> Customer_DeleteAssignedCaretakerAsync(Guid caretakerGuid);
        Task<SuccessfulRequestDto> Customer_DeleteAssignedCaretakerAsync(Guid caretakerGuid, CancellationToken cancellationToken);

        Task<CustomerDetailedPageDto> Customer_GetDetailedPageAsync(TimeInterval timeInterval, int? intervalOffset);
        Task<CustomerDetailedPageDto> Customer_GetDetailedPageAsync(TimeInterval timeInterval, int? intervalOffset,
            CancellationToken cancellationToken);

        Task<SuccessfulRequestDto> Customer_SetAliasForApplianceAsync(SetAliasForApplianceDto dto);
        Task<SuccessfulRequestDto> Customer_SetAliasForApplianceAsync(SetAliasForApplianceDto dto, CancellationToken cancellationToken);

        Task<CustomerAppliancesDto> Customer_GetAppliancesAsync();
        Task<CustomerAppliancesDto> Customer_GetAppliancesAsync(CancellationToken cancellationToken);

        Task<CustomerTasksDto> Customer_GetAssignedTasksAsync(TaskOption taskOption);
        Task<CustomerTasksDto> Customer_GetAssignedTasksAsync(TaskOption taskOption, CancellationToken cancellationToken);
    }
}