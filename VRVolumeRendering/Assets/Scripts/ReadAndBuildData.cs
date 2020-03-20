using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ReadAndBuildData : MonoBehaviour
{

    Texture3D texture;
    const string path = @"C:\Users\Husky Maker\Desktop\Xiao\vr-volume-rendering\VolumeRenderCStuff\body.dat";
    static List<short> data = new List<short>();

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
    void Start()
    {
        readData();
        Texture3D tex = CreateTexture3D(512, 512, 406);

        AssetDatabase.CreateAsset(tex, "Assets/Texture/myTexture.asset");
    }
    
    void Update()
    {

    }

    void readData()
    {
        if (File.Exists(path))
        {
            using (BinaryReader b = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                for (int i = 0; i < 512 * 512 * 406; i++)
                {
                    data.Add(b.ReadInt16());
                }

            }
        }
        
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
                    int value = (int)data[i * y * z + j * z + k];
                    Color c = lookUpColor(value);
                    colorArray[i * y * z + j * z + k] = c;
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
            if ((value <= opacityDic.Keys[i] && i == 0) || (value >= opacityDic.Keys[i] && i == opacityDic.Count - 1))
            {
                Vector3 rgb = colorDic.Values[i];
                float a = opacityDic.Values[i];
                return new Color(rgb.x, rgb.y, rgb.z, a);
            } else
            {
                if (value >= opacityDic.Keys[i] && value <= opacityDic.Keys[i + 1])
                {
                    // interpolate between two pairs
                    Vector3 rgb1 = colorDic.Values[i];
                    float a1 = opacityDic.Values[i];
                    Vector3 rgb2 = colorDic.Values[i + 1];
                    float a2 = opacityDic.Values[i + 1];
                    float percent = (value - opacityDic.Values[i]) / (opacityDic.Values[i + 1] - opacityDic.Values[i+1]);
                    rgb1.x *= percent;
                    rgb1.y *= percent;
                    rgb1.z *= percent;
                    a1 *= percent;
                    rgb2.x *= 1.0f - percent;
                    rgb2.y *= 1.0f - percent;
                    rgb2.z *= 1.0f - percent;
                    a2 *= 1.0f - percent;
                    return new Color(rgb1.x + rgb2.x, rgb1.y + rgb2.y, rgb1.z + rgb2.z, a1 + a2);
                }
            }
        }
        return new Color();
    }
}
