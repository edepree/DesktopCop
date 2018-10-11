using System;
using System.IO;
using System.Threading;

namespace DesktopCop
{
    class Program
    {
        static void Main(string[] args)
        {
            // Disover the Desktop for the current user.
            Console.WriteLine("Starting DesktopCop");
            
            var desktopDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            var deskopDirectoryPath = desktopDirectory.FullName;

            var publicDesktopDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
            var publicDesktopDirectoryPath = publicDesktopDirectory.FullName;

            // Create a FileWatcher to monitor for items added to the Dekstop.
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
        /// An event listener to monitor for new items added to the Windows Desktop and remove any LNK or URL file types
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The file created on the Desktop</param>
        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File {e.Name} Created");

            // Get File Extension to Determine Type
            string fileEnding = Path.GetExtension(e.Name);

            Console.WriteLine($"File Type {fileEnding} Discovered");

            // Remove offending files.
            switch (fileEnding)
            {
                case ".lnk":
                case ".url":
                    bool tryagain = true;
                    while (tryagain)
                    {
                        if (File.Exists(e.FullPath))
                        {
                            Console.WriteLine($"Removing {e.Name}");
                            try
                            {
                                File.Delete(e.FullPath);
                                tryagain = false; //We Have Deleted The File
                            }
                            catch (System.IO.IOException) { } //The Created Event is not guaranteed to be raised exactly once. These leads to a race condition.
                        }
                        else { tryagain = false; } //File No Longer Exists
                    }
                    break;
            }
        }
    }
}