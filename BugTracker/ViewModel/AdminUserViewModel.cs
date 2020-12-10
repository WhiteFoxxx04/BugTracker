﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.ViewModel
{
    public class AdminUserViewModel
    {
        public string UserId { get; set; }
        [Display(Name = "Email")]
        public string UserEmail { get; set; }
        [Display(Name = "Name")]
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Display(Name = "Roles")]
        public MultiSelectList Roles { get; set; }
        [Display(Name = "Current Roles")]
        public IList<string> CurrentRoles { get; set; }
        public List<string> SelectedRoles { get; set; }

    }
}