using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clipess.DBClient.EntityModels
{
    public class FormProject
    {
        [Key]
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
    }
}
