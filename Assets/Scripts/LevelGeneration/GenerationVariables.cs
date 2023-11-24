using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class LearningGoalSectionDefinition : ICloneable {
    [JsonProperty("-")]
    public int min;
    [JsonProperty("+")]
    public int max;

    public LearningGoalSectionDefinition() { }
    public LearningGoalSectionDefinition(int min, int max) {
        this.min = min;
        this.max = max;
    }

    public object Clone() {
        return MemberwiseClone();
    }
}

//This class contains all variables that can determine how a level is generated.
[Serializable]
public class GenerationVariables : ICloneable {
    [JsonProperty("l")]
    public List<LearningGoalSectionDefinition> learningGoalSections = new List<LearningGoalSectionDefinition>();
    [JsonProperty("c")]
    public int complexity = 50;
    [JsonProperty("d")]
    public int deadEnds = 0;
    [JsonProperty("s")]
    public int seed = 0;
    [JsonProperty("r")]
    public int amountOfRooms = 5;
    [JsonProperty("q")]
    public bool randomizedQuestions = true;

    public GenerationVariables(bool addDefault) {
        learningGoalSections.Add(new LearningGoalSectionDefinition(0, Constants.learningGoalLevels.Count - 1));
    }

    public object Clone() {
        GenerationVariables newVariables = (GenerationVariables)MemberwiseClone();
        newVariables.learningGoalSections = new List<LearningGoalSectionDefinition>();
        foreach (var t in learningGoalSections) {
            newVariables.learningGoalSections.Add((LearningGoalSectionDefinition)t.Clone());
        }
        return newVariables;
    }

    public static List<string> CheckIfEnoughExercises(QuestionList list, List<LearningGoalSectionDefinition> defs, int roomCount) {
        List<int> sections = new List<int>();
        for (int i = 0; i < roomCount; i++) sections.Add(GetLearningGoalSectionIndexForRoom(defs, i, roomCount));

        var sectionOccurences = Utils.CountOccurrences(sections);
        var availableSectionOccurences = Utils.CountOccurrences(list.questions.Where(x => x.enabled).Select(x => x.learningGoalLevel).ToList());

        List<string> invalidSections = new List<string>();
        foreach (var section in sectionOccurences) {
            if (availableSectionOccurences[section.Key] < section.Value) invalidSections.Add(Constants.learningGoalLevels[section.Key]);
        }

        return invalidSections;
    }

    //Eetermine what learning goal is selected for room i when there are roomCount rooms.
    public static int GetLearningGoalSectionIndexForRoom(List<LearningGoalSectionDefinition> defs, int i, int roomCount) {
        HashSet<int> indicesSet = new HashSet<int>();
        foreach (LearningGoalSectionDefinition def in defs) {
            for (int j = def.min; j <= def.max; j++) {
                indicesSet.Add(j);
            }
        }

        List<int> indices = indicesSet.ToList();

        if (indices.Count == 0) {
            for (int j = 0; j < Constants.learningGoalLevels.Count; j++) indices.Add(j);
        }

        float stepSize = (indices.Count - 1) / (float)(roomCount - 1);
        int index = Mathf.RoundToInt(i * stepSize);

        return indices[index];
    }

    public static string GetLearningGoalSectionForRoom(List<LearningGoalSectionDefinition> defs, int i, int roomCount) {
        return Constants.learningGoalLevels[GetLearningGoalSectionIndexForRoom(defs, i, roomCount)];
    }
}