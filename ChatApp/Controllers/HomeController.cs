using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChatApp.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;

namespace ChatApp.Controllers
{
    public class HomeController : Controller
    {
        DataBaseContext db;

        public HomeController(DataBaseContext context)
        {
            db = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            IEnumerable<string> users = GetUsers();

            return View(new DialogFormContainer
            {
                ownerName = User.Identity.Name,
                Users = users
            });
        }

        public IActionResult Dialog(string partnerName)
        {
            string ownerName = User.Identity.Name;

            var users = GetUsers();

            if (string.IsNullOrEmpty(ownerName)
                || string.IsNullOrEmpty(partnerName)
                || !users.Any(u => u == ownerName)
                || !users.Any(u => u == partnerName))
            {
                return View("ControllerError");
            }

            var story = GetStory(partnerName);


            return View(new DialogFormContainer
            {
                ownerName = ownerName,
                partnerName = partnerName,
                Users = users,
                Story = story
            });
        }

        [HttpPost]
        public IActionResult Dialog(string message, string partnerName)
        {
            string ownerName = User.Identity.Name;

            var users = GetUsers();

            if (!(string.IsNullOrEmpty(message)
                || string.IsNullOrEmpty(ownerName)
                || string.IsNullOrEmpty(partnerName)
                || !users.Any(u => u == ownerName)
                || !users.Any(u => u == partnerName)))
            {
                AddMessageToDialogue(message, ownerName, partnerName, ownerName, partnerName);

                // обменятся диалогами
                AddMessageToDialogue(message, ownerName, partnerName, partnerName, ownerName);

                int succesfulRecords = db.SaveChanges();
            }


            var story = GetStory(partnerName);

            return View(new DialogFormContainer
            {
                ownerName = ownerName,
                partnerName = partnerName,
                Users = users,
                Story = story
            });
        }

        private void AddMessageToDialogue(string message, string messageFromUser, string messageToUser, string dialogueOwner, string dialoguePartner)
        {
            User user = db.Users.Include(u => u.PartnersDictionary).FirstOrDefault(u => u.Email == dialogueOwner);

            // Добавление сообщения
            if (user.PartnersDictionary == null || user.PartnersDictionary.Count == 0)
            {
                // в пустую историю
                user.PartnersDictionary = new List<PartnerDictionary>()
                {
                    new PartnerDictionary()
                    {
                        PartnerName = dialoguePartner,
                        Messages = new List<Message>
                            {
                                new Message { MessageText = message, TimeStamp = DateTime.Now, From = messageFromUser, To = messageToUser }
                            }
                    }
                };
            }
            else
            {
                PartnerDictionary partner = user.PartnersDictionary.FirstOrDefault((u => u.PartnerName == dialoguePartner));
                if (partner == null)
                {
                    // диалога с заданным партнером нет, создать
                    user.PartnersDictionary.Add
                    (
                        new PartnerDictionary()
                        {
                            PartnerName = dialoguePartner,
                            Messages = new List<Message>
                            {
                                new Message { MessageText = message, TimeStamp = DateTime.Now, From = messageFromUser, To = messageToUser }
                            }
                        }
                    );
                }
                else
                {
                    // диалог с заданным партнером есть
                    if (partner.Messages == null || partner.Messages.Count == 0)
                    {
                        // но сообщений не было
                        partner.Messages = new List<Message>
                            {
                                new Message { MessageText = message, TimeStamp = DateTime.Now, From = messageFromUser, To = messageToUser }
                            };
                    }
                    else
                    {
                        // сообщения уже есть
                        partner.Messages.Add(new Message { MessageText = message, TimeStamp = DateTime.Now, From = messageFromUser, To = messageToUser });
                    }
                }
            }
        }

        private List<Message> GetStory(string partnerName)
        {
            string ownerName = User.Identity.Name;

            var currentUserDialogues = db.Users
                .Include(u => u.PartnersDictionary).ThenInclude(u => u.Messages)
                .ToList()
                .FirstOrDefault(u => u.Email == ownerName)
                .PartnersDictionary;

            if (currentUserDialogues == null
                || currentUserDialogues.Count == 0
                || currentUserDialogues.FirstOrDefault((u => u.PartnerName == partnerName)) == null)
            {
                return new List<Message>();
            }
            else
            {
                return GetUserDialogueWithPartner(currentUserDialogues.FirstOrDefault((u => u.PartnerName == partnerName)));
            }
        }

        private static List<Message> GetUserDialogueWithPartner(PartnerDictionary dialogueWithPartner)
        {
            if (dialogueWithPartner.Messages == null)
            {
                return new List<Message>();
            }
            else
            {
                return dialogueWithPartner.Messages.OrderBy(m => m.TimeStamp).ToList();
            }
        }

        private IEnumerable<string> GetUsers()
        {
            return db.Users.ToList().Select(u => u.Email);
        }
    }
}
