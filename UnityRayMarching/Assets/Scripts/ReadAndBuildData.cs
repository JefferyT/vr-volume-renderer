using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadAndBuildData
{

    Texture3D texture;
    const string path = @"C:\Users\Husky Maker\Desktop\Xiao\vr-volume-rendering\VolumeRenderCStuff\body.dat";
    SortedList<float, float> opacityDic = new SortedList<float, float>() {
        { -2048, 0 },
        { 142.677f, 0 },
        { 145.016f, 0.116071f },
        { 192.174f, 0.5625f },
        { 217.24f, 0.776786f },
        { 384.347f, 0.830357f },
        { 3661, 0.830357f },
    };
    SortedList<float, Vector3> colorDic = new SortedList<float, Vector3>() {
        { -2048, new Vector3(0, 0, 0) },
        { 142.677f, new Vector3(0, 0, 0) },
        { 145.016f, new Vector3(0.615686f, 0, 0.0156863f) },
        { 192.174f, new Vector3(0.909804f, 0.454902f, 0) },
        { 217.24f, new Vector3(0.972549f, 0.807843f, 0.611765f) },
        { 384.347f, new Vector3(0.909804f, 0.909804f, 1) },
        { 3661, new Vector3(1, 1, 1)},
    };
    void main()
    {
        
    }

    Texture3D CreateTexture3D(int x, int y, int z)
    {
        Color[] colorArray = new Color[x * y * z];
        texture = new Texture3D(x, y, z, TextureFormat.RGBA32, true);
        // float r = 1.0f / (size - 1.0f);
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                for (int k = 0; k < z; k++)
                {
                    // get color from table
                    Color c = new Color();
                    colorArray[i * y * z + (j * z) + k] = c;
                }
            }
        }
        texture.SetPixels(colorArray);
        texture.Apply();
        return texture;
    }

    
    Color lookUpColor(int value)
    {
        for (int i = 0; i < opacityDic.Count; i++)
        {

        }
        return new Color();
    }
}
