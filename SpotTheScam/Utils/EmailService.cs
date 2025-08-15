using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SpotTheScam.Utils
{
	public static class EmailService
	{
		public static void Send(string toEmail, string subject, string htmlBody)
		{
			string host = ConfigurationManager.AppSettings["SmtpHost"];
			int port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
			string user = ConfigurationManager.AppSettings["SmtpUser"];
			string pass = ConfigurationManager.AppSettings["SmtpPass"];
			string from = ConfigurationManager.AppSettings["SmtpFrom"] ?? user;
			string fromName = ConfigurationManager.AppSettings["SmtpFromName"] ?? "Spot The Scam";

			using (var msg = new MailMessage())
			{
				msg.From = new MailAddress(from, fromName);
				msg.To.Add(toEmail);
				msg.Subject = subject;
				msg.Body = htmlBody;
				msg.IsBodyHtml = true;

				using (var smtp = new SmtpClient(host, port))
				{
					smtp.EnableSsl = true;
					smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
					smtp.UseDefaultCredentials = false;
					smtp.Credentials = new NetworkCredential(user, pass);
					smtp.Send(msg);
				}
			}
		}

		public static async Task SendAsync(string toEmail, string subject, string htmlBody)
		{
			string host = ConfigurationManager.AppSettings["SmtpHost"];
			int port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
			string user = ConfigurationManager.AppSettings["SmtpUser"];
			string pass = ConfigurationManager.AppSettings["SmtpPass"];
			string from = ConfigurationManager.AppSettings["SmtpFrom"] ?? user;
			string fromName = ConfigurationManager.AppSettings["SmtpFromName"] ?? "Spot The Scam";

			using (var msg = new MailMessage())
			{
				msg.From = new MailAddress(from, fromName);
				msg.To.Add(toEmail);
				msg.Subject = subject;
				msg.Body = htmlBody;
				msg.IsBodyHtml = true;

				using (var smtp = new SmtpClient(host, port))
				{
					smtp.EnableSsl = true;
					smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
					smtp.UseDefaultCredentials = false;
					smtp.Credentials = new NetworkCredential(user, pass);
					await smtp.SendMailAsync(msg);
				}
			}
		}
	}
}


