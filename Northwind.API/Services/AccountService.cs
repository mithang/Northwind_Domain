using Northwind.API.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.API.Services
{
    public class AccountService
    {
        private readonly FacebookService _facebookService;
        private readonly JwtHandler _jwtHandler;

        public AccountService(FacebookService facebookService, JwtHandler jwtHandler)
        {
            _facebookService = facebookService;
            _jwtHandler = jwtHandler;
        }

        public async Task<AuthorizationTokensResource> FacebookLoginAsync(FacebookLoginResource facebookLoginResource)
        {
            if (string.IsNullOrEmpty(facebookLoginResource.facebookToken))
            {
                throw new Exception("Token is null or empty");
            }

            var facebookUser = await _facebookService.GetUserFromFacebookAsync(facebookLoginResource.facebookToken);
            //var domainUser = await unitOfWork.Users.GetAsync(facebookUser.Email);

            //return await CreateAccessTokens(domainUser, facebookLoginResource.DeviceId,
            //    facebookLoginResource.DeviceName);
            return new AuthorizationTokensResource ();
        }

        //private async Task<AuthorizationTokensResource> CreateAccessTokens(User user, string deviceId,
        //    string deviceName)
        //{
        //    var accessToken = _jwtHandler.CreateAccessToken(user.Id, user.Email, user.Role);
        //    var refreshToken = _jwtHandler.CreateRefreshToken(user.Id);

        //    return new AuthorizationTokensResource { AccessToken = accessToken, RefreshToken = refreshToken };
        //}
    }
}
