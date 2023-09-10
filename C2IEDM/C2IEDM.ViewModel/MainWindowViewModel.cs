using GalaSoft.MvvmLight;
using Craft.Logging;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using C2IEDM.Persistence;

namespace C2IEDM.ViewModel;

public class MainWindowViewModel : ViewModelBase
{
    private readonly Application.Application _application;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IDialogService _applicationDialogService;
    private readonly ILogger _logger;

    public LogViewModel LogViewModel { get; }

    public MainWindowViewModel(
        IUnitOfWorkFactory unitOfWorkFactory,
        IDialogService applicationDialogService,
        ILogger logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _applicationDialogService = applicationDialogService;

        _application = new Application.Application(
            unitOfWorkFactory,
            logger);

        LogViewModel = new LogViewModel();
        _logger = new ViewModelLogger(logger, LogViewModel);
    }
}