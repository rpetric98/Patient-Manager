using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Utilities
{
    public class JWTProvider
    {
        public static string CreateToken(string secureKey, int expiration, string subject = null)
        {
            if (string.IsNullOrEmpty(secureKey))
                throw new ArgumentNullException(nameof(secureKey));
        

            // Get secret key bytes
            var tokenKey = Encoding.UTF8.GetBytes(secureKey);
            var claims = new List<Claim>();
            // Create a token descriptor (represents a token, kind of a "template" for token)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiration),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            if (!string.IsNullOrEmpty(subject))
            {
                tokenDescriptor.Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, subject),
                new Claim(JwtRegisteredClaimNames.Sub, subject),
                });
            }
            else
            {
                Console.WriteLine("Warning: subject is null. Claims will not be added.");
            }

            var role = "Admin"; // for example...
            tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));

            // Create token using that descriptor, serialize it and return it
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            

            return tokenHandler.WriteToken(token);
        }
    }
}
