using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace FluidCA.Sim
{
    [RequireComponent(typeof(FluidSim))]
    public class SimGUI : MonoBehaviour
    {

        private FluidSim sim;

        public float SimMinMass
        {
            get { return sim.MinMass; }
            set { sim.MinMass = value; }
        }

        public float SimMaxMass
        {
            get { return sim.MaxMass; }
            set { sim.MaxMass = value; }
        }

        public float SimMaxCompress
        {
            get { return sim.MaxCompress; }
            set { sim.MaxCompress = value; }
        }

        public float SimWidth 
        {
            get { return sim.Width; }
            set { sim.Width = value; } 
        }

        public float SimHeight 
        {
            get { return sim.Height; }
            set { sim.Height = value; }
            
        }

        public float SimOffset
        {
            get { return sim.Offset; }
            set { sim.Offset = value; }
        }

        public float SimSpeed 
        {
            get { return sim.Speed; }
            set { sim.Speed = value; } 
        }

        public float PerlinDetail 
        {
            get { return sim.Detail; }
            set { sim.Detail = value; } 
        }

        public float PerlinVariance 
        {
            get { return sim.Variance; }
            set { sim.Variance = value; }
        }

        public bool PlaySim
        {
            get { return sim.runSim; }
            set { sim.runSim = value; }
        }

        public Slider widthSlider;
        public Slider heightSlider;
        public Slider detailSlider;
        public Slider compSlider;
        public Slider minMSlider;
        public Slider maxMSlider;
        public Slider speedSlider;
        public Slider variSlider;
        public Slider offsetSlider;
        public Toggle runToggle;

        public GameObject OpenButton;
        public GameObject ControlPanel;
        // Use this for initialization
        void Start()
        {
            sim = GetComponent<FluidSim>();
            if (ControlPanel.activeSelf)
                ControlPanel.SetActive(false);
            if (!OpenButton.activeSelf)
                OpenButton.SetActive(true);


            widthSlider.value = SimWidth;
            heightSlider.value = SimHeight;
            detailSlider.value = PerlinDetail;
            variSlider.value = PerlinVariance;
            speedSlider.value = SimSpeed;
            offsetSlider.value = SimOffset;
            minMSlider.value = SimMinMass;
            maxMSlider.value = SimMaxMass;
            compSlider.value = SimMaxCompress;
            runToggle.isOn = PlaySim;

        }

       
        public void OpenControls()
        {
            OpenButton.SetActive(false);
            ControlPanel.SetActive(true);
        }

        public void CloseControls()
        {
            ControlPanel.SetActive(false);
            OpenButton.SetActive(true);
        }

        public void ResetSim()
        {
            sim.Reset();
        }
    }
}