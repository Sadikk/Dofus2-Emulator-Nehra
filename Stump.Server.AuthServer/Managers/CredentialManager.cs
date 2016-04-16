using Stump.Core.Extensions;
using Stump.Core.IO;
using Stump.Core.Reflection;
using Stump.Server.AuthServer.Database;
using Stump.Server.AuthServer.Network;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stump.Server.AuthServer.Managers
{
    public class CredentialManager : Singleton<CredentialManager>
    {
        private readonly byte[] m_rawData;

        public CredentialManager()
        {
            this.m_rawData = File.ReadAllBytes("stump_patch.swf");
        }

        public byte[] RawData { get { return this.m_rawData; } }

        public bool DecryptCredentials(out Account account, AuthClient client, IEnumerable<byte> credentials)
        {
            try
            {
                account = null;

                string login;
                string password;

                using (var reader = new BigEndianReader(credentials.ToArray()))
                {
                    login = reader.ReadUTF();
                    password = reader.ReadUTF();
                    client.AesKey = reader.ReadBytes(32);
                }

                account = AccountManager.Instance.FindAccountByLogin(login);

                if (account != null)
                {
                    return account.PasswordHash == password.GetMD5();
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                account = null;
                return false;
            }
        }
    }
}
