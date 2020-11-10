using Microsoft.AspNetCore.Identity;
using System;

using Kraken.Api.Main.Models;

namespace Kraken.Api.Main.Services
{
    /// <summary>
    /// Facilitates basic authentication actions such as logging in and registering.
    /// </summary>
    public class AuthService
    {
        private readonly UserService _userService;
        private readonly TokenService _tokenService;

        /// <summary>
        /// Instantiates the authenticastion service.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="tokenService">The token service.</param>
        public AuthService(UserService userService, TokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Logs the user in by returning an authentication token if the user exists.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown if the credentials are incorrect.</exception>
        /// <returns>An authentication token, generated by the token service.</returns>
        public string Login(string email, string password)
        {
            User user = _userService.GetByEmail(email);

            if (user == null || !IsCorrectPassword(user, password))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            return _tokenService.GenerateToken(user);
        }

        /// <summary>
        /// Registers a user under the condition that the email and username are unique. Upon success,
        /// an authentication token is automatically generated and returned (thus negating the need to
        /// make another request to the login endpoint).
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="username">The user's name.</param>
        /// <param name="password">The user's password.</param>
        /// <exception cref="ArgumentException">Returned if the email addres or username is already in use.</exception>
        /// <returns>An authentication token.</returns>
        public string Register(string email, string username, string password)
        {
            if (UserExists(email, username))
            {
                throw new ArgumentException("User already exists.");
            }

            var user = new User
            {
                Email = email,
                Username = username,
                Password = GetHashedPassword(password)
            };

            _userService.Create(user);

            return _tokenService.GenerateToken(user);
        }

        /// <summary>
        /// Checks whether or not the user's password is incorrect.
        /// </summary>
        /// <param name="user">User information.</param>
        /// <param name="given">The password to check (must be unhashed, obviously).</param>
        /// <returns>Whether or not the password is correct.</returns>
        public bool IsCorrectPassword(User user, string given)
        {
            var hasher = new PasswordHasher<User>();
            PasswordVerificationResult result = hasher.VerifyHashedPassword(user, user.Password, given);

            return result == PasswordVerificationResult.Success;
        }

        /// <summary>
        /// Hashes a pasword. Note: to validate the password, use IsCorrectPassword instead of
        /// invoking this method again. Invoking this method again to verify the password will
        /// NOT work.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hashed password.</returns>
        public static string GetHashedPassword(string password)
        {
            return new PasswordHasher<User>().HashPassword(null, password);
        }

        /// <summary>
        /// Checks if there is a user with the given email address or username.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <param name="username">The username.</param>
        /// <returns>Whether or not a user exists by the given email address or username.</returns>
        public bool UserExists(string email, string username)
        {
            return _userService.GetByEmail(email) != null || _userService.GetByUsername(username) != null;
        }
    }
}