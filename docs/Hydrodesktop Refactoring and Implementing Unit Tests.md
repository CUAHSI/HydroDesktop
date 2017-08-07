The HD plug ins need to incorporate unit testings

General Rules:
* In Forms, 
	* Avoid including methods that do processing in the form code. 
	* move these methods to classes
	* Use an object to abstract the information on the form into a single object. 
		* Eg the search form has an object, Search Criteria. A method called GetCrieria reads the information from the form
	* if you need to display an error message consider throwing an exception or creating an event
	* If you find yourself copying code, or duplicating code that manages states of a group of controls, make a method that manages the state of those controls
* An object, and moving processing outside of the form allows for easier testing. 
	* You can create use cases for the object, and pass them to the processing methods.

# Plug-in directory layout for testing
In order for a plug in to be tested, the layout needs to be moved down a level, and the solution should be moved out of the plug-in directory. You can edit the solution file with a text editor.
Be sure to check that the dll's and other files are still being placed in the binaries directory( add a ../ to the output of the YouPlugIn project)
* Source/PlugIns/YourPlugIn
	* YouPlugIn directory
		* code for the plug
	* YourPlugIn.sln
	* YoutPlugInTests directory
		* Name Assembly "Tests.PlugInName.dll"
		* sampleFiles  (to test your plug in, if needed)

# Pages
* [Hydrodesktop Refactoring Examples](Hydrodesktop-Refactoring-Examples)
* [Hydrodesktop Simple Unit Tests](Hydrodesktop-Simple-Unit-Tests)
* [Hydrodesktop Multiple Inputs for One Unit Test Method](Hydrodesktop-Multiple-Inputs-for-One-Unit-Test-Method)
* [Hydrodesktop Unit Tests with Mock](Hydrodesktop-Unit-Tests-with-Mock)
* [Hydrodesktop User Interface Tests](Hydrodesktop-User-Interface-Tests)
* [Unit Tests](Unit-Tests) Older page