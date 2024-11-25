using AutoMapper;
using Bookstore.DTO.Author;
using Bookstore.DTO.Book;
using Bookstore.Model;

namespace Bookstore.Mapping
{
    public class AuthorMabber : Profile
    {
        public AuthorMabber()
        {
            AddUserMapping();
        }
        public void AddUserMapping()
        {
            CreateMap<Author, AuthorDTO>();
            CreateMap<AddAuthorDTO, Author>();
            CreateMap<EditAuthorDTO, Author>();
        }
    }
}
