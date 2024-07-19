using System.ComponentModel.DataAnnotations;

namespace Clipess.DBClient.EntityModels
{
    public class Sample
    {
        [Key]
        public int SampleId { get; set; }
        
        public string SampleName { get; set; }
    }
}
