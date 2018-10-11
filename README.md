# DesktopCop
DesktopCop is a simple C# program to monitor and remove files from a user's Windows desktop. This stemmed from an annoyance at work where icons and shortcuts were being pushed to workstation desktops and this processes could not be disabled or overridden for select users.

The intention of this program is to be paired with a Windows scheduled task to be executed on login. Leveraging a scheduled task versus a Windows service allows this program to be executed without required local administrator rights.

Once started, the program which runs indefinitely monitoring and removing .LNK and .URL files from the desktop.
