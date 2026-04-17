using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Application.Common;
using Products.Application.Common.DTOs;
using Products.Application.Services;

namespace Products.API.Controllers
{


    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>Login ; returns an access token and a refresh token.</summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthTokenResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuthErrorResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(new LoginDto(request.Username, request.Password));
            return Ok(ApiResponse<AuthTokenResult>.Success(result, "Login successful"));
        }

        /// <summary>Exchange a valid refresh token for a new access token + rotated refresh token.</summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthTokenResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuthErrorResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            var result = await _authService.RefreshAsync(new RefreshTokenDto(request.RefreshToken));
            return Ok(ApiResponse<AuthTokenResult>.Success(result, "Token refreshed"));
        }

        /// <summary>Revoke a refresh token (logout).</summary>
        [HttpPost("revoke")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<RevokeResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuthErrorResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Revoke([FromBody] RevokeRequest request)
        {
            var result = await _authService.RevokeAsync(new RevokeTokenDto(request.RefreshToken));
            return Ok(ApiResponse<RevokeResult>.Success(result));
        }
    }
}