using System.Collections;
using System.Collections.Generic;

//Class for calculating the map
public class CellularAutomata
{
    //Main
    public static Vertex[,] gen(Vertex[,] inputMap, int fillPercentage, int smoothCount, int smoothValue)
    {
        inputMap = CellularAutomata.randomize(inputMap, fillPercentage);
        for (int i = 0; i < smoothCount; i++)
        {
            inputMap = CellularAutomata.smooth(inputMap, smoothValue);
        }
        return inputMap;
    }

    //Fill map with vertices that are either activ or not activ, based on a %
    private static Vertex[,] randomize(Vertex[,] inputMap, int fillPercentage)
    {
        System.Random rand = new System.Random();
        for (int y = 0; y < inputMap.GetLength(1); y++)
        {
            for (int x = 0; x < inputMap.GetLength(0); x++)
            {
                if (rand.Next(0, 99) < fillPercentage) inputMap[x, y] = new Vertex(new int[] { x, y }, true);
                else inputMap[x, y] = new Vertex(new int[] { x, y }, false);
            }
        }
        return inputMap;
    }

    //Smooth every vertex by changing if they are activ based on their neighbour count awdf
    private static Vertex[,] smooth(Vertex[,] inputMap, int value)
    {
        for (int y = 0; y < inputMap.GetLength(1); y++)
        {
            for (int x = 0; x < inputMap.GetLength(0); x++)
            {
                if (CellularAutomata.getSurroundingCount(new int[] { x, y }, inputMap) > value) inputMap[x, y].IsActiv = true;
                else inputMap[x, y].IsActiv = false;
            }
        }
        return inputMap;
    }

    //Name says it all
    private static int getSurroundingCount(int[] pos, Vertex[,] inputMap)
    {
        int count = 0;
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if ((x == 0 || y == 0) && pos[0] + x >= 0 && pos[0] + x < inputMap.GetLength(0) && pos[1] + y >= 0 && pos[1] + y < inputMap.GetLength(1) && inputMap[pos[0] + x, pos[1] + y].IsActiv)
                {
                    count++;
                }
            }
        }
        return count;
    }
}
