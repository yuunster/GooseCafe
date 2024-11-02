using UnityEngine;
using System.IO;

public class ModelPreviewGenerator : MonoBehaviour
{
    public GameObject model;             // The model to capture
    public Vector2Int resolution = new Vector2Int(512, 512); // Set the resolution of the preview image

    public Camera previewCamera;
    private RenderTexture renderTexture;

    void Start()
    {
        SaveModelPreview();
    }

    public Texture2D GeneratePreview()
    {
        // Create a RenderTexture
        renderTexture = new RenderTexture(resolution.x, resolution.y, 16);
        previewCamera.targetTexture = renderTexture;

        // Render the camera view to the RenderTexture
        previewCamera.Render();

        // Convert RenderTexture to Texture2D
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // Now you have a Texture2D of the model view, you can save it to a file, display it, etc.
        return texture;
    }

    public void SaveTextureToFile(Texture2D texture, string filePath, bool asPNG = true)
    {
        byte[] bytes;

        // Encode texture to PNG or JPEG format
        if (asPNG)
        {
            bytes = texture.EncodeToPNG();
        }
        else
        {
            bytes = texture.EncodeToJPG();
        }

        // Write the encoded byte array to a file
        File.WriteAllBytes(filePath, bytes);

        Debug.Log($"Texture saved to {filePath}");
    }

    void SaveModelPreview()
    {
        // Generate the preview texture
        Texture2D previewTexture = GeneratePreview();

        // Define the file path
        string filePath = Path.Combine(Application.dataPath + "/OrderPreviews", $"{model.name}_preview.png");

        // Save the texture as a PNG file
        SaveTextureToFile(previewTexture, filePath, asPNG: true);

        // Clean up the Texture2D to prevent memory leaks
        Destroy(previewTexture);
    }
}
