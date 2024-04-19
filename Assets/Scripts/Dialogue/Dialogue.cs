﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public Content[] content;
}

[System.Serializable]
public class Content
{
    public Sprite sprite;

    public LocalizedString localizationKey;
    [NonSerialized] public string localizationOverride = "";
}
