using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class tracks random states for different procedural content generation systems like the level generator and the math exercise selector.
public enum RandomState {
    EXERCISE
}

public class RandomManager : MonoBehaviour {
    Dictionary<RandomState, System.Random> randoms = new Dictionary<RandomState, System.Random>();

    public float Range(RandomState randomState, float minInclusive, float maxInclusive) {
        System.Random random = GetState(randomState);
        double range = random.NextDouble();
        return (float)(minInclusive + range * (maxInclusive - minInclusive));
    }

    public int Range(RandomState randomState, int minInclusive, int maxExclusive) {
        System.Random random = GetState(randomState);
        return random.Next(minInclusive, maxExclusive);
    }

    private System.Random GetState(RandomState randomState) {
        if (randoms.TryGetValue(randomState, out System.Random state)) {
            return state;
        } else {
            randoms.Add(randomState, new System.Random());
            return randoms[randomState];
        }
    }

    public void InitializeStateRandomly(RandomState randomState) {
        InitializeState(randomState, (int)System.DateTime.Now.Ticks);
    }

    public void InitializeState(RandomState randomState, int seed) {
        randoms[randomState] = new System.Random(seed);
    }
}
