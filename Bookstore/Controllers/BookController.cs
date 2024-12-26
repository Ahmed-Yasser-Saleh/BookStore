using Bookstore.DTO;
using Bookstore.DTO.Author;
using Bookstore.DTO.Book;
using Bookstore.DTO.Catalog;
using Bookstore.Model;
using Bookstore.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static System.Reflection.Metadata.BlobBuilder;

namespace Bookstore.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Books")]
    public class BookController : ControllerBase
    {
        UnitOfwork db;
        public BookController(UnitOfwork db)
        {
            this.db = db;
        }
        [HttpGet]
        [Authorize(Roles = "Customer,Admin")]
        [SwaggerOperation(Summary = "select all books ")]
        [SwaggerResponse(200, "return all books", typeof(List<BookDTO>))]
        public IActionResult GetAll()
        {
            var books = db.bookrepository.Selectall();
            List<BookDTO> booksdto = new List<BookDTO>();
            foreach (var book in books)
            {
                var bk = new BookDTO();
                bk.Id = book.Id;
                bk.Title = book.Title;
                bk.AuthorName = book.author.FullName;
                bk.CatalogName = book.catalog.Name;
                bk.Price = book.Price;
                bk.Stock = book.Stock;
                bk.PublishDate = book.PublishDate;
                booksdto.Add(bk);
            }
            if (booksdto.Count == 0)
                return NotFound(new { Status = 404, ErrorMassege = "No books Found" });
            return Ok(booksdto);
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Customer,Admin")]
        [SwaggerOperation(Summary = "Get book by book id ")]
        [SwaggerResponse(200, "return book data", typeof(BookDTO))]
        [SwaggerResponse(404, "if no book founded")]
        public IActionResult Getbyid(int id) {
        var book = db.bookrepository.GetById(id);
        if(book == null) return NotFound(new { Status = 404, ErrorMassege = "No book Found" });
            var bk = new BookDTO()
            {
                Id = book.Id,
                Title = book.Title,
                AuthorName = book.author.FullName,
                CatalogName = book.catalog.Name,
                Price = book.Price,
                Stock = book.Stock,
                PublishDate = book.PublishDate
            };
            return Ok(bk);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [SwaggerResponse(200, "Book created",typeof(Book))]
        [SwaggerResponse(400, "Catalog or author not found or not valid data")]
        [Consumes("application/json")]
        [SwaggerOperation(
            Summary = "Create book",
            Description = "Create book on bookstore",
            Tags = new[] {"Admin Operations"}
            )
            ]
        public IActionResult Add(AddBookDTO bk)
        {
            if (bk == null)
                return BadRequest(new { Status = 400, ErrorMassege = "Empty input" });
            var CatalogExists = db.catalogrepository.CheckId(bk.CatalogId);
            if (!CatalogExists)
            {
                return BadRequest(new { Status = 400, ErrorMassege = "Catalog not Found" });
            }
            var AuthorExists = db.catalogrepository.CheckId(bk.CatalogId);
            if (!AuthorExists)
            {
                return BadRequest(new { Status = 400, ErrorMassege = "Author not Found" });
            }
            if (ModelState.IsValid)
            {
                if (!db.bookrepository.Checkname(bk.Title))
                {
                    var pd = new Book()
                    {
                        Title = bk.Title,
                        Price = bk.Price,
                        AuthorId = bk.AuthorId,
                        CatalogId = bk.CatalogId,
                        Stock = bk.Stock,
                        PublishDate = bk.PublishDate
                    };
                    db.bookrepository.Add(pd);
                    db.Save();
                    return Ok(new { Status = "Creation Successful" });
                }
                else
                    return BadRequest(new { Status = 404, ErrorMassege = $"There is book with title: {bk.Title}" });
            }
            else
                return BadRequest(ModelState);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Update book", Tags = new[] { "Admin Operations" })]
        [SwaggerResponse(204, "book updated succcesfully")]
        [SwaggerResponse(400, "invalid book data")]
        public IActionResult Edit(int id, EditBookDTO bk)
        {
            if (bk == null)
                return BadRequest(new { Status = 400, ErrorMassege = "Empty Input" });
            if(bk.Id != id)
                return BadRequest(new { Status = 400, ErrorMassege = "Not the same Id" });
            var CatalogExists = db.catalogrepository.CheckId(bk.CatalogId);
            if (!CatalogExists)
            {
                return BadRequest(new { Status = 400, ErrorMassege = "Catalog not Found" });
            }
            var AuthorExists = db.catalogrepository.CheckId(bk.CatalogId);
            if (!AuthorExists)
            {
                return BadRequest(new { Status = 400, ErrorMassege = "Author not Found" });
            }
            if (ModelState.IsValid)
            {
                var pd = new Book()
                {
                    Id = id,
                    Title = bk.Title,
                    Price = bk.Price,
                    AuthorId = bk.AuthorId,
                    CatalogId = bk.CatalogId,
                    Stock = bk.Stock,
                    PublishDate = bk.PublishDate
                };
                db.bookrepository.Edit(pd);
                db.Save();
                return Ok();
            }
            else return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete book", Tags = new[] { "Admin Operations" })]
        [SwaggerResponse(200, "if book deleted succcesfully")]
        public IActionResult Delete(int id)
        {
            var book = db.bookrepository.GetById(id);
            if (book == null) return NotFound(new { Status = 404, ErrorMassege = "Book not Found" });
            db.bookrepository.Delete(book);
            db.Save();
            return Ok();
        }

        [HttpGet("Search/{Name}")]
        [Authorize(Roles = "Customer,Admin")]
        [SwaggerOperation(Summary = "Search for Books.")]
        [SwaggerResponse(200, "Returns a list of Books.", typeof(List<AuthorDTO>))]
        [SwaggerResponse(404, "No Books found with This Name.")]
        public IActionResult Search(string Name)
        {
            var Books = db.bookrepository.Search(Name);
            List<BookDTO> booksdto = new List<BookDTO>();
            foreach (var book in Books)
            {
                var bk = new BookDTO();
                bk.Id = book.Id;
                bk.Title = book.Title;
                bk.AuthorName = book.author.FullName;
                bk.CatalogName = book.catalog.Name;
                bk.Price = book.Price;
                bk.Stock = book.Stock;
                bk.PublishDate = book.PublishDate;
                booksdto.Add(bk);
            }
            if (booksdto.Count == 0)
                return NotFound(new { Status = 404, ErrorMassege = "Books Not Found" });
            return Ok(booksdto);
        }
    }
}
