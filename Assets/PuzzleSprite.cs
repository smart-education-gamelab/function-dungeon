using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSprite : MonoBehaviour {
    public SpriteRenderer spriteRenderer;
    public float targetWidth = 1.0f;

    public void SetSprite(Sprite newSprite) {
        spriteRenderer.sprite = newSprite;
        if (newSprite != null) {
            float scaleFactor = targetWidth / newSprite.bounds.size.x;
            spriteRenderer.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
        }
    }
}
