namespace InnoClinic.Offices.Core.Models
{
    public class OfficeModel
    {
        public Guid Id { get; set; } 
        public string Address { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public string Latitude { get; set; } = string.Empty;
        public Guid PhotoId { get; set; }
        public string RegistryPhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
