using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clipess.DBClient.EntityModels
{
    public class FormTeam
    {
        [Key]
        public int TeamId { get; set; }
        public string TeamName { get; set; }
    }
}
