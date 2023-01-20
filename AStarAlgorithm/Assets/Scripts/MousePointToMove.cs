using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePointToMove : MonoBehaviour
{
    private class EndNotReachable : System.Exception
    {
        private const string massage = "시작 지점과 목표 지점이 이어지지 않음";
        public override string ToString()
        {
            return massage;
        }
    }

    public struct Node
    {
        public int TotalCost => FromStart + ToEnd;
        public int FromStart { get; set; }
        public int ToEnd { get; set; }
    }


    [SerializeField] private Camera mainCamera;
    [SerializeField] private TileCreater tileCreater;
    private Transform playerTransform;
    private float cameraHeight;

    // 사용할 배열들
    public Node[] Nodes { get; set; }
    private bool[] isVisited;
    private int mapSize;
    private int width, height;

    private readonly EndNotReachable endIsBlock = new EndNotReachable(true);
    private readonly EndNotReachable endNotReachablek = new EndNotReachable(false);

    public void Start()
    {
        playerTransform = tileCreater.PlayerTransform;
        cameraHeight = mainCamera.transform.position.y;

        width = tileCreater.Width;
        height = tileCreater.Width;

        mapSize = width * height;
        Nodes = new Node[mapSize];
        isVisited = new bool[mapSize];
    }
    public void Update()
    {
        // 오른쪽 버튼 클릭 시 플레이어의 현제 위치와 
        if (Input.GetMouseButtonDown(1))
        {
            try
            {
                var startIndex = tileCreater.GetTileIndex(playerTransform.position);

                Vector3 point = mainCamera.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * cameraHeight);
                var targetIndex = tileCreater.GetTileIndex(point);

                if(tileCreater.Tiles[targetIndex.Item1][targetIndex.Item2].IsBlocked)
                {
                    Debug.Log("도달 할 수 없음");
                    return;
                }

                Debug.Log($"시작 지점: {startIndex} 목표 지점: {targetIndex}");

            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log($"맵 밖을 선택함");
            }
        }
    }

    private bool SearchAStar(int start, int end)
    {
        ResetMap();
        Queue<int> queue = new Queue<int>();
        queue.Enqueue(start);

        int def = 0;
        bool foundEnd = false;
        while(queue.Count > 0)
        {
            int currentIndex = queue.Dequeue();
            Nodes[currentIndex].FromStart = def;

            // 시작 지점에서 끝 지점까지 도달했는지 확인
            foundEnd = currentIndex == end;
        }

        if (!foundEnd)
            throw endNotReachable;

        return true;
    }

    private void ResetMap()
    {
        for(int i = 0; i< mapSize; ++i)
        {
            isVisited[i] = false;
        }
    }
}