using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
public class DrawMesh : MonoBehaviour
{
    //Unity stuff
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int smoothCount;
    [SerializeField] private int smoothValue;
    [SerializeField] private int fillPercentage;
    private TriMesh mesh;
    private Vertex[,] map;
    private void Start()
    {
    }
    private void OnMouseUp()
    {
        map = CellularAutomata.gen(new Vertex[width, height], fillPercentage, smoothCount, smoothValue);

        string str = "\n";
        if (map != null)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    str = str + (map[x, y].IsActiv ? "1" : "?") + " ";
                }
                str = str + "\n";
            }
            System.Diagnostics.Debug.WriteLine(str);
        }
        mesh = new TriMesh(map);
        mesh.generateMesh(true, 3);
    }
    //Draw screen
    private void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    Gizmos.color = map[x, y].IsActiv ? (map[x, y].IsMeshUsed ? Color.red : Color.black) : (map[x, y].IsMeshUsed ? Color.blue : Color.white);
                    Gizmos.DrawCube(new Vector3(x, y), Vector3.one/2);
                }
            }
        }
        if(mesh != null)
        {
            foreach (Tri tri in mesh.Triangles)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(new Vector2(tri.Edges[0].Vertices[0].Position[0], tri.Edges[0].Vertices[0].Position[1]), new Vector2(tri.Edges[0].Vertices[1].Position[0], tri.Edges[0].Vertices[1].Position[1]));
                Gizmos.DrawLine(new Vector2(tri.Edges[1].Vertices[0].Position[0], tri.Edges[1].Vertices[0].Position[1]), new Vector2(tri.Edges[1].Vertices[1].Position[0], tri.Edges[0].Vertices[1].Position[1]));
                Gizmos.DrawLine(new Vector2(tri.Edges[2].Vertices[0].Position[0], tri.Edges[2].Vertices[0].Position[1]), new Vector2(tri.Edges[2].Vertices[1].Position[0], tri.Edges[0].Vertices[1].Position[1]));
            }

            //foreach (Edge edge in mesh.edges)
            //{
            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawLine(new Vector2(edge.Vertices[0].Position[0], edge.Vertices[0].Position[1]), new Vector2(edge.Vertices[1].Position[0], edge.Vertices[1].Position[1]));
            //}
        }
    }
}




