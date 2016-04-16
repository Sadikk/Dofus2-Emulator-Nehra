namespace Stump.Server.AuthServer.Database
{
    public class IpBanRelator
    {
        public static string FetchQuery = "SELECT * FROM ipbans";
        public static string FindByIP = "SELECT * FROM ipbans WHERE IPAsString={0}";
    }
}