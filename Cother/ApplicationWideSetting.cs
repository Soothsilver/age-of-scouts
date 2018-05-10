using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.IO;

namespace Cother
{
    /// <summary>
    /// When you call ApplicationSettingsManagement.SaveSettings, this field will be saved. It will be loaded when you call .LoadSettings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ApplicationWideSetting : Attribute
    {
    
    }
    /// <summary>
    /// This class handles the ApplicationWideSetting attribute and allows you to easily serialize your data to disk.
    /// </summary>
    public static class ApplicationSettingsManagement
    {
        /// <summary>
        /// Contains any last error that might have been produced by LoadSettings/SaveSettings. Perhaps insufficient disk space? 
        /// </summary>
        public static Exception LastError { get; set; }
        private static readonly string settingsFileName;


        /// <summary>
        /// Loads application-wide settings from file into the "container".
        /// In the container, you must specify fields  to save with [ApplicationWideSetting] attribute.
        /// They must be [Serializable].
        /// </summary>
        /// <param name="container">The object that has the specified fields</param>
        /// <returns>Were the settings loaded from file?</returns>
        public static bool LoadSettings(object container)
        {
            if (!File.Exists(settingsFileName))
                return false;
            try
            {
                List<SerializablePair> pairs;
                using (FileStream fs = new FileStream(settingsFileName, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    pairs = (List<SerializablePair>)bf.Deserialize(fs);
                    fs.Flush();
                }
                foreach (SerializablePair pair in pairs)
                {
                    pair.FieldInfo.SetValue(container, pair.Contents);
                }
                return true;
            }
            catch (Exception error)
            {
                LastError = error;
                return false;
            }
        }
        static ApplicationSettingsManagement()
        {
            settingsFileName = Path.Combine(
                 Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                 AppDomain.CurrentDomain.FriendlyName,
                 "ApplicationWideSettings.dat");
            Directory.CreateDirectory(path: Path.GetDirectoryName(settingsFileName));
        }
        /// <summary>
        /// Saves application-wide settings into a file from the container.
        /// In the container, you must specify fields to save with [ApplicationWideSetting] attribute.
        /// They must be [Serializable].
        /// </summary>
        /// <param name="container">The object that has the specified fields</param>
        /// <returns>Were the settings successfully saved?</returns>
        public static bool SaveSettings(object container)
        {
            try
            {
                Type typ = container.GetType();
                FieldInfo[] fields = typ.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                List<SerializablePair> pairs = new List<SerializablePair>();
                foreach (FieldInfo fi in fields)
                {
                    if (fi.GetCustomAttributes(typeof(ApplicationWideSetting), false).Length > 0)
                    {
                        object value = fi.GetValue(container);
                        pairs.Add(new SerializablePair(fi, value));
                    }
                }
                using (FileStream fs = new FileStream(settingsFileName, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, pairs);
                    fs.Flush();
                }
                return true;
            }
            catch (Exception e)
            {
                LastError = e;
                return false;
            }
        }

        [Serializable]
        private class SerializablePair
        {
            public readonly FieldInfo FieldInfo;
            public readonly object Contents;
            public SerializablePair(FieldInfo fieldInfo, object contents)
            {
                FieldInfo = fieldInfo;
                Contents = contents;
            }
        }
       
    }
}
