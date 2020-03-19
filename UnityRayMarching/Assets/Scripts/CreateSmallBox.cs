using UnityEngine;
namespace VolumeRendering
{
    public class CreateSmallBox : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject volumePrefab;
        public GameObject originalVolume;
        public GameObject rightController;
        public float width = 0.1f;
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
                    x = controller_local.x + 0.5f;
                    y = controller_local.y + 0.5f;
                    z = controller_local.z + 0.5f;
                    updateChildMesh(ref volume.mesh, controller_local, 0.1f);

                    volume.sliceXMax = x + width; volume.sliceXMin = x - width;
                    checkBound(ref volume.sliceXMin, ref volume.sliceXMax);
                    volume.sliceYMax = y + width; volume.sliceYMin = y - width;
                    checkBound(ref volume.sliceYMin, ref volume.sliceYMax);
                    volume.sliceZMax = z + width; volume.sliceZMin = z - width;
                    checkBound(ref volume.sliceZMin, ref volume.sliceZMax);
                    volume.intensity = 5;
                }
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

        void updateChildMesh(ref Mesh mesh, Vector3 cl, float w)
        {
            var vertices = new Vector3[] {
                new Vector3 (cl.x - w, -width, -width),
                new Vector3 (cl.x + w, -width, -width),
                new Vector3 (cl.x + w,  width, -width),
                new Vector3 (cl.x - w,  width, -width),
                new Vector3 (cl.x-width,  width,  width),
                new Vector3 (cl.x width,  width,  width),
                new Vector3 (cl.x width, -width,  width),
                new Vector3 (cl.x-width, -width,  width),


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

            mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.hideFlags = HideFlags.HideAndDontSave;
        }
    }
}