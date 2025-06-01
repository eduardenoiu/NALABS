using System;

namespace RCM.Helpers
{
    public static class EnvironmentContext
    {
        /// <summary>
        /// Indicates whether the application is running in a Continuous Integration environment.
        /// </summary>
        public static bool IsCI =>
            Environment.GetEnvironmentVariable("CI") == "true" || ConfigurationHelper.IsCI;

        /// <summary>
        /// Indicates whether the application is running in debug mode.
        /// </summary>
        public static bool IsDebug =>
#if DEBUG
        true;
#else
        false;
#endif

        /// <summary>
        /// Gets the machine name for logging or diagnostics.
        /// </summary>
        public static string MachineName => Environment.MachineName;

        /// <summary>
        /// Determines if running in interactive mode (not a service or CI pipeline).
        /// </summary>
        public static bool IsInteractive => Environment.UserInteractive;
    }
}
