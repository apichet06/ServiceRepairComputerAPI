using System.ComponentModel.DataAnnotations;

namespace ServiceRepairComputer.Models.Dto
{
    public class PositionDto
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(10)]
        public string? P_ID { get; set; }
        [MaxLength(150)]
        public string? P_Name { get; set; }
        [MaxLength(10)]
        public string? DV_ID { get; set; }
        public string? DV_Name { get; set; } //ชื่อแผนก
    }
}
