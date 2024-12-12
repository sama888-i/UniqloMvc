namespace Uniqlo2.Services.Abstracts
{
    public interface IEmailService
    {
        Task SendEmailconfirmation(string reciever,string name,string token);
    }
}
