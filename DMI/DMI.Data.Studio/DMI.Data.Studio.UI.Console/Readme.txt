Eksempler på brug:

DMI.Data.Studio.UI.Console extract -c m
DMI.Data.Studio.UI.Console lunch
DMI.Data.Studio.UI.Console die

Troubleshooting:
* Når jeg kører DMI.Data.Studio.UI.Console eller DMI.Data.Studio.UI.WPF mod en sqlite database, så oplever jeg af og til,
  at jeg får fejlen: "~Failed because of current state of the object". Senere kan det så virke, uden at jeg har lavet nogen 
  kodeændringer. Jeg mistænker, at det kan skyldes, at de 2 filer (vistnok obsdb_shm osv) bliver korrupte, så måske hænger
  det sammen med det. Jeg har også en mistanke om at det kan skyldes, at den ikke kan have det hele i memory, så man kan også
  prøve at lukke Chrome og/eller andre programmer.