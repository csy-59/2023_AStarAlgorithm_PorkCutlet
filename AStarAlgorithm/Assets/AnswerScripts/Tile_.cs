using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_ : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;

    public Vector2Int Index { get; set; } // 노드 인덱스, int 형 두가지가 있음

    // 가중치
    public int G { get; set; }
    public int H { get; set; }
    public int F => G + H;

    // 오픈 리스트 여부
    public bool IsOpen { get; set; } = false;
    public bool IsClose { get; set; } = false;

    // 장애물 여부
    public bool IsObstacle { get; set; }

    // 세팅
    public void Set(int G, int H)
    {
        if(IsOpen && this.G < G)
        {
            return;
        }

        this.G = G;
        this.H = H;
        IsOpen = true;

        Refresh();
    }

    public void Refresh()
    {
        if (IsObstacle)
        {
            meshRenderer.material.color = Color.black;
        }
        else if (IsOpen)
        {
            meshRenderer.material.color = Color.blue;
        }
        else if (IsClose)
        {
            meshRenderer.material.color = Color.red;
        }
        else
        {
            meshRenderer.material.color = Color.white;
        }
    }
}
