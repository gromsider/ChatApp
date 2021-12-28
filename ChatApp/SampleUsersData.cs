using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApp.Models;

namespace ChatApp
{
    public static class SampleUsersData
    {
        public static void Initialize(DataBaseContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        Email = "Andry@mail.ru",
                        Password = "123"
                    },
                    new User
                    {
                        Email = "Ann@mail.ru",
                        Password = "321"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
