namespace MyProject
{
    public interface IAppFolders
    {
        string TempFileDownloadFolder { get; }

        string DemoUploadFolder { get; }

        string DemoFileDownloadFolder { get; }

        public string ProductFileDownloadFolder { get; set; }

        public string ProductFileUploadFolder { get; set; }
    }
}