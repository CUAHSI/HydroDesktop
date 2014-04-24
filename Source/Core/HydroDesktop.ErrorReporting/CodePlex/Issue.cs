using System;
using System.IO;

namespace HydroDesktop.ErrorReporting.CodePlex
{
    internal class Issue
    {
        private FileInfo _fileToAttach;
        public string Description { get; set; }
        public string Summary { get; set; }

        /// <summary>
        /// File to attach
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Throws when file not exists or size of attached file is too big.</exception>
        public FileInfo FileToAttach
        {
            get { return _fileToAttach; }
            set
            {
                if (value != null)
                {
                    if (!value.Exists)
                    {
                        throw new ArgumentOutOfRangeException("value", "File to attach not exists.");
                    }
                    if (value.Length > 4 * 1024 * 1024) // Max 4 MB)
                    {
                        throw new ArgumentOutOfRangeException("value", "Size of attached file is too big. Maximum is 4 MB.");
                    }
                }
                
                _fileToAttach = value;
            }
        }
    }
}