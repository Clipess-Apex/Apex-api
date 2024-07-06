using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clipess.DBClient.EntityModels
{
    public class FormProjectTask
    {
        [Key]
        public int TaskId { get; set; }
        public string TaskName { get; set; }
    }
}
