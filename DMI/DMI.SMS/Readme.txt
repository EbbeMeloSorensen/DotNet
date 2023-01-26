Howto: Build and execute DMI.SMS.UI.Console on a Linux machine using VS Code
1) Åbn Dotnet folderen i VS Code
2) Åbn en terminal i VS Code og naviger hen til folderen ./DotNet/DMI/DMI.SMS/DMI.SMS.UI.Console
3) Eksekver: dotnet build
   (Så bygger den bare konsol-applikationen samt dens afhængigheder - den kan ikke umiddelbart bygge ting, der targetter windows,
    dvs sørg for at bygge individuelle projekter frem for hele solutions)
4) Eksekver: dotner run
   (Den er sat op til at køre uden parametre, så det skulle virke - og så tæller den rækker i stationinformation-tabellen og
    laver i øvrigt et eksport af data til en fil ved navn SMSData.json)

I øvrigt: Find kildekoden til den gamle WPF-applikation, som du brugte til at køre konsistenscheck og mange andre ting..
          .. ligger den i DMI GitLab? (som du ikke kan connecte til i øjeblikket)
