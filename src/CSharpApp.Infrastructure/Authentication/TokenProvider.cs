using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Infrastructure.Authentication;


public sealed class TokenProvider : ITokenProvider
{
    public string? AccessToken { get; private set; }
    public void SetToken(string token)
    {
        AccessToken = token;
    }
}
