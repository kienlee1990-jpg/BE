namespace KPITrackerAPI.DTOs.Admin
{
    public class UpdateUserDto
    {
        public long? DonViId { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public bool? IsActive { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
