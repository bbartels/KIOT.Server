using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using KIOT.Server.Core.Data.Persistence.Application;
using KIOT.Server.Core.Models.Application;
using KIOT.Server.Core.Response;
using KIOT.Server.Data.Persistence.Identity;

namespace KIOT.Server.Data.Persistence.Application
{
    internal class UserRepository : Repository<User>, IUserRepository
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        private KIOTContext KIOTContext => Context as KIOTContext;

        public UserRepository(KIOTContext context, IMapper mapper, UserManager<ApplicationUser> userManager) : base(context)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<DataResponse> RegisterCustomer(string firstName, string lastName, string username, string email,
            string password, string code, string phoneNumber)
        {
            if (await CustomerCodeExists(code))
            {
                return new DataResponse(new [] { new Error("RegisterCustomerFailed", "CustomerId is already registered with a different user.") });
            }

            var (identityUser, idTask) = RegisterUser(username, email, password);
            var result = await idTask;

            if (!result.Succeeded) { return new DataResponse(result.Errors.Select(x => new Error(x.Code, x.Description))); }

            var user = new Customer(identityUser.Id, firstName, lastName, identityUser.UserName, code, phoneNumber);

            try
            {
                KIOTContext.Customers.Add(user);
                await KIOTContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                await DeregisterUser(identityUser);
                return new DataResponse(new [] { new Error("RegisterCustomerFailed", "Unknown Error.") });
            }

            return new DataResponse();
        }

        public async Task<DataResponse> RegisterCaretaker(string firstName, string lastName, string username, string email,
            string password, string phoneNumber)
        {
            var (identityUser, idTask) = RegisterUser(username, email, password);
            var result = await idTask;

            if (!result.Succeeded) { return new DataResponse(result.Errors.Select(x => new Error(x.Code, x.Description))); }

            var user = new Core.Models.Application.Caretaker(identityUser.Id, firstName, lastName, identityUser.UserName, phoneNumber);

            try
            {
                KIOTContext.Caretakers.Add(user);
                await KIOTContext.SaveChangesAsync();
            }

            catch (Exception)
            {
                await DeregisterUser(identityUser);
                return new DataResponse(new [] { new Error("RegisterCustomerFailed", "Unknown Error.") });
            }

            return new DataResponse();
        }

        public async Task<DataResponse<IEnumerable<Claim>>> GetClaimsAsync(User user)
        {
            var claims = await _userManager.GetClaimsAsync(_mapper.Map<ApplicationUser>(user));

            return claims == null 
                ? new DataResponse<IEnumerable<Claim>>(null, new Error("InvalidUser", "User could not be found."))
                : new DataResponse<IEnumerable<Claim>>(claims);
        }

        public async Task<DataResponse<User>> GetUserByName(string username)
        {
            var appUser = await _userManager.FindByNameAsync(username);

            if (appUser != null)
            {
                var claims = await _userManager.GetClaimsAsync(appUser);
                appUser.Claims = claims;
                var user = await SingleOrDefaultAsync(u => u.IdentityId == appUser.Id);

                switch (user)
                {
                    case Customer c: { return new DataResponse<User>(_mapper.Map(appUser, c)); }
                    case Core.Models.Application.Caretaker c: { return new DataResponse<User>(_mapper.Map(appUser, c)); }
                    default: { throw new ArgumentException("Cannot resolve User type."); }
                }
            }

            return new DataResponse<User>(null, new Error("UserNotFound", $"Could not find user with username: {username}."));
        }

        public async Task<DataResponse> CheckCredentials(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(_mapper.Map<ApplicationUser>(user), password)
                ? new DataResponse()
                : new DataResponse(new[] { new Error("Authentication", "Could not authenticate user.") });
        }

        private (ApplicationUser, Task<IdentityResult>) RegisterUser(string username, string email, string password)
        {
            var identityUser = new ApplicationUser { UserName = username, Email = email };

            return (identityUser, _userManager.CreateAsync(identityUser, password));
        }

        private Task<IdentityResult> DeregisterUser(ApplicationUser user) { return _userManager.DeleteAsync(user); }

        private Task<bool> CustomerCodeExists(string code) { return KIOTContext.Customers.AnyAsync(c => c.Code == code); }
    }
}
