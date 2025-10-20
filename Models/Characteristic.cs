using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingProgram.Models
{
    public class Characteristic
    {
        [Key]
        public int Id { get; set; }
        public int Age { get; set; }
        public string City { get; set; }
        public string Gender { get; set; }

        public virtual List<Client> Clients { get; set; } = new();
    }
}
