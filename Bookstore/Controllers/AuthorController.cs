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
        [SwaggerOperation(Summary = "Retrieves a list of all authors.")]
        [SwaggerResponse(200, "Returns a list of authors.", typeof(List<AuthorDTO>))]
        [SwaggerResponse(404, "No authors found.")]
        public IActionResult GetAll()
        {
            var Authors = db.authorRepository.Selectall();
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
        [SwaggerOperation(Summary = "Retrieves an author by their ID.")]
        [SwaggerResponse(200, "Returns the author details.", typeof(AuthorDTO))]
        [SwaggerResponse(404, "Author not found.")]
        public IActionResult Getbyid(int id)
        {
            var Author = db.authorRepository.GetById(id);
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
        [SwaggerResponse(201, "Author was created successfully.")]
        [SwaggerResponse(400, "Invalid input data.")]
        public IActionResult Add(AddAuthorDTO ath)
        {
            if (ath == null)
                return BadRequest();
            if(ModelState.IsValid)
            {
                var author = _mapper.Map<Author>(ath);
                db.authorRepository.Add(author);
                db.Save();
                return Created();
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Update Author", Tags = new[] { "Admin Operations" })]
        [SwaggerResponse(200, "The author was updated successfully.")]
        [SwaggerResponse(400, "Invalid input or mismatch between ID and author data.")]
        [SwaggerResponse(404, "Author not found.")]
        public IActionResult Edit(int id, EditAuthorDTO ath)
        {
            if (ath == null)
                return BadRequest();
            if(id != ath.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                var author = _mapper.Map<Author>(ath);
                db.authorRepository.Edit(author);
                db.Save();
                return Ok();
            }
            else return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete Author", Tags = new[] { "Admin Operations" })]
        [SwaggerResponse(200, "Author was deleted successfully.")]
        [SwaggerResponse(404, "Author not found.")]
        public IActionResult Delete(int id)
        {
            var author = db.authorRepository.GetById(id);
            if (author == null) return NotFound();
            db.authorRepository.Delete(author);
            db.Save();
            return Ok();
        }

        [HttpGet("Search/{Name}")]
        [Authorize(Roles = "Customer,Admin")]
        [SwaggerOperation(Summary = "Search for authors.")]
        [SwaggerResponse(200, "Returns a list of authors.", typeof(List<AuthorDTO>))]
        [SwaggerResponse(404, "No authors found with This Name.")]
        public IActionResult Search(string Name)
        {
            var authors = db.authorRepository.Search(Name);
            List<AuthorDTO> authorsdto = new List<AuthorDTO>();
            foreach (var Author in authors)
            {
                var newauthor = _mapper.Map<AuthorDTO>(Author);
                authorsdto.Add(newauthor);
            }
            if (authorsdto.Count == 0)
                return NotFound("There are no authors with this Name");
            return Ok(authorsdto);
        }
    }
}
