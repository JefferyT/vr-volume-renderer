using UnityEngine;
namespace VolumeRendering
{
    public class CreateSmallBox : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject volumePrefab;
        public GameObject originalVolume;
        public GameObject rightController;
        public float width = 1f;
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (rightController == null) rightController = GameObject.Find("Right_Right OpenVR Controller");

            if (rightController != null)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    GameObject go = Instantiate(volumePrefab, new Vector3(Random.Range(1, 5), Random.Range(1, 5), Random.Range(1, 5)), Quaternion.identity);
                    VolumeRendering volume = go.GetComponent<VolumeRendering>();
                    float x, y, z;
                    
                    Vector3 controller_local = originalVolume.transform.InverseTransformPoint(rightController.transform.position);
                    //x = controller_local.x + 0.5f;
                    //y = controller_local.y + 0.5f;
                    //z = controller_local.z + 0.5f;
                    volume.intensity = 2;
                    volume.isMain = false;
                    checkControllerBound(ref controller_local, width);
                    volume.BuildMesh(controller_local, width);
                    //Mesh mesh = updateChildMesh(volume.mesh, Vector3.zero, width);
                    //volume.GetComponent<MeshFilter>().sharedMesh = mesh;
                    //volume.mesh = mesh;
                    //Debug.Log(mesh.vertices.ToString());
                    //foreach(Vector3 vec in mesh.vertices)
                    //{
                    //    Debug.Log(vec);
                    //}
                    //Debug.Log(mesh.triangles);
                    //volume.material = new Material(volume.shader);
                    //Debug.Log(volume.material);
                    //volume.GetComponent<MeshRenderer>().sharedMaterial = volume.material;
                    //volume.sliceXMax = x + width; volume.sliceXMin = x - width;
                    //checkBound(ref volume.sliceXMin, ref volume.sliceXMax);
                    //volume.sliceYMax = y + width; volume.sliceYMin = y - width;
                    //checkBound(ref volume.sliceYMin, ref volume.sliceYMax);
                    //volume.sliceZMax = z + width; volume.sliceZMin = z - width;
                    //checkBound(ref volume.sliceZMin, ref volume.sliceZMax);

                }
            }
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

        void checkBound(ref float min, ref float max)
        {
            if (max > 1.0)
            {
                max = 1.0f;
                min = max - 2 * width;
            }
            if (min < 0.0)
            {
                min = 0.0f;
                max = min + 2 * width;
            }
        }

        //Mesh updateChildMesh(Mesh mesh, Vector3 cl, float w)
        //{
        //    var vertices = new Vector3[] {
        //        new Vector3 (cl.x - w, cl.y - w, cl.z - w),
        //        new Vector3 (cl.x + w, cl.y - w, cl.z - w),
        //        new Vector3 (cl.x + w, cl.y + w, cl.z - w),
        //        new Vector3 (cl.x - w, cl.y + w, cl.z - w),
        //        new Vector3 (cl.x - w, cl.y + w, cl.z + w),
        //        new Vector3 (cl.x + w, cl.y + w, cl.z + w),
        //        new Vector3 (cl.x + w, cl.y - w, cl.z + w),
        //        new Vector3 (cl.x - w, cl.y - w, cl.z + w),
        //    };
        //    var triangles = new int[] {
        //        0, 2, 1,
        //        0, 3, 2,
        //        2, 3, 4,
        //        2, 4, 5,
        //        1, 2, 5,
        //        1, 5, 6,
        //        0, 7, 4,
        //        0, 4, 3,
        //        5, 4, 7,
        //        5, 7, 6,
        //        0, 6, 7,
        //        0, 1, 6,

        //    };
        //    mesh = new Mesh();
        //    mesh.vertices = vertices;
        //    mesh.triangles = triangles;
        //    mesh.RecalculateNormals();
        //    mesh.hideFlags = HideFlags.HideAndDontSave;

        //    Debug.Log(w);
        //    foreach (Vector3 vec in mesh.vertices)
        //    {
        //        Debug.Log(vec);
        //    }
        //    return mesh;
        //}
    }
}