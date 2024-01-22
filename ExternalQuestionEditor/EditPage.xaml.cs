using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExternalQuestionEditor {
    public partial class EditPage : WindowPage {
        public ObservableCollection<Question> SortedQuestions{ get; set; }

        int index = 100;
        public EditPage(MainWindow window) : base(window) {
            InitializeComponent();
            SortedQuestions = new ObservableCollection<Question>();
            DataContext = this;
            for(int i = 0; i < 100; i++) {
                SortedQuestions.Add(new Question("Question " + i));
            }
        }

        public override void OnPageEnter(WindowPageType previousPage) {

        }

        public override void OnPageExit() {

        }

        private void QuestionsSelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void AddClick(object sender, RoutedEventArgs e) {
            SortedQuestions.Add(new Question("Question " + index++));
            Console.WriteLine(SortedQuestions.Count);
        }

        private void RemoveClick(object sender, RoutedEventArgs e) {
            var itemsToRemove = new List<Question>();

            foreach (var selected in questionsList.SelectedItems) {
                itemsToRemove.Add((Question)selected);
            }

            foreach (var item in itemsToRemove) {
                SortedQuestions.Remove(item);
            }
        }
    }
}
