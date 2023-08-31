using System.ComponentModel.DataAnnotations;

namespace ServiceRepairComputer.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(10)]
        public string? CM_ID { get; set; }
        [MaxLength(200)]
        public string? Contents { get; set; }
        [Range(1, 5, ErrorMessage = "Score must be between 1 and 5.")]
        public int Score { get; set; } //ดาวน์ในการซ่อม
        public DateTime? CreatedAt { get; set; }
        [MaxLength(10)]
        public string? EmployeeId { get; set; } // รหัสUserที่แสดงความคิดเห็น
        [MaxLength(10)]
        public string? I_ID { get; set; } // รหัสปัญหาที่เกี่ยวข้อง
        // ข้อมูลอื่นๆ ของความคิดเห็น
    }

}
