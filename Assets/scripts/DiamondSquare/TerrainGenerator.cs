using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class TerrainGenerator : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] uvs;
    private int[] triangles;
    private Color[] colourMap;
    private int size;
    private float minHeight;
    private float maxHeight;
    private float[,] heightMap;


    public float range;
    public float Roughness;
    public int dimension;
    public bool randomCornerHeight;
    public Button generate;
    public Button reset;
    public Slider HeightrangeSlider;
    public Slider RoughnessSlider;
    public Slider DimensionSlider;
    public Toggle cornersRandomToggle;
    public ColorTerrains colorTerrains;

    public void Start()
    {
        generate.onClick.AddListener(Click);
        reset.onClick.AddListener(Reset);
        RoughnessSlider.onValueChanged.AddListener(Changesmoothcale);
        DimensionSlider.onValueChanged.AddListener(ChangeDimensionScale);
        HeightrangeSlider.onValueChanged.AddListener(ChangeHeightScale);
    }

    public void ChangeCornerState()
    {
        randomCornerHeight = cornersRandomToggle.isOn;
        
    }
    public void ChangeHeightScale(float sliderValue)
    {
        //5-60
        range = sliderValue;
        Click();
    }

    public void Changesmoothcale(float sliderValue)
    {
        //0-1
        Roughness = sliderValue;
        Click();
    }

    public void ChangeDimensionScale(float sliderValue)
    {
        //0-10
        dimension = (int)sliderValue;
        Click();

    }

    public void Click()
    {
        size = (int)Mathf.Pow(2, dimension);
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;
        CreateMesh();
        GenerateHeightMapDiamondSquare();
        UpdateMaxMinHeight();
        colourMap = colorTerrains.ColorTerrain(heightMap,vertices,minHeight,maxHeight,colourMap,size);
        UpdateMesh();
    }

    void CreateMesh()
    {
        vertices = new Vector3[size * size];
        uvs = new Vector2[size * size];
        triangles = new int[(size - 1) * (size - 1) * 6];
        colourMap = new Color[size * size];
        int i = 0;
        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                vertices[i] = new Vector3(x, 0, z);
                i++;
            }
        }
        int vert = 0;
        int tris = 0;
        for (int y = 0; y < size - 1; y++)
        {
            for (int x = 0; x < size - 1; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + size;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + size;
                triangles[tris + 5] = vert + size + 1;
                uvs[vert] = new Vector2(x / (float)size, y / (float)size);
                vert++;
                tris += 6;
            }
            vert++;
        }
    }
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colourMap;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        transform.position = new Vector3(-(int)Mathf.Pow(2, dimension - 1), -10, -(int)Mathf.Pow(2, dimension - 1));
    }

    public void UpdateMaxMinHeight()
    {
        maxHeight = int.MinValue;
        minHeight = int.MaxValue;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float currentHeight = heightMap[x, y];
                if (currentHeight > maxHeight)
                {
                    maxHeight = currentHeight;
                }

                if (currentHeight < minHeight)
                {
                    minHeight = currentHeight;
                }
            }
        }
    }

   

    public void GenerateHeightMapDiamondSquare()
    {
        // initialize heightMap with zeros
        heightMap = new float[size + 1, size + 1];

        // set seed for random number generator
        SetSeed(UnityEngine.Random.Range(0, 100));

        // Assign random value within (-range, range) to each corner
        if (randomCornerHeight)
        {
            heightMap[0, size] = UnityEngine.Random.Range(-range, range);
            heightMap[0, 0] = UnityEngine.Random.Range(-range, range);
            heightMap[size, 0] = UnityEngine.Random.Range(-range, range);
            heightMap[size, size] = UnityEngine.Random.Range(-range, range);
        }
        else
        {
            heightMap[0, size] = range;
            heightMap[0, 0] = range;
            heightMap[size, 0] = range;
            heightMap[size, size] = range;
        }

        DiamondSquareRecursive(0, 0, size, size, range);
    }

    void DiamondSquareRecursive(int x1, int y1, int x2, int y2, float range)
    {
        if (x2 - x1 == 1 && y2 - y1 == 1)
            return;

        int xm = (x1 + x2) / 2;
        int ym = (y1 + y2) / 2;

        float avgDiamond = (heightMap[x1, y1] + heightMap[x2, y1] + heightMap[x1, y2] + heightMap[x2, y2]) / 4.0f;
        heightMap[xm, ym] = avgDiamond + UnityEngine.Random.Range(-range, range);

        float avgSquareTop = (heightMap[x1, y1] + heightMap[x2, y1] + heightMap[xm, ym]) / 3.0f;
        float avgSquareLeft = (heightMap[x1, y1] + heightMap[x1, y2] + heightMap[xm, ym]) / 3.0f;
        float avgSquareRight = (heightMap[x2, y1] + heightMap[x2, y2] + heightMap[xm, ym]) / 3.0f;
        float avgSquareBottom = (heightMap[x1, y2] + heightMap[x2, y2] + heightMap[xm, ym]) / 3.0f;

        heightMap[xm, y1] = avgSquareTop + UnityEngine.Random.Range(-range, range);
        heightMap[xm, y2] = avgSquareBottom + UnityEngine.Random.Range(-range, range);
        heightMap[x1, ym] = avgSquareLeft + UnityEngine.Random.Range(-range, range);
        heightMap[x2, ym] = avgSquareRight + UnityEngine.Random.Range(-range, range);

        range *= Roughness;

        DiamondSquareRecursive(x1, y1, xm, ym, range / 2);
        DiamondSquareRecursive(xm, y1, x2, ym, range / 2);
        DiamondSquareRecursive(x1, ym, xm, y2, range / 2);
        DiamondSquareRecursive(xm, ym, x2, y2, range / 2);
    }

    public void SetSeed(int seed)
    {
        UnityEngine.Random.InitState(seed);
    }

    void Reset()
    {
        if (GetComponent<MeshFilter>().mesh)
        {
            GetComponent<MeshFilter>().mesh = null;
        }
    }
}


