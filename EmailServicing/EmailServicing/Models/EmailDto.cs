using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmailServicing.Models
{
    public class EmailDto
    {

        [Display(Name = "What's your name: ")]
        public string CandidateFullName { get; set; }
        [Display(Name = "What's your message: ")]
        public string EmailMessage { get; set; }
        [Display(Name = "To: ")]
        public string ReviewerAddress1 { get; set; }
   
        public string SelectedReviewer2 { get; set; }

        public string ReviewerAddress2 { get; set; }
        [Display(Name = "To: ")]
        public string ToEmailAddressCC { get; set; }
        [Display(Name = "First Name: ")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name: ")]
        public string LastName { get; set; }
        public int CandidateID { get; set; }
        public int RecruiterID { get; set; }
        public int CandidateResultKey { get; set; }
        [Display(Name = "To: ")]
        public string ToEmailAddress { get; set; }
        public string TestObject { get; set; }
        public string CandidateFullName1 { get; set; }




        public string Link { get; set; }
        public bool BrowserRemembered { get; set; }




    }
}