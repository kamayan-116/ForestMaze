using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dijkstra
{
    public static readonly int STEPMAX = 10000;
    private readonly MakeMaze mazedata;
    public readonly int[,] costmap;

    public Dijkstra(MakeMaze _mazedata)
    {
        mazedata = _mazedata;
        costmap = new int[mazedata.max + 2, mazedata.max + 2];
        for(int j=0; j<mazedata.max + 2; ++j)
        {
            for(int i=0; i<mazedata.max + 2; ++i)
            {
                costmap[j, i] = STEPMAX;
            }
        }
    }

    public List<Vector2Int> DijkstraFinding(Vector2Int _start, Vector2Int _goal)
    {
        costmap[_goal.y+1, _goal.x+1] = 0;
        
        for(int nowcost = 0; nowcost < STEPMAX; ++nowcost)
        {
            CalclateCost(nowcost);
        }

        // var result = new List<Vector2Int> {_start};
        var result = new List<Vector2Int>();

        var travelingpos = _start;
        // Debug.Log(costmap[travelingpos.y+1,travelingpos.x+1]);
        while(costmap[travelingpos.y+1,travelingpos.x+1] > 0)
        {
            int costtmp = costmap[travelingpos.y+1, travelingpos.x+1];
            Vector2Int resultpos = travelingpos;
            foreach (var position in MakeMaze.Positions)
            {
                var tmppos = travelingpos + position;
                if(costtmp > costmap[tmppos.y+1,tmppos.x+1])
                {
                    costtmp = costmap[tmppos.y+1, tmppos.x+1];
                    resultpos = tmppos;
                }
            }
            travelingpos = resultpos;
            result.Add(travelingpos);
        }

        return result;
    }

    private void CalclateCost(int _nowcost)
    {
        for(int j=0; j<mazedata.max + 2; ++j)
        {
            for(int i=0; i<mazedata.max + 2; ++i)
            {
                if (costmap[j, i] == _nowcost)
                {
                    SetValueAround(new Vector2Int(i-1, j-1), _nowcost);
                }
            }
        }
    }

    private void SetValueAround(Vector2Int _pos, int _nowcost)
    {
        foreach(var position in MakeMaze.Positions)
        {
            var tmppos = (_pos + position);
            if(!mazedata.IsOutOfBounds(tmppos.x, tmppos.y))
            {
                if(mazedata.cells[tmppos.x, tmppos.y] == MakeMaze.CellType.Path && costmap[tmppos.y+1, tmppos.x+1] > _nowcost + 1)
                {
                    costmap[tmppos.y+1, tmppos.x+1] = _nowcost + 1;
                }
            }
        }
    }
}
