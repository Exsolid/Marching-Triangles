using System.Collections;
using System.Collections.Generic;

public class Tri
{
    public Tri()
    {
        edges = new Edge[3];
    }
    private Edge[] edges;
    public Edge[] Edges { get { return edges; } set { edges = value; } }
}
