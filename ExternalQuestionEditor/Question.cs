using System;
using System.Collections.Generic;

namespace ExternalQuestionEditor {
    [Serializable]
    public class QuestionText : ICloneable {
        public string locale;
        public string question = "";
        public string correct = "";
        public string wrong1 = "";
        public string wrong2 = "";
        public string wrong3 = "";
        public string feedback = "";

        public object Clone() {
            return MemberwiseClone();
        }
    }


    public class Question {
        public string Name { get; set; }
        public int uniqueIdentifier;
        public QuestionType questionType;
        public FDImage image;

        public List<QuestionText> text = new List<QuestionText>();

        public string section = ""; //The section is now stored in the learning goal level. We no longer use this variable but since it was defined in the excel sheets we'll keep it just in case.
        public string variation = "";
        public int learningGoalLevel = 0;

        public Question(string name) {
            this.Name = name;
        }

        public Question() { }
    }
}
