using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingProgram.Models
{
    public class Logs
    {
        public int Id { get; set; }
        public string Header { get; set; }
        public string Message { get; set; }
        public DateTime DateAct { get; set; }
    }
}
