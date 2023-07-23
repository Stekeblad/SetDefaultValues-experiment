# SetDefaultValues experiment

This is an experiment to show that Optimizely CMS content properties cannot be initialized with
normal property initialization, that has no effect. To set an initial value for a property
you must override the method SetDefaultValues and do the assignment there.

See also the releated analyzer and code fix in my Stekeblad.Optimizely.Analyzers project: https://github.com/Stekeblad/stekeblad.optimizely.analyzers/blob/master/doc/Analyzers/SOA1019.md 

## Setup

1. Install .NET 6 SDK

2. Install the Optimizely/Episerver dotnet tool:
`dotnet tool update EPiServer.Net.Cli --global --add-source https://nuget.optimizely.com/feed/packages.svc/`

3. Clone/download this repository and cd into it

4. Create a empty Optimizely site database:
`dotnet-episerver create-cms-database src\DefaultValuesTest\SetDefaultValuesTest.csproj -S "(localDb)\MSSQLLocalDB" -E -dn "SetDefaultValuesTest" -du "SetDefaultValuesTestUser" -dp "databaseUserPassword"`
    - You may want to change the -S parameter (where your SQL Server is) and replace -E (use Windows authentication) with a SQL username and password. List all arguments and descriptions for the create-cms-database commmand with `dotnet-episerver create-cms-database --help`

## Run the experiment

1. `dotnet run`
    - The site binds to https://localhost:5000. If that is a problem, you need to update both Properties/launchSettings.json and two locations in Business/CreateSite.cs

2. Create admin user in the form that opens

3. Click the "Create pages" button on start page

4. Review the results. Note that the first instance of initializer page does not have
values for any of it's properties although it's heading property is assigned
a value using property initialization in Models/Pages/InitializerPage.cs! Compare with SetDefaultValuesPage.cs in the same folder.

5. Optionally: Look inside the CMS and try creating and editing pages, observing when new pages have a default value and not.