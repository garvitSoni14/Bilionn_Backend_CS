namespace Bilionn_backend.Models
{
    public class Employee: IAuditable
    {
        public long Id { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string MobileNumber { get; set; } = string.Empty;
         public Address? Address { get; set; }   
        public DateOnly JoiningDate { get; set; }
        public decimal Salary { get; set; }
        public string? PanNumber { get; set; }
        public string? AadhaarNumber { get; set; }
        public BankDetails? BankDetails { get; set; }
        public EmergencyContact? EmergencyContact { get; set; }

        public long UserAccountId { get; set; }
        public User UserAccount { get; set; } = null!;

        public long CreatedByAdminId { get; set; }
        public User CreatedByAdmin { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
