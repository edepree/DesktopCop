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
            
            // Create a FileWatcher to monitor for items added to the Dekstop.
            Console.WriteLine($"Monitoring {deskopDirectoryPath}");
            
            var fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = deskopDirectoryPath;
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.EnableRaisingEvents = true;

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
            
            // Confirm the file meets substring requirements.
            if(e.Name.Length > 4)
            {
                // Extract the last four characters of the file name to determin file type.
                var fileEnding = e.Name.Substring(e.Name.Length - 4).ToLower();
                Console.WriteLine($"File Type {fileEnding} Discovered");

                // Remove offending files.
                switch (fileEnding)
                {
                    case ".lnk":
                    case ".url":
                    {
                        if (File.Exists(e.FullPath))
                        {
                            Console.WriteLine($"Removing {e.Name}");
                            File.Delete(e.FullPath);
                        }
                        break;
                    }
                }
            }
        }
    }
}
