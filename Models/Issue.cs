using System.ComponentModel.DataAnnotations;

namespace ServiceRepairComputer.Models
{
    public class Issue
    {
        [Key]
        public int Id { get; set; } 
        [MaxLength(10)]
        public string? I_ID { get; set; }
        [MaxLength(200)]
        public string? Title { get; set; }
        [MaxLength(300)]
        public string? Description { get; set; }
        public IssueStatus Status { get; set; } = IssueStatus.แจ้งปัญหา; // ตั้งค่าตั้งต้นให้กับ enum
        public DateTime CreatedAt { get; set; } 
        [MaxLength(150)]
        public string? Path_Images { get; set; }
        [MaxLength(10)]
        public string? ComputerId { get; set; } // รหัสคอมพิวเตอร์ที่มีปัญหา
        [MaxLength(10)]
        public string? C_ID { get; set; }  // รหัสคอมพิวเตอร์ที่มีปัญหา
        [MaxLength(10)]
        public string? EmployeeId { get; set; }  // รหัสผู้แจ้งงาน
        [MaxLength(300)]
        public string? Comment { get; set; }  // ข้อความรายละเอียดปิดงาน
        [MaxLength(10)]
        public string? TechnicianId { get; set; } // รหัสช่างซ่อมที่รับงาน
        public DateTime? ReceiveAt { get; set; }
        public DateTime? StartJobAt { get; set; }
        public DateTime? EndJobAt { get; set; }
        public DateTime? SendJobAt { get; set; }
        
    }
    public enum IssueStatus
    {
        แจ้งปัญหา, //แจ้งปัญหา
        รับงาน, //รับงาน
        เริ่มงาน, //เริ่มงาน
        ปิดงาน,  //ปิกงาน
        ส่งงาน, //ส่งงาน
        Complete //ประเมินงานซ่อม
        
    }

    /*public enum IssueStatus
    {
        Issue, //แจ้งปัญหา
        Receive, //รับงาน
        StartJob, //เริ่มงาน
        EndJob,  //ปิกงาน
        SendJob, //ส่งงาน
       
    }*/
}
