
using System.Security.Cryptography;
namespace semsary_backend.Service
{
    public static class GenerateOtp
    {
        public static string Generate(int length = 6)
        {
            const string allowedChars = "0123456789";
            char[] otp = new char[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[length];

                rng.GetBytes(randomBytes);

                for (int i = 0; i < length; i++)
                {
                    int index = randomBytes[i] % allowedChars.Length;
                    otp[i] = allowedChars[index];
                }
            }

            return new string(otp);
        }
    }
}
