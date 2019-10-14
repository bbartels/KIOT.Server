using KIOT.Server.Business.Response.Application;
using KIOT.Server.Core.Presenter;
using KIOT.Server.Dto.Application;
using KIOT.Server.Dto.Application.Authentication;

namespace KIOT.Server.Api.Presenter.Application
{
    public class ExchangeRefreshTokenPresenter : Presenter<ExchangeRefreshTokenResponse>
    {
        public ExchangeRefreshTokenPresenter(ExchangeRefreshTokenResponse response) : base(response)
        {
            if (response.Succeeded)
            {
                Content = new AccessTokenDto
                {
                    AccessToken = response.AccessToken,
                    ExpiresIn = (int)response.ExpiresIn.TotalSeconds,
                    RefreshToken = response.RefreshToken
                };
            }
        }
    }
}
