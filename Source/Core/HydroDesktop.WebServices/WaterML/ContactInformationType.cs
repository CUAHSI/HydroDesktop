namespace HydroDesktop.WebServices.WaterML
{
    /// <summary>
    /// Represents WaterML 1.0/1.1 ContactInformationType
    /// </summary>
    public class ContactInformationType
    {
        public string ContactName { get; set; }
        public string TypeOfContact { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}