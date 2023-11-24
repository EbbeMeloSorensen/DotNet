Notice that all utility projects are based on .Net Standard 2.1, so they don't depend on the .Net Framework.
As such they are also supported by Visual Studio Code. Some ViewModel projects also depend on .Net Standard 2.1,
whereas others depend on .Net 6.0-windows. This is usually the case when the view model contains windows resources 
such as brushes.

Console applications are based on .Net 6.0, because then they also run on my Linux laptop.

Wpf Class libraries and Wpf Applications are based on .Net 6.0.

Unit Test projects are based on xUnit, because it supports Linux, and generally seems to be more popular than e.g.
MSTest

Generally, class libraries are based on on .Net Standard 2.1 when possible, and otherwise .Net 6.0.

Hmmm bem�rk, at man for wpf class libraries og wpf applications kan V�LGE mellem om de skal basere sig p�
.Net 6.0 eller .Net Core. Begge virker tilsyneladende, og man kan ikke bare forlade sig p� at den s�tter en
passende default, da den tilsyneladende bare foresl�r det samme som man valgte tidligere.

S� sp�rgsm�let er: Hvorn�r er det en fordel at v�lge .Net 6.0 og hvorn�r er det en fordel at v�lge .Net Core 3.1?

Iagttagelse: Man kan GODT eksekvere en WPF applikation fra Visual Studio Code. I hvert fald n�r:
* Man arbejder p� en Windows pc
* Applikationen baserer sig p� .Net 6.0

Pr definition s� svarer origo (x = 0) til seneste midnat, der er passeret (ved ikke om det er hensigtsm�ssigt)
og pr definition s� svarer x = 1 til kommende midnat

Hvis du regner om fra et tidspunkt til en x v�rdi, g�r du s�ledes:
x = (t - t_origo) / TimeSpan.FromDays(1)

og fra x til tid s�ledes:
t = t_origo + TimeSpan.FromDays(x)

1 2 3 4 5 6 7 .. 30 31 1

1 3 5 7 9 .. 29 1

1 6 11 16 21 26 31

1 11 21 31 
