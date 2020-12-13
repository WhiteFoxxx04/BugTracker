using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.ViewModel
{
    //This viewmodel is used to display the ticket information to the User
    public class TicketDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        [Display(Name = "Project")]
        public string ProjectTitle { get; set; }
        [Display(Name = "Type")]
        public string TicketType { get; set; }
        [Display(Name = "Priority")]
        public string TicketPriority { get; set; }
        [Display(Name = "Status")]
        public string TicketStatus { get; set; }
        [Display(Name = "Owner")]
        public string OwnerName { get; set; }
        [Display(Name = "Assigned To")]
        public string AssignedToUserName { get; set; }

        public ApplicationDbContext db = new ApplicationDbContext();
        public TicketDetailsViewModel(Ticket ticket)
        {
            this.Id = ticket.Id;
            this.Title = ticket.Title;
            this.Description = ticket.Description;
            this.Created = ticket.Created;
            this.Updated = ticket.Updated;
            //This one is for coverting Ids to the name for users to view
            this.ProjectTitle = getProjectTitle(ticket.ProjectId);
            this.TicketType = getType(ticket.TicketTypeId);
            this.TicketPriority = getPriority(ticket.TicketPriorityId);
            this.TicketStatus = getStatus(ticket.TicketStatusId);
            this.OwnerName = getName(ticket.OwnerUserId);
            //tickets are initially not assigned, so this field can be null
            if (!string.IsNullOrEmpty(ticket.AssignedToUserId))
            {
                this.AssignedToUserName = getName(ticket.AssignedToUserId);
            }
            else
            {
                this.AssignedToUserName = "Unassigned";
            }
        }

        private string getName(string userId)
        {
            var user = db.Users.Find(userId);
            return user.FirstName + " " + user.LastName;
        }

        private string getStatus(int ticketStatusId)
        {
            var ticketStatus = db.TicketStatuses.FirstOrDefault(x => x.Id == ticketStatusId);
            return ticketStatus.Name;
        }

        private string getPriority(int ticketPriorityId)
        {
            var ticketPriority  = db.TicketPriorities.FirstOrDefault(x => x.Id == ticketPriorityId);
            return ticketPriority.Name;
        }

        private string getType(int ticketTypeId)
        {
           var ticketType = db.TicketTypes.FirstOrDefault(x => x.Id == ticketTypeId);
           return ticketType.Name;
        }

        private string getProjectTitle(int projectId)
        {
            var project = db.Projects.FirstOrDefault(x => x.Id == projectId);
            return project.Name;
        }
    }
}