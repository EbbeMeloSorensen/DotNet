using System.Diagnostics;
using GalaSoft.MvvmLight.Command;
using Craft.ViewModels.Dialogs;

namespace DMI.Data.Studio.ViewModel
{
    public class AboutDialogViewModel : DialogViewModelBase
    {
        public string ApplicationTitle { get; private set; }
        public string Version { get; private set; }
        public string Description { get; private set; }
        private RelayCommand _openTutorialCommand;

        public RelayCommand OpenTutorialCommand
        {
            get { return _openTutorialCommand ?? (_openTutorialCommand = new RelayCommand(OpenTutorial)); }
        }

        public AboutDialogViewModel()
        {
            ApplicationTitle = "DMI Data Studio";
            Version = "0.1";
            Description =
                "Denne applikation er en prototype, hvormed man bl.a. kan inspicere indholdet af databaserne sms_prod, statdb og obsdb samt belyse eventuelle uoverensstemmelser mellem databaserne." +
                " sms_prod er den database, der administreres af SMS systemet (Station Management System), som blev leveret af Cowi i efteråret 2018." +
                " sms_prod omfatter pt danske, grønlandske og færøske stationer." +
                " statdb og obsdb indgår i nanoq-miljøet hos dmi og bruges henholdsvis til adminstration af stationer på globalt plan og administration af observationer modtaget fra disse stationer." +
                " Stationerne i sms-prod findes både i sms_prod og i statdb. sms_prod er i denne henseende master database, og dagligt opdateres statdb med eventuelle nye, opdaterede eller nedlagte stationer fra sms_prod.\n\n" +
                "Formålet med denne applikation er bl.a. at illustrere måder hvorpå man kan inspicere data om stationer og observationer. Det er ligeledes formålet at illustrere måder hvorpå man kan identificere og evt korrigere uoverensstemmelser mellem databaserne.\n\n" +
                "Som sådan er applikationens formål at tjene som en del af fundamentet for identifikation af de funktionelle behov for et udvidet station management system hos DMI." +
                " Den grundlæggende tanke bag dette approach er, at man som bruger ofte er mere bevidst om behovene for et givet system, hvis man har mulighed for at interagere med en mockup, som løbende kan tilpasses og udbygges på basis af feedback fra brugeren.";
        }

        private void OpenTutorial()
        {
            Process.Start("https://confluence.dmi.dk/display/~ebs/DMI+Data+Studio");
        }
    }
}
