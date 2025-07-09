using Bookstore.DTO;
using Bookstore.DTO.Author;
using Bookstore.DTO.Book;
using Bookstore.DTO.Catalog;
using Bookstore.Model;
using Bookstore.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Swashbuckle.AspNetCore.Annotations;
using static System.Net.Mime.MediaTypeNames;
using static System.Reflection.Metadata.BlobBuilder;

namespace Bookstore.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Books")]
    public class BookController : ControllerBase
    {
        UnitOfwork db;
        // private readonly IFileProvider fileProvider;
        public BookController(UnitOfwork db)
        {
            // this.fileProvider = fileProvider;
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
                bk.image = book.Image;
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
        public IActionResult Getbyid(int id)
        {
            var book = db.bookrepository.GetById(id);
            if (book == null) return NotFound(new { Status = 404, ErrorMassege = "No book Found" });
            var bk = new BookDTO()
            {
                Id = book.Id,
                Title = book.Title,
                AuthorName = book.author.FullName,
                CatalogName = book.catalog.Name,
                Price = book.Price,
                Stock = book.Stock,
                image = book.Image,
                PublishDate = book.PublishDate
            };
            return Ok(bk);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [SwaggerResponse(200, "Book created", typeof(Book))]
        [SwaggerResponse(400, "Catalog or author not found or not valid data")]
        [SwaggerOperation(
            Summary = "Create book",
            Description = "Create book on bookstore",
            Tags = new[] { "Admin Operations" }
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
                        Image = AddImage(bk.Image),
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
        public IActionResult Edit(int id, [FromForm] EditBookDTO bk)
        {
            if (bk == null)
                return BadRequest(new { Status = 400, ErrorMassege = "Empty Input" });
            if (bk.Id != id)
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
                    Image = AddImage(bk.Image),
                    PublishDate = bk.PublishDate
                };
                db.bookrepository.Edit(pd);
                db.Save();
                return Ok();
            }
            else return BadRequest(ModelState);
        }

        [HttpPost("addimage")]
        public IActionResult addimage(int id, IFormFile photo)
        {
            var pd = db.bookrepository.GetById(id);
            if (pd == null)
                return NotFound();
            pd.Image = AddImage(photo);
            db.Save();
            return Ok();
        }

        private string AddImage(IFormFile? photo) {
            if (photo!= null)
            {
                string folderName = "images";
                string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName);

                if (!Directory.Exists(wwwrootPath))
                {
                    Directory.CreateDirectory(wwwrootPath);
                }
                string fileName = photo.FileName;
                string filePath = Path.Combine(wwwrootPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }
                string path = Path.Combine(folderName, fileName).Replace("\\", "/");
                return path;
            }

            return null;
        }

        [HttpGet("GetImage/{id}")]
        public IActionResult GetImage(int id)
        {
            var bk = db.bookrepository.GetById(id);
            if (bk == null)
                return BadRequest($"There is no Book with id: {id}");
            string? filePath = null;
            if (bk.Image != null)
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", bk.Image.Replace("/", Path.DirectorySeparatorChar.ToString()));
                byte[] imageBytes; // Read the file with a FileStream to avoid locking issues
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    imageBytes = new byte[stream.Length];
                    stream.Read(imageBytes, 0, imageBytes.Length);
                }
                return File(imageBytes, "image/png");
            }
            else
            {
                return BadRequest($"There is no Image for Book with id: {id}");
            }
            
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
        [SwaggerResponse(200, "Returns a list of Books.", typeof(List<BookDTO>))]
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
                bk.image = book.Image;
                bk.PublishDate = book.PublishDate;
                booksdto.Add(bk);
            }
            if (booksdto.Count == 0)
                return NotFound(new { Status = 404, ErrorMassege = "Books Not Found" });
            return Ok(booksdto);
        }

        [HttpPost("RateBook")]
        public IActionResult Ratebook(int id, Rating Rate)
        {
            var book = db.bookrepository.GetById(id);
            if (book == null) return NotFound(new { Status = 404, ErrorMassege = $"No book Found" });
            book.rate = Rate;
            db.Save();
            return Ok();
        }

        [HttpPost("AddFavourite")]
        public IActionResult AddFavourite(int id)
        {
            var book = db.bookrepository.GetById(id);
            if (book == null) return NotFound(new { Status = 404, ErrorMassege = $"No book Found" });
            book.Isfavourite = true;
            db.Save();
            return Ok();
        }

        [HttpGet("LovelyBooks")]
        public IActionResult GetLovelyBooks() {
            var books = db.bookrepository.LovelyBooks();
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
                bk.image = book.Image;
                bk.PublishDate = book.PublishDate;
                booksdto.Add(bk);
            }
            if (booksdto.Count == 0)
                return NotFound(new { Status = 404, ErrorMassege = "No books added to favourite list" });
            return Ok(booksdto);
        }

        [HttpPut("Remove/FavoriteBook/{id}")]
        public IActionResult RemoveLovelyBook(int id)
        {
            var book = db.bookrepository.GetById(id);
            if (book == null) return NotFound(new { Status = 404, ErrorMassege = "Book not Found" });
            book.Isfavourite = false;
            db.bookrepository.Edit(book);
            db.Save();
            return Ok();
        }
    }
}
