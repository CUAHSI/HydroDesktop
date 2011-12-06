.. index:: 
   single: databases; working with
   single: working with databases

Working with Databases
======================

Temporal data are stored in a relational database called the Data Repository.  Catalogs of available data, which may not yet have been downloaded into the Data Repository, are maintained in another database called the Metadata Cache database.  

Tools in HydroDesktop know how to read these databases and present information from them to the user, and they also know how to properly save information to these databases.  Therefore, it's generally a good idea to use HydroDesktop to work with these databases.  However, some users may want to open the databases directly. This page describes how to locate and open HydroDesktop databases outside of HydroDesktop.

.. index:: 
   single: databases; locating
   single: locating databases

Locating the HydroDesktop Databases
-----------------------------------

The *Data Repository* and *Metadata Cache* databases are linked to the current project opened in HydroDesktop. To locate the Data Repository database:

#. In the Ribbon, in the **Table** tab, in the **Database** panel, click **Change**.  The location of the current database is shown in the dialog that opens.

The *Metadata Cache* database has the name ending with _cache.sqlite. To locate the Metadata cache database:

#. In the Ribbon, in the **Table** tab, in the **Database** panel, click **Change**. The location of the current metadata cache database is shown in the dialog that opens.

.. index:: 
   single: databases; viewing
   single: viewing databases

Viewing the HydroDesktop Databases
----------------------------------

HydroDesktop attempts to provide simplified and useful views into its databases through its user interface.  However, it is also possible to open and peruse the database contents directly.  By default, HydroDesktop uses SQLite as its database format.  You can view the contents of a SQLite database using the following free viewers:

* SQLite Database Browser - http://sourceforge.net/projects/sqlitebrowser/
* Firefox SQLite Manager Add-on (requires Firefox) - https://addons.mozilla.org/en-US/firefox/addon/5817/

