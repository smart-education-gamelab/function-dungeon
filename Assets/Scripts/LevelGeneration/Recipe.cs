using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.Rendering;

// The Recipe class defines a set of level generation rules, including (custom) grammar-based patterns and redirections to other recipes.

public enum RecipeType {
    GRAMMAR,
    CUSTOMGRAMMAR,
    REDIRECTION,
    NONE
}


[Serializable]
public class Recipe {
    public string name = "";
    public bool enabled = true;
    public RecipeType type;
    public int minTimesToExecute = -1;
    public int maxTimesToExecute = -1;
    public string redirectionName = "";
    public int amountOfRedirections = 1;
    public UnityEvent<LevelGenerator> onRecipeStartCallback = null;
    public UnityEvent<LevelGenerator> onRecipeEndCallback = null;
    public List<Grammar> grammars = new List<Grammar>();
    public List<UnityEvent<LevelGenerator, CustomGrammarEvent>> customGrammars = new List<UnityEvent<LevelGenerator, CustomGrammarEvent>>();
}
