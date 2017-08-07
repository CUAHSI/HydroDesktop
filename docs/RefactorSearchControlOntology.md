Refactoring 

For this example we will be using SearchControl.cs

To refactor a method we need to Plan:
* remove dependencies on internal variables of the form
	* pass in paramters to the method 
	* create objects to simplfy the parameter list (and validation and testing)
* remove calls to message box and user interface
	* replace with 
		* Events
		* or Exceptions that should be caught
* Create one or more specific exceptions

* [RefactorSearchControlOntologyPart1](RefactorSearchControlOntologyPart1)
* [RefactorSearchControlOntologyPart2](RefactorSearchControlOntologyPart2)
* [RefactorSearchControlOntologyPart3](RefactorSearchControlOntologyPart3)