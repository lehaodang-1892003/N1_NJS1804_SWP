﻿namespace ODTLearning.Models
{
    public class StudentProfileToUpdateModel
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
    }
}
