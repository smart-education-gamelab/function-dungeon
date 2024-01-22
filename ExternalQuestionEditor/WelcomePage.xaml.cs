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
    public partial class WelcomePage : WindowPage {
        private Brush normalTextColor;

        public WelcomePage(MainWindow window) : base(window) {
            InitializeComponent();
            normalTextColor = fileTextBox.Foreground;
        }

        public override void OnPageEnter(WindowPageType previousPage) {
            Utils.SetSpinnerLoading(loadingOverlay, spinner, false);
        }

        public override void OnPageExit() {
        }

        private void Rectangle_Drop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (string.Equals(System.IO.Path.GetExtension(files[0]), ".fdq", StringComparison.OrdinalIgnoreCase)) {
                    window.Data.SelectedFile = files[0];
                    fileTextBox.Text = System.IO.Path.GetFileName(files[0]);
                    fileTextBox.Foreground = normalTextColor;
                }
            }
        }

        private void OpenFileClick(object sender, RoutedEventArgs e) {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "fdq files (*.fdq)|*.fdq";
            dialog.Title = "Please select a Function Dungeon Questions file to open.";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                window.Data.SelectedFile = dialog.FileName;
                fileTextBox.Text = System.IO.Path.GetFileName(dialog.FileName);
                fileTextBox.Foreground = normalTextColor;
            }
        }

        private void NieuwClick(object sender, RoutedEventArgs e) {
            window.SetPage(WindowPageType.EDIT);
        }
    }
}
