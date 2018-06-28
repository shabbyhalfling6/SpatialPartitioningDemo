using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Circle
{
    public Vector2 m_pos;
    public float m_radius;
}

public class Grid
{
    public List<Circle> circles = new List<Circle>();
}

public class Spatial : MonoBehaviour
{
    private Texture2D tex;
    public int circleCount = 1000;
    private int gridSize;
    public int cellAmount = 32;

    void Start ()
    {
        //divide the squares pixels by the amount of cells desired
        gridSize = 1024 / cellAmount;
        //make a new grid and set its amount of values to the cell amount for both axis
        Grid[,] gridArray = new Grid[cellAmount, cellAmount];
        Random.InitState(0);
        tex = GetComponent<SpriteRenderer>().sprite.texture;

        //create and feed the new grid array the grid cells and their location data
        for (int x = 0; x < cellAmount; x++)
        {
            for (int y = 0; y < cellAmount; y++)
            {
                gridArray[x,y] = new Grid();
            }
        }

        for(int i = 0; i < circleCount; ++i)
        {
            //creation of circles with random positions and sizes
            Circle c = new Circle();
            c.m_pos = new Vector2(Random.Range(0, 1023), Random.Range(0, 1023));
            c.m_radius = Random.Range(1, 49);

            //get the left, right, bottom and top sides of the circle to test which grid cell it's in
            int leftSide = (int)(c.m_pos.x - c.m_radius) / gridSize;
            int rightSide = (int)(c.m_pos.x + c.m_radius) / gridSize;
            int bottomSide = (int)(c.m_pos.y + c.m_radius) / gridSize;
            int topSide = (int)(c.m_pos.y - c.m_radius) / gridSize;

            //testing if the circles bounding box is inside the black square and set it inside the parameters if it isn't
            if (leftSide < 0)
            {
                leftSide = 0;
            }
            if (rightSide > cellAmount - 1)
            {
                rightSide = cellAmount - 1;
            }
            if (topSide < 0)
            {
                topSide = 0;
            }
            if (bottomSide > cellAmount - 1)
            {
                bottomSide = cellAmount - 1;
            }

            //looping over the circle pixel by pixel to see what grid cells the circle overlaps with
            for (int x = leftSide; x <= rightSide; x++)
            {
                for (int y = topSide; y <= bottomSide; y++)
                {
                    //add the circle to the array the circle currently is in
                    gridArray[x, y].circles.Add(c);
                }
            }
        }

        Color32[] colours = new Color32[1024 * 1024];
        float t = Time.realtimeSinceStartup;
        for (int y = 0; y < 1024; ++y)
        {
            for (int x = 0; x < 1024; ++x)
            {
                float value = 0;
                int x_ = (int)x / gridSize;
                int y_ = (int)y / gridSize;
                for (int i = 0; i < gridArray[x_, y_].circles.Count; ++i)
                {
                    float d = (new Vector2((float)x, (float)y) - gridArray[x_, y_].circles[i].m_pos).magnitude;
                    if (d < gridArray[x_, y_].circles[i].m_radius)
                    {
                        value = value + (1.0f - d / gridArray[x_, y_].circles[i].m_radius);
                       value =  Mathf.Clamp(value, 0.0f, 1.0f);
                    }

                }
                colours[x + y * 1024].r = (byte)(value * 255);
                colours[x + y * 1024].g = (byte)(value * 255);
                colours[x + y * 1024].b = (byte)(value * 255);
                colours[x + y * 1024].a = 255;
            }
        }
        Debug.Log("Time taken = " + (Time.realtimeSinceStartup - t));
        tex.SetPixels32(colours);
        tex.Apply();
    }
}
