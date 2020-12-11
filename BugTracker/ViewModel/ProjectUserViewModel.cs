using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.ViewModel
{
    public class ProjectUserViewModel
    {
        [Display(Name = "Project Name" )]
        public string ProjectName { get; set; }
        public int ProjectId { get; set; }
        public MultiSelectList UsersAssignedtoProject { get; set; }
        public MultiSelectList UsersNotAssignedToProject { get; set; }
        public string[] UserToAdd { get; set; }
        public string[] UserToRemove { get; set; }
    }
}