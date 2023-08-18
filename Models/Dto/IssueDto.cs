using ServiceRepairComputer.Models;
using System.ComponentModel.DataAnnotations;

namespace ServiceRepairComputer.Models.Dto
{
    public class IssueDto
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(10)]
        public string? I_ID { get; set; }
        [MaxLength(200)]
        public string? Title { get; set; }
        [MaxLength(300)]
        public string? Description { get; set; }
        public string? Status { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        [MaxLength(150)]
        public string? Path_Images { get; set; }
        [MaxLength(10)]
        public string? ComputerId { get; set; } // รหัสคอมพิวเตอร์ที่มีปัญหา
        [MaxLength(10)]
        public string? CategoryId { get; set; } // รหัสหมวดหมู่ปัญหา
        [MaxLength(10)]
        public string? EmployeeId { get; set; }  // รหัสผู้แจ้งงาน
        [MaxLength(10)]
        public string? TechnicianId { get; set; } // รหัสช่างซ่อมที่รับงาน
        // ข้อมูลอื่นๆ ของปัญหา
    }

}

