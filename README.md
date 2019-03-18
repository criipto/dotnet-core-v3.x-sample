# Use Criipto Verify from ASP.Net Core 2.2

This is the slimmest of samples, just showing how to configure and enable OpenID Connect middelware in an ASP.NET Core 2.x web application to authentication using Criipto Verify brokered e-ID login.

Note that while this sample simply runs the e-ID login in full screen, you would often do it inside an iframe. An iframed approach requires a few extra tricks on top of what's shown in this sample. This is described elsewhere.

## Runing the sample

We use Visual Studio Code for development, but you may run the sample just the same with .NET Core standalone.

`dotnet run` will restore packages, compile, and launch the site. If you haven't changed your launch settings, you may point your browser to https://localhost:5001 to test login with test users.

## Test users

Read more about test users in the [Criipto Documentation](https://docs.criipto.com/how-to/test-users)

## Points of interest

You may read a more detailed description of the steps in the Criipto documentation, but the key points of interest is the wiring up of Criipto Verify are:

1. Wire up the middelware in `Startup.cs`. 
2. Change the Criipto settings in `appsettings.json`. Note though that it already includes values you may use to simply test the code and the login process. These values may be replaced with those from your own Criipto Verify tenant.
3. Add Login and Logout logic to the home controller, `/Controllers/HomeController.cs.` (Login logic is simply adding the `[Authorize]` to the route)
4. Add a page to be shown once authenticatedd, `/Views/Home/Protected.cshtml`. This page simply lists all the attriutes received from Criipto Verify.


