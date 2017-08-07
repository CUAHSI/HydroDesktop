# Nunit
[http://www.nunit.org/](http___www.nunit.org_)

# Rules and notes
* keep tests up to date
* In order to run tests from the test case runner, you need to be sure the database from Hydrodesktop.database is copied into the binaries directory
* Tests which use TestCases Attributes do not work in resharper 4 or 4.5 (maybe 5). Use the Nunit runner to run tests

# Categories
We should start using [Categories](http://www.nunit.org/index.php?p=category&r=2.5.3)
* Hydrodesktop Data
	* Data manager
	* Model
	* repository

# Tested Areas
* Hydrodektop.Data has unit test

# Test Database
 We need a standardized test database, so that the tests can be run on a known dataset.

# User Interfaces and file output
Look as using approval tests: http://approvaltests.sourceforge.net/
Basically, an approval test will fail the first time, and anytime the output differs. Then you approve the result. The results can be stored in source control, so that you can keep a history.