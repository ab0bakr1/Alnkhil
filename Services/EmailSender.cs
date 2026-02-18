using Microsoft.AspNetCore.Identity.UI.Services;

namespace alnakhil.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // لا نفعل شيء (للتطوير فقط)
            return Task.CompletedTask;
        }
    }
}
