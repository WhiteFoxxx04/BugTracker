using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Description must be between 200 characters")]
        public string Description { get; set; }

        public Project()
        {
            this.Tickets = new HashSet<Ticket>();
            this.ProjectUsers = new HashSet<ProjectUser>();
        }

        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }
    }
}