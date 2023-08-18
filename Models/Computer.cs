using System.ComponentModel.DataAnnotations;

namespace ServiceRepairComputer.Models
{
    public class Computer
    {
        [Key]
        public int Id { get; set; } 
        [MaxLength(20)]
        public string? ComputerId { get; set; }
        [MaxLength(250)]
        public string? Name { get; set; }
        [MaxLength(100)]
        public string? SerialNumber { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        // รายละเอียดอื่นๆ ของคอมพิวเตอร์
    }
}
