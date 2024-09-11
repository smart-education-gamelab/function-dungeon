using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PreviewCamera : MonoBehaviour {
    public void GeneratePreview() {
        LevelGenerator lg = FindObjectOfType<LevelGenerator>();

        float minX = lg.realXMin;
        float maxX = lg.realXMax + 1;
        float minY = lg.realYMin;
        float maxY = lg.realYMax + 1;

        float centerX = (minX + maxX) / 2f;
        float centerY = (minY + maxY) / 2f;

        Camera camera = GetComponent<Camera>();
        camera.transform.position = new Vector3(centerX, centerY, -10);

        float verticalRange = maxY - minY;
        camera.orthographicSize = verticalRange / 2f;

        int width = (int)((maxX - minX) * 50); //50 pixels per unit
        int height = (int)((maxY - minY) * 50);
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        renderTexture.filterMode = FilterMode.Bilinear;

        camera.targetTexture = renderTexture;
        camera.Render();

        Texture2D texture2D = RenderTextureToTexture2D(renderTexture);
        //SaveTextureToFile(texture2D, "LevelPreview.png");

        FindObjectOfType<LevelGeneratorPreview>(true).SetImage(texture2D);

        // Clean up
        camera.targetTexture = null;
        RenderTexture.active = null; // Added to avoid leaking the render texture.
        Destroy(renderTexture);

        Globals.UIManager.SetMenu("LevelGeneratorPreview");
    }

    Texture2D RenderTextureToTexture2D(RenderTexture renderTexture) {
        Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();
        return texture2D;
    }

    void SaveTextureToFile(Texture2D texture, string fileName) {
        byte[] bytes = texture.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, fileName);
        File.WriteAllBytes(path, bytes);
        Debug.Log($"Saved image to {path}");

        // Import asset
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.ImportAsset(path); // Make sure Unity imports the created file
#endif
    }

    void Update() {

    }
}
