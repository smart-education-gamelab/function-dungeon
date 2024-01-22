using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

/*A window page lives within the main window. The main window handles switching between different window pages.
 */
namespace ExternalQuestionEditor {
    public abstract class WindowPage : Page {
        protected MainWindow window;
        public WindowPage(MainWindow window) {
            this.window = window;
        }

        public abstract void OnPageEnter(WindowPageType previousPage);
        public abstract void OnPageExit();
    }
}
