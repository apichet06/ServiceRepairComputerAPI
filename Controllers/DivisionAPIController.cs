using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceRepairComputer.Data;
using ServiceRepairComputer.Models;
using ServiceRepairComputer.Models.Dto;

namespace ServiceRepairComputer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DivisionAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private IMapper _mapper;
        private ResponseDto _response;
        private MessageDto _message;

        public DivisionAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
            _message = new MessageDto();
        }

        [HttpGet]
        public async Task <ResponseDto> Get()
        {
            try
            {

                IEnumerable<Division> objList = await _db.Divisions.ToListAsync();
               _response.Result = _mapper.Map<IEnumerable<DivisionDto>>(objList);
                 

            }catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = _message.an_error_occurred + ex.Message;
            }
            return _response;
        }
         

        [HttpPost]
        public async Task <ResponseDto> Post([FromBody] Division division) {

            try
            {
                if (await _db.Divisions.AnyAsync(d => d.DV_Name == division.DV_Name))
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.already_exists + division.DV_Name;
                    return _response;
                }

                Division obj = _mapper.Map<Division>(division);

                string NextID = await GenerateAutoId();
                
                obj.DV_ID = NextID;
                _db.Divisions.Add(obj);
                await _db.SaveChangesAsync();
                
                _response.Result = _mapper.Map<DivisionDto>(obj);
                _response.Message = _message.InsertMessage;

            }catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message =_message.an_error_occurred + ex.Message;
            }
            return _response;
         
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task <ResponseDto> Put(int id , [FromBody] Division division)
        {
            try
            {
               Division? obj = await _db.Divisions.FirstOrDefaultAsync(c => c.Id == id);
                if (obj == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.Not_found;
                    return _response;
                }

                // เช็คว่ามี Division ที่มีชื่อซ้ำกันหรือไม่ (ไม่รวม Division ที่กำลังแก้ไข)
                if (await _db.Divisions.AnyAsync(d => d.Id != id && d.DV_Name == division.DV_Name))
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.already_exists + division.DV_Name;
                    return _response;
                }

                 obj!.DV_Name = division.DV_Name; 
                _db.Divisions.Update(obj);
                await _db.SaveChangesAsync();

                _response.Result = _mapper.Map<DivisionDto>(obj);
                _response.Message = _message.UpdateMessage;

            }catch (Exception ex)
            {
                _response.IsSuccess = false; 
                _response.Message =_message.an_error_occurred + ex.Message;
            }
            return _response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task <ResponseDto> Delete(int id)
        {
            try
            {
                Division? obj =await _db.Divisions.FirstOrDefaultAsync(_ => _.Id == id)!;

                if (obj == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.Not_found;
                    return _response;
                }
                _db.Divisions.Remove(obj);
               await _db.SaveChangesAsync();

                _response.Result = _mapper.Map<DivisionDto>(obj);
                _response.Message= _message.DeleteMessage;

            }catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message =_message.an_error_occurred + ex.Message;
            }

            return _response;
        }

        private async Task <string> GenerateAutoId()
        {
            string? lastId = await _db.Divisions.OrderByDescending(x => x.Id).Select(x => x.DV_ID).FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(lastId))
            {
                int num = int.Parse(lastId.Substring(2)) + 1;
                return "DV" + num.ToString("D3");
            }

            return "DV001"; // ถ้ายังไม่มีข้อมูลในฐานข้อมูล
        }

    }
}
