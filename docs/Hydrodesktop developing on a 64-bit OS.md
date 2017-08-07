Error you will see:
ERROR: Could not load file or assembly 'System.Data.SQLite, Version=1.0.60.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139' or one of its dependencies. An attempt was made to load a program with an incorrect format.
 
You have to rename
Binaries/System.Data.SQLite64bit.DLL
to Binaries/System.Data.SQLite.DLL

I've tried to force a load of the 64bit dll with a Assembly.LoadFrom("System.Data.SQLite.DLL"), but it's loaded earlier than I can dynamically load it (or I'm doing it incorrectly).

In the Hydrodesktop.Data/PlugIntests  project checks this and copies on [Setup](Setup)the setup copies this over, but it's not practical for developers.