using AutoMapper;
using Bookstore.DTO;
using Bookstore.DTO.Author;
using Bookstore.Model;
using Bookstore.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Authors")]
    public class AuthorController : ControllerBase
    {
        UnitOfwork db;
        IMapper _mapper;
        public AuthorController(UnitOfwork db, IMapper _mapper)
        {
            this.db = db;
            this._mapper = _mapper;

        }
        [HttpGet]
        [Authorize(Roles = "Customer,Admin")]
        public IActionResult GetAll()
        {
            var Authors = db.authorrepository.Selectall();
            List<AuthorDTO> authorsdto = new List<AuthorDTO>();
            foreach (var Author in Authors)
            {
                var newauthor = _mapper.Map<AuthorDTO>(Author);
                authorsdto.Add(newauthor);
            }
            if (authorsdto.Count == 0)
                return NotFound();
            return Ok(authorsdto);
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Customer,Admin")]
        public IActionResult Getbyid(int id)
        {
            var Author = db.authorrepository.GetById(id);
            if (Author == null) return NotFound();
            var author = _mapper.Map<AuthorDTO>(Author);
            return Ok(author);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(
            Summary = "Create Author",
            Tags = new[] { "Admin Operations" }
            )
            ]
        public IActionResult Add(AddAuthorDTO ath)
        {
            if (ath == null)
                return BadRequest();
            if(ModelState.IsValid)
            {
                var author = _mapper.Map<Author>(ath);
                db.authorrepository.Add(author);
                db.Save();
                return Created();
            }
            else
                return BadRequest(ModelState);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Update Author", Tags = new[] { "Admin Operations" })]
        public IActionResult Edit(int id, EditAuthorDTO ath)
        {
            if (ath == null)
                return BadRequest();
            if(id != ath.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                var author = _mapper.Map<Author>(ath);
                db.authorrepository.Edit(author);
                db.Save();
                return Ok();
            }
            else return BadRequest(ModelState);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete Author", Tags = new[] { "Admin Operations" })]
        public IActionResult Delete(int id)
        {
            var author = db.authorrepository.GetById(id);
            if (author == null) return NotFound();
            db.authorrepository.Delete(author);
            db.Save();
            return Ok();
        }
    }
}
