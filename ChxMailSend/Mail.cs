using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace ChxMailSend
{
    public class Mail
    {
        private string _MailFrom;
        private string _MailTo;

        public string MailFrom 
        { 
            get { return this._MailFrom; }
            set { this._MailFrom = new MailAddress(this._MailFrom).Address; }
        }

        public string MailTo
        {
            get { return this._MailTo; }
            set { this._MailTo = new MailAddress(this._MailTo).Address; }
        }


        public string MailSubject;
        public string MailBody;


        public Mail()
        {
        }

        public Mail(string MailFrom, string MailTo, string Subject, string Body)
        {
            this.MailFrom    = MailFrom;
            this.MailTo      = MailTo;
            this.MailSubject = Subject;
            this.MailBody    = Body;
        }

        public void Send()
        {
            MailMessage mail = new MailMessage(this.MailFrom, this.MailTo)
            {
                Subject = this.MailSubject,
                Body    = this.MailBody
            };
   
            SmtpClient client = new SmtpClient()
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host           = this.GetConfigValue("SmtpClientHost", "smtp.google.com"),
                Port           = int.Parse(this.GetConfigValue("SmtpClientPort", "25")),
                Credentials    = this.GetCredentialsFromConfig(),
                EnableSsl      = (this.GetConfigValue("SmtpClientEnableSsl","true") == "true")
            };

            using (client)
            {
                client.Send(mail);
            }
        }

        public string GetConfigValue(string name)
        {
            return this.GetConfigValue(name, "");
        }

        public string GetConfigValue(string name, string defaultValue)
        {
            string value = ConfigurationSettings.AppSettings.Get(name);

            if (value == null) {
                 value = defaultValue;
            }

            return value;
        }

        public NetworkCredential GetCredentialsFromConfig()
        {
            string[] value = this.GetConfigValue("SmtpClientCredentials").Split(';');
            var addr = new MailAddress(value[0]);

            return new NetworkCredential(addr.Address, value[1]);
        }
    }
}
