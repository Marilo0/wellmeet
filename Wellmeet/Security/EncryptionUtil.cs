namespace Wellmeet.Security
{
    public static class EncryptionUtil
    {
        public static string Encrypt(string plainText)
        {
            var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(plainText);
            return encryptedPassword;
        }

        public static bool IsValidPassword(string plainText, string cipherText)
        {
            var isValid = BCrypt.Net.BCrypt.Verify(plainText, cipherText);
            return isValid;
        }
    }
}
