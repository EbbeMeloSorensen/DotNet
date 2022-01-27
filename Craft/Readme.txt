Notice that all utility projects are based on .Net Standard 2.1, so they don't depend on the .Net Framework.
As such they are also supported by Visual Studio Code. Some ViewModel projects also depend on .Net Standard 2.1,
whereas others depend on .Net 6.0-windows. This is usually the case when the view model contains windows resources 
such as brushes.

Console applications are based on .Net 6.0, because then they also run on my Linux laptop.

Wpf Class libraries and Wpf Applications are based on .Net 6.0.

Unit Test projects are based on xUnit, because it supports Linux, and generally seems to be more popular than e.g.
MSTest

Generally, class libraries are based on on .Net Standard 2.1 when possible, and otherwise .Net 6.0.

Hmmm bemærk, at man for wpf class libraries og wpf applications kan VÆLGE mellem om de skal basere sig på
.Net 6.0 eller .Net Core. Begge virker tilsyneladende, og man kan ikke bare forlade sig på at den sætter en
passende default, da den tilsyneladende bare foreslår det samme som man valgte tidligere.

Så spørgsmålet er: Hvornår er det en fordel at vælge .Net 6.0 og hvornår er det en fordel at vælge .Net Core 3.1?

Iagttagelse: Man kan GODT eksekvere en WPF applikation fra Visual Studio Code. I hvert fald når:
* Man arbejder på en Windows pc
* Applikationen baserer sig på .Net 6.0