using System.ComponentModel.DataAnnotations;

namespace ServiceRepairComputer.Models.Dto
{
    public class EmployeeDto
    {

        [Key]
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public string? Title { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? DV_ID { get; set; }  //รหัสแผนก
        public string? DV_Name { get; set; } //ชื่อแผนก
        public string? P_ID { get; set; } //ตำแหน่ง
        public string? P_Name { get; set; } // ชื่อตำแหน่ง 
        public string? Status { get; set; } //ทำงานอยู่ ลาออก
        public string Fullname => $"{Title} {FirstName} {LastName}";
 

    }
}
