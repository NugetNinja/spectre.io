﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemDirectory = System.IO.Directory;
using SystemEnv = System.Environment;
using SystemFolder = System.Environment.SpecialFolder;

namespace Spectre.IO
{
    /// <summary>
    /// Represents the environment.
    /// </summary>
    public sealed class Environment : IEnvironment
    {
        /// <inheritdoc/>
        public DirectoryPath WorkingDirectory
        {
            get { return new DirectoryPath(SystemDirectory.GetCurrentDirectory()); }
        }

        /// <inheritdoc/>
        public DirectoryPath HomeDirectory
        {
            get { return new DirectoryPath(SystemEnv.GetFolderPath(SystemFolder.UserProfile)); }
        }

        /// <inheritdoc/>
        public IPlatform Platform { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Environment"/> class.
        /// </summary>
        public Environment()
            : this(new Platform())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Environment" /> class.
        /// </summary>
        /// <param name="platform">The platform.</param>
        public Environment(IPlatform platform)
        {
            Platform = platform;

            SetWorkingDirectory(new DirectoryPath(SystemDirectory.GetCurrentDirectory()));
        }

        /// <inheritdoc/>
        public string? GetEnvironmentVariable(string variable)
        {
            return SystemEnv.GetEnvironmentVariable(variable);
        }

        /// <inheritdoc/>
        public IDictionary<string, string?> GetEnvironmentVariables()
        {
            return SystemEnv.GetEnvironmentVariables()
                .Cast<System.Collections.DictionaryEntry>()
                .Aggregate(
                    new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase),
                    (dictionary, entry) =>
                    {
                        var key = (string)entry.Key;
                        if (!dictionary.TryGetValue(key, out _))
                        {
                            dictionary.Add(key, entry.Value as string);
                        }

                        return dictionary;
                    },
                    dictionary => dictionary);
        }

        /// <inheritdoc/>
        public void SetWorkingDirectory(DirectoryPath path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (path.IsRelative)
            {
                throw new IOException("Working directory can not be set to a relative path.");
            }

            SystemDirectory.SetCurrentDirectory(path.FullPath);
        }
    }
}