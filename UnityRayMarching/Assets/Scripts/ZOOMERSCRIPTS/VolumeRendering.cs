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

        [SerializeField] public Shader shader;
        public Material material;

        [SerializeField] Color color = Color.white;
        [Range(0f, 1f)] public float threshold = 0.5f; 
        [Range(0.5f, 5f)] public float intensity = 1.5f;
        [Range(0f, 1f)] public float sliceXMin = 0.0f, sliceXMax = 1.0f;
        [Range(0f, 1f)] public float sliceYMin = 0.0f, sliceYMax = 1.0f;
        [Range(0f, 1f)] public float sliceZMin = 0.0f, sliceZMax = 1.0f;
        [Range(0f, 5f)] public float pointer_intensity = 2f;
        [Range(0f, 0.1f)] public float plane_thickness = 0.05f;
        public Vector4 plane = new Vector4(0, 0, 0, 0);

        public Quaternion axis = Quaternion.identity;

        public Texture volume;
        private Camera vrCam;
        public bool isMain = true;
        private float width;
        public Mesh mesh;
        private GameObject rightController;
        public void Awake()
        {
            
        }
        public virtual void Start()
        {
            material = new Material(shader);
            this.width = 1f;

            //BuildMesh(0.5f);
            //GetComponent<MeshCollider>().sharedMesh = mesh;
            vrCam = Camera.main;
        }

        public Mesh BuildMesh (Vector3 pos, float width)
        {

            
            material = new Material(shader);
            mesh = Build(pos, width);
            GetComponent<MeshFilter>().sharedMesh = mesh;
            GetComponent<MeshRenderer>().sharedMaterial = material;
            updateMaterial();
            if (!isMain)
            {
                Debug.Log(shader);
                Debug.Log(volume);
            }
            
            return mesh;
        }

        void Update()
        {
            controllerUpdate();
            updateMainMesh();
            updateMaterial();
        }

        void updateMaterial()
        {
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
            Vector3 controller_local_coord = this.transform.InverseTransformPoint(rightController.transform.position);
            material.SetVector("_PointerPosition", controller_local_coord);
        }

        void updateMainMesh()
        {
            // scaling is fucking this up
            if (isMain) {
                this.width = Vector3.Distance(this.transform.position, vrCam.transform.position) / Mathf.Sqrt(2f);
                mesh = BuildMesh(Vector3.zero, (float)Math.Min(width, 0.6) - 0.1f);
            }
        }

        void controllerUpdate()
        {
            //try
            //{
            //    Debug.Log(rightController.transform.position + " " + rightController.transform.rotation);
            //}
            //catch (NullReferenceException e)
            //{
            Console.Write("Right controller not detected, retrying...");
            if(rightController == null) rightController = GameObject.Find("Right_Right OpenVR Controller");
            //}
            buildPlane(ref plane.x, ref plane.y, ref plane.z, ref plane.w, rightController);

        }

        // build scanning plane
        // pass in abcd that represent a plane in 3d, and a controller
        void buildPlane(ref float a, ref float b, ref float c, ref float d, GameObject controller)
        {
            if (controller == null) return;
            Vector3 position =  controller.transform.position; // world space
            Quaternion quaternion = controller.transform.rotation;
            float theta = (float) Math.Acos(quaternion.w) * 2;
            Vector3 v1 = new Vector3(quaternion.x, quaternion.y, quaternion.z);
            Vector3 v2_controller = new Vector3(1, 0, 0); // controller's space
            Vector3 v2 = controller.transform.TransformDirection(v2_controller); // to world space
            v2 = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, v1) * v2; // rotate with theta

            // transform everything to mesh's space
            Vector3 m_postion = this.transform.InverseTransformPoint(position);
            Vector3 m_v1 = this.transform.InverseTransformDirection(v1);
            Vector3 m_v2 = this.transform.InverseTransformDirection(v2);
            Vector3 p1 = m_postion;
            Vector3 p2 = m_postion + m_v1;
            Vector3 p3 = m_postion + m_v2;

            float a1 = p2.x - p1.x;
            float b1 = p2.y - p1.y;
            float c1 = p2.z - p1.z;
            float a2 = p3.x - p1.x;
            float b2 = p3.y - p1.y;
            float c2 = p3.z - p1.z;
            a = b1 * c2 - b2 * c1;
            b = a2 * c1 - a1 * c2;
            c = a1 * b2 - b1 * a2;
            d = (-a * p1.x - b * p1.y - c * p1.z);
        }
        
        Mesh Build(Vector3 cl, float w)
        {
            var vertices = new Vector3[] {
                new Vector3 (cl.x - w, cl.y - w, cl.z - w),
                new Vector3 (cl.x + w, cl.y - w, cl.z - w),
                new Vector3 (cl.x + w, cl.y + w, cl.z - w),
                new Vector3 (cl.x - w, cl.y + w, cl.z - w),
                new Vector3 (cl.x - w, cl.y + w, cl.z + w),
                new Vector3 (cl.x + w, cl.y + w, cl.z + w),
                new Vector3 (cl.x + w, cl.y - w, cl.z + w),
                new Vector3 (cl.x - w, cl.y - w, cl.z + w),
            };
            var triangles = new int[] {
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

