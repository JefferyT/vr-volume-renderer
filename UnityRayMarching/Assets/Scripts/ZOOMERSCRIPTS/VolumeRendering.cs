
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.XR;

namespace VolumeRendering
{

    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class VolumeRendering : MonoBehaviour
    {

        [SerializeField] protected Shader shader;
        protected Material material;

        [SerializeField] Color color = Color.white;
        [Range(0f, 1f)] public float threshold = 0.5f; 
        [Range(0.5f, 5f)] public float intensity = 1.5f;
        [Range(0f, 1f)] public float sliceXMin = 0.0f, sliceXMax = 1.0f;
        [Range(0f, 1f)] public float sliceYMin = 0.0f, sliceYMax = 1.0f;
        [Range(0f, 1f)] public float sliceZMin = 0.0f, sliceZMax = 1.0f;
        [Range(1f, 5f)] public float pointer_intensity = 2f;
        [Range(0f, 0.1f)] public float plane_thickness = 0.05f;
        public Vector4 plane = new Vector4(0, 0, 0, 0);

        public Quaternion axis = Quaternion.identity;

        public Texture volume;
        private Camera vrCam;
        private float width;
        private Mesh mesh;
        protected virtual void Start()
        {
            material = new Material(shader);
            this.width = 0.5f;

            BuildMesh(0.5f);
            //GetComponent<MeshCollider>().sharedMesh = mesh;
            vrCam = Camera.main;
        }

        private Mesh BuildMesh (float width)
        {
            mesh = Build(width);
            GetComponent<MeshFilter>().sharedMesh = mesh;
            GetComponent<MeshRenderer>().sharedMaterial = material;
            return mesh;
        }

        protected void Update()
        {
            this.width = Vector3.Distance(this.transform.position, vrCam.transform.position) / Mathf.Sqrt(2f);
            if (width < 0.6f)
            {
                mesh = BuildMesh(width - 0.1f);
            }
            
            material.SetTexture("_Volume", volume);
            material.SetColor("_Color", color);
            material.SetFloat("_Threshold", threshold);
            material.SetFloat("_Intensity", intensity);
            material.SetVector("_SliceMin", new Vector3(sliceXMin, sliceYMin, sliceZMin));
            material.SetVector("_SliceMax", new Vector3(sliceXMax, sliceYMax, sliceZMax));

            material.SetMatrix("_AxisRotationMatrix", Matrix4x4.Rotate(axis));
            material.SetFloat("_PointerIntensity", pointer_intensity);
            material.SetFloat("_ThicknessPlane", plane_thickness);
            material.SetVector("_PlaneScanPara", plane);

            Vector3 rightPosition = InputTracking.GetLocalPosition(XRNode.RightHand);
            //Debug.Log("right pos " + rightPosition);
            //  Debug.Log(vrCam.transform.position);
            
           
        }

        Mesh Build(float width)
        {
            var vertices = new Vector3[] {
                new Vector3 (-width, -width, -width),
                new Vector3 ( width, -width, -width),
                new Vector3 ( width,  width, -width),
                new Vector3 (-width,  width, -width),
                new Vector3 (-width,  width,  width),
                new Vector3 ( width,  width,  width),
                new Vector3 ( width, -width,  width),
                new Vector3 (-width, -width,  width),
                //new Vector3 (-0.25f, -0.25f, -0.25f), // 8 0
                //new Vector3 ( 0.25f, -0.25f, -0.25f), // 9 1
                //new Vector3 ( 0.25f,  0.25f, -0.25f), // 10 2
                //new Vector3 (-0.25f,  0.25f, -0.25f), // 11 3
                //new Vector3 (-0.25f,  0.25f,  0.25f), // 12 4
                //new Vector3 ( 0.25f,  0.25f,  0.25f), // 13 5
                //new Vector3 ( 0.25f, -0.25f,  0.25f), // 14 6
                //new Vector3 (-0.25f, -0.25f,  0.25f), // 15 7
                

            };
            var triangles = new int[] {
                // outter mesh
                0, 2, 1,
                0, 3, 2,
                2, 3, 4,
                2, 4, 5,
                1, 2, 5,
                1, 5, 6,
                0, 7, 4,
                0, 4, 3,
                5, 4, 7,
                5, 7, 6,
                0, 6, 7,
                0, 1, 6,
                ///// inner mesh
                //8, 10, 9,
                //8, 11, 10,
                //10, 11, 12,
                //10, 12, 13,
                //9, 10, 13,
                //9, 13, 14,
                //8, 15, 12,
                //8, 12, 11,
                //13, 12, 15,
                //13, 15, 14,
                //8, 14, 15,
                //8, 9, 14,
            };

            var mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.hideFlags = HideFlags.HideAndDontSave;
            return mesh;
        }

        void OnValidate()
        {
            Constrain(ref sliceXMin, ref sliceXMax);
            Constrain(ref sliceYMin, ref sliceYMax);
            Constrain(ref sliceZMin, ref sliceZMax);
        }

        void Constrain(ref float min, ref float max)
        {
            const float threshold = 0.025f;
            if (min > max - threshold)
            {
                min = max - threshold;
            }
            else if (max < min + threshold)
            {
                max = min + threshold;
            }
        }

        void OnDestroy()
        {
            Destroy(material);
        }

    }

}


