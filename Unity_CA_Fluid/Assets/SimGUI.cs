using UnityEngine;
using System.Collections;

namespace FluidCA.Sim
{
    [RequireComponent(typeof(FluidSim))]
    public class SimGUI : MonoBehaviour
    {

        private FluidSim sim;

        public float SimWidth 
        {
            get { return sim.Width; }
            set { sim.Width = value;} 
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