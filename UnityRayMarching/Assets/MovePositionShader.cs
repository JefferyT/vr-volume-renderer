using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePositionShader : MonoBehaviour
{
    public float speed;
    public Renderer rend;
    Material mat;
    Shader shader;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Custom/BALLSHADER");

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        rend.material.SetVector("_Centre", pos);
        transform.Rotate(Vector3.one, Time.deltaTime * speed);
    }
}
