using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingProgram.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; } 
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;

        [ForeignKey("ClientId")]
        public Client Client { get; set; }
    }
}
