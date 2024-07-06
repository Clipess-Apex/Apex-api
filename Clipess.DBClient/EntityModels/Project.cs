using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clipess.DBClient.EntityModels
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int ClientId { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate {  get; set; }
        public int? Budget { get; set; }
        public int ProjectStatusId { get; set; }
        public DateTime CreatedDate { get; set; }   
        public DateTime? UpdatedDate { get; set;}
        public string? DocumentURL { get; set; }
        public bool Deleted { get; set; }

    }
}
