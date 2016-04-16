namespace Stump.Server.AuthServer.Database
{
    public class AccountRelator
    {
        public static string FetchQuery = "SELECT * FROM accounts LEFT JOIN worlds_characters ON worlds_characters.AccountId = accounts.Id";
        public static string FindAccountById = "SELECT * FROM accounts LEFT JOIN worlds_characters ON worlds_characters.AccountId = accounts.Id WHERE accounts.Id = {0}";
        public static string FindAccountByLogin = "SELECT * FROM accounts LEFT JOIN worlds_characters ON worlds_characters.AccountId = accounts.Id WHERE accounts.Login = @0";
        public static string FindAccountByNickname = "SELECT * FROM accounts LEFT JOIN worlds_characters ON worlds_characters.AccountId = accounts.Id WHERE accounts.Nickname = {0}";
        private Account m_current;

        public Account Map(Account account, WorldCharacter character)
        {
            Account result;
            if (account == null)
            {
                result = this.m_current;
            }
            else
            {
                if (this.m_current != null && this.m_current.Id == account.Id)
                {
                    this.m_current.WorldCharacters.Add(character);
                    result = null;
                }
                else
                {
                    Account current = this.m_current;
                    this.m_current = account;
                    this.m_current.WorldCharacters.Add(character);
                    result = current;
                }
            }
            return result;
        }
    }
}