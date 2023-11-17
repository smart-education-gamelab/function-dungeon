using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Terminal represents a terminal symbol in the alphabet used for generating levels.
[Serializable]
public class Symbol {
    public string name;
    public char character;
    public SymbolType type;
    public TileBase tileBase;

    public Symbol() {
        name = "";
        character = '\0';
        type = SymbolType.NORMAL;
        tileBase = null;
    }
}