using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeRendering {
    public class ControlManager : MonoBehaviour
    {

        public VolumeRendering mainVolume;
        [Range(1, 5)] public int mode = 1;
        private bool mode2DefaultSet;
        private bool mode3DefaultSet;
        public GameObject volumePrefab;
        public GameObject rightController;
        public float width = 1f;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
            if (Input.GetKeyDown(KeyCode.Alpha1)) mode = 1; // normal mode: enable rotating, translating, scaling
            if (Input.GetKeyDown(KeyCode.Alpha2)) mode = 2; // pointer light mode
            if (Input.GetKeyDown(KeyCode.Alpha3)) mode = 3; // plane scane mode
            if (Input.GetKeyDown(KeyCode.Alpha4)) mode = 4; // slicing mode
            if (Input.GetKeyDown(KeyCode.Alpha5)) mode = 5; // cloning mode

            if (mode == 4)
            {
                // a lot of them is not working, overriding built-in keys?
                Debug.Log(mainVolume.sliceXMin);
                if (Input.GetKeyDown(KeyCode.A)) mainVolume.sliceXMin -= 0.1f;
                if (Input.GetKeyDown(KeyCode.D)) mainVolume.sliceXMin += 0.1f;

                if (Input.GetKeyDown(KeyCode.F)) mainVolume.sliceXMax -= 0.1f;
                if (Input.GetKeyDown(KeyCode.H)) mainVolume.sliceXMax += 0.1f;

                if (Input.GetKeyDown(KeyCode.S)) mainVolume.sliceYMin -= 0.1f;
                if (Input.GetKeyDown(KeyCode.W)) mainVolume.sliceYMin += 0.1f;

                if (Input.GetKeyDown(KeyCode.G)) mainVolume.sliceYMax -= 0.1f;
                if (Input.GetKeyDown(KeyCode.T)) mainVolume.sliceYMax += 0.1f;

                if (Input.GetKeyDown(KeyCode.Q)) mainVolume.sliceZMin -= 0.1f;
                if (Input.GetKeyDown(KeyCode.E)) mainVolume.sliceZMin += 0.1f;

                if (Input.GetKeyDown(KeyCode.R)) mainVolume.sliceZMax -= 0.1f;
                if (Input.GetKeyDown(KeyCode.Y)) mainVolume.sliceZMax += 0.1f;
                checkSlicingBound();
            }

            if (mode != 2) mode2DefaultSet = false;
            if (mode == 2)
            {
                if (!mode2DefaultSet)
                {
                    mainVolume.pointer_intensity = 3;
                    mode2DefaultSet = true;
                }
                if (Input.GetKeyDown(KeyCode.Z)) mainVolume.pointer_intensity -= 0.5f;
                if (Input.GetKeyDown(KeyCode.X)) mainVolume.pointer_intensity += 0.5f;
            }

            if (mode != 3) mode3DefaultSet = false;
            if (mode == 3)
            {
                if (!mode3DefaultSet)
                {
                    mainVolume.plane_thickness = 0.05f;
                    mode3DefaultSet = true;
                    
                }
                // need intensity control
                if (Input.GetKeyDown(KeyCode.C)) mainVolume.plane_thickness -= 0.01f;
                if (Input.GetKeyDown(KeyCode.V)) mainVolume.plane_thickness += 0.01f;
            }

            if (mode == 5)
            {
                if (rightController == null) rightController = GameObject.Find("Right_Right OpenVR Controller");

                if (rightController != null)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        GameObject go = Instantiate(volumePrefab, new Vector3(Random.Range(1, 5), Random.Range(1, 5), Random.Range(1, 5)), Quaternion.identity);
                        VolumeRendering volume = go.GetComponent<VolumeRendering>();

                        Vector3 controller_local = mainVolume.transform.InverseTransformPoint(rightController.transform.position);
                        
                        volume.intensity = 2;
                        volume.isMain = false;
                        checkControllerBound(ref controller_local, width);
                        volume.BuildMesh(controller_local, width);

                    }
                }
            }

        }

        void checkSlicingBound()
        {
            mainVolume.sliceXMin = Mathf.Max(mainVolume.sliceXMin, 0);
            mainVolume.sliceYMin = Mathf.Max(mainVolume.sliceYMin, 0);
            mainVolume.sliceZMin = Mathf.Max(mainVolume.sliceZMin, 0);
            mainVolume.sliceXMax = Mathf.Min(mainVolume.sliceXMax, 1);
            mainVolume.sliceYMax = Mathf.Min(mainVolume.sliceYMax, 1);
            mainVolume.sliceZMax = Mathf.Min(mainVolume.sliceZMax, 1);
        }

        void checkControllerBound(ref Vector3 cl, float w)
        {
            if (cl.x - w < -0.5)
            {
                cl.x = w + -0.5f;
            }
            if (cl.y - w < -0.5)
            {
                cl.y = w + -0.5f;
            }
            if (cl.z - w < -0.5)
            {
                cl.z = w + -0.5f;
            }

            if (cl.x + w > 0.5)
            {
                cl.x = -w + 0.5f;
            }
            if (cl.y + w > 0.5)
            {
                cl.y = -w + 0.5f;
            }
            if (cl.z + w > 0.5)
            {
                cl.z = -w + 0.5f;
            }
        }
    }

}
