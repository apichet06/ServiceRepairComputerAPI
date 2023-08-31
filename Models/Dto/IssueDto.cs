using ServiceRepairComputer.Models;
using System.ComponentModel.DataAnnotations;

namespace ServiceRepairComputer.Models.Dto
{
    public class IssueDto
    {
        [Key]
        public int Id { get; set; } 
        public string? I_ID { get; set; } 
        public string? Title { get; set; } 
        public string? Description { get; set; }
        public string? Status { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public string? Path_Images { get; set; } 
        public string? ComputerId { get; set; } // รหัสคอมพิวเตอร์ที่มีปัญหา 
        public string? C_ID { get; set; } //ปัญหา // รหัสคอมพิวเตอร์ที่มีปัญหา 
        public string? EmployeeId { get; set; }  // รหัสผู้แจ้งงาน
        public string? Comment { get; set; }  // ข้อความรายละเอียดปิดงาน
        public string?  Status_Name { get; set; } 
        public string? TechnicianId { get; set; } // รหัสช่างซ่อมที่รับงาน  
        public DateTime? ReceiveAt { get; set; }
        public DateTime? StartJobAt { get; set; }
        public DateTime? EndJobAt { get; set; }
        public DateTime? SendJobAt { get; set; }
        public ComputerDto? Computer { get; set; } // เพิ่ม properties Computer
        public CategoriesDto? Category { get; set; } // เพิ่ม properties Category
        public EmployeeDto? Employee { get; set; } //  เพิ่ม properties Employee
        public EmployeeDto? Technician { get; set; } //  เพิ่ม properties Technician
       
    }

}

