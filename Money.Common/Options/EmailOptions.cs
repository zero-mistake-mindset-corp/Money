namespace Money.Common.Options;

public class EmailOptions
{
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string SenderEmail { get; set; }
    public string Password { get; set; }
}
