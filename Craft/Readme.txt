Notice that all utility projects are based on .Net Standard 2.1, i.e. they depend on .Net Core and not .Net Framework.
As such they are supported by Visual Studio Code. Some View Model projects also depend on .Net Standard 2.1, whereas
others depend on .Net Framework (.Net 6.0). This is usually the case when the view model contains windows resources 
such as brushes.

Hmmm bemærk, at man for wpf class libraries og wpf applications kan VÆLGE mellem om de skal basere sig på
.Net 6.0 eller .Net Core. Begge virker tilsyneladende, og man kan ikke bare forlade sig på at den sætter en
passende default, da den tilsyneladende bare foreslår det samme som man valgte tidligere.

Så spørgsmålet er: Hvornår er det en fordel at vælge .Net 6.0 og hvornår er det en fordel at vælge .Net Core 3.1?