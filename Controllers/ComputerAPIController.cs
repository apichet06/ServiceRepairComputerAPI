using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceRepairComputer.Data;
using ServiceRepairComputer.Models;
using ServiceRepairComputer.Models.Dto;
using System.Globalization;

namespace ServiceRepairComputer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComputerAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private MessageDto _message;
        private IMapper _mapper;
        public ComputerAPIController(AppDbContext db,IMapper mapper)
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
            _message = new MessageDto();
        }
        [HttpGet]
        public async Task<ResponseDto> Get()
        {
            try
            {
                IEnumerable<Computer> objList = await _db.Computers.ToListAsync();
                _response.Result = _mapper.Map<IEnumerable<ComputerDto>>(objList);

            }catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = _message.an_error_occurred + ex.Message;
            }
            return _response;
        }

        [HttpPost]
        public async Task<ResponseDto> Post([FromBody] Computer computer) {

            try
            {
                if (await _db.Computers.AnyAsync(c => c.SerialNumber == computer.SerialNumber))
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.already_exists + computer.SerialNumber;
                    return _response;
                }

                Computer obj = _mapper.Map<Computer>(computer);
                string nexID = await GenerateId(); 
                obj.ComputerId = nexID; 
                _db.Add(obj);
               await _db.SaveChangesAsync();
                _response.Result = _mapper.Map<ComputerDto>(obj);
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
        public async Task<ResponseDto> Put(int id, Computer computer)
        {
            try
            {
                // Find the existing computer by ID
                Computer? obj = await _db.Computers.FirstOrDefaultAsync(c => c.Id == id)!;

                if (obj == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.Not_found;
                    return _response;
                }

                // Check if the updated SerialNumber already exists in the database
                if (await _db.Computers.AnyAsync(c => c.Id != id && c.SerialNumber == computer.SerialNumber))
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.already_exists + computer.SerialNumber;
                    return _response;
                }

                obj.Name = computer.Name;
                obj.SerialNumber = computer.SerialNumber;
                obj.Description = computer.Description;

                // Save changes to the database
               await  _db.SaveChangesAsync();

                _response.Result = _mapper.Map<ComputerDto>(obj);
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
                Computer? obj =await _db.Computers.FirstOrDefaultAsync(c => c.Id == id)!;

                if (obj == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.Not_found;
                    return _response;
                }
                _db.Computers.Remove(obj);
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


        private async Task<string> GenerateId()
        {
            string? lastId = await _db.Computers.OrderByDescending(u => u.ComputerId).Select(u => u.ComputerId).FirstOrDefaultAsync()!;

            if (lastId != null)
            {
                int lastNumber = int.Parse(lastId.Replace("CP", ""));
                int nextNumber = lastNumber + 1;
                return $"CP{nextNumber:0000000}";
            }
            else
            {
                return "CP0000001";
            }
        }
    }
}
