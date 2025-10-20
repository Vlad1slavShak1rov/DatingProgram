using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingProgram.Models
{
    public class Likes
    {
        [Key]
        public int Id { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public DateOnly Created { get; set; }
        public Enums.LikesStatus LikesStatus { get; set; }

        [ForeignKey("FromUserId")]
        public virtual User FromUser { get; set; }
        [ForeignKey("ToUserId")]
        public virtual User ToUser { get; set; }
    }
}
