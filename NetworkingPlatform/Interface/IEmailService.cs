using System.Threading.Tasks;

namespace NetworkingPlatform.Interface
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
    }
}
