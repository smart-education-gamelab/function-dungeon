using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGeneratorPreview : MonoBehaviour {
    public RawImage previewImage;

    public void SetImage(Texture2D image) {
        previewImage.texture = image;
        RectTransform rTransform = previewImage.transform as RectTransform;
        rTransform.sizeDelta = new Vector2(100 * image.width / image.height, 100);
    }

    public void Close() {
        Globals.UIManager.CloseMenu();
    }
}
