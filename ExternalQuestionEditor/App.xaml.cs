using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ExternalQuestionEditor {
    public partial class App : Application {
        public static int threadID;
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            threadID = Thread.CurrentThread.ManagedThreadId;
        }
    }
}
