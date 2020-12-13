using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.ViewModel;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        [Authorize]
        public ActionResult Index()
        {
            var id = User.Identity.GetUserId();
            if (User.IsInRole("Admin"))
            {
                return View(db.Tickets.ToList());
            }
            if (User.IsInRole("Project Manager"))
            {
                var query = db.Projects.Where(x => x.ProjectUsers.Any(y => y.UserId == id));
                var projects = query.ToList();
                var ticketList = new List<Ticket>();
                if (projects.Count > 0)
                {
                    foreach (Project p in projects)
                    {
                        var projTickets = p.Tickets;
                        ticketList.AddRange(projTickets);
                    }
                }
                return View(ticketList);
            }
            if (User.IsInRole("Developer"))
            {
                var tickets = db.Tickets.Where(x => x.AssignedToUserId == id);
                return View(tickets.ToList());
            }
            if (User.IsInRole("Submitter"))
            {
                var tickets = db.Tickets.Where(x => x.OwnerUserId == id);
                return View(tickets.ToList());
            }
            return RedirectToAction("Index", "Home", null);
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }


        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult AssignUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            var model = new TicketAssignViewModel();
            model.Ticket = ticket;
            if (!string.IsNullOrEmpty(ticket.AssignedToUserId))
            {
                model.SelectedUser = ticket.AssignedToUserId;
            }
            var helper = new ProjectUserHelper();
            var userIDList = helper.UsersInProject(ticket.ProjectId);
            var userInfoList = helper.getUserInfo(userIDList);
            if (!string.IsNullOrEmpty(model.SelectedUser))
            {
                model.ProjUsersList = new SelectList(userInfoList, "UserId", "UserName", model.SelectedUser);
            }
            else
            {
                model.ProjUsersList = new SelectList(userInfoList, "UserId", "UserName");
            }
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Tickets/AssignUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignUser(int tId, string SelectedUser)
        {
            if (ModelState.IsValid)
            {
                var ticket = db.Tickets.Find(tId);
                //if there is already a user assigned, check if it is the same user
                //if it is, we won't create another ticket notification
                if (ticket.AssignedToUserId != null && ticket.AssignedToUserId.Equals("Selected User"))
                {
                    return RedirectToAction("Index");
                }
                //otherwise, update the ticket and create an entry in ticket notification table
                else
                {
                    ticket.AssignedToUserId = SelectedUser;
                    var tn = new TicketNotification { TicketId = tId, UserId = SelectedUser };
                    db.TicketNotifications.Add(tn);
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("AssignUser", new { id = tId });
            }
        }


        // GET: Tickets/Create
        [Authorize]
        public ActionResult Create()
        {
            var ticketView = new TicketCreateViewModel();
            ticketView.Projects = new SelectList(db.Projects, "Id", "Name");
            ticketView.TicketTypes = new SelectList(db.TicketTypes, "Id", "Name");
            ticketView.TicketPriorities = new SelectList(db.TicketPriorities, "Id", "Name");
            //ticketView.TicketStatuses = new SelectList(db.TicketStatuses, "Id", "Name");
            return View(ticketView);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Description,SelectedProject,SelectedType,SelectedPriority")] TicketCreateViewModel tvm)
        {
            if (ModelState.IsValid)
            {
                var ticket = new Ticket();
                ticket.Title = tvm.Title;
                ticket.Description = tvm.Description;
                ticket.Created = DateTimeOffset.Now;
                ticket.ProjectId = tvm.SelectedProject;
                ticket.TicketTypeId = tvm.SelectedType;
                ticket.TicketPriorityId = tvm.SelectedPriority;
                var query = from p in db.TicketStatuses
                            where p.Name == "New"
                            select p.Id;
                ticket.TicketStatusId = query.First();
                ticket.OwnerUserId = User.Identity.GetUserId();
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tvm);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            else
            {
                var ticketEdit = new TicketEditViewModel();
                ticketEdit.Id = ticket.Id;
                ticketEdit.Title = ticket.Title;
                ticketEdit.Created = ticket.Created;
                ticketEdit.Updated = ticket.Updated;
                ticketEdit.Description = ticket.Description;

                //setting the default selected values of the TicketEditViewModel to the current values in the ticket
                ticketEdit.SelectedProject = ticket.ProjectId;
                ticketEdit.SelectedType = ticket.TicketTypeId;
                ticketEdit.SelectedPriority = ticket.TicketPriorityId;
                ticketEdit.SelectedStatus = ticket.TicketStatusId;
                ticketEdit.OwnerUserId = ticket.OwnerUserId;
                ticketEdit.AssignedToUserId = ticket.AssignedToUserId;

                ticketEdit.Projects = new SelectList(db.Projects, "Id", "Name", ticketEdit.SelectedProject);//ticket.ProjectId
                ticketEdit.TicketTypes = new SelectList(db.TicketTypes, "Id", "Name", ticketEdit.SelectedType);//ticket.TicketTypeId
                ticketEdit.TicketPriorities = new SelectList(db.TicketPriorities, "Id", "Name", ticketEdit.SelectedPriority);//ticket.TicketPriorityId
                ticketEdit.TicketStatuses = new SelectList(db.TicketStatuses, "Id", "Name", ticketEdit.SelectedStatus);//ticket.TicketStatusId

                return View(ticketEdit);
            }

        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,SelectedProject,SelectedType,SelectedPriority,SelectedStatus,OwnerUserId,AssignedToUserId")] TicketEditViewModel tevModel)
        {
            if (ModelState.IsValid)
            {
                Ticket ticket = db.Tickets.Find(tevModel.Id);
                ticket.Title = tevModel.Title;
                ticket.Description = tevModel.Description;
                ticket.Created = tevModel.Created;
                ticket.Updated = DateTimeOffset.Now;
                ticket.ProjectId = tevModel.SelectedProject;
                ticket.TicketTypeId = tevModel.SelectedType;
                ticket.TicketPriorityId = tevModel.SelectedPriority;
                ticket.TicketStatusId = tevModel.SelectedStatus;
                ticket.OwnerUserId = tevModel.OwnerUserId;
                ticket.AssignedToUserId = tevModel.AssignedToUserId;

                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tevModel);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}