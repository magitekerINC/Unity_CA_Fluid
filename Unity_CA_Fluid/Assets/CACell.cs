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
            return cells[x % Width, y % Height];
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

    }

    public enum CellType { Solid = 0, Air = 1, Water = 2, NumTypes }

    public class CACell : MonoBehaviour
    {
        public FluidSim sim { get; set; }
        public int cellID = 0;
        public Color cellColor { set { rend.color = value; } }
        private SpriteRenderer rend;

        // Use this for initialization
        void Awake()
        {
            rend = GetComponent<SpriteRenderer>();
            
        }


        public void UpdateCell(CellData cell)
        {
            switch(cell.cType)
            {
                case CellType.Solid:
                    rend.color = Color.gray;
                    break;
                case CellType.Air:
                    rend.color = Color.white;
                    break;
                case CellType.Water:
                    rend.color = Color.blue;
                    break;
            }
        }

    }
}