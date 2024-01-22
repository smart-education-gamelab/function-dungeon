using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExternalQuestionEditor {
    public partial class MainWindow : Window {
        public Data Data { get; }
        private WindowPageType currentWindowPage = WindowPageType.NONE;
        private WindowPage activeWindowPage = null;

        Dictionary<WindowPageType, WindowPage> windowPages = new Dictionary<WindowPageType, WindowPage>();

        public MainWindow() {
            InitializeComponent();
            Data = new Data();
            windowPages.Add(WindowPageType.WELCOME, new WelcomePage(this));
            windowPages.Add(WindowPageType.EDIT, new EditPage(this));
            SetPage(WindowPageType.WELCOME);
        }

        public void SetPage(WindowPageType page) {
            WindowPageType previousPage = currentWindowPage;
            if (page == currentWindowPage) return;
            activeWindowPage?.OnPageExit();
            if (!windowPages.ContainsKey(page)) throw new Exception($"Window page of type '{page}' has not been registered");
            activeWindowPage = windowPages[page];
            mainFrame.Content = activeWindowPage;
            currentWindowPage = page;
            activeWindowPage?.OnPageEnter(previousPage);
        }

        /*Window*/
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e) { SystemCommands.MinimizeWindow(this); }
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e) { SystemCommands.MaximizeWindow(this); }
        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e) { SystemCommands.RestoreWindow(this); }
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e) { SystemCommands.CloseWindow(this); }
        private void WindowStateChangeRaised(object sender, EventArgs e) {
            if (WindowState == WindowState.Maximized) {
                MainWindowBorder.BorderThickness = new Thickness(6);
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            } else {
                MainWindowBorder.BorderThickness = new Thickness(0);
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
        }

        private void Close_Application(object sender, RoutedEventArgs e) {
            App.Current.Shutdown();
        }

        private void mainFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e) {
            if (e.NavigationMode == NavigationMode.Back || e.NavigationMode == NavigationMode.Forward) {
                e.Cancel = true;
            }
        }

        private void HamburgerMenuItem_MouseDown(object sender, MouseButtonEventArgs e) {
            HamburgerMenuItem item = sender as HamburgerMenuItem;
            TextBlock tb = item.Content as TextBlock;
            switch (tb.Text) {
                case "Start": SetPage(WindowPageType.WELCOME); break;
                case "Thema": {
                        //App.ToggleTheme();
                        UpdateLayout();
                        InvalidateVisual();
                    }
                    break;
            }
        }
    }
}
