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
        public float MinMass { get; set; }  //TODO
        public float MaxMass { get; set; }  //TODO
        public float MaxCompress { get; set; } //TODO
        public float Offset { get; set; }
        public float Speed { get; set; }
        public float Detail { get; set; }
        public float Variance { get; set; }
        public bool runSim { get; set; }
        public float simTimer = 0f;
        public float TimeUnit = 3000f;
        public float ratio = 0f, Row = 0f, Column= 0f;
        private int Count = 0;

        public CACell cellPrefab;
        private List<CACell> cellList = new List<CACell>();
        private CAField<CellData> caFront, caBack;

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
            caFront = new CAField<CellData>(Width, Height);
            caBack = new CAField<CellData>(Width, Height);
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
            caFront.Clear();
            caBack.Clear();
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

                pos.x += (cellPrefab.transform.localScale.x * ratio) + offset.x;
                if (pos.x > end.x)
                {
                    pos.x = corner.x;
                    pos.y += (cellPrefab.transform.localScale.y * ratio) + offset.y;
                }

            }


            Count = count;

            Row = Mathf.Ceil(end.y / (cellPrefab.transform.localScale.y * ratio + offset.y));
            Column = Mathf.Ceil(end.x / (cellPrefab.transform.localScale.x * ratio + offset.x));


            //caFront = new CAField<CellData>(Width, Height);
            //caBack = new CAField<CellData>(Width, Height);
        }


        private Vector2 getPosition(int index)
        {
            if (index >= Count)
            {
                return Vector2.zero;
            }

            Vector2 result = Vector2.zero;
            result.x = Mathf.Ceil(index % Row);
            result.y = Mathf.Floor(index / Row);

            return result;
        }

        private int getIndex(Vector2 pos)
        {
            int result = 0;

            result = (int)(pos.y + (pos.x * Column));

            return result;
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
            float flowScore = 0f;
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    var curr = caFront.getCell(x, y);
                    if (curr.cType == CellType.Solid || curr.cellMass <= 0)
                    {
                        continue;
                    }

                    //Below
                    if (y - 1 >= 0)
                    {

                    }

                    //Left
                    if (x + 1 < Width)
                    {

                    }

                    //Right
                    if (x - 1 >= 0)
                    {

                    }

                    //Above
                    if (y + 1 < Height)
                    {

                    }

                }
            }

            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    var curr = caFront.getCell(x, y);
                    if (curr.cType == CellType.Solid)
                    {
                        continue;
                    }

                    if (curr.cellMass > MinMass)
                    {
                        curr.cType = CellType.Water;
                    }
                    else
                    {
                        curr.cType = CellType.Air;
                        curr.cellMass = 0f;
                    }
                }
            }
        }

        public CellData getCellData(int id)
        {
            if (id < Count && caFront != null)
            {
                int xPos = (int)(id % Width);
                int yPos = (int)((id - xPos) / Width);

                return caFront.getCell(xPos, yPos);
            }

            throw new System.IndexOutOfRangeException();
        }

        private float stableMass(float mass)
        {
            if (mass <= 1f)
                return 1f;
            else if (mass < 2 * MaxMass + MaxCompress)
                return (MaxMass * MaxMass + mass * MaxCompress) / (MaxMass + MaxCompress);
            else
                return (mass + MaxCompress) * 0.5f;
        }

#if UNITY_EDITOR

        void OnDrawGizmos()
        {

        }
#endif
    }
}