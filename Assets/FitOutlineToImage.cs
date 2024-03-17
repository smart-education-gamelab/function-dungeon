using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FitOutlineToImage : MonoBehaviour {
    public RawImage image;

    void Update() {
        RectTransform rTransform = transform as RectTransform;
        rTransform.sizeDelta = (image.transform as RectTransform).sizeDelta + new Vector2(10, 10);
    }
}
