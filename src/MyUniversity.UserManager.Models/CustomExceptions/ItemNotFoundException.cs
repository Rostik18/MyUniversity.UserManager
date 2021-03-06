using System;

namespace MyUniversity.UserManager.Models.CustomExceptions
{
    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message)
        {
        }
    }
}
