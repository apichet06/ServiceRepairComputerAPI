using System.ComponentModel.DataAnnotations;

namespace ServiceRepairComputer.Models
{
    public class Position
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(10)] 
        public string? P_ID { get; set; }
        [MaxLength(150)]
        public string? P_Name { get; set; }
        [MaxLength(10)]
        public string? DV_ID { get; set; }
    }
}
