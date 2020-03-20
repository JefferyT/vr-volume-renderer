using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace VolumeRendering {
    public class UserInterface : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject textInterface;
        public GameObject mainVolume;
        public GameObject control;

        public TextMeshPro text;
        public VolumeRendering volume;
        public ControlManager manager;
        private int prev_mode = 0;
        private float prev_point_intensity = -1;
        private float prev_plane_thickness = -1;
        private float Xmax, Xmin, Ymax, Ymin, Zmax, Zmin = -1;
        private int prev_num_clones = -1;
        void Start()
        {
            // fixed!   
            text = textInterface.GetComponent<TextMeshPro>();
            volume = mainVolume.GetComponent<VolumeRendering>();
            manager = control.GetComponent<ControlManager>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!isSameState())
            {
                changeText();
                updateAllRecord();
            }

        }

        void changeText()
        {
            string content = "";
            
            if (manager.mode == 1)
            {
                content += "Mode 1 : \"Basic\"\n";
                content += "Translation, Rotation,\n";
                content += "Scaling enabled.";
            }

            if (manager.mode == 2)
            {
                content += "Mode 2 : \"Controller Light\"\n";
                content += "Controllers Generate \n";
                content += "a Point-Light Source\n";
                content += "Intensity: ";
                content += volume.pointer_intensity;
                
            }

            if (manager.mode == 3)
            {
                content += "Mode 3 : \"Scanning Plane\"\n";
                content += "Controllers Generate \n";
                content += "a Scanning Plane\n";
                content += "Width(Thickness): ";
                content += volume.plane_thickness * 2;
                
            }

            if (manager.mode == 4)
            {
                content += "Mode 4 : \"Slicing\"\n";
                content += "Slicing Enabled\n";
                content += "Xmin = " + volume.sliceXMin + " Xmax = " + volume.sliceXMax + "\n";
                content += "Ymin = " + volume.sliceYMin + " Ymax = " + volume.sliceYMax + "\n";
                content += "Zmin = " + volume.sliceZMin + " Zmax = " + volume.sliceZMax + "\n";
            }

            if (manager.mode == 5)
            {
                content += "Mode 5 : \"Cloning\"\n";
                content += "Controllers can now clone\n";
                content += "a block of the volume\n";
                content += "# Clones: ";
                content += manager.numClones;
            }

            textInterface.GetComponent<TextMeshPro>().text = content;
        }

        bool isSameState()
        {
            return (prev_mode == manager.mode && volume.pointer_intensity == prev_point_intensity
                && volume.plane_thickness == prev_plane_thickness &&
                Xmax == volume.sliceXMax && Xmin == volume.sliceXMin &&
                Ymax == volume.sliceYMax && Ymin == volume.sliceYMin &&
                Zmax == volume.sliceZMax && Zmin == volume.sliceZMin &&
                prev_num_clones == manager.numClones);
        }

        void updateAllRecord()
        {
            prev_mode = manager.mode;
            prev_point_intensity = volume.pointer_intensity;
            prev_plane_thickness = volume.plane_thickness;
            Xmax = volume.sliceXMax;
            Xmin = volume.sliceXMin;
            Ymax = volume.sliceYMax;
            Ymin = volume.sliceYMin;
            Zmax = volume.sliceZMax;
            Zmin = volume.sliceZMin;
            prev_num_clones = manager.numClones;
        }
        
    } 
}
