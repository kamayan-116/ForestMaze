using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ダイクストラ法に関するプログラム
public class Dijkstra
{
    private static readonly int STEPMAX = 10000;  // 初期化の際に入れる変数
    private readonly MakeMaze mazedata;
    private readonly int[,] costmap;  // 各Mapのコストを入れる二次元配列

    // costmapの初期化（コンストラクタ）
    public Dijkstra(MakeMaze _mazedata)
    {
        mazedata = _mazedata;
        // mazedata.maxが壁を含まないサイズに対し、costmapは壁を含んだ計算が必要のため「+2」
        costmap = new int[mazedata.max + 2, mazedata.max + 2];
        for(int j=0; j<mazedata.max + 2; ++j)
        {
            for(int i=0; i<mazedata.max + 2; ++i)
            {
                costmap[j, i] = STEPMAX;
            }
        }
    }

    /// <summary>
    /// ダイクストラ法の計算
    /// </summary>
    /// <param name="_start">スタートセル</param>
    /// <param name="_goal">ゴールセル</param>
    /// <returns></returns>
    public List<Vector2Int> DijkstraFinding(Vector2Int _start, Vector2Int _goal)
    {
        // ゴール地点を0に設定
        // _goal = (0,0)はcostmap[1,1]になる⇒以下の計算も同様の理屈
        costmap[_goal.y+1, _goal.x+1] = 0;
        
        //全Mapのコスト計算
        for(int nowcost = 0; nowcost < STEPMAX; ++nowcost)
        {
            CalclateCost(nowcost);
        }

        // ダイクストラ法の結果を入れるリスト
        var result = new List<Vector2Int>(); 

        var travelingpos = _start;  // ダイクストラ法のスタート位置（初期値：スタート座標）
        // Debug.Log(costmap[travelingpos.y+1,travelingpos.x+1]);

        // スタートのcostmapが0より大きい間継続＝ゴールに到達していない
        while(costmap[travelingpos.y+1,travelingpos.x+1] > 0)
        {
            // 現在のスタート位置のコストをcosttmpに代入
            int costtmp = costmap[travelingpos.y+1, travelingpos.x+1];
            Vector2Int resultpos = travelingpos;

            // スタート座標の前後左右を計算
            foreach (var position in MakeMaze.Positions)
            {
                var tmppos = travelingpos + position;
                // 前後左右のコストが少なければ更新
                if(costtmp > costmap[tmppos.y+1,tmppos.x+1])
                {
                    costtmp = costmap[tmppos.y+1, tmppos.x+1];
                    resultpos = tmppos;
                }
            }

            // その座標を次のスタート座標にし、resultのリストに加える
            travelingpos = resultpos;
            result.Add(travelingpos);
        }

        return result;
    }

    // 全Map中の_nowcostに当たる場所のコスト計算を行う関数
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

    // _posの場所の前後左右に新たなコストを代入する関数
    private void SetValueAround(Vector2Int _pos, int _nowcost)
    {
        foreach(var position in MakeMaze.Positions)
        {
            // 前後左右の場所をtmpposに代入
            var tmppos = (_pos + position);
            // tmpposが壁ではないか確認
            if(!mazedata.IsOutOfBounds(tmppos.x, tmppos.y))
            {
                // 前後左右が道であり、costmapの値が_nowcost+1より大きい場合に更新
                if(mazedata.cells[tmppos.x, tmppos.y] == MakeMaze.CellType.Path && costmap[tmppos.y+1, tmppos.x+1] > _nowcost + 1)
                {
                    costmap[tmppos.y+1, tmppos.x+1] = _nowcost + 1;
                }
            }
        }
    }
}
