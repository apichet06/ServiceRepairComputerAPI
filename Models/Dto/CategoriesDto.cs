using System.ComponentModel.DataAnnotations;

namespace ServiceRepairComputer.Models.Dto
{
    public class CategoriesDto
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(10)]
        public string? C_ID { get; set; }
        [MaxLength(300)]
        public string? Name { get; set; }
        //รายละเอียดอื่นๆ ของหมวดหมู่ปัญหา ปัญหาการเชื่อมต่ออินเตอร์เน็ต,ปัญหาฮาร์ดแวร์,ปัญหาซอฟต์แวร์,ปัญหาเครื่องและอุปกรณ์อื่นๆ
    }
}
