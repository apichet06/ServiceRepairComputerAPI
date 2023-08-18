using System.ComponentModel.DataAnnotations;

namespace ServiceRepairComputer.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string? CM_ID { get; set; }
        [MaxLength(100)]
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        [MaxLength(10)]
        public string? TechnicianId { get; set; } // รหัสช่างซ่อมที่แสดงความคิดเห็น
        [MaxLength(10)]
        public string? I_ID { get; set; } // รหัสปัญหาที่เกี่ยวข้อง
        // ข้อมูลอื่นๆ ของความคิดเห็น
    }

}
