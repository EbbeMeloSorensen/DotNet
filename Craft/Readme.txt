Notice that all utility projects are based on .Net Standard 2.1, i.e. they depend on .Net Core and not .Net Framework.
As such they are supported by Visual Studio Code. Some View Model projects also depend on .Net Standard 2.1, whereas
others depend on .Net Framework (.Net 6.0). This is usually the case when the view model contains windows resources 
such as brushes.

Hmmm bem�rk, at man for wpf class libraries og wpf applications kan V�LGE mellem om de skal basere sig p�
.Net 6.0 eller .Net Core. Begge virker tilsyneladende, og man kan ikke bare forlade sig p� at den s�tter en
passende default, da den tilsyneladende bare foresl�r det samme som man valgte tidligere.

S� sp�rgsm�let er: Hvorn�r er det en fordel at v�lge .Net 6.0 og hvorn�r er det en fordel at v�lge .Net Core 3.1?