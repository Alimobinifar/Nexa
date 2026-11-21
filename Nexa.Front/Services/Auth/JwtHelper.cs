using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Nexa.Front.Services.Auth
{
    public class JwtHelper
    {
        public static ClaimsPrincipal ToPrincipal(string accessToken, string scheme)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

            var claims = jwt.Claims
                .Where(c => c.Type is ClaimTypes.NameIdentifier or ClaimTypes.Name
                            or ClaimTypes.Role or "tenant_id")
                .ToList();

            return new ClaimsPrincipal(new ClaimsIdentity(claims, scheme));
        }
    }
}
