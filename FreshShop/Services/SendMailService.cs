using System;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

public class MailSettings
{
    public string Mail { get; set; }
    public string DisplayName { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
}

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
    Task SendSmsAsync(string number, string message);
}

public class SendMailService : IEmailSender
{
    private readonly MailSettings mailSettings;
    private readonly ILogger<SendMailService> logger;

    // mailSetting được Inject qua dịch vụ hệ thống
    // Có inject Logger để xuất log
    public SendMailService(IOptions<MailSettings> mailSettings, ILogger<SendMailService> logger)
    {
        this.mailSettings = mailSettings.Value ?? throw new ArgumentNullException(nameof(mailSettings));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage();
        message.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
        message.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = htmlMessage };
        message.Body = builder.ToMessageBody();

        using var smtp = new MailKit.Net.Smtp.SmtpClient();

        try
        {
            logger.LogInformation($"Đang kết nối tới SMTP server {mailSettings.Host}:{mailSettings.Port}...");
            await smtp.ConnectAsync(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);

            logger.LogInformation($"Đang xác thực với tài khoản email {mailSettings.Mail}...");
            await smtp.AuthenticateAsync(mailSettings.Mail, mailSettings.Password);

            logger.LogInformation($"Đang gửi email tới {email}...");
            await smtp.SendAsync(message);
            logger.LogInformation($"Email đã được gửi thành công tới {email}");
        }
        catch (Exception ex)
        {
            logger.LogError($"Lỗi khi gửi email tới {email}: {ex.Message}");
            throw; // Rethrow exception để có thể xử lý ở nơi gọi phương thức này
        }
        finally
        {
            await smtp.DisconnectAsync(true);
            logger.LogInformation("Đã ngắt kết nối với server SMTP");
        }
    }

    public Task SendSmsAsync(string number, string message)
    {
        // Cài đặt dịch vụ gửi SMS tại đây
        try
        {
            System.IO.Directory.CreateDirectory("smssave");
            var smsFileName = string.Format(@"smssave/{0}-{1}.txt", number, Guid.NewGuid());
            System.IO.File.WriteAllTextAsync(smsFileName, message);
            logger.LogInformation($"SMS đã được lưu vào {smsFileName}");
        }
        catch (Exception ex)
        {
            logger.LogError($"Lỗi khi lưu SMS: {ex.Message}");
            throw; // Rethrow exception nếu có lỗi
        }
        
        return Task.CompletedTask;
    }
}
