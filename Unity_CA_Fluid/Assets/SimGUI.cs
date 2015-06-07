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
            set { 
                sim.MinMass = value;
                minMVal.text = value.ToString();
            }
        }

        public float SimMaxMass
        {
            get { return sim.MaxMass; }
            set { 
                sim.MaxMass = value;
                maxMVal.text = value.ToString();
            }
        }

        public float SimMaxCompress
        {
            get { return sim.MaxCompress; }
            set { 
                sim.MaxCompress = value;
                compVal.text = value.ToString();
            }
        }

        public float SimWidth 
        {
            get { return sim.Column; }
            set {
                PlaySim = false;
                sim.Column = value;
                widthVal.text = value.ToString();
            } 
        }

        public float SimHeight 
        {
            get { return sim.Row; }
            set {
                PlaySim = false;
                sim.Row = value;
                heightVal.text = value.ToString();
            }
            
        }

        public float SimCellSize
        {
            get { return sim.CellSize; }
            set
            {
                sim.CellSize = value;
                cellSizeVal.text = value.ToString();
            }
        }

        public float SimOffset
        {
            get { return sim.Offset; }
            set { 
                sim.Offset = value;
                offsetVal.text = value.ToString();
            }
        }

        public float SimSpeed 
        {
            get { return sim.Speed; }
            set { 
                sim.Speed = value;
                speedVal.text = value.ToString();
            } 
        }

        public float PerlinDetail 
        {
            get { return sim.Detail; }
            set { 
                sim.Detail = value;
                detailVal.text = value.ToString();
            } 
        }

        public float PerlinVariance 
        {
            get { return sim.Variance; }
            set { 
                sim.Variance = value;
                variVal.text = value.ToString();
            }
        }

        public bool PlaySim
        {
            get { return sim.runSim; }
            set 
            {
                palRunToggle.isOn = value;
                cfgRunToggle.isOn = value;
                sim.runSim = value; 
            }
        }

        public Slider widthSlider;
        public Slider heightSlider;
        public Slider cellSizeSlider;
        public Slider detailSlider;
        public Slider compSlider;
        public Slider minMSlider;
        public Slider maxMSlider;
        public Slider speedSlider;
        public Slider variSlider;
        public Slider offsetSlider;
        public Toggle palRunToggle;
        public Toggle cfgRunToggle;

        public Text cellSizeVal;
        public Text widthVal;
        public Text heightVal;
        public Text detailVal;
        public Text compVal;
        public Text minMVal;
        public Text maxMVal;
        public Text speedVal;
        public Text offsetVal;
        public Text variVal;

        public GameObject OpenButton;
        public GameObject ControlPanel;
        // Use this for initialization
        void Start()
        {
            if (ControlPanel.activeSelf)
                ControlPanel.SetActive(false);
            if (!OpenButton.activeSelf)
                OpenButton.SetActive(true);
            sim = GetComponent<FluidSim>();
            //StartUp();

        }

        void StartUp()
        {

            detailSlider.value = PerlinDetail;
            variSlider.value = PerlinVariance;
            //offsetSlider.value = SimOffset;
            minMSlider.value = SimMinMass;
            maxMSlider.value = SimMaxMass;
            compSlider.value = SimMaxCompress;
            palRunToggle.isOn = cfgRunToggle.isOn = PlaySim = false;
            cellSizeSlider.value = SimCellSize;
        }

       
        public void OpenControls()
        {
            sim.guiOpen = true;
            OpenButton.SetActive(false);
            ControlPanel.SetActive(true);
        }

        public void CloseControls()
        {
            sim.guiOpen = false;
            ControlPanel.SetActive(false);
            OpenButton.SetActive(true);
        }

        public void ResetSim()
        {
            PlaySim = false;
            sim.ResetSim();
        }

        public void ChangeCellBrush(string type)
        {
            switch(type)
            {
                case "solid":
                    sim.cellBrush = CellType.Solid;
                    break;
                case "air":
                    sim.cellBrush = CellType.Air;
                    break;
                case "water":
                    sim.cellBrush = CellType.Water;
                    break;
                default:
                    break;
            }
        }

        public void ClearSim()
        {
            sim.ClearSim();
            PlaySim = false;
        }
    }
}