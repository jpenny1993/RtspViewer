using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace RtspViewer.WinUi.Services
{
    public interface INavigation
    {
        NavigationViewItem GetCurrentNavigationViewItem();

        NavigationViewItem GetNavigationViewItem(Type type);

        IEnumerable<NavigationViewItem> GetNavigationViewItems();

        void SetCurrentNavigationViewItem<TType>() where TType : Page;

        void SetCurrentNavigationViewItem(NavigationViewItem item);
    }
}
