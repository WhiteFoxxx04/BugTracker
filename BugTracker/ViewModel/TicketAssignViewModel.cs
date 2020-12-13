using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.ViewModel
{
    public class TicketAssignViewModel
    {
        public Ticket Ticket { get; set; }
        [Display(Name = "Project")]
        public SelectList ProjUsersList { get; set; }
        [Required(ErrorMessage = "No user selected")]
        public string SelectedUser { get; set; }

    }
}