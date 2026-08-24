using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> Login();
        Task<Profile> GetProfile();
    }
}

