using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace KIOT.Server.Core.Models.Application
{
    public abstract class User : BaseEntity
    {
        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();

        public string IdentityId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; set; }

        public string HashedPassword { get; private set; }
        public string Name => $"{FirstName} {LastName}";

        public IReadOnlyList<Claim> Claims { get; private set; }
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
        public ICollection<MobileDeviceForUser> UsesDevices { get; private set; }
        public ICollection<PushToken> PushTokens { get; private set; }


        protected User() { }
        protected User(string identityId, string firstName, string lastName, string username, string phoneNumber)
        {
            IdentityId = identityId;
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            PhoneNumber = phoneNumber;
        }

        public void AddRefreshToken(string token, string userId, IPAddress remoteAddress, int daysValid = 3)
        {
            _refreshTokens.Add(new RefreshToken(token, userId, DateTime.UtcNow + TimeSpan.FromDays(daysValid), remoteAddress));
        }

        public void RemoveRefreshToken(string refreshToken)
        {
            _refreshTokens.Remove(_refreshTokens.SingleOrDefault(t => t.Token == refreshToken));
        }

        public bool CheckRefreshToken(string refreshToken)
        {
            return _refreshTokens.Any(t => t.Token == refreshToken && t.IsActive);
        }

        public bool HasPushTokenRegistered(string pushToken)
        {
            return PushTokens.Any(pt => pt.Token == pushToken);
        }

        public bool HasDeviceRegistered(string installationId)
        {
            return UsesDevices.Any(d => d.MobileDevice.InstallationId == installationId);
        }

        public MobileDevice GetMobileDevice(string installationId)
        {
            return UsesDevices.Where(ud => ud.MobileDevice.InstallationId == installationId)
                .Select(ud => ud.MobileDevice).FirstOrDefault();
        }

        public void AddMobileDevice(MobileDevice dv)
        {
            if (HasDeviceRegistered(dv.InstallationId)) { return; }

            UsesDevices.Add(new MobileDeviceForUser(this, dv));
        }

        public void AddPushToken(PushToken token)
        {
            if (!HasDeviceRegistered(token.MobileDevice.InstallationId)) { throw new ArgumentException($"User does not have MobileDevice registered."); }

            InvalidatePushTokensForDevice(token.MobileDevice.InstallationId);
            PushTokens.Add(token);
        }

        public void InvalidatePushTokensForDevice(string installationId)
        {
            UsesDevices.SingleOrDefault(d => d.MobileDevice.InstallationId == installationId)
                ?.MobileDevice.InvalidateRegisteredPushTokens();
        }

        public IEnumerable<PushToken> GetValidPushTokens()
        {
            return PushTokens.Where(pt => pt.IsValid);
        }
    }
}
