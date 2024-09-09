/*
  This sample code is for demonstrating and testing the functionality
  of Unity Capture, and is placed in the public domain.

  This code generates a scrolling color texture simply for the purposes of demonstration.
  Other uses may include sending a video, another webcam feed or a static image to the output.
*/

using UnityEngine;

public class CaptureTexture : MonoBehaviour
{
    public int width = 320;
    public int height = 240;
    public MeshRenderer outputRenderer;
    Texture2D activeTex;
    UnityCaptures.Interface captureInterface;
    int y = 0;
    Color color = Color.red;

    void Start()
    {
        // Create texture and capture interface
        activeTex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        captureInterface = new UnityCaptures.Interface(UnityCaptures.ECaptureDevice.CaptureDevice1);

        if (outputRenderer != null) outputRenderer.material.mainTexture = activeTex;
    }

    void OnDestroy()
    {
        //Cleanup capture interface
        captureInterface.Close();
    }

    void Update()
    {
        // Draw next line on texture 
        for (int x = 0; x < width; x++)
        {
            activeTex.SetPixel(x, y, color);
        }

        y += 1;
        if (y > height)
        {
            y = 0;
            color = new Color(color.g, color.b, color.r);
        }

        activeTex.Apply();

        // Update the capture texture
        UnityCaptures.ECaptureSendResult result = captureInterface.SendTexture(activeTex);
        if (result != UnityCaptures.ECaptureSendResult.SUCCESS)
            Debug.Log("SendTexture failed: " + result);
    }
}
