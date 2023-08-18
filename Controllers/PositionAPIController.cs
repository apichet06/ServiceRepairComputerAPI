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
    public class PositionAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private IMapper _mapper;
        private ResponseDto _response;
        private MessageDto _message;


        public PositionAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new  ResponseDto();
            _message = new MessageDto();
        }
         

        [HttpGet]
        [Route("")]
        public async Task<ResponseDto> Get(string DV_ID=null!)
        {
            try
            {
        

                IEnumerable<Position> objList = await _db.Positions.ToListAsync();
                List<PositionDto> mappedList = _mapper.Map<List<PositionDto>>(objList);
        
                foreach (var positionDto in mappedList)
                {
                    // Fetch department and position names and set them in the DTO
                    var department = await _db.Divisions.FirstOrDefaultAsync(d => d.DV_ID == positionDto.DV_ID);
                    if (department != null)
                    {
                        positionDto.DV_Name = department.DV_Name;
                    } 
                }

                if (!string.IsNullOrEmpty(DV_ID))
                {
                    objList = await _db.Positions.Where(c => c.DV_ID == DV_ID).ToListAsync();
                }
                else
                {
                    objList = await _db.Positions.ToListAsync();
                }

                _response.Result = _mapper.Map<IEnumerable<PositionDto>>(mappedList);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = _message.an_error_occurred + ex.Message;
            }
             
            return _response;
        }
        [HttpPost]
        public async Task<ResponseDto> Post([FromBody] Position position)
        {
            try
            {
                if (_db.Positions.Any(p=>p.P_Name == position.P_Name))
                {
                    _response.IsSuccess=false;
                    _response.Message = _message.already_exists;
                    return _response;
                }
  
                Position obj = _mapper.Map<Position>(position);
                string NextId = await GenarateID();
                 
                obj.P_ID = NextId; 
                _db.Positions.Add(obj);
               await _db.SaveChangesAsync();
  
                _response.Result  = _mapper.Map<PositionDto>(position);
                _response.Message = _message.InsertMessage;


            } catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = _message.an_error_occurred + ex.Message;
            }
            return _response;
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ResponseDto> Put(int id, [FromBody] Position position)
        {
            try
            {
                Position? obj = await _db.Positions.FirstOrDefaultAsync(c => c.Id == id)!;

                if (obj == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.already_exists;
                    return _response;
                }
                obj.P_Name = position.P_Name;
                obj.DV_ID = position.DV_ID;
               await _db.SaveChangesAsync();

                _response.Result = _mapper.Map<PositionDto>(obj);
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
            try { 
                Position? obj = await _db.Positions.FirstOrDefaultAsync(c=>c.Id == id);
                if(obj == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.Not_found;
                    return _response;
                }

                _db.Positions.Remove(obj);
               await _db.SaveChangesAsync();
                _response.Message= _message.DeleteMessage;
                _response.Result = _mapper.Map<PositionDto>(obj);

            }catch  (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = _message.an_error_occurred + ex.Message;
            }
            return _response;
        }
        private async Task<string> GenarateID()
        {
            string? lastId = await _db.Positions.OrderByDescending(p => p.Id).Select(x=> x.P_ID).FirstOrDefaultAsync();
             
            if (!string.IsNullOrEmpty(lastId))
            {
                int num = int.Parse(lastId.Substring(1)) + 1;
                return "P" + num.ToString("D3");
            }

            return "P001";


        }


    }
}
