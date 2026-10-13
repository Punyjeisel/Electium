using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly IPartyLeaderService _partyLeaderService;

        public AuthenticationService(
            IUserService userService,
            IPartyLeaderService partyLeaderService)
        {
            _userService = userService;
            _partyLeaderService = partyLeaderService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userService.ValidateCredentialAsync(
                request.UserName, request.Password);

            if (user == null)
                return new LoginResponseDto { Result = LoginResult.InvalidCredentials };

            if (!user.IsActive)
                return new LoginResponseDto { Result = LoginResult.UserInactive };

            if (user.Role == UserRole.PoliticalLeader)
            {
                var leaders = await _partyLeaderService.GetAllWithIncludesAsync();
                var linked = leaders.Any(l => l.UserId == user.Id && l.IsActive);

                if (!linked)
                    return new LoginResponseDto { Result = LoginResult.LeaderWithoutParty };
            }

            return new LoginResponseDto
            {
                Result = LoginResult.Success,
                UserId = user.Id,
                UserName = user.UserName,
                Role = user.Role
            };
        }
    }
}
