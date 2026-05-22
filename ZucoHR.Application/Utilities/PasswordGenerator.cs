using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Application.Utilities
{
    using System.Security.Cryptography;
    using System.Text;

    public static class PasswordGenerator
    {
        public static string Generate(
            int length = 12,
            bool includeUpper = true,
            bool includeLower = true,
            bool includeNumbers = true,
            bool includeSymbols = true)
        {
            if (length < 6)
                throw new ArgumentException("Password length must be at least 6");

            var upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var lower = "abcdefghijklmnopqrstuvwxyz";
            var numbers = "0123456789";
            var symbols = "!@#$%^&*()-_=+[]{}|;:,.<>?";

            var charPool = new StringBuilder();

            if (includeUpper) charPool.Append(upper);
            if (includeLower) charPool.Append(lower);
            if (includeNumbers) charPool.Append(numbers);
            if (includeSymbols) charPool.Append(symbols);

            if (charPool.Length == 0)
                throw new ArgumentException("At least one character set must be selected");

            var password = new StringBuilder();
            var randomBytes = new byte[length];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            for (int i = 0; i < length; i++)
            {
                var idx = randomBytes[i] % charPool.Length;
                password.Append(charPool[idx]);
            }

            return password.ToString();
        }
    }
}
