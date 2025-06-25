namespace ClientPortalBifurkacioni.Helpers
{
    public static class ConfigurationAccessor
    {
        private static IConfiguration? _configuration;

        public static void SetConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string? GetValue(string key)
        {
            return _configuration?[key];
        }

        public static string? GetSectionValue(string section, string key)
        {
            return _configuration?.GetSection(section)?[key];
        }
    }
}
