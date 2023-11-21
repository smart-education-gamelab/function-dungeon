using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

/*This class defines a grammar rule which consists of a pattern, one or more rewrite patterns and the width & height of said pattern.
 * A pattern is a string representation of a grid of symbols, where each symbol represents a tile type.
 * Rewrite patterns are alternative patterns that can replace the original patterns.
 */

// Class representing a single pattern variation
[Serializable]
public class PatternVariation {
    public string pattern;
    public int width, height;

    public PatternVariation(string pattern, int width, int height) {
        this.pattern = pattern;
        this.width = width;
        this.height = height;
    }
}

// Class representing a pair of pattern variations, including the original pattern and its rewrite.
[Serializable]
public class PatternVariationPair {
    public string pattern;
    public string rewritePattern;
    public int width, height;

    public PatternVariationPair(string pattern, string rewritePattern, int width, int height) {
        this.pattern = pattern;
        this.rewritePattern = rewritePattern;
        this.width = width;
        this.height = height;
    }
}

class PatternVariationPairComparer : IEqualityComparer<PatternVariationPair> {
    public bool Equals(PatternVariationPair x, PatternVariationPair y) {
        return x.pattern == y.pattern && x.rewritePattern == y.rewritePattern && x.width == y.width && x.height == y.height;
    }

    public int GetHashCode(PatternVariationPair obj) {
        return obj.pattern.GetHashCode() + obj.rewritePattern.GetHashCode() + obj.width.GetHashCode() + obj.height.GetHashCode();
    }
}

public class GrammarPlacedEvent {
    public TileDefinition tile;
    public int executionCount;
    public char newCharacter;
    public readonly string rewritePattern;

    public GrammarPlacedEvent(TileDefinition tile, char newSymbol, int executionCount, string rewritePattern) {
        this.tile = tile;
        this.newCharacter = newSymbol;
        this.executionCount = executionCount;
        this.rewritePattern = rewritePattern;
    }
}

public class CustomGrammarEvent {
    public TileDefinition tile;
    public char newSymbol;
    public bool shouldStop;

    public CustomGrammarEvent(TileDefinition tile) {
        this.tile = tile;
        this.newSymbol = tile.character;
        this.shouldStop = false;
    }
}

[Serializable]
public class Grammar {
    public string name;
    public bool enabled = true;
    public int width = 0;
    public int height = 0;
    public string pattern = "";
    public string[] rewritePatterns = new string[0];
    public UnityEvent<LevelGenerator, GrammarPlacedEvent> patternPlacedCallback;

    [NonSerialized] public List<PatternVariationPair> patternVariations = new List<PatternVariationPair>();


    /*Method to generate all possible variations of a grammar rule. Possible variations are vertical & horizontal flip and all rotations of them.
     Duplicate variations are removed as a performance optimization.
    */
    public void GenerateVariations() {
        patternVariations = new List<PatternVariationPair>();
        int size = width * height;
        string cleanPattern = pattern.Replace(" ", "");
        if (cleanPattern.Length != size) UnityEngine.Debug.LogError($"Grammar {name} string length doesn't match: {rewritePatterns[0].Length} != {size}");

        List<PatternVariation> roughPatternVariations = GetAllVariations(cleanPattern);
        List<List<PatternVariation>> roughRewritePatternsVariations = new List<List<PatternVariation>>();

        int index = 0;
        foreach (string rewritePattern in rewritePatterns) {
            cleanPattern = rewritePattern.Replace(" ", "");
            if (cleanPattern.Length != size) UnityEngine.Debug.LogError($"Grammar {name}:{index} string length doesn't match: {cleanPattern.Length} != {size}");
            roughRewritePatternsVariations.Add(GetAllVariations(rewritePattern.Replace(" ", "")));
            index++;
        }

        for (int i = 0; i < roughRewritePatternsVariations.Count; i++) {
            List<PatternVariation> rewritePatternVariations = roughRewritePatternsVariations[i];

            for (int j = 0; j < rewritePatternVariations.Count; j++) {
                PatternVariation patternVariation = roughPatternVariations[j];
                PatternVariation rewritePatternVariation = rewritePatternVariations[j];
                patternVariations.Add(new PatternVariationPair(patternVariation.pattern, rewritePatternVariation.pattern, patternVariation.width, patternVariation.height));
            }
        }
        patternVariations = patternVariations.Distinct(new PatternVariationPairComparer()).ToList();
    }

    private List<PatternVariation> GetRotationVariations(PatternVariation inputPattern) {
        PatternVariation str90 = new PatternVariation(LevelGenerationUtils.RotateString90(inputPattern.pattern, inputPattern.width, inputPattern.height), inputPattern.height, inputPattern.width);
        PatternVariation str180 = new PatternVariation(LevelGenerationUtils.RotateString90(str90.pattern, inputPattern.height, inputPattern.width), inputPattern.width, inputPattern.height);
        PatternVariation str270 = new PatternVariation(LevelGenerationUtils.RotateString90(str180.pattern, inputPattern.width, inputPattern.height), inputPattern.height, inputPattern.width);
        return new List<PatternVariation> { inputPattern, str90, str180, str270 };
    }

    private List<PatternVariation> GetAllVariations(string inputPattern) {
        string flippedHorizontal = LevelGenerationUtils.FlipStringHorizontal(inputPattern, width, height);
        string flippedVertical = LevelGenerationUtils.FlipStringVertical(inputPattern, width, height);
        List<PatternVariation> allVariations = new List<PatternVariation>();
        allVariations.AddRange(GetRotationVariations(new PatternVariation(inputPattern, width, height)));
        allVariations.AddRange(GetRotationVariations(new PatternVariation(flippedHorizontal, width, height)));
        allVariations.AddRange(GetRotationVariations(new PatternVariation(flippedVertical, width, height)));
        return allVariations;
    }
}
