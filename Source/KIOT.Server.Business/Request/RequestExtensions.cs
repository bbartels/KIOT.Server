using System;
using System.Security.Claims;
using FluentValidation;
using KIOT.Server.Core.Models.Application;

namespace KIOT.Server.Business.Request
{
    public static class RequestExtensions
    {
        public static Guid GetGuid(this ClaimsIdentity identity)
        {
            return Guid.TryParse(identity.FindFirst("guid")?.Value, out var guid) ? 
                guid : throw new ArgumentException();
        }
        public static Guid? GetGuidOrDefault(this ClaimsIdentity identity)
        {
            return Guid.TryParse(identity.FindFirst("guid")?.Value, out var guid) ? 
                (Guid?)guid : null;
        }

        public static string GetUserType(this ClaimsIdentity identity)
        {
            return identity.FindFirst("role")?.Value;
        }

        public static IRuleBuilderOptions<T, TProperty> MustBeCaretaker<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder) where TProperty : ClaimsIdentity
        {
            return ruleBuilder.Must(x => x.GetUserType() == nameof(Caretaker));
        }

        public static IRuleBuilderOptions<T, TProperty> MustBeCustomer<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder) where TProperty : ClaimsIdentity
        {
            return ruleBuilder.Must(x => x.GetUserType() == nameof(Customer));
        }

        public static IRuleBuilderOptions<T, TProperty> MustBeCustomerOrCaretaker<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder) where TProperty : ClaimsIdentity
        {
            return ruleBuilder.Must(x => x.GetUserType() == nameof(Customer) || x.GetUserType() == nameof(Caretaker));
        }

        public static IRuleBuilderOptions<T, TProperty> MustBeAuthenticated<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder) where TProperty : ClaimsIdentity
        {
            return ruleBuilder.NotNull().Must(x => x.IsAuthenticated)
                .Must(x => x.GetGuidOrDefault() != null)
                .Must(x => x.GetGuid() != Guid.Empty);
        }
    }
}
