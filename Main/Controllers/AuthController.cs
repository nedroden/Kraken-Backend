using Microsoft.AspNetCore.Mvc;
using System;

using Kraken.Api.Main.Services;

namespace Kraken.Api.Main.Controllers
{
    /// <summary>
    /// Represents a return value containing a token.
    /// </summary>
    public class Token
    {
        /// <value>The value of the token.</value>
        public string TokenValue { get; set; }
    }

    /// <summary>
    /// Represents the format used for logging in.
    /// </summary>
    public class LoginRequest
    {
        /// <value>An email address.</value>
        public string Email { get; set; }

        /// <value>A password.</value>
        public string Password { get; set; }
    }

    /// <summary>
    /// Represents the format used for registration.
    /// </summary>
    public class RegistrationRequest
    {
        /// <value>A username.</value>
        public string Username { get; set; }

        /// <value>An email.</value>
        public string Email { get; set; }

        /// <value>A password.</value>
        public string Password { get; set; }

        /// <value>The same password as specified before.</value>
        public string PasswordConfirmation { get; set; }
    }

    /// <summary>
    /// The auth controller.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private AuthService _authService;

        /// <summary>
        /// Creates a new auth controller.
        /// </summary>
        /// <param name="authService">The auth service.</param>
        public AuthController(AuthService authService) => _authService = authService;

        /// <summary>
        /// Handles login requests.
        /// </summary>
        /// <param name="request">The user's email address and password.</param>
        /// <returns>Status 200 with token information if successful, otherwise 401.</returns>
        [HttpPost("login")]
        public ActionResult<Token> Login(LoginRequest request)
        {
            ActionResult<Token> response;

            try
            {
                string token = _authService.Login(request.Email, request.Password);

                response = Ok(new Token { TokenValue = token });
            }
            catch (Exception exception)
            {
                response = Unauthorized(new { error = exception.Message });
            }

            return response;
        }

        /// <summary>
        /// Handles registration requests. Also logs the user in automatically.
        /// </summary>
        /// <param name="request">Account information, such as username and password.</param>
        /// <returns>
        /// Status 200 along with a login token if successful, 400 if email/username is
        /// in use or upon password mismatch.
        /// </returns>
        [HttpPost("register")]
        public ActionResult<Token> Register(RegistrationRequest request)
        {
            ActionResult<Token> response;

            if (request.Password != request.PasswordConfirmation)
            {
                return BadRequest("Password mismatch.");
            }

            try
            {
                string token = _authService.Register(request.Email, request.Username, request.Password);

                response = Ok(new Token { TokenValue = token });
            }
            catch (ArgumentException exception)
            {
                response = BadRequest(exception.Message);
            }

            return response;
        }
    }
}