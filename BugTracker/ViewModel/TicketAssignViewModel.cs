using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.ViewModel
{
    public class TicketAssignViewModel
    {
        public Ticket Ticket { get; set; }
        public SelectList ProjectUsersList {get; set; }
        public string SelectedUser { get; set; }

    }
}