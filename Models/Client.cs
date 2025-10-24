using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingProgram.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? CharacteristicId { get; set; } = null;
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string Contact { get; set; }

        [ForeignKey("UserId")]
        public virtual User Users { get; set; }
        public virtual List<DatingForm> DatingForm { get; set; }
        [ForeignKey("CharacteristicId")]
        public virtual Characteristic? Characteristic { get; set; }
        public virtual List<Pair> ManPairs { get; set; }
        public virtual List<Pair> WomanPairs { get; set; }
        public virtual List<ClientPhoto> ClientPhotos { get; set; }
        public virtual List<Likes> FromClient { get; set; }
        public virtual List<Likes> ToClient { get; set; }
    }
}
