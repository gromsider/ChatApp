using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class PartnerDictionary
    {
        public int Id { get; set; }
        public string PartnerName { get; set; }
        public List<Message> Messages { get; set; }
    }
}
