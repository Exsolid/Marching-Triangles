using System.Collections;
using System.Collections.Generic;
public class Edge
{
    public Edge()
    {
        vertices = new Vertex[2];
    }
    private Tri parent;
    public Tri Parent { get { return parent; } set { parent = value; } }
    private Vertex[] vertices;
    public Vertex[] Vertices { get { return vertices; } set { vertices = value; } }
}
