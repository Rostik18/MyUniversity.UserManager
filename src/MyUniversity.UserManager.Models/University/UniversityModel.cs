﻿namespace MyUniversity.UserManager.Models.University
{
    public class UniversityModel
    {
        /// <summary>
        /// Is a unique identifier
        /// </summary>
        public string TenantId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
    }
}
