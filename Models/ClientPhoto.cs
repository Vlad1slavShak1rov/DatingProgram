using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingProgram.Models
{
    public class ClientPhoto
    {
        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Path { get; set; }
        public DateTime DateAdded { get; set; }

        public virtual List<Client> Clients { get; set; } = new();
    }
}
