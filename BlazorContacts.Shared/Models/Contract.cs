using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorContacts.Shared.Models
{
    public class Contact
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
}
