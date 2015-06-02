using UnityEngine;
using System.Collections.Generic;
using FluidCA.Util;
using System;
using System.Collections;

namespace FluidCA.Sim
{
    public class CellData
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
        public float CellSize { get; set; }
        public float MinMass { get; set; } 
        public float MaxMass { get; set; }  
        public float MaxCompress { get; set; } 
        public float Offset { get; set; }
        public float Speed { get; set; }
        public float Detail { get; set; }
        public float Variance { get; set; }
        public bool runSim { get; set; }
        public float Row { get; set;}
        public float Column {get; set;}

        public float simTimer = 0f;
        public float TimeUnit = 3000f;
        public float ratio = 0f;
        public float minFlow = 0.01f, maxFlow = 0.5f;

        private int Count = 0;

        public CACell cellPrefab;
        private List<CACell> cellList = new List<CACell>();
        private CAField<CellData> caFront, caBack;

        // Use this for initialization
        void Awake()
        {
            runSim = false;
            Width = Screen.width;
            Height = Screen.height;
            ratio = Height / Width;
            Offset = 0.00f;
            Speed = 3000f;
            Detail = 10f;
            Variance = 100f;
            TimeUnit = 3000f;
            MinMass = 0.0001f;
            MaxMass = 1.5f;
            MaxCompress = 0.02f;
            CellSize = cellPrefab.transform.localScale.x;
            Row = 32f;
            Column = 52f;
            StartCoroutine(Init());

        }

        public void Reset()
        {
            StopAllCoroutines();
            CleanUp();
            StartCoroutine(Init());
        }

        void CleanUp()
        {
            for (int i = 0; i < cellList.Count; ++i)
            {
                Destroy(cellList[i].gameObject);
            }

            cellList.Clear();
            caFront.Clear();
            caBack.Clear();
        }

        IEnumerator Init()
        {

            var corner = Camera.main.ScreenToWorldPoint(Vector3.zero);
            //ratio = Screen.height / Screen.width;
            /*var end = Camera.main.ScreenToWorldPoint(
                new Vector3(
                Screen.width,
                Screen.height
                ));*/

            var offset = new Vector3(Offset, Offset);

            var end = new Vector2(
                (offset.x + CellSize * ratio) * Column,
                (offset.y + CellSize * ratio) * Row
                );


            //corner *= ratio;
            corner.z = 1f;

            var pos = corner;
            var count = 0;

            for (int i = 0; i <= Row; ++i)
            {
                for (int j = 0; j <= Column; ++j)
                {
                    var gObj = Instantiate(cellPrefab, pos, Quaternion.identity) as CACell;
                    gObj.transform.localScale = new Vector3(CellSize, CellSize);
                    gObj.cellID = count;
                    gObj.sim = this;
                    gObj.gameObject.transform.parent = gameObject.transform;
                    ++count;
                    gObj.name = cellPrefab.name;
                    cellList.Add(gObj);

                    pos.x += (CellSize * ratio) + offset.x;
                    if (pos.x >= end.x)
                    {
                        pos.x = corner.x;
                        pos.y += (CellSize * ratio) + offset.y;
                    }

                }
            }
            yield return 0;
            Count = count;

            caFront = new CAField<CellData>(Column, Row);
            caBack = new CAField<CellData>(Column, Row);

            initCAData(ref caFront);
            //initCAData(ref caBack);
            caBack.Copy(ref caFront);

            UpdateCells();
        }

        private void initCAData(ref CAField<CellData> data)
        {

            var landMap = PerlinNoise.generatePerlinNoise(
                   (int)Column,
                   (int)Row,
                   Detail,
                   Variance
                   );


            for (int i = 0; i < Column; ++i)
                for (int j = 0; j < Row; ++j)
                {

                    var landType = (int)(landMap[i][j] % 4);
                    var cType = CellType.Air;
                    var mass = 0f;
                    switch (landType)
                    {
                        case 0:
                            cType = CellType.Solid;
                            mass = 1.0f;
                            break;
                        case 1:
                            cType = CellType.Air;
                            mass = 0.0f;
                            break;
                        case 2:
                            cType = CellType.Water;
                            mass = 2.0f;
                            break;
                        case 3:
                            cType = CellType.Air;
                            mass = 0.0f;
                            break;
                        default:
                            cType = CellType.Water;
                            mass = 1.0f;
                            break;
                    }

                    var cData = new CellData(cType, mass);
                    data.setCell(i, j, cData);
                }
        }


        private Vector2 getPosition(int index)
        {
            if (index >= Count)
            {
                return Vector2.zero;
            }

            Vector2 result = Vector2.zero;
            result.x = Mathf.Ceil(index % Column);
            result.y = Mathf.Floor(index / Column);

            return result;
        }

        private int getIndex(int posx, int posy)
        {
            int result = 0;

            result = (int)(posy + (posx * Column));

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
                    UpdateCells();
            }

        }

        void UpdateCells()
        {
            for (int i = 0; i < cellList.Count; ++i)
            {
                cellList[i].UpdateCell(getCellData(cellList[i].cellID));
            }
        }

        void TickCA()
        {
#if UNITY_EDITOR
            //Debug.Log("Tick");
#endif
            if (caFront.Equals(null) ||
                caBack.Equals(null))
                return;

            float spd = Speed;
            float flowScore = 0f;
            for (int x = 0; x < Column; ++x)
            {
                for (int y = 0; y < Row; ++y)
                {
                    var curr = caFront[x, y];
                    float flow = 0f;

                    if (curr.cType != CellType.Solid && curr.cellMass > 0)
                    {

                        float rMass = curr.cellMass;

                        //Below
                        if (rMass > 0 && y - 1 >= 0 && caFront[x, y - 1].cType != CellType.Solid)
                        {
                            flow = stableMass((rMass + caFront[x, y - 1].cellMass) - caFront[x, y - 1].cellMass);
                            if (flow > minFlow) flow *= 0.5f;

                            flow = Mathf.Clamp(flow, 0f, Mathf.Min(spd, rMass));

                            caBack[x, y].cellMass -= flow;
                            caBack[x, y - 1].cellMass += flow;
                            flowScore += flow;
                            rMass -= flow;
                        }

                        //Left
                        if (rMass > 0 && x + 1 < Column && caFront[x + 1, y].cType != CellType.Solid)
                        {
                            flow = stableMass((rMass + caFront[x + 1, y].cellMass) - caFront[x + 1, y].cellMass);
                            if (flow > minFlow) flow *= 0.5f;

                            flow = Mathf.Clamp(flow, 0f, rMass);

                            caBack[x, y].cellMass -= flow;
                            caBack[x + 1, y].cellMass += flow;
                            flowScore += flow;
                            rMass -= flow;
                        }

                        //Right
                        if (rMass > 0 && x - 1 >= 0 && caFront[x - 1, y].cType != CellType.Solid)
                        {
                            flow = stableMass((rMass + caFront[x - 1, y].cellMass) - caFront[x - 1, y].cellMass);
                            if (flow > minFlow) flow *= 0.5f;

                            flow = Mathf.Clamp(flow, 0f, rMass);

                            caBack[x, y].cellMass -= flow;
                            caBack[x - 1, y].cellMass += flow;
                            flowScore += flow;
                            rMass -= flow;
                        }

                        //Above
                        if (rMass > 0 && y + 1 < Row && caFront[x, y + 1].cType != CellType.Solid)
                        {
                            flow = stableMass((rMass + caFront[x, y + 1].cellMass) - caFront[x, y + 1].cellMass);
                            if (flow > minFlow) flow *= 0.5f;

                            flow = Mathf.Clamp(flow, 0f, Mathf.Min(spd, rMass));

                            caBack[x, y].cellMass -= flow;
                            caBack[x, y + 1].cellMass += flow;
                            flowScore += flow;
                            rMass -= flow;
                        }

                        if (caBack[x, y].cellMass > MinMass)
                        {
                            caBack[x, y].cType = CellType.Water;
                        }
                        else
                        {
                            caBack[x, y].cType = CellType.Air;
                            caBack[x, y].cellMass = 0f;
                        }
                    }
                }
            }

            /*
            for (int x = 0; x < Column; ++x )
            {
                caBack[x,0].cellMass = 0f;
                caBack[x,0].cType = CellType.Air;
            }*/

            caFront.Copy(ref caBack);

        }

        public CellData getCellData(int id)
        {
            if (id < Count && !caFront.Equals(null))
            {
                int xPos = (int)(id % Column);
                int yPos = (int)((id - xPos) / Column);


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