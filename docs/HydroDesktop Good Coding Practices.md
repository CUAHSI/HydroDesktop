This will be a page that lists some best practices

# General
**Directories**
Use the HydroDekstop methods. If you need a directory, ask if it can be added. That way if we change them
* project
	* Apply to a specific project
* User Settings (aka User/AppData/...)
	* HydroDesktop.Configuration.Settings.Instance.
	* This get's buried deep. Do we want to have
* application
	* HydroDesktop.Configuration.Settings.Instance.
	* read, avoid writing
* temp
	* HydroDesktop.Configuration.Settings.Instance.
	* clean up after yourself

**Fixed Strings**
In order to allow for customization, item names, like the name of a plug-in (or in the future label names) should be stored in a location that can be edited
.Net provides two types:
* settings.settings (will become app.config or xxx.dll.config)
* resources

* string resource Examples
	* Items like the name of the plug-in 

* settings examples
If you have a filename or directory, 
Use .NetSettings settings


**Reading files**
make a method to read a file from disk. If the location of the file changes, it will be easier to find.

# Refactoring
* **In Forms**
	* Avoid including methods that do processing in the form code. 
	* move these methods to classes
	* Use an object to abstract the information on the form into a single object. 
		* Eg the search form has an object, Search Criteria. A method called GetCrieria reads the information from the form
	* if you need to display an error message consider throwing an exception or creating an event
	* If you find yourself copying code, or duplicating code that manages states of a group of controls, make a method that manages the state of those controls
* An object, and moving processing outside of the form allows for easier testing. 
	* You can create use cases for the object, and pass them to the processing methods.


