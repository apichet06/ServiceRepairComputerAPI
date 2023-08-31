using AutoMapper;
using AutoMapper.QueryableExtensions;
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


        [HttpGet("ChartComplete")]
          public async Task<ResponseDto> ChartComplete()
          {
              try
              {
                var result = await (from a in _db.Issues
                              join sx in _db.Issues on a.I_ID equals sx.I_ID into sxGroup
                              from sx in sxGroup.DefaultIfEmpty()
                              where a.Status == IssueStatus.Complete // เงื่อนไขใน WHERE clause
                              group new { IssueA = a, IssueSX = sx } by sx.Status into g
                              select new
                              {
                                  CompleteSt = g.Count(x => x.IssueA.Status == IssueStatus.Complete),
                                  cntStatus = g.Max(x => x.IssueSX.Status) 
                              }).ToListAsync();

                foreach (var item in result)
                  {
                      Console.WriteLine($"statusCn: {item.CompleteSt}, Status: {item.cntStatus} "); // เปลี่ยนชื่อ property ที่ซ้ำกัน
                  }

                  _response.Result = result;
                  _response.IsSuccess = true;
              }
              catch (Exception ex)
              {
                  _response.IsSuccess = false;
                  _response.Message = _message.an_error_occurred + ex.Message;
              }

              return _response;
          }
     

        [HttpGet("Chart")]
        public async Task<ResponseDto> Chart()
        {
            try
            {
                var enumValues = Enum.GetValues(typeof(IssueStatus)).Cast<IssueStatus>(); // ดึงค่า Enum ทั้งหมด

                var result = await (from a in _db.Issues
                                    join sx in _db.Issues on a.I_ID equals sx.I_ID into sxGroup
                                    from sx in sxGroup.DefaultIfEmpty()
                                    let status = sx.Status
                                    group new { IssueA = a, IssueSX = sx } by new { a.Status, status } into g
                                    select new
                                    {
                                        statusCn = g.Count(),
                                        IssueStatus = g.Key.Status.ToString() // Convert Enum to string
                                    }).ToListAsync();

                // สร้าง Dictionary ที่มี Enum ทั้งหมดเป็น key และกำหนดค่าจาก result เข้ามาเป็น value
                var statusDictionary = enumValues.ToDictionary(
                    status => status.ToString(),
                    status => result.FirstOrDefault(item => item.IssueStatus == status.ToString())?.statusCn ?? 0
                );

                // สร้าง List ของ Enum และจำนวนที่เกี่ยวข้อง
                var statusCounts = enumValues.Select(status => new
                {
                    Status = status.ToString(),
                    Count = statusDictionary[status.ToString()]
                }).ToList();

                // สร้าง List ของ Enum ที่มีและไม่มีข้อมูล
                var allStatuses = enumValues.Select(status => new
                {
                    Status = status,
                    Status_name = status.ToString(), 
                    Count = statusDictionary.ContainsKey(status.ToString()) ? statusDictionary[status.ToString()] : 0
                }).ToList();

                _response.Result = allStatuses; // นำ List ของ Enum ทั้งหมดมาใช้งานในการแสดงผล

                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = _message.an_error_occurred + ex.Message;
            }

            return _response;
        }



        [HttpGet]
        public async Task<ResponseDto> Index()
        {
            try
            {
                var query = from a in _db.Issues
                            join b in _db.Computers on a.ComputerId equals b.ComputerId into compJoin
                            from b in compJoin.DefaultIfEmpty()
                            join c in _db.Categories on a.C_ID equals c.C_ID into catJoin
                            from c in catJoin.DefaultIfEmpty()
                            join d in _db.Employees on a.EmployeeId equals d.EmployeeId into empJoin
                            from d in empJoin.DefaultIfEmpty()
                            join e in _db.Employees on a.TechnicianId equals e.EmployeeId into techJoin
                            from e in techJoin.DefaultIfEmpty()
                            join f in _db.Divisions on d.DV_ID equals f.DV_ID into divJoin
                            from f in divJoin.DefaultIfEmpty()
                            select new IssueDto
                            {
                                Id = a.Id,
                                I_ID = a.I_ID,
                                Title = a.Title,
                                Description = a.Description,
                                CreatedAt = a.CreatedAt,
                                Path_Images = a.Path_Images,
                                ComputerId = a.ComputerId,
                                C_ID = a.C_ID,
                                Status = ((int)a.Status).ToString(),
                                Status_Name = a.Status.ToString(),
                                EmployeeId = a.EmployeeId,
                                Comment = a.Comment,
                                TechnicianId = a.TechnicianId,
                                ReceiveAt = a.ReceiveAt,
                                StartJobAt = a.StartJobAt,
                                EndJobAt = a.EndJobAt,
                                SendJobAt = a.SendJobAt,
                                Computer = b != null ? new ComputerDto
                                {
                                    Id = b.Id,
                                    ComputerId = b.ComputerId,
                                    Name = b.Name,
                                    SerialNumber = b.SerialNumber,
                                    Description = b.Description
                                } : null,
                                Category = c != null ? new CategoriesDto
                                {
                                    Id = c.Id,
                                    C_ID = c.C_ID,
                                    Name = c.Name
                                } : null,
                                Employee = d != null ? new EmployeeDto
                                {
                                    Id = d.Id,
                                    EmployeeId = d.EmployeeId,
                                    Title = d.Title,
                                    FirstName = d.FirstName,
                                    LastName = d.LastName,
                                    Username = d.Username,
                                    Email = d.Email,
                                    Password = d.Password,
                                    DV_ID = d.DV_ID,
                                    DV_Name = f.DV_Name,
                                    P_ID = d.P_ID,
                                    Status = d.Status
                                } : null,
                                Technician = e != null ? new EmployeeDto
                                {
                                    Id = e.Id,
                                    EmployeeId = e.EmployeeId,
                                    Title = e.Title,
                                    FirstName = e.FirstName,
                                    LastName = e.LastName,
                                    Username = e.Username,
                                    Email = e.Email,
                                    Password = e.Password,
                                    DV_ID = e.DV_ID,
                                    DV_Name = f.DV_Name,
                                    P_ID = e.P_ID,
                                    Status = e.Status
                                } : null
                            };
               
               // var issueDtos = await query.ToListAsync();
                var issueDtos = await query.ProjectTo<IssueDto>(_mapper.ConfigurationProvider).ToListAsync();
                _response.Result = issueDtos;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = _message.an_error_occurred + ex.Message;
            }

            return _response;
        }

    /*    [HttpGet]
        public async Task<ResponseDto> GetEnumValues()
        {
            try
            {
                var query = from issue in _db.Issues
                            join computer in _db.Computers on issue.ComputerId equals computer.ComputerId into computerGroup
                            from computerLeftJoin in computerGroup.DefaultIfEmpty()
                            join category in _db.Categories on issue.C_ID equals category.C_ID into categoryGroup
                            from categoryLeftJoin in categoryGroup.DefaultIfEmpty()
                            join employee in _db.Employees on issue.EmployeeId equals employee.EmployeeId into employeeGroup
                            from employeeLeftJoin in employeeGroup.DefaultIfEmpty()
                            join technician in _db.Employees on issue.TechnicianId equals technician.EmployeeId into technicianGroup
                            from technicianLeftJoin in technicianGroup.DefaultIfEmpty()
                            join division in _db.Divisions on employeeLeftJoin.DV_ID equals division.DV_ID into divisionGroup
                            from divisionLeftJoin in divisionGroup.DefaultIfEmpty()
                            select new IssueDto
                            {
                                Id = issue.Id,
                                I_ID = issue.I_ID,
                                Title = issue.Title,
                                Description = issue.Description,
                                CreatedAt = issue.CreatedAt, 
                                Path_Images = issue.Path_Images,
                                ComputerId = issue.ComputerId,
                                C_ID = issue.C_ID,
                                Status = ((int)issue.Status).ToString(),
                                Status_Name = issue.Status.ToString(),
                                EmployeeId = issue.EmployeeId,
                                Comment   = issue.Comment,
                                TechnicianId = issue.TechnicianId,
                                ReceiveAt = issue.ReceiveAt,
                                StartJobAt =issue.StartJobAt,
                                EndJobAt  =issue.EndJobAt,
                                SendJobAt  = issue.SendJobAt,
                                Computer = computerLeftJoin != null ? new ComputerDto
                                {
                                    Id = computerLeftJoin.Id,
                                    ComputerId = computerLeftJoin.ComputerId,
                                    Name = computerLeftJoin.Name,
                                    SerialNumber = computerLeftJoin.SerialNumber,
                                    Description = computerLeftJoin.Description
                                } : null,
                                Category = categoryLeftJoin != null ? new CategoriesDto
                                {
                                    Id = categoryLeftJoin.Id,
                                    C_ID = categoryLeftJoin.C_ID,
                                    Name = categoryLeftJoin.Name
                                } : null,
                                Employee = employeeLeftJoin != null ? new EmployeeDto
                                {
                                    Id = employeeLeftJoin.Id,
                                    EmployeeId = employeeLeftJoin.EmployeeId,
                                    Title = employeeLeftJoin.Title,
                                    FirstName = employeeLeftJoin.FirstName,
                                    LastName = employeeLeftJoin.LastName,
                                    Username = employeeLeftJoin.Username,
                                    Email = employeeLeftJoin.Email,
                                    Password = employeeLeftJoin.Password,
                                    DV_ID = employeeLeftJoin.DV_ID,
                                    DV_Name = divisionLeftJoin.DV_Name,
                                    P_ID = employeeLeftJoin.P_ID,
                                    Status = employeeLeftJoin.Status
                                } : null,
                                Technician = technicianLeftJoin != null ? new EmployeeDto
                                {
                                    Id = technicianLeftJoin.Id,
                                    EmployeeId = technicianLeftJoin.EmployeeId,
                                    Title = technicianLeftJoin.Title,
                                    FirstName = technicianLeftJoin.FirstName,
                                    LastName = technicianLeftJoin.LastName,
                                    Username = technicianLeftJoin.Username,
                                    Email = technicianLeftJoin.Email,
                                    Password = technicianLeftJoin.Password,
                                    DV_ID = technicianLeftJoin.DV_ID,
                                    DV_Name = divisionLeftJoin.DV_Name,
                                    P_ID = technicianLeftJoin.P_ID,
                                    Status = technicianLeftJoin.Status
                                } : null
                            };

                //_response.Result = await query.ToListAsync();
                var issueDtos = await query.ProjectTo<IssueDto>(_mapper.ConfigurationProvider).ToListAsync();
                _response.Result = issueDtos;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = _message.an_error_occurred + ex.Message;
            }

            return _response;
        }

        */

        [HttpGet("enumvalues")]
        public async Task<ActionResult<ResponseDto>> GetEnumValuesAsync()
        {
          
            try
            {
                await Task.Run(() =>
                {
                    IssueStatus[] issueStatusValues = (IssueStatus[])Enum.GetValues(typeof(IssueStatus));
                    string[] issueStatusNames = Enum.GetNames(typeof(IssueStatus));

                    _response.Result = new
                    {
                        Values = issueStatusValues,
                        Names = issueStatusNames
                    };
                    _response.IsSuccess = true;
                });
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

                    obj.Status = issue.Status;
                    if (!string.IsNullOrEmpty(issue.Comment))
                    {
                        obj.Comment = issue.Comment;
                    }
                    if (!string.IsNullOrEmpty(issue.TechnicianId))
                    {
                        obj.TechnicianId = issue.TechnicianId;
                    }

                    if (!string.IsNullOrEmpty(issue.Title))
                    {
                        obj.Title = issue.Title;
                    }
                    if (!string.IsNullOrEmpty(issue.Description))
                    {
                        obj.Description = issue.Description;
                    }
                    if (!string.IsNullOrEmpty(issue.C_ID))
                    {
                        obj.C_ID = issue.C_ID;
                    }
                    if (!string.IsNullOrEmpty(issue.ComputerId))
                    {
                        obj.ComputerId = issue.ComputerId;
                    }

                    if (issue.ReceiveAt.HasValue && issue.ReceiveAt != obj.ReceiveAt)
                    {
                        obj.ReceiveAt = issue.ReceiveAt;
                    }
                    if (issue.StartJobAt.HasValue && issue.StartJobAt != obj.StartJobAt)
                    {
                        obj.StartJobAt = issue.StartJobAt;
                    }

                    if (issue.EndJobAt.HasValue && issue.EndJobAt != obj.EndJobAt)
                    {  
                        obj.EndJobAt = issue.EndJobAt;
                    }

                    if (issue.SendJobAt.HasValue && issue.SendJobAt != obj.SendJobAt)
                    {
                        obj.SendJobAt = issue.SendJobAt;
                    }

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

                    string uploadsFolderPath = Path.Combine("Uploads", DateTime.Now.ToString("yyyyMM")); 
                    string extension = Path.GetExtension(imageFile.FileName);
                    string uniqueFileName = obj.I_ID + extension; // สร้างชื่อไฟล์ที่ไม่ซ้ำ
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

        [HttpGet("{*imagePath}")]
        public async Task<IActionResult> GetImage(string imagePath)
        {
            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), imagePath);

                // if (System.IO.File.Exists(filePath))
                // {
                string ext = Path.GetExtension(filePath).ToLower();
                string contentType = ext switch
                {
                    ".png" => "image/png",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    _ => "application/octet-stream" // สามารถเปลี่ยนเป็น "image/jpeg" หรือ "image/png" ได้ตามต้องการ
                };

                byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(imageBytes, contentType);
                // }

                //return NotFound();
            }
            catch (Exception ex)
            {

                // ทำการจัดการข้อผิดพลาดที่เกิดขึ้นในกรณีที่เกิดข้อผิดพลาดในระหว่างการประมวลผล
                // คุณสามารถทำการบันทึกหรือจัดการข้อผิดพลาดอื่น ๆ ตามความเหมาะสม
                return StatusCode(500, ex.Message);
            }
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
