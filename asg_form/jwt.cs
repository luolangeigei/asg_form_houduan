namespace asg_form
{
    public class JWTOptions
    {
        public string SigningKey { get; set; }
        public int ExpireSeconds { get; set; }
    }
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class LoginRequest_2
    {
        public string UserEmail { get; set; }
        public string Password { get; set; }
    }
}
