using System;

namespace MyUniversity.UserManager.Models.CustomExceptions
{
    public class BadArgumentException : Exception
    {
        public BadArgumentException(string message) : base(message)
        {
        }
    }
}
