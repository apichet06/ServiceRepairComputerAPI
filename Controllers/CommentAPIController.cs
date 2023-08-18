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
    public class CommentAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private MessageDto _message;
        private IMapper _mapper;

        public  CommentAPIController(AppDbContext db, IMapper mapper)
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
                IEnumerable<Comment> objList = await _db.Comments.ToListAsync();
                _response.Result = _mapper.Map<IEnumerable<CommentDto>>(objList);
  
            }catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = _message.an_error_occurred + ex.Message;

            }

            return _response;
        }

        [HttpPost]
        public async Task<ResponseDto> Post([FromBody] Comment comment) {

            try
            {
                Comment obj = _mapper.Map<Comment>(comment);
                 
                string NextID = await GenerateAutoId();
                obj.CM_ID = NextID; 
                _db.Comments.Add(obj);
               await _db.SaveChangesAsync();
                 
                _response.Result = _mapper.Map<CommentDto>(obj); 
                _response.Message= _message.InsertMessage;

            }catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = _message.an_error_occurred + ex.Message;
            }
             
            return _response;
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ResponseDto> Put(int id, Comment comment)
        {
            try
            {
                Comment? obj = await _db.Comments.FirstOrDefaultAsync(c=>c.Id == id)!;

                if (obj == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.Not_found;
                    return _response;
                }

                obj.Content = comment.Content;
                obj.TechnicianId = comment.TechnicianId;
                obj.I_ID = comment.I_ID;

                _db.Comments.Update(obj);
                await _db.SaveChangesAsync();

                _response.Result = _mapper.Map<CommentDto>(obj);
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

        public async Task<ResponseDto> Delete(int id)
        {
            try { 
              Comment? obj = await _db.Comments.FirstOrDefaultAsync(x=>x.Id == id);
                if(obj == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = _message.Not_found;
                    return _response;
                }
            
                _db.Comments.Remove(obj);
               await  _db.SaveChangesAsync();
            
            
            }catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = _message.an_error_occurred + ex.Message;
            }
            return _response;
        }

        private async Task<string> GenerateAutoId()
        {
            string? lastId = await _db.Comments.OrderByDescending(x => x.Id).Select(x => x.CM_ID).FirstOrDefaultAsync()!;

            if (lastId != null && lastId.StartsWith("CM"))
            {
                int num = int.Parse(lastId.Substring(2)) + 1;
                return "CM" + num.ToString("D7");
            }

            return "CM0000001"; // ถ้ายังไม่มีข้อมูลในฐานข้อมูล
        }

    }
}
