using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceRepairComputer.Data;
 
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ServiceRepairComputer.Models;
using ServiceRepairComputer.Models.Dto;
 
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ServiceRepairComputer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private MessageDto _message;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;
        public EmployeeAPIController(AppDbContext db, IMapper mapper, IConfiguration configuration)
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
            _configuration = configuration;
            _message = new MessageDto();
        }
        [HttpGet] 
        public async Task<ResponseDto> Get()
        {
            try
            {
                IEnumerable<Employee> objList = await _db.Employees.ToListAsync();
                List<EmployeeDto> mappedList = _mapper.Map<List<EmployeeDto>>(objList);
                
                foreach (var employeeDto in mappedList)
                {
                    // Fetch department and position names and set them in the DTO
                    var department = await _db.Divisions.FirstOrDefaultAsync(d => d.DV_ID == employeeDto.DV_ID);
                    if (department != null)
                    {
                        employeeDto.DV_Name = department.DV_Name;
                    }

                    var position = await _db.Positions.FirstOrDefaultAsync(p => p.P_ID == employeeDto.P_ID);
                    if (position != null)
                    {
                        employeeDto.P_Name = position.P_Name;
                    }
                }
                 

                _response.Result = mappedList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = _message.an_error_occurred + ex.Message;
            }
            return _response;
        }
        private async Task<string> GenerateAutoId()
        {
            string? lastId = await _db.Employees.OrderByDescending(x => x.Id).Select(x => x.EmployeeId).FirstOrDefaultAsync();

            if (lastId != null && lastId.StartsWith("EMP"))
            {
                int num = int.Parse(lastId.Substring(3)) + 1;
                return "EMP" + num.ToString("D7");
            }

            return "EMP0000001"; // ถ้ายังไม่มีข้อมูลในฐานข้อมูล
        }

        [HttpPost]
        public async Task<ResponseDto> Post([FromBody] Employee employee)
        {
            try
            {
                if(await _db.Employees.AnyAsync(c=>c.FirstName == employee.FirstName && c.LastName == employee.LastName))
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.already_exists + employee.LastName +" "+ employee.FirstName;
                    return _response;
                }

                if (await _db.Employees.AnyAsync(c => c.Username == employee.Username))
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.already_exists + employee.Username;
                    return _response;
                }


                Employee obj = _mapper.Map<Employee>(employee);

                string  NextId =   await GenerateAutoId();
                obj.EmployeeId = NextId;

                _db.Employees.Add(obj);
                await _db.SaveChangesAsync();
                _response.Result = _mapper.Map<EmployeeDto>(obj);
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
        public async Task<ResponseDto> put(int id,[FromBody] Employee employee)
        {
            try
            {
                Employee? obj = await _db.Employees.FirstOrDefaultAsync(x => x.Id == id);

                if (obj == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.Not_found;
                    return _response;
                }
                if (await _db.Employees.AnyAsync(c => c.Id != id && c.FirstName == employee.FirstName && c.LastName == employee.LastName))
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.already_exists + employee.LastName + " " + employee.FirstName;

                    return _response;
                }
                if (await _db.Employees.AnyAsync(c=> c.Id != id && c.Username == employee.Username)) {
                    _response.IsSuccess = false;
                    _response.Message = _message.already_exists + employee.Username;
                    return _response;

                } 

                obj.FirstName = employee.FirstName;
                obj.LastName = employee.LastName;
                obj.Email = employee.Email; 
                obj.Username = employee.Username;
                obj.Password = employee.Password;
                obj.DV_ID = employee.DV_ID;
                obj.P_ID = employee.P_ID;
                obj.Status = employee.Status;
               
                _db.Employees.Update(obj);
               await _db.SaveChangesAsync(); 
                _response.Result = _mapper.Map<EmployeeDto>(obj);
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
        public async Task<ResponseDto> Delete(int id)
        {
            try
            {
                Employee? obj = await _db.Employees.FirstOrDefaultAsync(x => x.Id == id);

                if (obj == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.Not_found;
                   return _response;
                }
                 
                _db.Employees.Remove(obj);
               await _db.SaveChangesAsync();
                _response.Message = _message.DeleteMessage;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = _message.an_error_occurred + ex.Message;
            }

            return _response;
        }

        [Route("Login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CheckLogin(Employee model)
        {
            // ตรวจสอบ username และ password ในฐานข้อมูล
            var employee = await _db.Employees
                .SingleOrDefaultAsync(e => e.Username == model.Username && e.Password == model.Password);

            if (employee != null)
            {
                // สร้าง Token ด้วยข้อมูลของ employee
                var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
            new Claim("UserId", model.Username!)
        };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(10),
                    signingCredentials: signIn);

                // คืนค่า Token เป็น Response
                return Ok(new
                {
                    Message = "ล็อกอินสำเร็จ!",
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    resulte = employee 

                });
            }
            else
            {
                return BadRequest(new
                {
                    Message = "ชื่อหรือรหัสผ่านของคุณไม่ถูกต้อง!"
                });
            }
        }


    }
}
