using UnityEngine;
using System.Collections.Generic;
using FluidCA.Util;

namespace FluidCA.Sim
{

    public class FluidSim : MonoBehaviour
    {

        public CACell cellPrefab;
        public float width = 0, height = 0;

        private List<CACell> cellList = new List<CACell>();

        // Use this for initialization
        void Start()
        {

            Init();
        }

        void Init()
        {
            width = Screen.width;
            height = Screen.height;

            var ratio = height / width;

            var corner = Camera.main.ScreenToWorldPoint(Vector3.zero);
            var end = Camera.main.ScreenToWorldPoint(
                new Vector3(
                width,
                height
                )
                );
            var offset = new Vector3(0.05f, 0.05f);

            corner *= ratio;
            corner.z = 1f;

            var pos = corner;
            var count = 0;

            while (pos.y <= end.y)
            {

                var gObj = Instantiate(cellPrefab, pos, Quaternion.identity) as CACell;
                gObj.cellID = count;
                count++;

                cellList.Add(gObj);

                pos.x += cellPrefab.transform.localScale.x * ratio + offset.x;
                if (pos.x > end.x)
                {
                    pos.x = corner.x;
                    pos.y += cellPrefab.transform.localScale.y * ratio + offset.y;
                }

            }
            Debug.Log(count);
        }


        // Update is called once per frame
        void Update()
        {

        }

        void OnDrawGizmos()
        {

        }
    }
}