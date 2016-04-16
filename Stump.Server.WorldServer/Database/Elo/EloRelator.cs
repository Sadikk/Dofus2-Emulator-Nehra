namespace Stump.Server.WorldServer.Database.Elo
{
    public class EloRelator
    {
        public static string FetchQuery = "SELECT * FROM elo_array ORDER BY Probability DESC";
    }
}
