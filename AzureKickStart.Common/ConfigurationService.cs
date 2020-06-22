using System;
using System.Configuration;
using System.Linq;

namespace AzureKickStart.Common
{
    public class ConfigurationService
    {
        public T GetAppSettingValue<T>(string key, T defaultValue = default(T))
        {
            if (ConfigurationManager.AppSettings.AllKeys.Any(s => s == key))
            {
                var value = ConfigurationManager.AppSettings[key];
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (Exception)
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }
        public string GetConnectionStringValue(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }
    }
}
