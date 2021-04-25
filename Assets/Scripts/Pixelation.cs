using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixelation : MonoBehaviour
{
    public int GUIDepth = 0;
    public RenderTexture renderTexture;
    public bool pixelate = true;

    void Start()
    {
        if (pixelate)
        {
            // Lets take a small number based on the real ratio.
            renderTexture.width = Screen.width / 4;
            renderTexture.height = Screen.height / 4;
        }
        else
        {
            renderTexture.width = Screen.width;
            renderTexture.height = Screen.height;
        }
    }

    void OnGUI()
    {
        GUI.depth = GUIDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), renderTexture);
    }

    int NearestSuperiorPowerOf2(int n)
    {
        return (int)Mathf.Pow(2, Mathf.Ceil(Mathf.Log(n) / Mathf.Log(2)));
    }
}