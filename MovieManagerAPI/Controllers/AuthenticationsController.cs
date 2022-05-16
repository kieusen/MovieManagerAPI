using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MovieManagerAPI.Data;
using MovieManagerAPI.Data.ViewModels.Authentication;
using MovieManagerAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MovieManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationsController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterVM userReg)
        {
            if (!ModelState.IsValid)            
                return BadRequest("Please, provide all required fields");            

            var userExists = await _userManager.FindByEmailAsync(userReg.Email);

            if (userExists != null)
                return BadRequest($"User {userReg.Email} already exists");

            ApplicationUser newUser = new ApplicationUser()
            {
                Email = userReg.Email,
                UserName = userReg.UserName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            // Create New User
            var result = await _userManager.CreateAsync(newUser, userReg.Password);

            if (!result.Succeeded)
                return BadRequest("User could not be created!");

            // Add Role
            switch (userReg.Role)
            {
                case "Admin":
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Admin);
                    break;
                default:
                    await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                    break;
            }

            return Created(nameof(Register), $"User {userReg.Email} created");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginVM userLog)
        {
            if (!ModelState.IsValid)
                return BadRequest("Please, provide all required fields");
            var user = await _userManager.FindByEmailAsync(userLog.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, userLog.Password))
            {
                var tokenValue = await GenerateJwtTokenAsync(user);
                return Ok(tokenValue);
            }

            return Unauthorized();
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            if (tokenModel == null)
                return BadRequest("Invalid client request");

            // Token Parameters
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"])),

                ValidateIssuer = true,
                ValidIssuer = _configuration["JWT:Issuer"],

                ValidateAudience = true,
                ValidAudience = _configuration["JWT:Audience"],
                ValidateLifetime = false 
            };

            try
            {
                var tokenHandle = new JwtSecurityTokenHandler();
                var tokenInVerification = tokenHandle.ValidateToken(tokenModel.AccessToken, tokenValidationParameters, out SecurityToken securityToken);

                // Check format, alg            
                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return BadRequest("Invalid token");

                // Check accessToken expire?
                //var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                //var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                //if (expireDate < DateTime.UtcNow)
                //    return BadRequest("Access token has not yet expired");

                // Check refreshtoke exist in DB
                var storedToken = _context.RefreshTokens.FirstOrDefault(x => x.Token == tokenModel.RefreshToken);
                if (storedToken == null)
                    return BadRequest("Refresh token does not exist");

                // Check refreshtoke is revoked?
                if (storedToken.IsRevoked)
                    return BadRequest("Refresh token has been revoked");

                // Check accesstoken id == JwtId in RefreshToken
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                    return BadRequest("Token does not match");

                // Update token is revoked
                storedToken.IsRevoked = true;
                _context.RefreshTokens.Update(storedToken);
                await _context.SaveChangesAsync();

                // create new token
                var user = await _userManager.FindByIdAsync(storedToken.UserId);
                var token = await GenerateJwtTokenAsync(user);

                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private DateTime ConvertUnixTimeToDateTime(long utcExprireDate)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(utcExprireDate).ToUniversalTime();
            return dateTime;
        }

        private async Task<AuthResultVM> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add User Roles
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(10),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToke = new RefreshToken()
            {
                JwtId = token.Id,
                IsRevoked = false,
                UserId = user.Id,
                DateAdded = DateTime.UtcNow,
                DateExprire = DateTime.UtcNow.AddMonths(6),
                Token = GenerateRefreshToken()
            };

            await _context.RefreshTokens.AddAsync(refreshToke);
            await _context.SaveChangesAsync();

            return new AuthResultVM()
            {
                Token = jwtToken,
                RefreshToken = refreshToke.Token,
                ExpriresAt = token.ValidTo
            };
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
