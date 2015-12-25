/**
* Issa Haddar
* 201307043
* Cmps 258
* section 1
*/
using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;
using System.IO;

namespace ExcelEmail
{
    /**
     * This Class is responsible for sending the emails
     */ 
    public class SMTPEmail
    {
        /*---Instance Fields----*/
        private string myEmail;
        private string myName;
        private string myPassword;
        private StringBuilder report = new StringBuilder();
        /*---------------------*/
        

        /**
         * Return the report of the email that has been/notbeen sent
         */
        public String getReport() {
            return report.ToString();
        }

        /**
        * Set the password to the passed one
        */
        public bool setPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length <= 8)
                return false;
            else
                myPassword = password;
            return true;
        }

        /**
         * Set the email to the passed one
         */
        public bool setEmail(string email)
        {
            //evaluate whether the passed email address is syntactically valid or not.
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
         @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
         @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(email))
            {
                myEmail = email;
                return true;
            }
            else
                return false;
        }

        /**
         * Set the name to the passed one
         */
        public bool setName(string name)
        {
            if (!Regex.IsMatch(name, "^[a-zA-Z]{4,10} [a-zA-Z]{3,12}$"))
                return false;
            else
                myName = name;
            return true;
        }

        /**
         * Send an email to the recipient
         */
        public void SendEmail(string recipientName, string recipientEmail, string subject, string Body)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail))
                MessageBox.Show(recipientEmail + " has an invalid Email", "Caption", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //setting the report file Destination My Documents
            try
            {
                SmtpClient mySmtpClient = new SmtpClient("smtp.gmail.com", 587);        //gmail 
                mySmtpClient.EnableSsl = true; //Gmail works on Server Secured Layer
                //set smtp clien with basic authentication
                mySmtpClient.UseDefaultCredentials = false;
                NetworkCredential basicCredential = new NetworkCredential(myEmail, myPassword);
                mySmtpClient.Credentials = basicCredential;

                //add from, to mail addresses
                MailAddress from = new MailAddress(myEmail, myName);
                MailAddress recipient = new MailAddress(recipientEmail, recipientName);
                MailMessage email = new MailMessage(from, recipient);

                //add reply to
                MailAddress replyTo = new MailAddress(myEmail);
                email.ReplyToList.Add(replyTo);


                //set subject and encoding
                email.Subject = "Attendance Record";
                email.SubjectEncoding = System.Text.Encoding.UTF8;

                //set body and body encoding
                email.Body = Body;
                email.BodyEncoding = System.Text.Encoding.UTF8;

                //text or html
                email.IsBodyHtml = true;
                mySmtpClient.Send(email);
                report.AppendFormat("An email was succesfuly sent to {0}, Email: {1}\n\n", recipientName, recipientEmail);
            }
            catch (SmtpException)
            {
                report.AppendFormat("Email sending fialed for the recipient: {0}, Email: {1}\n\n", recipientName, recipientEmail);
            }

        }
    }
}
