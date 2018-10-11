using System;
using System.IO;
using System.Threading;

namespace DesktopCop
{
    class Program
    {
        static void Main(string[] args)
        {
            // Disover the desktop for the current user.
            Console.WriteLine("Starting DesktopCop");
            
            var desktopDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            var deskopDirectoryPath = desktopDirectory.FullName;

            var publicDesktopDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
            var publicDesktopDirectoryPath = publicDesktopDirectory.FullName;

            // Create a FileWatcher to monitor for items added to the dekstop.
            Console.WriteLine($"Monitoring {deskopDirectoryPath}");
            Console.WriteLine($"Monitoring {publicDesktopDirectoryPath}");

            var fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = deskopDirectoryPath;
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.EnableRaisingEvents = true;

            var publicFileSystemWatcher = new FileSystemWatcher();
            publicFileSystemWatcher.Path = publicDesktopDirectoryPath;
            publicFileSystemWatcher.Created += FileSystemWatcher_Created;
            publicFileSystemWatcher.EnableRaisingEvents = true;

            // Run the program forever by waiting on an event which will never happen.
            Console.WriteLine("Entering Blocking State");
            var NeverQuit = new ManualResetEvent(false);
            NeverQuit.WaitOne();
        }

        /// <summary>
        /// An event listener to monitor for new items added to the Windows desktop and remove any LNK or URL file types
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The file created on the Desktop</param>
        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File {e.Name} Created");

            // Get a file's extension to determine the file type.
            string fileEnding = Path.GetExtension(e.Name);

            Console.WriteLine($"File Type {fileEnding} Discovered");

            // Remove offending files from monitored directories.
            switch (fileEnding)
            {
                case ".lnk":
                case ".url":
                    bool TryAgain = true;
                    while (TryAgain)
                    {
                        if (File.Exists(e.FullPath))
                        {
                            Console.WriteLine($"Removing {e.Name}");
                            try
                            {
                                File.Delete(e.FullPath);
                                TryAgain = false; // The file as been successfully deleted.
                            }
                            catch (System.IO.IOException) { } // The Created event is not guaranteed to be raised exactly once. These leads to a race condition.
                        }
                        else { TryAgain = false; } // The file no longer exists.
                    }
                    break;
            }
        }
    }
}