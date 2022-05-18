using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using RtspViewer.WinUi.Constants;
using RtspViewer.WinUi.Models;
using RtspViewer.WinUi.Services;
using RtspViewer.WinUi.Views;

namespace RtspViewer.WinUi
{
    public partial class MainWindow : Window, INavigation, IThematic
    {
        public MainWindow(SettingsModel settings)
        {
            Title = "RTSP Viewer";
            InitializeComponent();
            ApplyTheme(settings.IsLightTheme);
            RebuildCameraMenuItems(settings);
        }

        public void ApplyTheme(bool isLightTheme)
        {
            Root.RequestedTheme = isLightTheme
                ? ElementTheme.Light
                : ElementTheme.Dark;
        }

        public void RebuildCameraMenuItems(SettingsModel settings) 
        {
            const int separatorIndex = 1;
            for (var menuItemIndex = NavigationView.MenuItems.Count - 1; menuItemIndex > separatorIndex; menuItemIndex--)
            {
                NavigationView.MenuItems.RemoveAt(menuItemIndex);
            }

            // No point adding a single camera page when the dashboard exists
            if (settings.Streams.Length <= 1) return;

            foreach (var streamModel in settings.Streams) 
            {
                var streamMenuItem = new NavigationViewItem
                {
                    Content = streamModel.Name,
                    Tag = streamModel.Identifier,
                    Icon = new FontIcon
                    {
                        Glyph = Glyphs.Video
                    }
                };
                NavigationView.MenuItems.Add(streamMenuItem);
            }
        }

        public NavigationViewItem GetCurrentNavigationViewItem()
        {
            return NavigationView.SelectedItem as NavigationViewItem;
        }

        public NavigationViewItem GetNavigationViewItem(Type type)
        {
            if (!typeof(Page).IsAssignableFrom(type))
            {
                throw new ArgumentException("Type must inherit Page", nameof(type));
            }

            return GetNavigationViewItems()
                .First(i => i.Tag.ToString() == type.Name);
        }

        public IEnumerable<NavigationViewItem> GetNavigationViewItems()
        {
            var menuItems = NavigationView.MenuItems
                .Select(menuItem => (NavigationViewItem)menuItem);

            var nestedMenuItems = menuItems
                .SelectMany(menuItem => menuItem.MenuItems
                   .Select(nestedMenuItem => (NavigationViewItem)nestedMenuItem));

            var footerItems = NavigationView.FooterMenuItems
                .Select(footerItem => (NavigationViewItem)footerItem);

            var nestedFooterItems = footerItems
                .SelectMany(footerItem => footerItem.MenuItems
                   .Select(nestedFooterItem => (NavigationViewItem)nestedFooterItem));

            return Enumerable.Empty<NavigationViewItem>()
                .Concat(menuItems)
                .Concat(nestedMenuItems)
                .Concat(footerItems)
                .Concat(nestedFooterItems);
        }

        public void SetCurrentNavigationViewItem<TType>() where TType : Page
        {
            var pageViewType = typeof(TType);
            var pageNavigationView = GetNavigationViewItem(pageViewType);
            NavigateToPage(pageViewType, pageNavigationView);
        }

        public void SetCurrentNavigationViewItem(NavigationViewItem item)
        {
            if (item == null)
            {
                return;
            }

            if (item.Tag == null)
            {
                return;
            }

            var menuItemIdentifier = item.Tag.ToString();
            var pageViewType = Guid.TryParse(menuItemIdentifier, out _)
                ? typeof(VideoPage)
                : GetPageType(menuItemIdentifier);

            NavigateToPage(pageViewType, item, menuItemIdentifier);
        }

        private void NavigateToPage(Type pageViewType, NavigationViewItem navigationViewItem, object parameter = null)
        {
            if (pageViewType == typeof(MainPage) ||
                pageViewType == typeof(VideoPage))
            {
                // Don't show page headers on the video views
                NavigationView.AlwaysShowHeader = false;
                NavigationView.Header = null;
                ContentFrame.Margin = new Thickness(0);
            }
            else
            {
                NavigationView.Header = navigationViewItem.Content;
                NavigationView.AlwaysShowHeader = true;
                ContentFrame.Margin = pageViewType == typeof(SettingsPage)
                    ? new Thickness(16)
                    : new Thickness(52, 16, 16, 16);
            }

            ContentFrame.Navigate(pageViewType, parameter, new DrillInNavigationTransitionInfo());
            NavigationView.SelectedItem = navigationViewItem;
        }

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            SetCurrentNavigationViewItem<MainPage>();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            SetCurrentNavigationViewItem(args.SelectedItemContainer as NavigationViewItem);
        }

        private Type GetPageType(string typeName)
        {
            return GetType().Assembly.GetTypes()
                .Where(type => typeof(Page).IsAssignableFrom(type))
                .First(type => type.Name.StartsWith(typeName));
        }
    }
}
