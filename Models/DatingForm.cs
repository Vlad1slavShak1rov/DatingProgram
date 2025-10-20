using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingProgram.Models
{
    public class DatingForm
    {
        [Key]
        public int Id { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public string Description { get; set; }
        public string PurposeDating { get; set; }
        public Enums.Gender PreferredGender { get; set; }
        public DateOnly DateCreated { get; set; }


        public virtual List<Client> Clients { get; set; } = new();
    }
}
