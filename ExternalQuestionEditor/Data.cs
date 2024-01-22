using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalQuestionEditor {
    public class Data {
        public string SelectedFile { get; set; }
        public List<string> Locales { get; set; }
        public List<Question> Questions {get; set; }
    }
}
