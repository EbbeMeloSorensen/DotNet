This project was made with the project template ASP.Net Core Web API, targetting .Net Core 6.0.
Notice that I have used the built in IOC container for injecting a dependency to the DD.Persistence.Memory
assembly, as described here: https://www.tutorialsteacher.com/core/dependency-injection-in-aspnet-core

To run the web api, switch to IIS Express at the button with the green triangle in the standard toolbar,
and subsequently click it. This will bring up a Swagger page in a browser, where you can test the api.
You may also test it by means of Postman.