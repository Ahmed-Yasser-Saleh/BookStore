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

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Catalogs")]
    public class CatalogController : ControllerBase
    {
        UnitOfwork db;
        public CatalogController(UnitOfwork db)
        {
            this.db = db;
        }
        [HttpGet]
        [Authorize(Roles = "Customer,Admin")]
        [SwaggerOperation(Summary = "select all Catalogs ")]
        [SwaggerResponse(200, "return all Catalogs", typeof(List<GetCatalogDTO>))]
        [SwaggerResponse(404, "There are no Catalogs to display")]
        public IActionResult GetAll()
        {
            var catalogs = db.catalogrepository.Selectall();
            List<GetCatalogDTO> catalogsdto = new List<GetCatalogDTO>();
            foreach (var catalog in catalogs)
            {
                var ctg = new GetCatalogDTO();
                ctg.Id = catalog.Id;
                ctg.Name = catalog.Name;
                ctg.Description = catalog.Description;
                catalogsdto.Add(ctg);
            }
            if (catalogsdto.Count == 0)
                return NotFound(new { Status = 404, ErrorMassege = "Catalogs Not Found" });
            return Ok(catalogsdto);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Customer,Admin")]
        [SwaggerOperation(Summary = "Get catalog by id ")]
        [SwaggerResponse(200, "return Catalog data", typeof(GetCatalogDTO))]
        [SwaggerResponse(404, "no Catalog founded")]
        public IActionResult Getbyid(int id)
        {
            var catalog = db.catalogrepository.GetById(id);
            if (catalog == null) return NotFound(new { Status = 404, ErrorMassege = "Catalog Not Found" });
            var ctg = new GetCatalogDTO()
            {
               Id = catalog.Id,
               Name = catalog.Name,
               Description = catalog.Description,
            };
            return Ok(ctg);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Add Catalog", Tags = new[] { "Admin Operations" })]
        [SwaggerResponse(200, "Catalog was created successfully.")]
        [SwaggerResponse(400, "Invalid input data.")]
        public IActionResult Add(AddcatalogDTO ctg)
        {
            if (ctg == null)
                return BadRequest(new { Status = 400, ErrorMassege = "Empty Input" });
            if (ModelState.IsValid)
            {
                if (!db.catalogrepository.Checkname(ctg.Name))
                {
                    var catalog = new Catalog
                    {
                        Name = ctg.Name,
                        Description = ctg.Description,
                    };
                    db.catalogrepository.Add(catalog);
                    db.Save();
                    return Ok(new { Status = "Creation Successful" });
                }
                else
                    return BadRequest(new { Status = 400, ErrorMassege = $"There is catalog with name : {ctg.Name}" });
            }
            else
                return BadRequest(ModelState);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Update Catalog", Tags = new[] { "Admin Operations" })]
        [SwaggerResponse(204, "Catalog updated succcesfully")]
        [SwaggerResponse(400, "invalid Catalog data")]
        public IActionResult Edit(int id, EditCatalogDTO ctg)
        {
            if (ctg == null)
                return BadRequest(new { Status = 400, ErrorMassege = "Empty Input" });
            if (ctg.id != id)
                return BadRequest(new { Status = 400, ErrorMassege = "Not the same id" });
            if (ModelState.IsValid)
            {
                var catalog = new Catalog
                {
                    Id = id,
                    Name = ctg.Name,
                    Description = ctg.Description,
                };
                db.catalogrepository.Edit(catalog);
                db.Save();
                return Ok();
            }
            else return BadRequest(ModelState);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete Catalog", Tags = new[] { "Admin Operations" })]
        [SwaggerResponse(200, "Catalog deleted succcesfully")]
        public IActionResult Delete(int id)
        {
            var catalog = db.catalogrepository.GetById(id);
            if (catalog == null) return NotFound(new { Status = 404, ErrorMassege = "Catalog Not Found" });
            db.catalogrepository.Delete(catalog);
            db.Save();
            return Ok();
        }

        [HttpGet("Search/{Name}")]
        [Authorize(Roles = "Customer,Admin")]
        [SwaggerOperation(Summary = "Search for Catalogs.")]
        [SwaggerResponse(200, "Returns a list of Catalogs.", typeof(List<GetCatalogDTO>))]
        [SwaggerResponse(404, "No Catalogs found with This Name.")]
        public IActionResult Search(string Name)
        {
            var catalogs = db.catalogrepository.Search(Name);
            List<GetCatalogDTO> catalogsdto = new List<GetCatalogDTO>();
            foreach (var catalog in catalogs)
            {
                var ctg = new GetCatalogDTO();
                ctg.Id = catalog.Id;
                ctg.Name = catalog.Name;
                ctg.Description = catalog.Description;
                catalogsdto.Add(ctg);
            }
            if (catalogsdto.Count == 0)
                return NotFound(new { Status = 404, ErrorMassege = "Catalog Not Found" });
            return Ok(catalogsdto);
        }
    }
}
