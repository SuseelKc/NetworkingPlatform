using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetworkingPlatform.Configuration;
using NetworkingPlatform.Interface;
using NetworkingPlatform.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NetworkingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly string _secretKey;
        private readonly IEmailService _emailService;
      

        public AuthController(UserManager<Users> userManager, SignInManager<Users> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _secretKey = GenerateSecretKey(32); // Generate a secret key with 256 bits (32 bytes)
            _emailService = emailService;
          
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(Users model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(); // User not found
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.PasswordHash, false);
            if (!result.Succeeded)
            {
                return Unauthorized(); // Invalid password
            }

            var token = GenerateJwtToken(user);
            // Include additional user data in the response object
            var userData = new
            {
                id = user.Id,
                UserName = user.UserName,
                email = user.Email,
                role = user.UserRole
                // Add other user properties as needed
            };

            return Ok(new { Token = token, User = userData });
        }

        private string GenerateJwtToken(Users user)
        {
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Set token expiration time (e.g., 1 hour from now)
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateSecretKey(int keySizeInBytes)
        {
            // Generate a secure random key of the specified size
            var rng = new RNGCryptoServiceProvider();
            var keyBytes = new byte[keySizeInBytes];
            rng.GetBytes(keyBytes);
            return Convert.ToBase64String(keyBytes);
        }

        //forgotpassword
        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // User not found, return success to prevent email enumeration
                return Ok();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"http://localhost:5173/forget-password?type=new&email={email}&token={token}";

            // Customize email subject and body as needed
            var subject = "Password Reset";
            var body = $"Please reset your password by clicking <a href='{callbackUrl}'>here</a>.";

            await _emailService.SendAsync(email, subject, body);

            return Ok();
        }

        //resetPassword
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // User not found
                return NotFound();
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                // Password reset failed
                return BadRequest(result.Errors);
            }
        }


    }
}
