namespace Bookstore.Email
{
    public class EmailStringBody
    {
        public static string SendActiveEmail(string email, string token, string component, string message)
        {
            string encodetoken = Uri.EscapeDataString(token);

            // Example: build a clickable link with the token and component
            string link = $"https://bookstoreproject.runasp.net/api/Account/{component}?email={email}&token={encodetoken}";

            string body = $@"
            <html>
                <body style='font-family: Arial, sans-serif; line-height:1.6'>
                    <h2>{message}</h2>
                    <p>Please confirm your action by clicking the link below:</p>
                    <p>
                        <a href='{link}' 
                           style='display:inline-block; padding:10px 15px; background:#4CAF50; color:white; text-decoration:none; border-radius:5px;'>
                           Verify Email
                        </a>
                    </p>
                </body>
            </html>";

            return body;
        }
        public static string SendForgotPassword(string email, string token, string component, string message)
        {
            string encodetoken = Uri.EscapeDataString(token);

            // link to frontend with new password then go to api reset pw with data
            string link = $"https://bookstoreproject.runasp.net/Reset-Password.html?email={email}&token={encodetoken}";

            string body = $@"
            <html>
                <body style='font-family: Arial, sans-serif; line-height:1.6'>
                    <h2>{message}</h2>
                    <p>
                        <a href='{link}' 
                           style='display:inline-block; padding:10px 15px; background:#4CAF50; color:white; text-decoration:none; border-radius:5px;'>
                           Reset Password
                        </a>
                    </p>
                 
                </body>
            </html>";

            return body;
        }
    }
}
