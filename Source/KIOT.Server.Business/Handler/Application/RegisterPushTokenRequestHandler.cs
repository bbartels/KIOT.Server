using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

using KIOT.Server.Business.Request.Application;
using KIOT.Server.Core.Data.Persistence;
using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;

namespace KIOT.Server.Business.Handler.Application
{
    internal class RegisterPushTokenRequestHandler : IRequestHandler<RegisterPushTokenRequest, CommandResponse>
    {
        private readonly IValidator<RegisterPushTokenRequest> _validator;
        private readonly IUnitOfWork<IUserRepository, User> _unitOfWork;
        private readonly IUnitOfWork<IMobileDeviceRepository, MobileDevice> _mUnitOfWork;

        public RegisterPushTokenRequestHandler(IValidator<RegisterPushTokenRequest> validator,
            IUnitOfWork<IUserRepository, User> unitOfWork, IUnitOfWork<IMobileDeviceRepository, MobileDevice> mUnitOfWork)
        {
            _validator = validator;
            _unitOfWork = unitOfWork;
            _mUnitOfWork = mUnitOfWork;
        }

        public async Task<CommandResponse> Handle(RegisterPushTokenRequest request, CancellationToken cancellationToken)
        {
            var validator = _validator.Validate(request);
            if (!validator.IsValid)
            {
                return new CommandResponse(validator);
            }

            var dto = request.PushTokenDto;

            var user = await _unitOfWork.Repository.GetByGuidAsync(request.UserGuid,
                $"{nameof(User.UsesDevices)}.{nameof(MobileDeviceForUser.MobileDevice)}.{nameof(MobileDevice.PushTokens)},{nameof(User.PushTokens)}");


            if (!user.HasDeviceRegistered(dto.InstallationId))
            {
                var device = await _mUnitOfWork.Repository.SingleOrDefaultAsync(d => d.InstallationId == dto.InstallationId)
                    ?? new MobileDevice(request.PushTokenDto.DeviceName, (MobileOS)dto.MobileOs, dto.InstallationId);
                    user.AddMobileDevice(device);
            }

            user.AddPushToken(new PushToken(user.GetMobileDevice(dto.InstallationId), dto.PushToken));

            try
            {
                _unitOfWork.Repository.Update(user);
                await _unitOfWork.SaveChangesAsync();
            }

            catch (Exception)
            {
                return new CommandResponse(new Error("CouldNotRegisterToken", "Unable to register token with user."));
            }

            return new CommandResponse();
        }
    }
}
