﻿namespace MedicalDashboardAPI.DTOs
{
    public class UserRegisterDto
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Gender { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
    }
}