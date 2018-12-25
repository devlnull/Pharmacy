using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Pharmacy.Services.Providers
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await Task.Factory.StartNew(async () =>
            {
                await Task.Delay(3000, CancellationToken.None);
                Console.WriteLine($"send email to => {email} \nWith Subject => {subject} \n Content => {htmlMessage}");
            });
        }
    }
}
