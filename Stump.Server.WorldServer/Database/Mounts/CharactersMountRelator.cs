namespace Database.Mounts
{
    class CharactersMountRelator
    {
        public static string FetchQuery = "SELECT * FROM characters_mount";
        public static string FetchByOwner = "SELECT * FROM characters_mount WHERE OwnerId = {0}";
    }
}
