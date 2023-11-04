namespace NativeMessaging
{
    internal class Manifest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExecuteablePath { get; set; }
        public string Type { get { return "stdio"; } }
        public string[] AllowedOrigins { get; set; }

        public Manifest(
            string hostname,
            string description,
            string executeablePath,
            string[] allowedOrigins)
        {
            Name = hostname;
            Description = description;
            AllowedOrigins = allowedOrigins;
            ExecuteablePath = executeablePath;
        }
    }
}
