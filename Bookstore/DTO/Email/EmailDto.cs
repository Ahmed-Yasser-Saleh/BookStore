namespace Bookstore.DTO.Email
{
    public class EmailDto
    {
        public EmailDto(string to, string from, string subject, string content)
        {
            this.To = to;
            this.from = from;   
            this.subject = subject;
            this.content = content;
        }
        public string To { get; set; }
        public string from { get; set; }
        public string subject { get; set; }
        public string content { get; set; }
    }
}
