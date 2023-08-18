using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceRepairComputer.Data;
using ServiceRepairComputer.Models;
using ServiceRepairComputer.Models.Dto;
using System.IO;

namespace ServiceRepairComputer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueAPIController : ControllerBase
    {
        private AppDbContext _db;
        private IMapper _mapper;
        private ResponseDto _response;
        private MessageDto _message;
        public IssueAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
            _message = new MessageDto();
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Issue> objList = _db.Issues.ToList();
                _response.Result = _mapper.Map<IEnumerable<IssueDto>>(objList);

                //  แมป enum กับสตริง 
                if (_response.Result is IEnumerable<IssueDto> issueDtos)
                {
                    foreach (var issueDto in issueDtos)
                    {
                        issueDto.Status = issueDto.Status!.ToString();
                    }
                }
 
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = _message.an_error_occurred + ex.Message;
            }

            return _response;
        }

        [HttpPost]
        public async Task<ResponseDto> Post([FromForm] Issue issue, IFormFile imageFile = null!)
        {
            try
            {
         

                // กำหนด Auto ID ให้กับ I_ID
                string NextID = await GenerateAutoId();
                issue.I_ID = NextID;
                // เช็คว่ามีการอัปโหลดรูปภาพหรือไม่
                if (imageFile != null && imageFile.Length > 0)
                {
                    // กำหนด path ให้กับรูปภาพที่จะอัปโหลด
                    string uploadsFolderPath = Path.Combine("Uploads", DateTime.Now.ToString("yyyyMM"));
                    if (!Directory.Exists(uploadsFolderPath))
                    {
                        Directory.CreateDirectory(uploadsFolderPath);
                    }

                    string extension = Path.GetExtension(imageFile.FileName);
                    string uniqueFileName = NextID + extension;
                    string filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                    // บันทึกไฟล์รูปภาพลงใน server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(fileStream);
                    }

                    // ให้ตัวแปร Path_Images ในอ็อบเจ็กต์ Issue มีค่าเป็น path ของรูปภาพที่อัปโหลด
                    issue.Path_Images = filePath;
                }

                // สร้างอ็อบเจ็กต์ Issue และบันทึกลงฐานข้อมูล
                Issue obj = _mapper.Map<Issue>(issue);
                _db.Issues.Add(obj);
               await _db.SaveChangesAsync();

                _response.Result = _mapper.Map<IssueDto>(obj);
                _response.Message = _message.InsertMessage;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = _message.an_error_occurred + ex.Message;
            }

            return _response;
        }


        [HttpPut]
        [Route("{id:int}")]
        public async Task<ResponseDto> Put(int id, [FromForm] Issue issue, IFormFile imageFile = null!)
        {
            try
            {
                Issue? obj = await _db.Issues.FirstOrDefaultAsync(c => c.Id == id)!;

                if (obj == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.Not_found;
                    return _response;
                }

                obj.Title = issue.Title;
                obj.Description = issue.Description;
                obj.CategoryId = issue.CategoryId;
                obj.ComputerId = issue.ComputerId; 
                obj.ResolvedAt = issue.ResolvedAt;

                // Check if there's a new image to upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Delete existing image
                    if (!string.IsNullOrEmpty(obj.Path_Images))
                    {
                        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), obj.Path_Images);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    // Upload new image
                    string uploadsFolderPath = Path.GetDirectoryName(obj.Path_Images)!;
                    string extension = Path.GetExtension(imageFile.FileName);
                    string uniqueFileName = Path.GetFileNameWithoutExtension(obj.Path_Images) + extension; // Reuse original file name
                    string filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    obj.Path_Images = filePath;
                }

                _db.Issues.Update(obj);
              await _db.SaveChangesAsync();

                _response.Result = _mapper.Map<IssueDto>(obj);
                _response.Message = _message.UpdateMessage;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = _message.an_error_occurred + ex.Message;
            }

            return _response;
        }


        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ResponseDto> Delete(int id) {

            try
            {
                Issue? issue = await _db.Issues.FirstOrDefaultAsync(x => x.Id == id)!;
                if (issue == null)
                {
                    _response.IsSuccess=false;
                    _response.Message = _message.Not_found;
                    return _response;
                }

                // Delete the image file
                if (!string.IsNullOrEmpty(issue.Path_Images))
                {
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), issue.Path_Images);
                    if (System.IO.File.Exists(imagePath)) // ใช้ชื่อเต็มของคลาส File
                    {
                        System.IO.File.Delete(imagePath); // ใช้ชื่อเต็มของคลาส File
                    }
                }

                _db.Issues.Remove(issue);
                await _db.SaveChangesAsync(); 
                
                _response.Message = _message.DeleteMessage;
                _response.Result = _mapper.Map<IssueDto>(issue);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = _message.an_error_occurred + ex.Message;
            }
            return _response;
        
        }

        private async  Task <string> GenerateAutoId()
        {
            string? lastId = await _db.Issues.OrderByDescending(x => x.Id).Select(x => x.I_ID).FirstOrDefaultAsync()!;

            if (lastId != null && lastId.StartsWith("IS"))
            {
                int num = int.Parse(lastId.Substring(2)) + 1;
                return "IS" + num.ToString("D7");
            }
            return "IS0000001"; // ถ้ายังไม่มีข้อมูลในฐานข้อมูล
        }
 
    }
}
