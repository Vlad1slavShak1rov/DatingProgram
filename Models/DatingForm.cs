using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingProgram.Models
{
    public class DatingForm
    {
        [Key]
        public int Id { get; set; }
        
        public int ClientId { get; set; }   
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public string Description { get; set; }
        public string PurposeDating { get; set; }
        public DateTime DateCreated { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
    }
}
