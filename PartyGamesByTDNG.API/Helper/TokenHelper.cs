using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PartyGamesByTDNG.API.Helper;

public class TokenHelper
{
    public static bool IsTokenValid(string Token)
    {

        return Authenticate(Token);
    }


    private static bool Authenticate(string token)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Program.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        List<Exception> validationFailures = null;
        SecurityToken validatedToken;
        var validator = new JwtSecurityTokenHandler();

        // These need to match the values used to generate the token
        TokenValidationParameters validationParameters = new TokenValidationParameters();
        validationParameters.ValidateIssuer = true;
        validationParameters.ValidateAudience = true;
        validationParameters.ValidateLifetime = true;
        validationParameters.ValidateIssuerSigningKey = true;

        validationParameters.ValidIssuer = Program.Issuer;
        validationParameters.ValidAudience = Program.Issuer;
        validationParameters.IssuerSigningKey = key;


        validationParameters.LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
            {
                //var a = options.ClaimsIssuer;
                // string tokenstring = securityToken.UnsafeToString();
                // var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenstring);
                // var claim = token.Claims.First(c => c.Type == "sample").Value;


                // bool valid = Validators.ValidateLifetime()
                // --> my custom check <--

                var convnow = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);



                if (convnow >= (expires ?? DateTime.Now))
                {
                    return false;
                }
                else
                {
                    var timespan = (expires ?? DateTime.Now) - convnow;

                    if (timespan.TotalSeconds <= 10)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }


            };


        if (validator.CanReadToken(token))
        {
            ClaimsPrincipal principal;
            try
            {
                // This line throws if invalid
                principal = validator.ValidateToken(token, validationParameters, out validatedToken);

                // // If we got here then the token is valid
                // if (principal.HasClaim(c => c.Type == ClaimTypes.Email))
                // {
                //     return principal.Claims.Where(c => c.Type == ClaimTypes.Email).First().Value;
                // }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.InnerException);
            }
        }

        return false;
    }
}
