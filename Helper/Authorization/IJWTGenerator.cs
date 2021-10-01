using System.Collections.Generic;
using System.Security.Claims;

namespace idolapi.Helper.Authorization
{
    public interface IJWTGenerator
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
    }
}