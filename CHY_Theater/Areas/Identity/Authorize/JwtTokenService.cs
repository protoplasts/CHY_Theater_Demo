using CHY_Theater_Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CHY_Theater.Areas.Identity.Authorize
{
	public class JwtTokenService
	{//IConfiguration for accessing app settings
		private readonly IConfiguration _configuration;
		private readonly UserManager<ApplicationUser> _userManager;

		public JwtTokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
		{
			_configuration = configuration;
			_userManager = userManager;
		}

		public async Task<string> GenerateJwtToken(ApplicationUser user)
		{
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(ClaimTypes.Name, user.Name)
			};

			// Get user roles and add them to claims
			var roles = await _userManager.GetRolesAsync(user);
			claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
			//Creating the Security Key
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			//Setting Token Expiration
			var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

			//Creating and Returning the Token
			var token = new JwtSecurityToken(
				_configuration["JwtIssuer"],
				_configuration["JwtAudience"],
				claims,
				expires: expires,
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
