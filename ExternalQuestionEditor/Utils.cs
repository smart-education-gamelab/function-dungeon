using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ExternalQuestionEditor {
    public static class Utils {
        public static void InvokeAndWaitForOnMainThread(Action action) {
            if (Thread.CurrentThread.ManagedThreadId == App.threadID) {
                action();
            } else {
                bool finished = false;
                App.Current.Dispatcher.Invoke(() => {
                    action();
                    finished = true;
                });

                while (!finished) Thread.Sleep(1);
            }
        }

        public static void SetSpinnerLoading(Grid grid, Spinner spinner, bool loading) {
            InvokeAndWaitForOnMainThread(() => {
                grid.Visibility = loading ? Visibility.Visible : Visibility.Collapsed;
                spinner.SetPlaying(loading);
            });
        }
    }
}
