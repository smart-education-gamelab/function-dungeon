using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum SymbolType {
    NORMAL,
    WILDCARD,
}

[Serializable]
public class ExtraTileData {
    public string information;
    public int roomIndex = 0;

    public ExtraTileData(string information, int roomIndex) {
        this.information = information;
        this.roomIndex = roomIndex;
    }
}

[Serializable]
public class TileDefinition {
    public int x;
    public int y;
    public char character;
    public ExtraTileData extraData;

    public TileDefinition(char tile, int x, int y) {
        this.character = tile;
        this.x = x;
        this.y = y;
        extraData = new ExtraTileData("", -1);
    }
}
