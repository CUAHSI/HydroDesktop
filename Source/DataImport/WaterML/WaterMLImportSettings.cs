namespace DataImport.WaterML
{
    public class WaterMLImportSettings : IFileImportSettings
    {
        public string PathToFile{get;set;}
        public string ThemeName { get; set; }
    }
}