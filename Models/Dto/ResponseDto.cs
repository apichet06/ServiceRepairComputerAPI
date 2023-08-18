using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServiceRepairComputer.Models.Dto
{
    public class ResponseDto
    {
        public object? Result { get; set; }
        public bool? IsSuccess { get; set; } = true;
        public string Message { get; set; } = "";
    }

    public class MessageDto
    {
        public string InsertMessage { get; set; } = "บันทึกข้อมูลสำเร็จ!";
        public string UpdateMessage { get; set; } = "อับเดทข้อมูลสำเร็จ!";
        public string DeleteMessage { get; set; } = "ลบข้อมูลสำเร็จ!";
        public string Not_found { get; set; } = "ไม่พบข้อมูล!";
        public string already_exists { get; set; } = "ข้อมูลนี้มีอยู่แล้ว! : ";
        public string an_error_occurred { get; set; } = "เกิดข้อผิดพลาด : ";
    }

}