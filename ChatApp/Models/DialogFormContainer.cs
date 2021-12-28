using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class DialogFormContainer
    {
        public string ownerName { get; set; }
        public string partnerName { get; set; }
        public IEnumerable<string> Users { get; set; }
        public List<Message> Story { get; set; }
    }
}
