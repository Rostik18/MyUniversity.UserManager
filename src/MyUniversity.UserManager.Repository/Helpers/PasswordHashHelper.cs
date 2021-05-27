using System.Linq;
using System.Text;
using Grpc.Core;

namespace MyUniversity.UserManager.Repository.Helpers
{
    public static class PasswordHashHelper
    {
        private const int ExpectedHashLength = 64;
        private const int ExpectedSaltLength = 128;

        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Password cannot be null or whitespace"));
            }

            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Password cannot be null or whitespace"));
            }

            if (storedHash.Length != ExpectedHashLength)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid length of password hash"));
            }

            if (storedSalt.Length != ExpectedSaltLength)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid length of password salt"));
            }

            using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return !computedHash.Where((t, i) => t != storedHash[i]).Any();
        }
    }
}
