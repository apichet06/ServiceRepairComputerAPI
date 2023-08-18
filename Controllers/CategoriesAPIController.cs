using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using ServiceRepairComputer.Data;
using ServiceRepairComputer.Models;
using ServiceRepairComputer.Models.Dto;

namespace ServiceRepairComputer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private ResponseDto _response;
        private MessageDto _message;

        public CategoriesAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
            _message = new MessageDto();
        }

        [HttpGet]
        public async Task<ResponseDto> Get()
        {
            try
            {
                IEnumerable<Categories> objList =await _db.Categories.ToListAsync();
                 _response.Result =  _mapper.Map<IEnumerable<Categories>>(objList);
 

            }catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = _message.an_error_occurred + ex.Message;

            }
            return _response;
        }

        [HttpPost]
        public async Task<ResponseDto> Post([FromBody] Categories categories) {
            try
            {
                if (await _db.Categories.AnyAsync(c => c.Name == categories.Name))
                {
                    _response.IsSuccess=false;
                    _response.Message = _message.already_exists + categories.Name;
                    return _response;
                }

                Categories obj = _mapper.Map<Categories>(categories);
                string nexID = await GenerateAutoId();
                obj.C_ID = nexID;
                _db.Categories.Add(obj);
                await _db.SaveChangesAsync();

                _response.Result = _mapper.Map<CategoriesDto>(obj); 
                _response.Message = _message.InsertMessage;
 
            }catch (Exception ex)
            {
                _response.IsSuccess=false;
                _response.Message = _message.an_error_occurred + ex.Message;
            }

            return _response;
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ResponseDto> Put(int id ,Categories categories)
        {
            try
            {
               
                Categories?  obj = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
                if (obj == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.Not_found;
                    return _response;
                }

                if (await _db.Categories.AnyAsync(c =>c.Id != id && c.Name == categories.Name))
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.already_exists + categories.Name;
                    return _response;
                }

                obj.Name = categories.Name; 
                _db.Categories.Update(obj);
                await _db.SaveChangesAsync();

                _response.Result = _mapper.Map<CategoriesDto>(obj);
                _response.Message = _message.UpdateMessage;

            }catch (Exception ex)
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
                Categories? obj = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
                if (obj == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.Not_found;
                    return _response;
                }
                 


                _db.Categories.Remove(obj);
                await _db.SaveChangesAsync();
                _response.Result = _mapper.Map<CategoriesDto>(obj);
                _response.Message = _message.DeleteMessage;

            }catch (Exception ex)
            {
                _response.IsSuccess=false;
                _response.Message = _message.an_error_occurred + ex.Message;
            }

            return _response;
        
        }
         
    
        private async Task<string> GenerateAutoId()
        {
            string? lastId = await _db.Categories.OrderByDescending(x => x.Id).Select(x => x.C_ID).FirstOrDefaultAsync();

            if (lastId != null && lastId.StartsWith("C"))
            {
                int num = int.Parse(lastId.Substring(1)) + 1;
                return "C" + num.ToString("D3");
            }

            return "C001"; // ถ้ายังไม่มีข้อมูลในฐานข้อมูล
        }
         
    }
}
