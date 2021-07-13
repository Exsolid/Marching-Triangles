using System.Collections;
using System.Collections.Generic;
//Base class for every point in space
public class Vertex
{
    private int[] pos;
    private bool isActiv;
    private bool isMeshUsed;
    private bool isClusterUsed;
    private HashSet<Edge> parents;
    public bool IsActiv { get { return isActiv; } set { isActiv = value; } }
    public int[] Position { get { return pos; } set { pos = value; } }
    public bool IsMeshUsed { get { return isMeshUsed; } set { isMeshUsed = value; } }
    public bool IsClusterUsed { get { return isClusterUsed; } set { isClusterUsed = value; } }
    public HashSet<Edge> Parents { get { return parents; } set { parents = value; } }

    public Vertex(int[] pos, bool isActiv)
    {
        this.isActiv = isActiv;
        this.pos = pos;
        parents = new HashSet<Edge>();
    }

    public Vertex getOtherOf(Edge edge)
    {
        if (edge.Vertices[0].Equals(this)) return edge.Vertices[1];
        else if (edge.Vertices[1].Equals(this)) return edge.Vertices[0];
        else return null;
    }
}

public class VertexComparer : IComparer<Vertex>
{
    private bool sortByX;
    public VertexComparer(bool sortByX)
    {
        this.sortByX = sortByX;
    }
    public int Compare(Vertex vertexA, Vertex vertexB)
    {
        if (sortByX)
        {
            int ori = vertexA.Position[0].CompareTo(vertexB.Position[0]);
            if (ori != 0) return ori;
            return vertexA.Position[1].CompareTo(vertexB.Position[1]);
        }
        else
        {
            int ori = vertexA.Position[1].CompareTo(vertexB.Position[1]);
            if (ori != 0) return ori;
            return vertexA.Position[0].CompareTo(vertexB.Position[0]);
        }
    }
}