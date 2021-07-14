using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
public class TriMesh
{
    public List<Tri> Triangles { get { return triangles; } }
    public List<Edge> edges;
    private List<Tri> triangles;
    private Vertex[,] inputMap;

    public TriMesh(Vertex[,] inputMap)
    {
        this.inputMap = inputMap;
        triangles = new List<Tri>();
        edges = new List<Edge>();
    }

    public void generateMesh(bool useActivVertices, int useEveryXForMesh)
    {
        System.Diagnostics.Debug.WriteLine(this.getSizeOfTriangle(new int[] { 7, 4 }, new int[] { 9, 7 }, new int[] { 8, 6 }));
        List<Cluster> clusters = saveClusters(useActivVertices, useEveryXForMesh);
        foreach (Cluster cluster in clusters)
        {
            generateTris(cluster);
        }
    }

    private void generateTris(Cluster cluster)
    {
        Queue<Edge> initialEdges = generateInitialEdges(cluster);
        foreach (Edge e in initialEdges)
        {
            edges.Add(e);
        }
        Edge first = null;
        do
        {
            Edge current = initialEdges.Dequeue();
            if (first == null || (current.Vertices[0].Position[0] == current.Vertices[1].Position[0] && current.Vertices[0].Position[0] < first.Vertices[0].Position[0]))
            {
                first = current;
            }
        } while (initialEdges.Count != 0);
        initialEdges.Enqueue(first);
        string str = "\n";
        if (inputMap != null)
        {
            for (int y = 0; y < inputMap.GetLength(1); y++)
            {
                for (int x = 0; x < inputMap.GetLength(0); x++)
                {
                    str = str + (inputMap[x, y].IsActiv ? (inputMap[x, y].IsMeshUsed ? "-" : "1") : "?") + " ";
                }
                str = str + "\n";
            }
            System.Diagnostics.Debug.WriteLine(str);
        }
        str = "";
        do
        {
            Vertex best = null;
            Edge current = initialEdges.Dequeue();
            if (current.Vertices[0] == null || current.Vertices[1] == null)
            {
                int i = 0;
            }
            int[] averagePos = new int[] { (int)Math.Floor((current.Vertices[0].Position[0] + current.Vertices[1].Position[0]) / 2f), (int)Math.Floor((current.Vertices[0].Position[1] + current.Vertices[1].Position[1]) / 2f )};
            System.Diagnostics.Debug.WriteLine("Comparing for: " + "[" + averagePos[0] + "]" + "[" + averagePos[1] + "]");
            foreach (Vertex vertex in cluster.AllMeshPoints)
            {
                if (best != null) System.Diagnostics.Debug.WriteLine("Comparing: " + "[" + vertex.Position[0] + "]" + "[" + vertex.Position[1] + "]" + " to current best " + "[" + best.Position[0] + "]" + "[" + best.Position[1] + "]" + (Math.Sqrt(Math.Pow(best.Position[0] - averagePos[0], 2) + Math.Pow(best.Position[1] - averagePos[1], 2)) + ">" + (Math.Sqrt(Math.Pow(vertex.Position[0] - averagePos[0], 2) + Math.Pow(vertex.Position[1] - averagePos[1], 2)))));
                bool isInRightDir = isInRightDirection(vertex.Position, current);
                if ((best == null && isInRightDir)
                    || (best != null && !vertex.Equals(current.Vertices[0]) && !vertex.Equals(current.Vertices[1]) && isInRightDir
                        && isCloserTo(averagePos, best.Position, vertex.Position)))
                {
                    System.Diagnostics.Debug.WriteLine("true");
                    best = vertex;
                }
            }
            if (best != null) System.Diagnostics.Debug.WriteLine("Finished with " + "[" + best.Position[0] + "]" + "[" + best.Position[1] + "]");
            if (best != null && current != null)
            {
                Edge connection0 = null;
                Edge connection1 = null;
                foreach (Edge edge in best.Parents)
                {
                    if (best.getOtherOf(edge) != null)
                    {
                        if (best.getOtherOf(edge).Equals(current.Vertices[0]))
                        {
                            connection0 = edge;
                        }
                        if (best.getOtherOf(edge).Equals(current.Vertices[1]))
                        {
                            connection1 = edge;
                        }
                    }
                }
                if (connection0 == null)
                {
                    connection0 = new Edge();
                    connection0.Vertices[1] = best;
                    best.Parents.Add(connection0);
                    connection0.Vertices[0] = current.Vertices[0];
                    current.Vertices[0].Parents.Add(connection0);
                    initialEdges.Enqueue(connection0);
                }
                if (connection1 == null)
                {
                    connection1 = new Edge();
                    connection1.Vertices[0] = best;
                    best.Parents.Add(connection1);
                    connection1.Vertices[1] = current.Vertices[1];
                    current.Vertices[1].Parents.Add(connection1);
                    initialEdges.Enqueue(connection1);
                }
                if (connection1.Vertices[1] == null || connection1.Vertices[0] == null || connection0.Vertices[0] == null || connection0.Vertices[1] == null)
                {
                    int i = 0;
                }
                Tri tri = new Tri();
                tri.Edges = new Edge[] { current, connection0, connection1 };
                triangles.Add(tri);
            }
        } while (initialEdges.Count != 0);
    }

    private bool isInRightDirection(int[] current, Edge egde)
    {
        int[] minPos = new int[] { Math.Min(egde.Vertices[0].Position[0], egde.Vertices[1].Position[0]), Math.Min(egde.Vertices[0].Position[1], egde.Vertices[1].Position[1]) };
        int[] maxPos = new int[] { Math.Max(egde.Vertices[0].Position[0], egde.Vertices[1].Position[0]), Math.Max(egde.Vertices[0].Position[1], egde.Vertices[1].Position[1]) };
        if (egde.Vertices[0].Position[0] == egde.Vertices[1].Position[0]) return current[0] > egde.Vertices[0].Position[0];
        else if (egde.Vertices[0].Position[1] == egde.Vertices[1].Position[1]) return current[1] < egde.Vertices[0].Position[0];
        else if ((egde.Vertices[0].Position[0] < egde.Vertices[1].Position[0] && egde.Vertices[0].Position[1] > egde.Vertices[1].Position[1])
                 || (egde.Vertices[0].Position[0] > egde.Vertices[1].Position[0] && egde.Vertices[0].Position[1] < egde.Vertices[1].Position[1]))
        {
            //x2 or x1
            //1c    2c
            return isInTriangle(egde.Vertices[0].Position, egde.Vertices[1].Position, maxPos, current) || (current[0] > minPos[0] && current[1] > minPos[1]) || (current[0] > maxPos[0] && current[1] > maxPos[1]);
        }
        else if ((egde.Vertices[0].Position[0] < egde.Vertices[1].Position[0] && egde.Vertices[0].Position[1] < egde.Vertices[1].Position[1])
                 || (egde.Vertices[0].Position[0] > egde.Vertices[1].Position[0] && egde.Vertices[0].Position[1] > egde.Vertices[1].Position[1]))
        {
            //2c or 1c
            //x1    x2
            return isInTriangle(egde.Vertices[0].Position, egde.Vertices[1].Position, minPos, current) || (current[0] > minPos[0] && current[1] < minPos[1]) || (current[0] > maxPos[0] && current[1] < maxPos[1]);
        }
        //If edge lays past current: c2  ... 1x
        //                           1x      c2
        return false;
    }

    private bool isCloserTo(int[] posToCompare, int[] oldPos, int[] newPos)
    {
        return Math.Sqrt(Math.Pow(oldPos[0] - posToCompare[0], 2) + Math.Pow(oldPos[1] - posToCompare[1], 2))
            > Math.Sqrt(Math.Pow(newPos[0] - posToCompare[0], 2) + Math.Pow(newPos[1] - posToCompare[1], 2));
    }

    private bool isInTriangle(int[] point1, int[] point2, int[] point3, int[] newPoint)
    {
        float total = getSizeOfTriangle(point1, point2, point3);
        float newTotal1 = getSizeOfTriangle(point1, point2, newPoint);
        float newTotal2 = getSizeOfTriangle(newPoint, point2, point3);
        float newTotal3 = getSizeOfTriangle(point1, newPoint, point3);
        return total == newTotal1 + newTotal2 + newTotal3 && newTotal1 != 0 && newTotal2 != 0 && newTotal3 != 0;
    }

    private float getSizeOfTriangle(int[] point1, int[] point2, int[] point3)
    {
        float length1 = (float)Math.Sqrt(Math.Pow(point1[0] - point2[0], 2) + Math.Pow(point1[1] - point2[1], 2));
        float length2 = (float)Math.Sqrt(Math.Pow(point1[0] - point3[0], 2) + Math.Pow(point1[1] - point3[1], 2));
        float length3 = (float)Math.Sqrt(Math.Pow(point3[0] - point2[0], 2) + Math.Pow(point3[1] - point2[1], 2));
        float semi = (length1 + length2 + length3) / 2;
        return (float)Math.Sqrt(semi * (semi - length1) + (semi - length2) * (semi - length3));
    }
    private void printList(List<Vertex> list, bool x)
    {
        foreach (Vertex ver in list)
        {
            System.Diagnostics.Debug.WriteLine("[" + ver.Position[0] + "]" + "[" + ver.Position[1] + "]");
        }
        System.Diagnostics.Debug.WriteLine("--------------------");
    }

    private void printList(Edge[] list)
    {
        foreach (Edge e in list)
        {
            if (e.Vertices[0] == null) e.Vertices[0] = new Vertex(new int[] { -1, -1 }, true);
            if (e.Vertices[1] == null) e.Vertices[1] = new Vertex(new int[] { -1, -1 }, true);
            string str = "\n";
            if (e.Vertices[0].Position[0] == e.Vertices[1].Position[0]) str = str + " On Y: " + "[" + e.Vertices[0].Position[0] + "]" + "[" + e.Vertices[0].Position[1] + "]" + "-->" + "[" + e.Vertices[1].Position[0] + "]" + "[" + e.Vertices[1].Position[1] + "]";
            else str = str + " On X: " + "[" + e.Vertices[0].Position[0] + "]" + "[" + e.Vertices[0].Position[1] + "]" + "-->" + "[" + e.Vertices[1].Position[0] + "]" + "[" + e.Vertices[1].Position[1] + "]";

            System.Diagnostics.Debug.WriteLine(str);
        }
        System.Diagnostics.Debug.WriteLine("--------------------");
    }

    private Queue<Edge> generateInitialEdges(Cluster cluster)
    {
        Queue<Edge> edges = new Queue<Edge>();
        //Create edges on X
        printList(cluster.Bounds, true);
        cluster.Bounds.Sort(new VertexComparer(true));
        printList(cluster.Bounds, true);
        IEnumerator<Vertex> enumerator = cluster.Bounds.GetEnumerator();
        Vertex prev = null;
        Edge edge = new Edge();
        while (enumerator.MoveNext())
        {
            if (prev == null)
            {
                edge.Vertices[0] = enumerator.Current;
                enumerator.Current.IsMeshUsed = true;
                cluster.AllMeshPoints.Add(enumerator.Current);
                enumerator.Current.Parents.Add(edge);
            }
            else if (edge.Vertices[1] == null && (prev.Position[0] != enumerator.Current.Position[0] || prev.Position[1] + 1 != enumerator.Current.Position[1]))
            {
                if (prev != edge.Vertices[0])
                {
                    edge.Vertices[1] = prev;
                    if (!prev.IsMeshUsed)
                    {
                        prev.IsMeshUsed = true;
                        cluster.AllMeshPoints.Add(prev);
                    }
                    prev.Parents.Add(edge);
                    edges.Enqueue(edge);
                }

                edge = new Edge();
                edge.Vertices[0] = enumerator.Current;
                if (!enumerator.Current.IsMeshUsed)
                {
                    enumerator.Current.IsMeshUsed = true;
                    cluster.AllMeshPoints.Add(enumerator.Current);
                }
                enumerator.Current.Parents.Add(edge);
            }
            prev = enumerator.Current;
        }
        if (edge.Vertices[1] == null && (prev.Position[0] != cluster.Bounds[cluster.Bounds.Count - 1].Position[0] || prev.Position[1] + 1 != cluster.Bounds[cluster.Bounds.Count - 1].Position[1]))
        {
            if (prev != edge.Vertices[0])
            {
                edge.Vertices[1] = prev;
                if (!prev.IsMeshUsed)
                {
                    prev.IsMeshUsed = true;
                    cluster.AllMeshPoints.Add(prev);
                }
                prev.Parents.Add(edge);
                edges.Enqueue(edge);
            }
        }

        //Create edges on Y
        cluster.Bounds.Sort(new VertexComparer(false));
        enumerator = cluster.Bounds.GetEnumerator();
        prev = null;
        edge = new Edge();
        while (enumerator.MoveNext())
        {
            if (prev == null)
            {
                edge.Vertices[0] = enumerator.Current;
                enumerator.Current.IsMeshUsed = true;
                cluster.AllMeshPoints.Add(enumerator.Current);
                enumerator.Current.Parents.Add(edge);
            }
            else if (edge.Vertices[1] == null && (prev.Position[1] != enumerator.Current.Position[1] || prev.Position[0] + 1 != enumerator.Current.Position[0]))
            {
                if (prev != edge.Vertices[0])
                {
                    edge.Vertices[1] = prev;
                    if (!prev.IsMeshUsed)
                    {
                        prev.IsMeshUsed = true;
                        cluster.AllMeshPoints.Add(prev);
                    }
                    prev.Parents.Add(edge);
                    edges.Enqueue(edge);
                }

                edge = new Edge();
                edge.Vertices[0] = enumerator.Current;
                if (!enumerator.Current.IsMeshUsed)
                {
                    enumerator.Current.IsMeshUsed = true;
                    cluster.AllMeshPoints.Add(enumerator.Current);
                }
                enumerator.Current.Parents.Add(edge);
            }
            prev = enumerator.Current;
        }
        if (edge.Vertices[1] == null && (prev.Position[1] != cluster.Bounds[cluster.Bounds.Count - 1].Position[1] || prev.Position[0] + 1 != cluster.Bounds[cluster.Bounds.Count - 1].Position[0]))
        {
            if (prev != edge.Vertices[0])
            {
                edge.Vertices[1] = prev;
                if (!prev.IsMeshUsed)
                {
                    prev.IsMeshUsed = true;
                    cluster.AllMeshPoints.Add(prev);
                }
                prev.Parents.Add(edge);
                edges.Enqueue(edge);
            }
        }
        printList(edges.ToArray());
        return edges;
    }

    private List<Cluster> saveClusters(bool useActivVertices, int useEveryXForMesh)
    {
        List<Vertex> notUsed = new List<Vertex>(convertArray());
        List<Cluster> clusters = new List<Cluster>();
        int counter = 0;
        IEnumerator<Vertex> enumerator = notUsed.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.IsActiv == useActivVertices && !enumerator.Current.IsClusterUsed)
            {
                Cluster cluster = getClusterOfPoint(new int[] { counter % inputMap.GetLength(1), counter / inputMap.GetLength(1) }, useActivVertices, useEveryXForMesh);
                clusters.Add(cluster);
            }
            counter++;
        }
        return clusters;
    }
    //Use flood fill to determin all neighbouring vertices of a kind and return them as a cluster
    private Cluster getClusterOfPoint(int[] position, bool useActivVertices, int useEveryXForMesh)
    {
        List<Vertex> used = new List<Vertex>();
        if (useActivVertices == inputMap[position[0], position[1]].IsActiv)
        {
            used.Add(inputMap[position[0], position[1]]);
        }
        Cluster cluster = new Cluster();
        int counter = 0;
        int usedCounter = 0;
        while (usedCounter < used.Count)
        {
            Vertex current = used[usedCounter];
            usedCounter++;
            if (current != null && useActivVertices == current.IsActiv && !current.IsClusterUsed)
            {
                bool wasUsedInBounds = saveBounds(cluster, current, useActivVertices);
                counter++;
                if (!wasUsedInBounds && counter >= useEveryXForMesh)
                {
                    cluster.AllMeshPoints.Add(current);
                    current.IsMeshUsed = true;
                    counter = 0;
                }
                cluster.Vertices.Add(current);
                current.IsClusterUsed = true;

                if (current.Position[0] - 1 >= 0 && !used.Contains(inputMap[current.Position[0] - 1, current.Position[1]]))
                {
                    used.Add(inputMap[current.Position[0] - 1, current.Position[1]]);
                }
                if (current.Position[0] + 1 < inputMap.GetLength(0) && !used.Contains(inputMap[current.Position[0] + 1, current.Position[1]]))
                {
                    used.Add(inputMap[current.Position[0] + 1, current.Position[1]]);
                }
                if (current.Position[1] - 1 >= 0 && !used.Contains(inputMap[current.Position[0], current.Position[1] - 1]))
                {
                    used.Add(inputMap[current.Position[0], current.Position[1] - 1]);
                }
                if (current.Position[1] + 1 < inputMap.GetLength(1) && !used.Contains(inputMap[current.Position[0], current.Position[1] + 1]))
                {
                    used.Add(inputMap[current.Position[0], current.Position[1] + 1]);
                }
            }
        }
        return cluster;
    }

    //Saves the vertex in a separate list, if it is part of the boundaries of the cluster
    private bool saveBounds(Cluster cluster, Vertex vertex, bool useActivVertices)
    {
        int[] pos = vertex.Position;
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (pos[0] + x == -1 || pos[0] + x == inputMap.GetLength(0) || pos[1] + y == -1 || pos[1] + y == inputMap.GetLength(1) || (pos[0] + x >= 0 && pos[0] + x < inputMap.GetLength(0) && pos[1] + y >= 0 && pos[1] + y < inputMap.GetLength(1)) && inputMap[pos[0] + x, pos[1] + y].IsActiv != useActivVertices)
                {
                    cluster.Bounds.Add(inputMap[pos[0], pos[1]]);
                    return true;
                }
            }
        }
        return false;
    }

    private Vertex[] convertArray()
    {
        Vertex[] newArray = new Vertex[inputMap.GetLength(0) * inputMap.GetLength(1)];
        int counter = 0;
        for (int y = 0; y < inputMap.GetLength(1); y++)
        {
            for (int x = 0; x < inputMap.GetLength(0); x++)
            {
                newArray[counter] = inputMap[x, y];
                counter++;
            }
        }
        return newArray;
    }
}
