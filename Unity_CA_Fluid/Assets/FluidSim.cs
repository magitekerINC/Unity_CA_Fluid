using UnityEngine;
using System.Collections.Generic;
using FluidCA.Util;

namespace FluidCA.Sim
{
    public struct CellData
    {
        public CellType cType;
        public float cellMass;

        public CellData(CellType _type, float _mass)
        {
            cellMass = _mass;
            cType = _type;
        }

    }

    public class FluidSim : MonoBehaviour
    {

        public float Width { get; set; }
        public float Height { get; set; }
        public float Offset { get; set; }
        public float Speed { get; set; }
        public float Detail { get; set; }
        public float Variance { get; set; }
        public bool runSim { get; set; }
        public float simTimer = 0f;
        public float TimeUnit = 3000f;
        private float ratio = 0f;

        public CACell cellPrefab;
        private List<CACell> cellList = new List<CACell>();

        // Use this for initialization
        void Start()
        {
            runSim = false;
            Width = Screen.width;
            Height = Screen.height;
            ratio = Height / Width;
            Offset = 0.001f;
            Speed = 1f;
            Detail = 10f;
            Variance = 10f;
            TimeUnit = 60f / Time.deltaTime;

            Init();
        }

        public void Reset()
        {
            CleanUp();
            Init();
        }

        void CleanUp()
        {
            foreach (CACell c in cellList)
            {
                Destroy(c.gameObject);
            }

            cellList.Clear();
        }

        void Init()
        {
           
       
            var corner = Camera.main.ScreenToWorldPoint(Vector3.zero);
            var end = Camera.main.ScreenToWorldPoint(
                new Vector3(
                Width,
                Height
                ));

            var offset = new Vector3(Offset, Offset);

            //corner *= ratio;
            corner.z = 1f;

            var pos = corner;
            var count = 0;

            while (pos.y <= end.y)
            {

                var gObj = Instantiate(cellPrefab, pos, Quaternion.identity) as CACell;
                gObj.cellID = count;
                gObj.sim = this;
                count++;

                cellList.Add(gObj);

                pos.x += cellPrefab.transform.localScale.x * ratio + offset.x;
                if (pos.x > end.x)
                {
                    pos.x = corner.x;
                    pos.y += cellPrefab.transform.localScale.y * ratio + offset.y;
                }

            }
           
        }


        // Update is called once per frame
        void Update()
        {
            if (runSim)
            {
                simTimer -= Speed * Time.deltaTime;
                if (simTimer <= 0f)
                {
                    simTimer += TimeUnit;
                    TickCA();
                }
            }
        }

        void TickCA()
        {

        }

        public CellData getCellData(int id)
        {
            throw new System.NotImplementedException();
        }

#if UNITY_EDITOR

        void OnDrawGizmos()
        {

        }
#endif
    }
}