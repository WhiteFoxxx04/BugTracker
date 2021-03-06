﻿using BugTracker.Models;
using BugTracker.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Helpers
{
    public class ProjectUserHelper
    {
        private ApplicationDbContext db;
        public UserManager<ApplicationUser> userManager;

        public ProjectUserHelper()
        {
            this.db = new ApplicationDbContext();
            this.userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        public bool IsUserInProject(string userId, int projectId)
        {
            var query1 = db.ProjectUsers.Where(c => c.ProjectId == projectId);
            var query2 = query1.Where(u => u.UserId.Equals(userId));
            if (query2.ToList().Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //get a list of all Projects a user is assigned to
        public List<Project> ListUserProjects(string userId)
        {
            var projQuery = db.ProjectUsers.Where(u => u.UserId.Equals(userId));
            var projList = projQuery.ToList();
            var projectsList = new List<Project>();

            if (projList.Count > 0)
            {
                foreach (var project in projList)
                {
                    //var titleQuery = db.Projects.Where(p => p.Id.Equals(project));
                    var query = from p in db.Projects
                                where p.Id == project.Id
                                select p;
                    foreach (var a in query)
                    {
                        projectsList.Add(a);
                    }
                }
            }
            return projectsList;
        }
        public bool AddUserToProject(string userId, int projectId)
        {
            ProjectUser pUser = new ProjectUser { UserId = userId, ProjectId = projectId };
            db.ProjectUsers.Add(pUser);
            var result = db.SaveChanges();
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool RemoveUserFromProject(string userId, int projectId)
        {
            if (IsUserInProject(userId, projectId))
            {
                var deleteEntries =
                from pUsers in db.ProjectUsers
                where pUsers.UserId == userId && pUsers.ProjectId == projectId
                select pUsers;

                foreach (var entry in deleteEntries)
                {
                    var delete = db.ProjectUsers.Remove(entry);
                    //db.ProjectUsers.DeleteOnSubmit(entry);
                }
                var result = db.SaveChanges();
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        //this returns a list of UserIDs associated with a project
        public List<string> UsersInProject(int projectId)
        {
            //var userQuery = db.ProjectUsers.Where(p => p.ProjectId == projectId);
            var query = from p in db.ProjectUsers
                        where p.ProjectId == projectId
                        select p.UserId;

            var usersList = new List<string>();
            foreach (var u in query)
            {
                usersList.Add(u);
            }
            return usersList;
        }
        //this returns a list of UserIDs not linked with a project
        public List<string> UsersNotInProject(int projectId)
        {
            var usersList = UsersInProject(projectId);
            var allUsers = new List<string>();

            //make a list of userIds of every user in the database
            foreach (var user in db.Users)
            {
                allUsers.Add(user.Id);
            }
            var notList = allUsers.Except(usersList);
            var notStrings = new List<string>();

            foreach (var u in notList)
            {
                notStrings.Add(u.ToString());
            }
            return notStrings;
        }
        public List<UserInfoViewModel> getUserInfo(List<string> userIds)
        {
            //set up a list to contain user info objects
            var userInfoList = new List<UserInfoViewModel>();

            foreach (var userId in userIds)
            {
                var myUser = new UserInfoViewModel();
                myUser.UserId = userId;
                var appUser = db.Users.Find(userId);
                myUser.FirstName = appUser.FirstName;
                myUser.LastName = appUser.LastName;
                myUser.Email = appUser.Email;
                myUser.UserName = myUser.FirstName + " " + myUser.LastName;
                var myUserHelper = new UserRolesHelper();
                myUser.CurrentRoles = myUserHelper.ListUserRoles(userId);
                userInfoList.Add(myUser);
            }
            return userInfoList;
        }

    }
}