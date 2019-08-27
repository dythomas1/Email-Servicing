using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using Cts.TechCheck.Data;
using Cts.TechCheck.Dto;
using System.Web.Routing;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using log4net;

#pragma warning disable 1591 // XML comment suppression

namespace Cts.TechCheck.Service.EmailService
{
    /// <summary>
    /// STAN: Summarize this class
    /// <author></author>
    /// <date></date>
    /// <copyright>
    /// Copyright © <year></year>Computer Technology Solutions, Inc. ALL RIGHTS RESERVED
    /// </copyright>
    /// <productName>CTS Tech Check</productName>
    /// </summary>
    public class EmailService : BaseService
    {
        public EmailService(Context ctx) : base(ctx.DbContext) { }

        private static readonly ILog Log = LogManager.GetLogger(typeof(EmailService));
        ///<summary>Takes in DTO object and sends the information to the recipients </summary>
        /// <author>Drew Thomas</author>
        /// <date>6/29/2016</date>
        /// <exceptions></exceptions>
        /// <return></return>
        /// <param name="email"></param>
        /// <history>
        /// Date   Author    Change Reason     Change Description
        /// ----------------------------------------------------------------
        /// 
        /// ----------------------------------------------------------------
        /// </history>
        public bool EmailReviewer(EmailDto email)
        {
            Log.Debug("The email Reviewer funciton is called");
            bool sent = false;

            //Setting the email properties with a default email body
            var mailMessage = new MailMessage();
            mailMessage.Subject = email.CandidateFullName + ": Request for Review";
            mailMessage.Body = email.EmailMessage + " This is the link you can use to access the candidate's results: </br> </br> </br>" + email.Link;
            mailMessage.IsBodyHtml = true;
            mailMessage.To.Add(email.ReviewerAddress1);

            if (!string.IsNullOrEmpty(email.SelectedReviewer2))
                mailMessage.To.Add(email.ReviewerAddress2);

            //CC check to make sure an actual email
            if (!string.IsNullOrEmpty(email.ToEmailAddressCC))
            {
                var valid = new EmailAddressAttribute();
                bool actualEmail = false;
                actualEmail = valid.IsValid(email.ToEmailAddressCC);
                if (actualEmail == true)
                    mailMessage.To.Add(email.ToEmailAddressCC);
                else
                    return false;
            }
            
            //Sending the actual email
            using (SmtpClient mailClient = new SmtpClient())
            {
                mailClient.Send(mailMessage);
                Log.Debug("Your email was sent to the reviewer");
                sent = true;
            }

            return sent;
        }


        



        ///<summary>Takes in CandidateInvitationID adn sends an email to the recruiter that has been assigned to this candidate </summary>
        /// <author>Drew Thomas</author>
        /// <date>6/18/2016</date>
        /// <exceptions></exceptions>
        /// <return></return>
        /// <param >candidateInvitationKey, currentUrl, controllersContext</param>
        /// <history>
        /// Date   Author    Change Reason     Change Description
        /// ----------------------------------------------------------------
        /// 
        /// ----------------------------------------------------------------
        /// </history>
        public void EmailRecruiter(int candidateInvitiationKey, string currentUrl, RequestContext controllersContext)
        {
            
            //query that returns a emaildto form the candidateInvitiationsKey
            var email = (
                from ci in Db.CandidateInvitations
                join cre in Db.CandidateResultEvaluations on ci.CandidateInvitationKey equals cre.CandidateInvitationKey
                join c in Db.Candidates on ci.CandidateKey equals c.CandidateKey
                join u in Db.Users on c.UserKey equals u.UserKey
                where ci.CandidateInvitationKey == candidateInvitiationKey
                select new EmailDto
                {
                    CandidateID = ci.CandidateKey,
                    FirstName= c.FirstName,
                    LastName=c.LastName,
                    RecruiterID = c.UserKey.Value,
                    CandidateResultKey = cre.CandidateResultKey,
                    ToEmailAddress = u.EmailAddress
                }).FirstOrDefault();

           
            email.Link = GenerateRecruiterResultsLink(currentUrl, email.CandidateResultKey, controllersContext);

            //setting the email properties
            var mailMessage = new MailMessage();
            mailMessage.Subject = email.CandidateFullName1 + ": Test Submission Confirmation";
            mailMessage.Body = email.CandidateFullName1 + " has completed his/her evaluation test and this is the link to his/her results if you would like to view them: </br> </br> </br>"
            + email.Link;
            mailMessage.IsBodyHtml = true;

            mailMessage.To.Add(email.ToEmailAddress);

            //sending the email
            using (SmtpClient mailClient = new SmtpClient())
            {
                mailClient.Send(mailMessage);
                Log.Debug("The email was successfuly sent to the Recruiter, letting them know that the candidate has finished their test");
            }
        }
        ///<summary>Takes the stirng of the host, an email dto object and request for controller context
        /// and outputs a link that will be used to send to the reviewer so that they can follow the link to the candidate results</summary>
        /// <author>Drew Thomas</author>
        /// <date>7/26/2016</date>
        /// <exceptions></exceptions>
        /// <return>string link</return>
        /// <param >currentUrl,emaildto, controllersContext</param>
        /// <history>
        /// Date   Author    Change Reason     Change Description
        /// ----------------------------------------------------------------
        /// 
        /// ----------------------------------------------------------------
        /// </history>
        public string GenerateReveiwerResultsLink(string currentUrl, EmailDto email, RequestContext controllersContext)
        {
           
            UrlHelper url = new UrlHelper(controllersContext);
            string link;

            //Use within LocalHost
            if (currentUrl.Contains("localhost"))
            {
                link = url.Action("CandidateResults", "Reviewer", new RouteValueDictionary(new { key = email.CandidateResultKey }), "http", "localhost");
            }

            //Use on VM
            else
            {
                link = url.Action("CandidateResults", "Reviewer", new RouteValueDictionary(new { key= email.CandidateResultKey }), "http", "vmtechcheck01.askcts.com");
            }

            return link;
        }
        ///<summary>Takes the stirng of the host, a candidate Result Key and request for controller context
        /// and outputs a link that will be used to send to the recruiter so that they can follow the link to the candidate results</summary>
        /// <author>Drew Thomas</author>
        /// <date>7/26/2016</date>
        /// <exceptions></exceptions>
        /// <return>string link</return>
        /// <param >currentUrl,emaildto, controllersContext</param>
        /// <history>
        /// Date   Author    Change Reason     Change Description
        /// ----------------------------------------------------------------
        /// 
        /// ----------------------------------------------------------------
        /// </history>
        public string GenerateRecruiterResultsLink(string currentUrl, int candidateResultKey, RequestContext controllersContext)
        {
            // Creates the base link for the candidate to access
            UrlHelper url = new UrlHelper(controllersContext);
            string link;
            //Use within LocalHost
            if (currentUrl.Contains("localhost"))
            {
                link = url.Action("CandidateResults", "Recruiter", new RouteValueDictionary(new { key = candidateResultKey }), "http", "localhost");
            }

            //Use on VM
            else
            {
                link = url.Action("CandidateResults", "Recruiter", new RouteValueDictionary(new { key = candidateResultKey }), "http", "vmtechcheck01.askcts.com");
            }
            return link;
        }







    }
}
