﻿namespace UnityEditor
{
    static class CustomRuleTileMenu
    {
        [MenuItem("Assets/Create/Custom Rule Tile Script", false, 89)]
        static void CreateCustomRuleTile()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Assets/2D Extras/Editor/Tiles/RuleTile/ScriptTemplates/NewCustomRuleTile.cs.txt", "NewCustomRuleTile.cs");
        }
    }
}
