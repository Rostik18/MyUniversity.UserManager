using System;

namespace MyUniversity.UserManager.Models.CustomExceptions
{
    public class AccessForbiddenException : Exception
    {
        public AccessForbiddenException(string message) : base(message)
        {
        }
    }
}
