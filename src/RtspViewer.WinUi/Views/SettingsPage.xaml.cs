using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using RtspViewer.WinUi.ViewModels;

namespace RtspViewer.WinUi.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private static App App => Application.Current as App;

        public ICommand AddStreamCommand => new RelayCommand(() =>
        {
            ViewModel.Current = ViewModel.AddItem(new StreamViewModel 
            {
                Name = $"Stream {ViewModel.Items.Count + 1}"
            });
        });

        public ICommand EditStreamCommand => new RelayCommand(() =>
        {
            if (ViewModel.Current?.Identifier is not null)
            {
                ViewModel.UpdateItem((StreamViewModel)ActiveStreamGrid.DataContext);
            }
        });

        public ICommand CancelEditStreamCommand => new RelayCommand(() =>
        {
            ViewModel.Current = null;
        });

        public ICommand DeleteStreamCommand => new RelayCommand<string>((string identifier) =>
        {
            if (identifier is not null)
            {
                ViewModel.DeleteItem(identifier);
            }
        });

        public ICommand ToggleThemeCommand => new RelayCommand(() => 
        {
            ViewModel.IsLightTheme = !ViewModel.IsLightTheme;
            App.Theme.ApplyTheme(ViewModel.IsLightTheme);
        });

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = new SettingsViewModel(App.Settings);
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            App.SaveSettings(SettingsViewModel.ToModel(ViewModel));
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            base.OnNavigatingFrom(e);
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.Current) && ViewModel.HasCurrent)
            {
                StreamListView.ScrollIntoView(ViewModel.Current);
            }
        }

        private void ListViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType is PointerDeviceType.Mouse or PointerDeviceType.Pen)
            {
                VisualStateManager.GoToState(sender as Control, "HoverButtonsShown", true);
            }
        }

        private void ListViewItem_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(sender as Control, "HoverButtonsHidden", true);
        }

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ViewModel.Filter = args.QueryText;
        }

        private void StreamListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.HasCurrent)
            {
                ActiveStreamGrid.DataContext = ViewModel.Current.Clone();
            }
            else 
            {
                ActiveStreamGrid.DataContext = new StreamViewModel();
            }
        }
    }
}
