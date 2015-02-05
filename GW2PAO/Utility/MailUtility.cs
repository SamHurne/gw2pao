using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Utility
{
    public class MailUtility
    {
        public const string MAIL_USER = "[REMOVED]";
        public const string MAIL_PASS = "[REMOVED]";

        /// <summary>
        /// Sends an e-mail
        /// </summary>
        public static void Email(string to,
                                 string body,
                                 string subject,
                                 string fromAddress,
                                 string fromDisplay,
                                 string credentialUser,
                                 string credentialPassword,
                                 params MailAttachment[] attachments)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.Body = body;
                mail.IsBodyHtml = false;
                mail.To.Add(new MailAddress(to));
                mail.From = new MailAddress(fromAddress, fromDisplay, Encoding.UTF8);
                mail.Subject = subject;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.Priority = MailPriority.Normal;
                foreach (MailAttachment ma in attachments)
                {
                    mail.Attachments.Add(ma.File);
                }
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new System.Net.NetworkCredential(credentialUser, credentialPassword);
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch
            {
                // Swallow
            }
        }
    }


    public class MailAttachment
    {
        #region Fields
        private MemoryStream stream;
        private string filename;
        private string mediaType;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the data stream for this attachment
        /// </summary>
        public Stream Data { get { return stream; } }
        /// <summary>
        /// Gets the original filename for this attachment
        /// </summary>
        public string Filename { get { return filename; } }
        /// <summary>
        /// Gets the attachment type: Bytes or String
        /// </summary>
        public string MediaType { get { return mediaType; } }
        /// <summary>
        /// Gets the file for this attachment (as a new attachment)
        /// </summary>
        public Attachment File { get { return new Attachment(Data, Filename, MediaType); } }
        #endregion

        #region Constructors
        /// <summary>
        /// Construct a mail attachment form a byte array
        /// </summary>
        /// <param name="data">Bytes to attach as a file</param>
        /// <param name="filename">Logical filename for attachment</param>
        public MailAttachment(byte[] data, string filename)
        {
            this.stream = new MemoryStream(data);
            this.filename = filename;
            this.mediaType = MediaTypeNames.Application.Octet;
        }

        /// <summary>
        /// Construct a mail attachment from a string
        /// </summary>
        /// <param name="data">String to attach as a file</param>
        /// <param name="filename">Logical filename for attachment</param>
        public MailAttachment(string data, string filename)
        {
            this.stream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(data));
            this.filename = filename;
            this.mediaType = MediaTypeNames.Text.Html;
        }
        #endregion
    }
}
