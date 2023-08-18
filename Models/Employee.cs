using System.ComponentModel.DataAnnotations;

namespace ServiceRepairComputer.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; } 
        [MaxLength(10)] 
        public string? EmployeeId { get; set; }
        [MaxLength(10)] 
        public string? Title { get; set; }
        [MaxLength(100)]
        public string? FirstName { get; set; }
        [MaxLength(100)]
        public string? LastName { get; set; }
        [MaxLength(100)]
        public string? Username { get; set; }
        [MaxLength(100)]
        public string? Email { get; set; }
        [MaxLength(50)]
        public string? Password { get; set; } = "123456";
        [MaxLength(10)]
        public string? DV_ID { get; set; } //รหัสแผนก
        [MaxLength(10)]
        public string? P_ID { get; set; } //ตำแหน่ง
        [MaxLength(50)]
        public string? Status { get; set; }   //ทำงานอยู่ ลาออก
     
    }

  
}
