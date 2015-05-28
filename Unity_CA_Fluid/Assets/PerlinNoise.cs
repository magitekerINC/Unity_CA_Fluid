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
        public static List<List<float>> generatePerlinNoise(int width, int height, float detail, float variance)
        {
            List<List<float>> noise = new List<List<float>>();
            noise.Add(generatePerlin1Dim(width, 0, variance, detail));
            for (int i = 0; i < height; ++i)
            {
                var prev = noise[noise.Count - 1];
                var next = generatePerlin1Dim(width, 50, 50f, detail);

                noise.Add(prev);
                for (int j = 1; j < detail - 1f; ++j)
                {
                    var curr = new List<float>();
                    curr.Capacity = width;
                    for (int k = 0; k < curr.Capacity; ++k)
                    {
                        var val = Mathf.Round(Mathf.Lerp(k / detail, prev[k], next[k]));
                        curr[k] = (float.NaN == val ? 0 : val);
                    }
                    noise.Add(curr);
                }
                noise.Add(next);
            }

            for (int i = 1; i < width-1; ++i)
            {
                for (int j = 1; j < height - 1; ++j)
                {
                    float agg = 0;
                    for (int k = i - 1; k < i + 2; ++k)
                    {
                        for (int n = j - 1; n < j + 2; ++n)
                        {
                            agg = noise[k][n];
                        }
                    }
                    noise[i][j] = Mathf.Round(agg / 9f);
                }
            }
                return noise;
        }

    }
}