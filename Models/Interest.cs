using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingProgram.Models
{
    public class Interest
    {
        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Description { get; set; }

        public virtual Client Client { get; set; }
    }
}
