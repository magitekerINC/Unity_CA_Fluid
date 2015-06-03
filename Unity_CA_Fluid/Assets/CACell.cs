using UnityEngine;
using System.Collections.Generic;
using FluidCA.Util;
using System;

namespace FluidCA.Sim
{


    public struct CAField<T>
    {
        private int Width, Height;
        private T[,] cells;

        public CAField(float _width, float _height)
        {
            Width = (int)_width;
            Height = (int)_height;

            cells = new T[
                Width,
                Height
                ];

      
        }

        public T getCell(int x, int y)
        {
            return cells[x, y];
        }

        T[,] getCells()
        {
            return cells;
        }

        public void setCell(int x, int y, T _cell)
        {
            cells[x, y] = _cell;
        }

        public List<T> getNeightbors(int x, int y)
        {
            List<T> result = new List<T>();

            /*  OOO
             *  X00
             *  000
             */
            if (x - 1 >= 0)
            {
                result.Add(cells[x - 1, y]);


                /*  XO0
                 *  000
                 *  000
                 */
                if (y - 1 >= 0)
                {
                    result.Add(cells[x - 1, y - 1]);
                }

                /*  OO0
                 *  000
                 *  X00
                 */
                if (y + 1 < Height)
                {
                    result.Add(cells[x - 1, y + 1]);
                }

            }

            /*  OOO
             *  00X
             *  000
             */
            if (x + 1 < Width)
            {
                result.Add(cells[x + 1, y]);


                /*  OOX
                 *  000
                 *  000
                 */
                if (y - 1 >= 0)
                {
                    result.Add(cells[x + 1, y - 1]);
                }

                /*  OO0
                 *  000
                 *  00X
                 */
                if (y + 1 < Height)
                {
                    result.Add(cells[x + 1, y + 1]);
                }

            }


            /*  OX0
             *  000
             *  000
             */
            if (y - 1 >= 0)
            {
                result.Add(cells[x, y - 1]);
            }

            /*  OO0
             *  000
             *  0X0
             */
            if (y + 1 < Height)
            {
                result.Add(cells[x, y + 1]);
            }

            return result;
        }

        public void Copy(ref CAField<T> _other)
        {
            this.cells = _other.getCells();
        }

        public void Clear()
        {
            Array.Clear(cells, 0, cells.Length);
        }

        public T this[int xkey, int ykey]
        {
            get { return cells[xkey, ykey]; }
            set { cells[xkey, ykey] = value; }
        }

    }

    public enum CellType : int
    { 
        Solid = 0,
        Air = 1,
        Water = 2,
        NumTypes
    }

    public class CACell : MonoBehaviour
    {
        public FluidSim sim { get; set; }
        public int cellID = 0;
        public Color cellColor { get; set; }
        private SpriteRenderer rend;

        // Use this for initialization
        void Awake()
        {
            rend = GetComponent<SpriteRenderer>();
            cellColor = Color.white;
        }

        void Update()
        {
                rend.color = cellColor;
              //  UpdateCell(sim.getCellData(cellID));
        }

        public void UpdateCell(CellData cell)
        {
            switch(cell.cType)
            {
                case CellType.Solid:
                    cellColor = Color.gray;
                    break;
                case CellType.Air:
                    cellColor = Color.white;
                    break;
                case CellType.Water:
                    cellColor = Color.Lerp(Color.white, Color.blue,
                        Mathf.Max(
                        (cell.cellMass - sim.MinMass) / (sim.MaxMass - sim.MinMass),
                        0.2f));
                    break;
            }
        }

    }
}