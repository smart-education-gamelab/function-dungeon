using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class StopwatchWrapper {
    private readonly Stopwatch sw;

    public StopwatchWrapper() {
        sw = new Stopwatch();
        sw.Start();
    }

    public void Print(string name = "") {
        sw.Stop();
        if (name.Length == 0) UnityEngine.Debug.Log($"Took {sw.ElapsedMilliseconds}ms");
        else UnityEngine.Debug.Log($"{name} took {sw.ElapsedMilliseconds}ms");
        sw.Restart();
    }

    public long Get() {
        sw.Stop();
        long elapsed = sw.ElapsedMilliseconds;
        sw.Restart();
        return elapsed;
    }
}

public static class LevelGenerationUtils {
    public static string RotateString90(string toRotate, int width, int height) {
        StringBuilder rotatedGrid = new StringBuilder(toRotate.Length);

        for (int x = 0; x < width; x++) {
            for (int y = height - 1; y >= 0; y--) {
                int index = y * width + x;
                rotatedGrid.Append(toRotate[index]);
            }
        }

        return rotatedGrid.ToString();
    }

    public static string FlipStringHorizontal(string toFlip, int x, int y) {
        int size = x * y;
        char[] newStr = new char[size];

        for (int yy = 0; yy < y; yy++) {
            for (int xx = 0; xx < x; xx++) {
                newStr[xx + (yy * x)] = toFlip[((x - 1) - xx) + (yy * x)];
            }
        }

        return new string(newStr);
    }

    public static string FlipStringVertical(string toFlip, int x, int y) {
        int size = x * y;
        char[] newStr = new char[size];

        for (int yy = 0; yy < y; yy++) {
            for (int xx = 0; xx < x; xx++) {
                newStr[xx + (yy * x)] = toFlip[xx + ((y - yy - 1) * x)];
            }
        }

        return new string(newStr);
    }

    public static Dictionary<K, V> ListToDictionary<K, V>(List<V> list, string name, Func<V, K> listEntryToDictionaryKey) {
        Dictionary<K, V> dict = new Dictionary<K, V>();
        foreach (V entry in list) {
            K key = listEntryToDictionaryKey(entry);
            if (dict.ContainsKey(key)) {
                UnityEngine.Debug.LogError($"Duplicate dictionary entry found in {name}: {key}");
            } else {
                dict.Add(key, entry);
            }
        }
        return dict;
    }

    /*Maps a float from one range to another*/
    public static float Remap(float val, float minIn, float maxIn, float minOut, float maxOut) {
        return minOut + (val - minIn) * (maxOut - minOut) / (maxIn - minIn);
    }
}
