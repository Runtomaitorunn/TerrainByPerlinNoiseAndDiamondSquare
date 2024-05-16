using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

public class ColorTerrains : MonoBehaviour
{
    public TerrainType[] regions;
    public Color[] ColorTerrain(float[,] heightMap, Vector3[] vertices, float minHeight,float maxHeight,Color[] colourMap,int size)
    {
        regions[0].height = minHeight + (maxHeight - minHeight) * 0.4f;
        regions[1].height = minHeight + (maxHeight - minHeight) * 0.8f;
        regions[2].height = minHeight + (maxHeight - minHeight);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float currentHeight = heightMap[x, y];
                vertices[y * size + x].y = currentHeight;

                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * size + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }
        return colourMap;
    }
}


