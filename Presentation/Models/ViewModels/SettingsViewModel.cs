using System.ComponentModel;
using System.Windows.Input;

namespace Presentation.Models.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly Settings _settings = App.appSettings;
        // Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                if (_currentView != value)
                {
                    _currentView = value;
                    OnPropertyChanged(nameof(CurrentView));
                }
            }
        }

        public ICommand NavigateCommand { get; }

        public SettingsViewModel()
        {
            // Use the custom RelayCommand class
            NavigateCommand = new RelayCommand<string>(Navigate);

            // Initial view creation and DataContext setup
            _currentView = _settings.GeneralSettings;
        }

        private void Navigate(string viewName)
        {
            // The new object must be a WPF UserControl for content to display
            object? newView;

            switch (viewName)
            {
                case "BarSpotter":
                    newView = _settings.BarSpotterSettings;
                    break;
                case "FuelCalculator":
                    newView = _settings.FuelSettings;
                    break;
                case "InputGraph":
                    newView = _settings.InputGraphSettings;
                    break;
                case "Wind":
                    newView = _settings.WindSettings;
                    break;
                default:
                    newView = _settings.GeneralSettings;
                    break;
            }

            CurrentView = newView;

            // // Check if the instantiated object is a WPF control/element
            // if (newView is UserControl userControl)
            // {
            //     // // 1. Manually set the DataContext of the new View (UserControl)
            //     // // to the current SettingsViewModel instance ('this').
            //     // userControl.DataContext = this;
            //
            //     // 2. Set CurrentView, which triggers the ContentControl in the MainWindow
            //     // to display the newly configured UserControl.
            //     CurrentView = userControl;
            // }
        }
    }
}
