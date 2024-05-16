using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GenerateTerrain : MonoBehaviour
{
    public int size = 20;
    public float heightScale = 5f;
    public float detailScale = 5f;
    public float frequency = 1f;
    public float amplitude = 1f;
    public int octaves = 4; 
    public float p = 2;

    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;
    private Color[] colourMap;
    private float minHeight;
    private float maxHeight;
    private float[,] heightMap;
    private Mesh mesh;

    public ColorTerrains colorTerrains;
    public Button generate;
    public Button reset;
    public Slider frequencySlider;
    public Slider amplitudeSlider;
    public Slider heightSlider;
    public Slider smoothSlider;
    public Slider octaveSlider;
    public Slider randomForOctaveSlider;

    void Start()
    {
        generate.onClick.AddListener(GenerateMeshAndColor);
        reset.onClick.AddListener(Reset);
        frequencySlider.onValueChanged.AddListener(ChangeFrequencyScale);
        amplitudeSlider.onValueChanged.AddListener(ChangeAmplitudeScale);   
        heightSlider.onValueChanged.AddListener(ChangeHeightScale);
        smoothSlider.onValueChanged.AddListener(ChangeDetailScale);
        octaveSlider.onValueChanged.AddListener(ChangeOctaveNumber);
        randomForOctaveSlider.onValueChanged.AddListener(ChangeRandomForOctave);
        
    }

    public void GenerateMeshAndColor()
    {
        // Generate heightMap
        heightMap = GenerateHeightMap();
        //generate terrain mesh
        Generate();
        //updatemax,min heights
        UpdateMaxMinHeight();
        // Call ColorTerrain to assign colors based on heightMap
        colourMap = colorTerrains.ColorTerrain(heightMap, vertices, minHeight, maxHeight, colourMap, size);
        //update mesh
        UpdateMesh();

    }


    void Generate()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "GeneratedTerrain";
        vertices = new Vector3[(size + 1) * (size + 1)];
        triangles = new int[size * size * 6];
        uvs = new Vector2[vertices.Length];
        colourMap = new Color[(size + 1) * (size + 1)];
        for (int i = 0, y = 0; y <= size; y++)
        {
            for (int x = 0; x <= size; x++, i++)
            {
                float value = 0f;
                // Use octaves and random offsets to generate heightMap
                for (int o = 0; o <= octaves; o++)
                {
                    float randomOffset = Random.Range(-p, p);
                    amplitude += randomOffset;
                    frequency += randomOffset;

                    value += Mathf.PerlinNoise(x * frequency / detailScale, y * frequency / detailScale) * amplitude;
                }
                vertices[i] = new Vector3(x, value * heightScale, y);
                uvs[i] = new Vector2(x / (float)size, y / (float)size);
            }
        }
        int vert = 0;
        int tris = 0;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + size + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + size + 1;
                triangles[tris + 5] = vert + size + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
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

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colourMap;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        transform.position = new Vector3(-(int)size/2 ,-10, -(int)size/2);
    }

    float[,] GenerateHeightMap()
    {
        float[,] heightMap = new float[size + 1, size + 1];

        for (int y = 0; y <= size; y++)
        {
            for (int x = 0; x <= size; x++)
            {
                float value = 0f;

                // Use octaves and random offsets to generate heightMap
                for (int o = 0; o <= octaves; o++)
                {
                    float randomOffset = Random.Range(-p, p);
                    amplitude += randomOffset;
                    frequency += randomOffset;

                    value += Mathf.PerlinNoise(x * frequency / detailScale, y * frequency / detailScale) * amplitude;
                }

                heightMap[x, y] = value * heightScale;
            }
        }

        return heightMap;
    }

    void Reset()
    {
        if (GetComponent<MeshFilter>().mesh)
        {
            GetComponent<MeshFilter>().mesh = null;
        }
    }

    void ChangeFrequencyScale(float sliderValue)
    {
        frequency = sliderValue;

        GenerateMeshAndColor();
    }

    void ChangeAmplitudeScale(float sliderValue)
    {
        amplitude = sliderValue;

        GenerateMeshAndColor();
    }
    void ChangeHeightScale(float sliderValue)
    {
        heightScale = sliderValue;

        GenerateMeshAndColor();
    }

    void ChangeDetailScale(float sliderValue)
    {
        detailScale = sliderValue;

        GenerateMeshAndColor();
    }

    void ChangeOctaveNumber(float sliderValue)
    {
        octaves = (int)sliderValue;

        GenerateMeshAndColor();
    }

    void ChangeRandomForOctave(float sliderValue)
    {
        p = sliderValue;

        GenerateMeshAndColor();
    }

}
