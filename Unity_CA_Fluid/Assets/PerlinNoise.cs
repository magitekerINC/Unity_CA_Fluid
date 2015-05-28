using UnityEngine;
using System.Collections.Generic;

namespace FluidCA.Util
{

    public class PerlinNoise
    {
        /// <summary>
        /// Returns a height map of floats.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="variance"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static List<float> generatePerlin1Dim(int width, int height, float variance, float detail)
        {
            List<float> noise = new List<float>();
            for (int i = 0; i < width / detail + 2; ++i)
            {
                noise.Add(Mathf.Round(height + Mathf.Max(0f, Random.Range(0f,1f) * variance)));

            }


            List<float> heightMap = new List<float>();
            for(int i=0; i < width; ++i)
            {
            
                heightMap.Add(Mathf.Round(
                    Mathf.Lerp(
                    (i % detail)/ detail,
                    noise[(int)Mathf.Round(i / detail)],
                    noise[(int)Mathf.Round(i / detail) + 1]
                    )));
            }
            for(int i=1; i < width; ++i)
            {
            
                heightMap[i] = Mathf.Round(
                    (heightMap[i-1] + heightMap[i] + heightMap[i+1]) / 3
                    );
            }

            return heightMap;
        }


        /// <summary>
        /// Returns a width by height perlin noise map
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static List<float> generatePerlinNoise(int width, int height, float detail)
        {
            List<float> noise = new List<float>();

            return noise;
        }

    }
}