Project Description:

Clean your database is a simple project removes unused stored procedures from your project what ever the Data Access Layer you are using

This application works and tested on the following DAL (Data Access Layers):

ADO.NET
Entity Framework
Linq 2 SQL
Purpose

When a developer works on a long-term project, it could happen that he stop using some stored procedures for any reason (change request, fix bug, update a module, ... etc), and since the stored procedure is the most preferred way to perform all CRUD transactions on the database, so you can create at least 3 stored procedures per table, and it may cause a headache when you try perform a cleaning operation to remove unused objects specially the stored procedures, so I made this application to help the developers deleting the stored procedures they are not using.

And it is pretty simple to use in only 3 steps:
Browse the project's folder
Set the connection string to the required database
Start the operation

After the searching completed successfully, you can either export the cleaning script and apply the cleaning later, or by performing the cleaning immediately
Original post:
https://cyd.codeplex.com/
