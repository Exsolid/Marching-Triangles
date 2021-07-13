using System.Collections;
using System.Collections.Generic;
public class Cluster
{
    public Cluster()
    {
        vertices = new List<Vertex>();
        bounds = new List<Vertex>();
        allMeshPoints = new List<Vertex>();
    }

    private List<Vertex> allMeshPoints;
    public List<Vertex> AllMeshPoints { get { return allMeshPoints; } set { allMeshPoints = value; } }
    private List<Vertex> vertices;
    public List<Vertex> Vertices { get { return vertices; } set { vertices = value; } }
    private List<Vertex> bounds;
    public List<Vertex> Bounds { get { return bounds; } set { bounds = value; } }
}
