Notice that all utility projects are based on .Net Standard 2.1, i.e. they depend on .Net Core and not .Net Framework.
As such they are supported by Visual Studio Code. Some View Model projects also depend on .Net Standard 2.1, whereas
others depend on .Net Framework (.Net 6.0). This is usually the case when the view model contains windows resources 
such as brushes.
