namespace Bookstore.DTO.Account
{
    public class ChangePasswordDTO
    {
        public string username {  get; set; }
        public string oldpassword { get; set; }
        public string newpassword { get; set; }
        public string confirmpassword { get; set; }
    }
}
