using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Index = TileCreater.NodeIndex;

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
    private PlayerMovement playerMovement;
    private float cameraHeight;

    // 사용할 배열들
    public Node[][] Nodes { get; set; }
    private bool[][] isVisited;
    private int width, height;

    private int[] indexAdder = { -1, 0, 1};

    private readonly EndNotReachable endNotReachable = new EndNotReachable();

    public void Start()
    {
        playerTransform = tileCreater.PlayerTransform;
        playerMovement = tileCreater.PlayerTransform.GetComponent<PlayerMovement>();
        cameraHeight = mainCamera.transform.position.y;

        width = tileCreater.Width;
        height = tileCreater.Width;

        Nodes = new Node[width][];
        isVisited = new bool[width][];
        for(int i = 0; i < height; ++i)
        {
            Nodes[i] = new Node[height];
            isVisited[i] = new bool[height];
        }
    }
    public void Update()
    {
        // 오른쪽 버튼 클릭 시 플레이어의 현제 위치와 
        if (Input.GetMouseButtonDown(1))
        {
            try
            {
                Index startIndex = tileCreater.GetTileIndex(playerTransform.position);

                Vector3 point = mainCamera.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * cameraHeight);
                Index targetIndex = tileCreater.GetTileIndex(point);

                if(tileCreater.Tiles[targetIndex.X][targetIndex.Y].IsBlocked)
                {
                    Debug.Log("도달 할 수 없음");
                    return;
                }

                Debug.Log($"시작 지점: {startIndex} 목표 지점: {targetIndex}");

                SearchAStar(startIndex, targetIndex);

            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log($"맵 밖을 선택함 {e.StackTrace}");
            }
            catch(EndNotReachable e)
            {
                Debug.Log(e.Message);
            }
        }
    }

    private void SearchAStar(Index start, Index end)
    {
        Queue<Index> queue = new Queue<Index>();
        queue.Enqueue(start);

        // 시작 지점 기준으로 찾기
        ResetMap();
        int def = 0;
        bool foundEnd = false;
        int curSize = 1;
        while(queue.Count > 0)
        {
            Index currentIndex = queue.Dequeue();
            --curSize;
            isVisited[currentIndex.X][currentIndex.Y] = true;
            Nodes[currentIndex.X][currentIndex.Y].FromStart = def;
            // 시작 지점에서 끝 지점까지 도달했는지 확인
            foundEnd = currentIndex == end;

            // 큐에 넣기
            foreach(int xAdder in indexAdder)
            {
                foreach(int yAdder in indexAdder)
                {
                    int newX = currentIndex.X + xAdder;
                    int newY = currentIndex.Y + yAdder;

                    // 도달 할 수 없는 인덱스일 경우 넘기기
                    if(newX >= width || newX < 0 || newY >= height || newY < 0)
                    {
                        continue;
                    }

                    // 해당 인덱스를 이미 접근한 경우 넘기기
                    if(isVisited[newX][newY])
                    {
                        continue;
                    }

                    // 해당 인덱스의 타일이 벽인 경우 넘기기
                    if(tileCreater.Tiles[newX][newY].IsBlocked)
                    {
                        continue;
                    }

                    queue.Enqueue(new Index() { X = newX, Y = newY });
                }
            }
         
            if (curSize <= 0)
            {
                ++def;
                curSize = queue.Count;
            }
        }

        // 시작 지점에서 끝 지점을 찾을 수 없다면 예외 발생
        if (!foundEnd)
            throw endNotReachable;

        // 끝 지점에서 찾기
        ResetMap();
        def = 0;
        foundEnd = false;
        curSize = 1;
        queue.Enqueue(end);
        while (queue.Count > 0)
        {
            Index currentIndex = queue.Dequeue();
            --curSize;
            isVisited[currentIndex.X][currentIndex.Y] = true;
            Nodes[currentIndex.X][currentIndex.Y].ToEnd = def;
            // 시작 지점에서 끝 지점까지 도달했는지 확인
            foundEnd = currentIndex == end;

            // 큐에 넣기
            foreach (int xAdder in indexAdder)
            {
                foreach (int yAdder in indexAdder)
                {
                    int newX = currentIndex.X + xAdder;
                    int newY = currentIndex.Y + yAdder;

                    // 도달 할 수 없는 인덱스일 경우 넘기기
                    if (newX >= width || newX < 0 || newY >= height || newY < 0)
                    {
                        continue;
                    }

                    // 해당 인덱스를 이미 접근한 경우 넘기기
                    if (isVisited[newX][newY])
                    {
                        continue;
                    }

                    // 해당 인덱스의 타일이 벽인 경우 넘기기
                    if (tileCreater.Tiles[newX][newY].IsBlocked)
                    {
                        continue;
                    }

                    queue.Enqueue(new Index() { X = newX, Y = newY });
                }
            }

            if (curSize < 0)
            {
                ++def;
                curSize = queue.Count;
            }
        }
    }

    public Index GetNextIndex(Index currentIndex, out bool isMoveable)
    {
        isMoveable = false;
        Index result = new Index();
        int min = Nodes[currentIndex.X - 1][currentIndex.Y - 1].TotalCost;
        foreach (int xAdder in indexAdder)
        {
            foreach (int yAdder in indexAdder)
            {
                int newX = currentIndex.X + xAdder;
                int newY = currentIndex.Y + yAdder;

                // 도달 할 수 없는 인덱스일 경우 넘기기
                if (newX >= width || newX < 0 || newY >= height || newY < 0)
                {
                    continue;
                }
                // 해당 인덱스의 타일이 벽인 경우 넘기기
                if (tileCreater.Tiles[newX][newY].IsBlocked)
                {
                    continue;
                }

                isMoveable = true;

                int cost = Nodes[newX][newY].TotalCost;
                if(cost < min)
                {
                    result.X = newX;
                    result.Y = newY;
                }
            }
        }

        return result;
    }

    private void ResetMap()
    {
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                isVisited[i][j] = false;
            }
        }
    }
}