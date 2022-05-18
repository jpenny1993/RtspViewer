using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using RtspViewer.WinUi.Models;

namespace RtspViewer.WinUi.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private const int MaxAllowedStreams = 9;
        private readonly ObservableCollection<StreamViewModel> _items = new ObservableCollection<StreamViewModel>();

        [ObservableProperty]
        private bool _isLightTheme;

        //[ObservableProperty]
        //[AlsoNotifyChangeFor(nameof(HasCurrent))]
        private StreamViewModel? _current;

        //[ObservableProperty]
        //[AlsoNotifyChangeFor(nameof(FilteredItems))]
        private string? _filter;

        [ObservableProperty]
        private bool _isAddEnabled;

        public SettingsViewModel()
        {
            _items = new ObservableCollection<StreamViewModel>();
            _isAddEnabled = true;
        }

        public SettingsViewModel(SettingsModel model)
        {
            _isLightTheme = model.IsLightTheme;
            _items = new ObservableCollection<StreamViewModel>(model.Streams.Select(StreamViewModel.FromModel));
            CheckStreamLimit();
        }

        public ObservableCollection<StreamViewModel> Items => _items;

        public ObservableCollection<StreamViewModel> FilteredItems =>
            _filter is null
                ? _items
                : new ObservableCollection<StreamViewModel>(_items.Where(i => ApplyFilter(i, _filter)));

        public StreamViewModel? Current
        {
            get => _current;
            set
            {
                SetProperty(ref _current, value);
                OnPropertyChanged(nameof(HasCurrent));
            }
        }

        public string? Filter
        {
            get => _filter;
            set
            {
                var current = Current;

                SetProperty(ref _filter, value);
                OnPropertyChanged(nameof(FilteredItems));

                if (current is not null && FilteredItems.Contains(current))
                {
                    Current = current;
                }
            }
        }

        public bool HasCurrent => _current is not null;

        public StreamViewModel AddItem(StreamViewModel item)
        {
            _items.Add(item);
            if (_filter is not null)
            {
                OnPropertyChanged(nameof(_items));
            }

            CheckStreamLimit();

            return item;
        }

        public StreamViewModel UpdateItem(StreamViewModel item)
        {
            var hasCurrent = HasCurrent;

            var original = _items.First(i => i.Identifier == item.Identifier);
            var i = _items.IndexOf(original);
            _items[i] = item; // Raises CollectionChanged.

            if (_filter is not null)
            {
                OnPropertyChanged(nameof(_items));
            }

            if (hasCurrent && !HasCurrent)
            {
                Current = item;
            }

            return item;
        }

        public void DeleteItem(string identifier)
        {
            var item = _items.First(c => c.Identifier == identifier);
            _items.Remove(item);

            if (_filter is not null)
            {
                OnPropertyChanged(nameof(_items));
            }

            CheckStreamLimit();
        }

        public bool ApplyFilter(StreamViewModel item, string filter)
        {
            var term = filter?.Trim() ?? string.Empty;
            return item.Name.Contains(term, StringComparison.InvariantCultureIgnoreCase) ||
                   item.Address.Contains(term, StringComparison.InvariantCultureIgnoreCase);
        }

        private void CheckStreamLimit()
        {
            IsAddEnabled = Items.Count < MaxAllowedStreams;
        }

        public static SettingsModel ToModel(SettingsViewModel viewModel)
        {
            return new SettingsModel
            {
                IsLightTheme = viewModel.IsLightTheme,
                Streams = viewModel._items.Select(StreamViewModel.ToModel).ToArray()
            };
        }
    }
}
