using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace CatalogoBackend.Utils
{
    public class HashHelper
    {
        private const int SaltSize = 16; //? bytes
        private const int HashSize = 32; //? bytes
        private const int Iterations = 4; //? Número de rondas de Argon2
        private const int MemorySizeKB = 65536; //? Memoria en KB (~64 MB)
        private const int DegreeOfParallelism = 4; //? Hilos paralelos

        private const string Format = "Argon2id";

        public static string GenerateToken(int bytes = 32)
        {
            var raw = RandomNumberGenerator.GetBytes(bytes); //? 32 bytes -> 256 bits
            //? Base64 URL-safe (sin =)
            return Convert.ToBase64String(raw).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        public static string HashTokenHex(string token, string secret)
        {
            var key = Encoding.UTF8.GetBytes(secret);
            using var hmac = new HMACSHA256(key);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(hash).ToLowerInvariant(); //? 64 chars lowercase hex
        }

        public static string HashPassword(string password)
        {
            if (password == null) throw new ArgumentException(nameof(password));

            //* Se genera un salt aleatorio
            var salt = new byte[SaltSize];
            RandomNumberGenerator.Fill(salt);

            //* Se crea la instancia de Argon2id
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                Iterations = Iterations,
                MemorySize = MemorySizeKB,
                DegreeOfParallelism = DegreeOfParallelism
            };

            var hash = argon2.GetBytes(HashSize);

            //* Se construye el string versionado: "Argon2id$iterations$memory$parallelism$saltBase64$hashBase64"
            var saltB64 = Convert.ToBase64String(salt);
            var hashB64 = Convert.ToBase64String(hash);

            return $"{Format}${Iterations}${MemorySizeKB}${DegreeOfParallelism}${saltB64}${hashB64}";

        }

        public static bool VerifyPassword(string hashedPasswordDb, string providedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPasswordDb)) return false;
            if (providedPassword == null) throw new ArgumentNullException(nameof(providedPassword));

            //* Separa el hash en partes y evalua su formato
            var parts = hashedPasswordDb.Split("$", StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 6) return false;
            if (!string.Equals(parts[0], Format, StringComparison.OrdinalIgnoreCase)) return false;

            //* Convierte los valores del hash de string a int
            if (!int.TryParse(parts[1], out int iterations)) return false;
            if (!int.TryParse(parts[2], out int memorySizeKB)) return false;
            if (!int.TryParse(parts[3], out int degreeOfParallelism)) return false;

            byte[] salt, hash;
            try
            {
                salt = Convert.FromBase64String(parts[4]);
                hash = Convert.FromBase64String(parts[5]);
            }
            catch
            {
                return false;
            }

            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(providedPassword))
            {
                Salt = salt,
                Iterations = iterations,
                MemorySize = memorySizeKB,
                DegreeOfParallelism = degreeOfParallelism
            };

            var computedHash = argon2.GetBytes(hash.Length);

            return CryptographicOperations.FixedTimeEquals(computedHash, hash);
        }
    }
}
