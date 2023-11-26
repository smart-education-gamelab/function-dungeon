using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelGenerationMenu : MonoBehaviour {
    public TextMeshProUGUI roomsValue, deadEndsValue, complexityValue, seedValue;
    public Slider rooms, deadEnds, complexity, seed;
    public GameObject togglePrefab; // Prefab of your toggle
    public Transform contentPanel;
    private List<Toggle> toggles = new List<Toggle>();

    private GenerationVariables variables;

    private void Start() {
        rooms.value = 3;
        deadEnds.value = 0;
        complexity.value = 50;
        seed.value = Random.Range(1000, 99999);

        OnVariableChanged();
        int index = 0;
        foreach (string section in Constants.learningGoalLevels) {
            GameObject newToggle = Instantiate(togglePrefab, contentPanel);
            newToggle.GetComponentInChildren<TextMeshProUGUI>().text = section;
            toggles.Add(newToggle.GetComponent<Toggle>());
            toggles.Last().isOn = index < 3;
            index++;
        }
    }

    public void Generate() {
        variables = new GenerationVariables();
        variables.seed = (int)seed.value;
        variables.deadEnds = (int)deadEnds.value;
        variables.complexity = (int)complexity.value;
        variables.amountOfRooms = (int)rooms.value;

        for (int i = 0; i < toggles.Count; i++) {
            if (toggles[i].isOn) {
                variables.learningGoalSections.Add(new LearningGoalSectionDefinition(i, i));
            }
        }

        Globals.UIManager.CloseMenu();
        Globals.SetLevelGenerationVariables(variables);
        Globals.SceneManager.SetScene("LevelGeneration");
    }

    public void Back() {
        Globals.UIManager.SetMenu("Main");
    }

    public void OnVariableChanged() {
        roomsValue.text = $"[{rooms.value}]";
        deadEndsValue.text = $"[{deadEnds.value}%]";
        complexityValue.text = $"[{complexity.value * 2}%]";
        seedValue.text = $"[{seed.value}]";
    }
}
