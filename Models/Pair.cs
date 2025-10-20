using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingProgram.Models
{
    public class Pair
    {
        [Key]
        public int Id { get; set; }
        public int ManId { get; set; }
        public int GirlId { get; set; }
        public Enums.PairStatus PairStatus { get; set; }

        [ForeignKey("ManId")]
        public virtual User Man { get; set; }
        [ForeignKey("GirlId")]
        public virtual User Women { get; set; }
    }
}
