using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject textInterface;
    public GameObject mainVolume;
    public GameObject Manager;

    public TextMeshPro text;
    void Start()
    {
        // fixed!   
        text = textInterface.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        textInterface.GetComponent<TextMeshPro>().text = "nut";
    }
}
