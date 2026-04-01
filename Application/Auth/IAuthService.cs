using System.Threading.Tasks;

namespace Application.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);

        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
