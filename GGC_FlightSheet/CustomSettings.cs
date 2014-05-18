using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace au.org.GGC {
    /// <summary>
    /// This class handles custom Application scope settings.
    /// The settings are stored under the CommonApplicationData path.
    /// Like: C:\ProgramData\[Program name]\[Settings class name].xml
    /// Settings are read & write and safe for restricted users.
    /// </summary>
    /// <typeparam name="T">T is the settings class.</typeparam>
    /// <remarks>License is CPOL</remarks>
    public class CustomProperties<T> where T : class {
        private readonly FileInfo settingsFile;
        private static CustomProperties<T> settings;

        /// <summary>
        /// Gets the settings object
        /// </summary>
        public T Default { get; private set; }

        /// <summary>
        /// Gets the settings file.
        /// </summary>
        public string SettingsFile { get { return settingsFile.FullName; } }

        /// <summary>
        /// Gets the settings as singleton
        /// </summary>
        public static CustomProperties<T> Settings {
            get { return settings ?? (settings = new CustomProperties<T>()); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomProperties&lt;T&gt;"/> class.
        /// </summary>
        protected CustomProperties() {
            // Set the path to the settings file.
            string executableName = System.Windows.Forms.Application.ExecutablePath;
            FileInfo executableFileInfo = new FileInfo(executableName);
            string executableDirectoryName = executableFileInfo.DirectoryName;

            string commonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string filename = String.Format(@"{0}.xml", typeof(T).Name);
            settingsFile = new FileInfo(Path.Combine(executableDirectoryName, filename));
            // Check the path to the file
            if (settingsFile.Directory != null && !settingsFile.Directory.Exists)
                settingsFile.Directory.Create();
            // Load settings, if not found use defaults.
            Default = Load();
            // Use defaults
            if (Default == null)
                Reset();
        }


        /// <summary>
        /// Loads the specified xml file.
        /// </summary>
        /// <returns>Object T</returns>
        private T Load() {
            // Check if settings file exist
            if (!settingsFile.Exists) return default(T);
            // Load settings
            var serializer = new XmlSerializer(typeof(T));
            using (XmlReader reader = new XmlTextReader(File.OpenRead(settingsFile.FullName))) {
                return (T)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        public void Save() {
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new XmlTextWriter(File.Create(settingsFile.FullName), Encoding.UTF8)) {
                writer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, Default);
            }
        }

        /// <summary>
        /// Reset the settings
        /// </summary>
        public void Reset() {
            // Create the settings using initial values
            const bool initializeObject = true;
            Default = (T)Activator.CreateInstance(typeof(T), new object[] { initializeObject });
            Save();
        }

        /// <summary>
        /// Get the product name.
        /// </summary>
        /// <returns></returns>
        private static string GetProductname() {
            // Get the product name
            //var currentApplication = Application.Current;
            //var mainAssembly = currentApplication != null ? Application.Current.MainWindow.GetType().Assembly : Assembly.GetEntryAssembly();
            var productName = String.Empty;
            var mainAssembly = Assembly.GetEntryAssembly();
            if (mainAssembly != null) {
                var assemblyProductAttribute =
                  ((AssemblyProductAttribute[])mainAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)).Single();
                productName = assemblyProductAttribute.Product;
            }
            // If unable to obtain the product name, set it to the name of the executing assembly.
            if (productName.Length == 0)
                productName = Assembly.GetExecutingAssembly().GetName().Name;
            // return name
            return productName;
        }

    }
}

