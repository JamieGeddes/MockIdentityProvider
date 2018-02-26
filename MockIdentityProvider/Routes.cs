namespace MockIdentityProvider
{
    public class Routes
    {
        public Routes()
        {
            AuthorizeRoute = "/connect/authorize";
            TokenRoute = "/connect/token";
            UserInfoRoute = "/connect/userinfo";
        }

        public string AuthorizeRoute { get; set; }
        public string TokenRoute { get; set; }
        public string UserInfoRoute { get; set; }
    }
}
