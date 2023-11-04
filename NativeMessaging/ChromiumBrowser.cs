using Microsoft.Win32;

namespace NativeMessaging
{
    /// <summary>
    /// Represent any browser derived from Google's Chromium.
    /// </summary>
    public partial class ChromiumBrowser
    {
        private readonly string regHostnameKeyLocation;

        /// <summary>
        /// The name of the browser application.
        /// </summary>
        public string BrowserName { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ChromiumBrowser"/> object.
        /// </summary>
        /// <param name="browserName">The name of the browser application.</param>
        /// <param name="RegKeyBaseLocation">Base location for the browser 
        /// settings in the Windows Registry.</param>
        public ChromiumBrowser(string browserName, string RegKeyBaseLocation)
        {
            BrowserName = browserName;

            regHostnameKeyLocation
                = RegKeyBaseLocation + "\\NativeMessagingHosts\\";
        }

        /// <summary>
        /// Checks if the host is registered with the browser
        /// </summary>
        /// <param name="ManifestPath">Path to the Native Messaging Host manifest file</param>
        /// <param name="Hostname">The hostname for the Native Messaging Host application</param>
        /// <returns><see langword="true"/> if the required information is present in the registry.</returns>
        public bool IsRegistered(string Hostname, string ManifestPath)
        {
            if (OperatingSystem.IsWindows())
            {
                string targetKeyPath = regHostnameKeyLocation + Hostname;

                RegistryKey? regKey
                    = Registry.CurrentUser.OpenSubKey(targetKeyPath, true);

                if (regKey != null
                    && regKey?.GetValue("")?.ToString() == ManifestPath)
                {
                    return true;
                }
            }
            else
            {
                throw new NotImplementedException(
                    "Registration verification not implemented on this " +
                    "platform.");
            }

            return false;
        }

        /// <summary>
        /// Register the application to open with the browser.
        /// </summary>
        /// <param name="Hostname">The hostname for the Native Messaging Host application</param>
        /// <param name="ManifestPath">Path to the Native Messaging Host manifest file</param>
        public void Register(string Hostname, string ManifestPath)
        {
            if (OperatingSystem.IsWindows())
            {
                string targetKeyPath = regHostnameKeyLocation + Hostname;

                RegistryKey? regKey
                    = Registry.CurrentUser.OpenSubKey(targetKeyPath, true);

                if (regKey == null)
                {
                    regKey = Registry.CurrentUser.CreateSubKey(targetKeyPath);
                }

                regKey.SetValue("", ManifestPath, RegistryValueKind.String);

                regKey.Close();

                Log.LogMessage(
                    "Registered host (" + Hostname + ") with browser "
                    + BrowserName);
            }
            else if (OperatingSystem.IsMacOS())
            {
                //Google Chrome: ~/Library/Application Support/Google/Chrome/NativeMessagingHosts/com.my_company.my_application.json
                //Chromium: ~/Library/Application Support/Chromium/NativeMessagingHosts/com.my_company.my_application.json
                var browsers = new List<string>() {
                    "/Library/Application Support/Google/Chrome/NativeMessagingHosts/",
                    "/Library/Application Support/Chromium/NativeMessagingHosts/"
                };
                AddManifests(ManifestPath, browsers);
            }
            else if (OperatingSystem.IsLinux())
            {
                //Google Chrome: ~/.config/google-chrome/NativeMessagingHosts/com.my_company.my_application.json
                //Chromium: ~/.config/chromium/NativeMessagingHosts/com.my_company.my_application.json
                var browsers = new List<string>() {
                    "/.config/google-chrome/NativeMessagingHosts/",
                    "/.config/chromium/NativeMessagingHosts/"
                };
                AddManifests(ManifestPath, browsers);
            }
            else
            {
                throw new NotImplementedException(
                    "Registration not implemented on this platform.");
            }
        }

        /// <summary>
        /// De-register the application to open with the browser.
        /// </summary>
        /// <param name="Hostname">The hostname for the Native Messaging Host application</param>
        public void Unregister(string Hostname, string ManifestPath)
        {
            if (OperatingSystem.IsWindows())
            {
                string targetKeyPath = regHostnameKeyLocation + Hostname;

                RegistryKey? regKey
                    = Registry.CurrentUser.OpenSubKey(targetKeyPath, true);

                if (regKey != null)
                {
                    regKey.DeleteSubKey("", true);
                }

                regKey?.Close();

                Log.LogMessage(
                    "Unregistered host (" + Hostname + ") with browser "
                    + BrowserName);
            }
            else if (OperatingSystem.IsMacOS())
            {
                //Google Chrome: ~/Library/Application Support/Google/Chrome/NativeMessagingHosts/com.my_company.my_application.json
                //Chromium: ~/Library/Application Support/Chromium/NativeMessagingHosts/com.my_company.my_application.json
                var browsers = new List<string>() {
                    "/Library/Application Support/Google/Chrome/NativeMessagingHosts/",
                    "/Library/Application Support/Chromium/NativeMessagingHosts/"
                };
                RemoveManifests(ManifestPath, browsers);
            }
            else if (OperatingSystem.IsLinux())
            {
                //Google Chrome: ~/.config/google-chrome/NativeMessagingHosts/com.my_company.my_application.json
                //Chromium: ~/.config/chromium/NativeMessagingHosts/com.my_company.my_application.json
                var browsers = new List<string>() {
                    "/.config/google-chrome/NativeMessagingHosts/",
                    "/.config/chromium/NativeMessagingHosts/"
                };
                RemoveManifests(ManifestPath, browsers);
            }
            else
            {
                throw new NotImplementedException(
                   "Registration removal not implemented on this platform.");
            }
        }

        private static void RemoveManifests(string ManifestPath, List<string> browsers)
        {
            var fileName = Path.GetFileName(ManifestPath);
            foreach (var browser in browsers)
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), browser, fileName);
                if (File.Exists(path))
                    File.Delete(path);
            }
        }

        private static void AddManifests(string ManifestPath, List<string> browsers)
        {
            var fileName = Path.GetFileName(ManifestPath);
            foreach (var browser in browsers)
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), browser, fileName);
                File.Copy(ManifestPath, path, true);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return BrowserName;
        }
    }
}
