using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Goobeer.Tools.Email
{
    public class EMailHelper
    {
        public void Send(MailAddress from, MailMessage msg, SmtpClientConfig scc)
        {
            using (SmtpClient client = new SmtpClient())
            {
                client.Host = scc.SmtpHost;
                client.Credentials = new NetworkCredential(scc.FromUserName, scc.FromUserPwd);
                msg.From = from;
                client.Send(msg);
            }
        }

        public async Task SendAsync(MailAddress from, MailMessage msg, SmtpClientConfig scc)
        {
            using (SmtpClient client = new SmtpClient())
            {
                client.Host = scc.SmtpHost;
                client.Credentials = new NetworkCredential(scc.FromUserName, scc.FromUserPwd);
                msg.From = from;
                await client.SendMailAsync(msg);
            }
        }

        private void SendTest()
        {
            var from = new MailAddress("aa@aa.aa");
            var msg = new MailMessage(from, new MailAddress("bb@bb.bb"));

            msg.Subject = "email title";
            msg.Body = "email body";
            msg.BodyEncoding = System.Text.Encoding.UTF8;
            //send to many
            for (int i = 0; i < 3; i++)
            {
                msg.To.Add(new MailAddress(string.Format("{0}@{0}.{0}", i)));
            }

            SmtpClientConfig scc = new SmtpClientConfig() { SmtpHost = "smtp.aa.aa", FromUserName = "send account name", FromUserPwd = "send account pwd" };
            Send(from, msg, scc);
        }

        private void SendAsyncTest()
        {
            var from = new MailAddress("aa@aa.aa");
            var msg = new MailMessage(from, new MailAddress("bb@bb.bb"));

            msg.Subject = "email title";
            msg.Body = "email body";
            msg.BodyEncoding = System.Text.Encoding.UTF8;
            //send to many
            for (int i = 0; i < 3; i++)
            {
                msg.To.Add(new MailAddress(string.Format("{0}@{0}.{0}", i)));
            }

            SmtpClientConfig scc = new SmtpClientConfig() { SmtpHost = "smtp.aa.aa", FromUserName = "send account name", FromUserPwd = "send account pwd" };

            var task = SendAsync(from, msg, scc);
            Task.Run(() => { return task; });
        }
    }

    public class SmtpClientConfig
    {
        public string SmtpHost { get; set; }
        public string FromUserName { get; set; }
        public string FromUserPwd { get; set; }
    }
}