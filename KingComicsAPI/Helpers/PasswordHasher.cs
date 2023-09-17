using System.Security.Cryptography;

namespace KingComicsAPI.Helpers
{
    public class PasswordHasher
    {
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        private static readonly int SaltSize = 16;
        private static readonly int HashSize = 20;
        private static readonly int Iterations = 10000;

        public static string HashPassword(string password)
        {
            byte[] salt;
            rng.GetBytes(salt=new byte[SaltSize]);

            var key = new Rfc2898DeriveBytes(password,salt, Iterations);
            var hash = key.GetBytes(HashSize);

            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            var base64Has = Convert.ToBase64String(hashBytes);

            return base64Has;
        }

        public static bool VerifyPassword(string password, string base64Has) 
        {
            var hasgBytes = Convert.FromBase64String(base64Has);

            var salt = new byte[SaltSize];
            Array.Copy(hasgBytes, 0, salt, 0, SaltSize);

            var key = new Rfc2898DeriveBytes(password, salt, Iterations);

            byte[] hash = key.GetBytes(HashSize);

            for (var i = 0; i < HashSize; i++)
            {
                if (hasgBytes[i + SaltSize]!=hash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
