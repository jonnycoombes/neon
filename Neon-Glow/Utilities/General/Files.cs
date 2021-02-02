﻿#region

using System;
using System.IO;
using System.Linq;
using JCS.Neon.Glow.Types;
using Serilog;

#endregion

namespace JCS.Neon.Glow.Utilities.General
{
    /// <summary>
    ///     Class which contains a bunch of useful file I/O related helper methods/functions
    /// </summary>
    public static class Files
    {
        /// <summary>
        ///     Private static logger
        /// </summary>
        private static readonly ILogger _log = Log.ForContext(typeof(Files));

        /// <summary>
        ///     Tries to determine the current home path based on calls through to System.Environment
        ///     underlying functions
        /// </summary>
        /// <returns></returns>
        public static Option<string> GetCurrentHomePath()
        {
            Logs.MethodCall(_log);
            var home = Environment.GetEnvironmentVariable("HOME");
            if (home != null) return Option<string>.Some(home);
            try
            {
                return Option<string>.Some(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            }
            catch
            {
                Logs.Warning(_log, "Failed to locate the current HOME directory");
                return Option<string>.None;
            }
        }

        /// <summary>
        ///     Attempts to generate a path which consists of the current home directory
        ///     catted with a supplied suffix
        /// </summary>
        /// <param name="suffix">A set of components that will be appended to the current home path using Path.Combine</param>
        /// <returns>A string option which is None if the home directory can't be located</returns>
        public static Option<string> GetHomeSubdirectoryPath(params string[] suffix)
        {
            Logs.MethodCall(_log);
            var homeOption = GetCurrentHomePath();
            if (homeOption.IsSome(out var home))
            {
                var components = new[] {home};
                components = components.Concat(suffix).ToArray();
                return Option<string>.Some(Path.Combine(components));
            }

            Logs.Warning(_log, "Failed to locate the current HOME directory");
            return Option<string>.None;
        }
    }
}