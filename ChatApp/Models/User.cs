using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        // Словарь: партнер чата и сообщения с ним
        public List<PartnerDictionary> PartnersDictionary { get; set; }
    }
}
