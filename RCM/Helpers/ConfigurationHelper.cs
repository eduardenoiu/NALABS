using RCM.Metrics;
using System;
using System.Configuration;


namespace RCM.Helpers
{
    public class ConfigurationHelper
    {
        /// <summary>
        /// Method to get an app setting string value by a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSetting(string key)
        {
            // Check if the key exists in appSettings
            string value = ConfigurationManager.AppSettings[key];

            // If the value is not found, return null.
            if (string.IsNullOrEmpty(value))
            {
                return null;
                //throw new ConfigurationErrorsException($"AppSetting with key '{key}' not found.");
            }

            return value;
        }

        /// <summary>
        /// Method to get an integer value from appSettings
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetAppSettingInt(string key)
        {
            string value = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrEmpty(value))
            {
                return 0;
                //throw new ConfigurationErrorsException($"AppSetting with key '{key}' not found.");
            }

            // Try to parse the value to an integer
            if (!int.TryParse(value, out int result))
            {
                return 0;
                // throw new FormatException($"AppSetting with key '{key}' is not a valid integer.");
            }

            return result;
        }

        /// <summary>
        /// Retrieves the maximum acceptable metric values for NALABS from the application's app.config file.
        /// These values define threshold limits for various software requirement quality metrics.
        /// </summary>
        /// <returns>
        /// A <see cref="MaximumAcceptableMetricValues"/> object containing the configured threshold values.
        /// </returns>
        public static MaximumAcceptableMetricValues GetMaximumAcceptableMetricValues()
        {
            // Get the custom MaximumAcceptableMetricValue section from the configuration
            var section = ConfigurationManager.GetSection("MaximumAcceptableMetricValue") as System.Collections.Specialized.NameValueCollection;

            // Create an instance of the class to hold the values
            var values = new MaximumAcceptableMetricValues();

            if (section == null)
            {
                return values;
                //throw new ConfigurationErrorsException("MaximumAcceptableMetricValue section not found.");
            }

            // Map the values from the section to the class properties
            values.Conjunctions = int.Parse(section["Conjunctions"] ?? "0");
            values.VaguePhrases = int.Parse(section["VaguePhrases"] ?? "0");
            values.Optionality = int.Parse(section["Optionality"] ?? "0");
            values.Subjectivity = int.Parse(section["Subjectivity"] ?? "0");
            values.References = int.Parse(section["References"] ?? "0");
            values.Weakness = int.Parse(section["Weakness"] ?? "0");
            values.Imperatives = int.Parse(section["Imperatives"] ?? "0");
            values.Continuances = int.Parse(section["Continuances"] ?? "0");
            values.Imperatives2 = int.Parse(section["Imperatives2"] ?? "0");
            values.References2 = int.Parse(section["References2"] ?? "0");
            values.ARI = int.Parse(section["ARI"] ?? "0");

            return values;
        }
    }
}
