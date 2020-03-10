using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePositionShader : MonoBehaviour
{
    Material mat;
    Shader shader;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        shader = Shader.Find("Custom/BALLSHADER");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        shader.SetGlobalVector("_Center", pos);
    }
}
