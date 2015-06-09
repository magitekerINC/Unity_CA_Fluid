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

        private bool resetSim = false;
        private bool clearSim = false;

        public float simTimer = 0f;
        public float TimeUnit = 3000f;
        public float ratio = 0f;
        public float flow = 0f, flowScore = 0f, rMass = 0f, spd = 1f;
        private CellData curr;

        private int Count = 0;
        public bool guiOpen = false;
        public CACell cellPrefab;
        private List<CACell> cellList = new List<CACell>();
        private CAField<CellData> caFront, caBack;
        public CellType cellBrush = CellType.Solid;

        // Use this for initialization
        void Awake()
        {
            runSim = false;
            Width = Screen.width;
            Height = Screen.height;
            ratio = Width / Height;
            Offset = 0f;
            Speed = 1000f;
            Detail = 20f;
            Variance = 100f;
            TimeUnit = 1000f;
            MinMass = 0.0001f;
            MaxMass = 1f;
            MaxCompress = 0.2f;
            CellSize = cellPrefab.transform.localScale.x;
            Row = 39;//33f;
            Column = 63; // 52f;
            Init();

        }

        public void ResetSim()
        {
            resetSim = true;
        }

        void Reset()
        {
            resetSim = false;
            StopAllCoroutines();
            CleanUp();
            Init();
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

        public void ClearSim()
        {
            clearSim = true;
            

        }

        private void Clear()
        {
            clearSim = false;

            for(int i=1; i < Column-1; ++i)
                for (int j = 1; j < Row-1; ++j)
                {
                    caFront[i, j].cType = CellType.Air;
                    caFront[i, j].cellMass = 0f;
                }

            caBack.Copy(ref caFront);
            UpdateCells();

        }

        void Init()
        {
           
            var corner = new Vector3(CellSize * 0.5f, CellSize * 0.5f);

            var vE = 2f * Camera.main.orthographicSize;
            var hE = vE * ratio;

            Row = Mathf.Floor((vE / (CellSize * 0.64f)));
            Column = Mathf.Floor((hE / (CellSize * 0.64f)));

            cellList = new List<CACell>((int)(Row * Column));

            //corner *= ratio;
            corner.z = 1f;

            var pos = corner;
            var count = 0;

            for (int i = 0; i < Row; ++i)
            {
                for (int j = 0; j < Column; ++j)
                {
                    var gObj = Instantiate(cellPrefab, pos, Quaternion.identity) as CACell;
                    gObj.transform.localScale = new Vector3(CellSize, CellSize);
                    gObj.cellID = count;
                    gObj.sim = this;
                    gObj.gameObject.transform.parent = gameObject.transform;
                    ++count;
                    gObj.name = cellPrefab.name;
                    cellList.Add(gObj);

                    pos.x += (CellSize) * 0.64f + Offset;
                }

                pos.x = corner.x;
                pos.y += (CellSize) * 0.64f + Offset;
            }

            var cPos = new Vector3(
                (Column * CellSize * 0.64f) * 0.5f,
                (Row * CellSize * 0.64f) * 0.5f,
                Camera.main.transform.position.z
                );


            Camera.main.transform.position = cPos;

            Count = count;

            caFront = new CAField<CellData>(Column, Row);
            caBack = new CAField<CellData>(Column, Row);

            initCAData(ref caFront);

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
                    var cType = CellType.Solid;
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
                            mass = 1.0f;
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

                    if (i == 0 ||
                        i == Column - 1 ||
                        j == 0 || j == Row - 1)
                    {
                        cType = CellType.Solid;
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
            result.x = index % Column;
            result.y = Mathf.Floor(index / Column);

            return result;
        }

        private int getIndex(int posx, int posy)
        {
            int result = 0;

            result = (int)(posx + (posy * Column));

            return result;
        }


        // Update is called once per frame
        void Update()
        {
            if (runSim)
            {
                // simTimer -= Speed * Time.deltaTime;
                // if (simTimer <= 0f)
                // {
                //     simTimer += TimeUnit;
                TickCA();
                // }
                UpdateCells();

            }
            if (resetSim)
                Reset();
            else if (clearSim)
                Clear();

        }

        void UpdateCells()
        {
            
            for(int x=0; x < Column; ++x)
                for (int y = 0; y < Row; ++y)
                {
                    var index = getIndex(x, y);
                    cellList[index].UpdateCell(caFront[x, y]);
                }

        }

        void TickCA()
        {

            spd = 1f;
            flowScore = 0f;
            for (int y = 1; y < Row - 1f; ++y)
            {
                for (int x = 1; x < Column -1f; ++x)
                {
                    curr = caFront[x, y];
                    flow = 0f;

                    if (curr.cType != CellType.Solid && curr.cellMass > 0)
                    {

                        rMass = curr.cellMass;

                        //Below
                        if (rMass > 0 && caFront[x, y - 1].cType != CellType.Solid)
                        {
                            flow = stableMass(rMass + caFront[x, y - 1].cellMass) - caFront[x, y - 1].cellMass;
                            if (flow > MinMass) flow *= 0.5f;

                            flow = Mathf.Clamp(flow, 0f, Mathf.Min(spd, rMass));

                            caBack[x, y].cellMass -= flow;
                            caBack[x, y - 1].cellMass += flow;
                            flowScore += flow;
                            rMass -= flow;
                        }


                        //Left
                        if (rMass > 0 && caFront[x + 1, y].cType != CellType.Solid)
                        {
                            flow = (curr.cellMass - caFront[x + 1, y].cellMass) * 0.5f;
                            if (flow > MinMass) flow *= 0.5f;

                            flow = Mathf.Clamp(flow, 0f, rMass);

                            caBack[x, y].cellMass -= flow;
                            caBack[x + 1, y].cellMass += flow;
                            flowScore += flow;
                            rMass -= flow;
                        }

                        //Right
                        if (rMass > 0 && caFront[x - 1, y].cType != CellType.Solid)
                        {
                            flow = (curr.cellMass - caFront[x - 1, y].cellMass) * 0.5f;
                            if (flow > MinMass) flow *= 0.5f;

                            flow = Mathf.Clamp(flow, 0f, rMass);

                            caBack[x, y].cellMass -= flow;
                            caBack[x - 1, y].cellMass += flow;
                            flowScore += flow;
                            rMass -= flow;
                        }

                        //Above
                        if (rMass > 0 && caFront[x, y + 1].cType != CellType.Solid)
                        {
                            flow = rMass - stableMass(rMass + caFront[x, y + 1].cellMass);
                            if (flow > MinMass) flow *= 0.5f;

                            flow = Mathf.Clamp(flow, 0f, Mathf.Min(spd, rMass));

                            caBack[x, y].cellMass -= flow;
                            caBack[x, y + 1].cellMass += flow;
                            flowScore += flow;
                            rMass -= flow;
                        }

                    }
                }
            }

            for (int x = 1; x < Column; ++x)
                for (int y = 1; y < Row; ++y)
                {
                    if (caBack[x, y].cType != CellType.Solid)
                    {
                        if (caBack[x, y].cellMass >= MinMass)
                        {
                            caBack[x, y].cType = CellType.Water;

                        }
                        else
                        {
                            caBack[x, y].cType = CellType.Air;
                        }
                    }
                }



            caFront.Copy(ref caBack);

        }

        private Vector2 getCellPos(int id)
        {
            var result = Vector2.zero;
            result.x = (int)(id % Column);
            result.y = (int)((id - result.x) / Column);

            return result;
        }

        public CellData getCellData(int id)
        {
            if (id < Count && !caFront.Equals(null))
            {
                var pos = getCellPos(id);

                return caFront[
                    (int)pos.x,
                    (int)pos.y
                    ];
            }

            throw new System.IndexOutOfRangeException();
        }

        public void PaintCell(int cellID)
        {
            changeCell(cellID, cellBrush);
        }

        public void changeCell(int cellID, CellType type)
        {
            if (guiOpen == false)
            {
                var pos = getCellPos(cellID);
                caFront[(int)pos.x, (int)pos.y].cType = type;
                switch(type)
                {
                    case CellType.Water:
                    caFront[(int)pos.x, (int)pos.y].cellMass = 1f;
                    break;
                    case CellType.Air:
                    caFront[(int)pos.x, (int)pos.y].cellMass = 0f;
                    break;
                }
                UpdateCells();
            }
        }

        private float stableMass(float mass)
        {
            if (mass <= 1f)
                return 1f;
            else if (mass < 2f * MaxMass + MaxCompress)
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