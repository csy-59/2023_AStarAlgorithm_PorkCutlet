using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeIndex = TileCreater.NodeIndex;

public class MousePointToMove : MonoBehaviour
{
    public class Node
    {
        public int TotalCost => FromStart + ToEnd;
        public int FromStart { get; set; }
        public int ToEnd { get; set; }
        public NodeIndex Parent { get; set; }
    }
    private readonly int[] SearchNode = { -1, 0, 1};

    [SerializeField] private Camera mainCamera;
    [SerializeField] private TileCreater tileCreater;
    private Transform playerTransform;
    private float cameraHeight;

    // 사용할 배열들
    public Node[][] Nodes { get; set; }
    private bool[][] isVisited;
    private int width, height;

    public void Start()
    {
        playerTransform = tileCreater.PlayerTransform;
        cameraHeight = mainCamera.transform.position.y;

        width = tileCreater.Width;
        height = tileCreater.Width;

        Nodes = new Node[width][];
        isVisited = new bool[width][];
        for(int i = 0; i < width; ++i)
        {
            Nodes[i] = new Node[height];
            isVisited[i] = new bool[height];
            for(int j = 0; j < height; ++j)
            {
                Nodes[i][j] = new Node();
            }
        }
    }
    public void Update()
    {
        // 오른쪽 버튼 클릭 시 플레이어의 현제 위치와 
        if (Input.GetMouseButtonDown(1))
        {
            try
            {
                Vector3 point = mainCamera.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * cameraHeight);
                NodeIndex targetIndex = tileCreater.GetTileIndex(point);

                if (tileCreater.Tiles[targetIndex.X][targetIndex.Y].IsBlocked)
                {
                    Debug.Log("도달 할 수 없음");
                    return;
                }
                NodeIndex startIndex = tileCreater.GetTileIndex(playerTransform.position);

                Debug.Log($"시작 지점: {startIndex} 목표 지점: {targetIndex}");

                if(!SearchAStar(startIndex, targetIndex))
                {
                    Debug.Log($"도달 할 수 없음");
                }
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log($"맵 밖을 선택함");
            }
        }
    }

    private bool SearchAStar(NodeIndex start, NodeIndex end)
    {
        ResetMap();
        List<NodeIndex> openList = new List<NodeIndex>();
        List<NodeIndex> closedList = new List<NodeIndex>();

        isVisited[start.X][start.Y] = true;
        openList.Add(start);
        while (openList.Count > 0)
        {
            // *아래를 다음의 조건이 될 때까지 돌린다.
            // (1) end 노드를 찾음
            // (2) 열린 목록을 비게되는 경우 >> 이경우 노달 할 수 없음

            // 0. 열린 목록에서 자신을 빼서, 닫힌 노드로 이동
            NodeIndex curIndex = openList[^1];
            Node curNode = Nodes[curIndex.X][curIndex.Y];
            //tileCreater.PaintPossibleTile(curIndex);
            openList.RemoveAt(openList.Count - 1);
            closedList.Add(curIndex);
            // 현재 노드가 목표 노드라면 멈춤
            if (curIndex == end)
            {
                break;
            }

            byte diagonalBits = 0b0000;
            // 1. 위 아래 양옆 벽 검사 => 벽이면 그 주변 노드 못가게
            int weight = curNode.FromStart + 10;
            // 위
            if (!AddNodeToOpenList(new NodeIndex(curIndex.X - 1, curIndex.Y),
                curIndex, weight, in end, ref openList, in closedList))
            {
                diagonalBits |= 0b1000;
            }
            // 아래
            if (!AddNodeToOpenList(new NodeIndex(curIndex.X + 1, curIndex.Y),
                curIndex, weight, in end, ref openList, in closedList))
            {
                diagonalBits |= 0b0100;
            }
            //왼쪽
            if (!AddNodeToOpenList(new NodeIndex(curIndex.X, curIndex.Y - 1),
                curIndex, weight, in end, ref openList, in closedList))
            {
                diagonalBits |= 0b0010;
            }
            //오른쪽
            if (!AddNodeToOpenList(new NodeIndex(curIndex.X, curIndex.Y + 1),
                curIndex, weight, in end, ref openList, in closedList))
            {
                diagonalBits |= 0b0001;
            }

            weight = curNode.FromStart + 14;
            // 2. 갈 수 있는 노드는 열린 목록에 추가,
            if ((diagonalBits & 0b1010) == 0) // 왼쪽 위
            {
                AddNodeToOpenList(new NodeIndex(curIndex.X - 1, curIndex.Y - 1),
                curIndex, weight, in end, ref openList, in closedList);
            }
            if ((diagonalBits & 0b1001) == 0) // 오른쪽 위
            {
                AddNodeToOpenList(new NodeIndex(curIndex.X - 1, curIndex.Y + 1),
                curIndex, weight, in end, ref openList, in closedList);
            }
            if ((diagonalBits & 0b0110) == 0) // 왼쪽 아래
            {
                AddNodeToOpenList(new NodeIndex(curIndex.X + 1, curIndex.Y - 1),
                curIndex, weight, in end, ref openList, in closedList);
            }
            if ((diagonalBits & 0b0101) == 0) // 오른쪽 아래
            {
                AddNodeToOpenList(new NodeIndex(curIndex.X + 1, curIndex.Y + 1),
                curIndex, weight, in end, ref openList, in closedList);
            }

            // 3. 현재 열린 목록에 있는 노드 중 Total cost가 가장 작은 노드를 다음 노드로 간다.
            // >> 노드 추가 시 소팅 하는 것으로 해결
        }

        // 도달하지 못하는 경우
        if(openList.Count <= 0)
        {
            return false;
        }

        // 4. 닫힌 노드의 부모 노드를 추적하여 찾아간다.
        string routeString = string.Empty;
        NodeIndex nextNodeToMove = closedList[^1];
        do {
            routeString += nextNodeToMove.ToString() + ">>";
            tileCreater.PaintRouteTile(nextNodeToMove);
            nextNodeToMove = Nodes[nextNodeToMove.X][nextNodeToMove.Y].Parent;
        } while (nextNodeToMove != start);
        routeString += start.ToString();
        Debug.Log(routeString);

        return true;
    }

    private bool AddNodeToOpenList(NodeIndex newNode, in NodeIndex parentNode, int weight, in NodeIndex endNode,
        ref List<NodeIndex> openNode, in List<NodeIndex> closedNode)
    {
        // 인덱스 값이 이상함
        if(newNode.X < 0 || newNode.X >= width ||
            newNode.Y < 0 || newNode.Y >= height)
        {
            return false;
        }

        // 벽임
        if(tileCreater.Tiles[newNode.X][newNode.Y].IsBlocked)
        {
            return false;
        }

        int fromStart = weight;
        int toEnd = (Mathf.Abs(endNode.X - newNode.X) + Mathf.Abs(endNode.Y - newNode.Y)) * 10;
        
        // 2-1. 새로 추가한 노드는 그 노드의 부모 노드를 현재 노드로 설정한다.
        if (!isVisited[newNode.X][newNode.Y])
        {
            isVisited[newNode.X][newNode.Y] = true;

            Node addNode = Nodes[newNode.X][newNode.Y];
            addNode.FromStart = fromStart;
            addNode.ToEnd = toEnd;
            addNode.Parent = parentNode;

            openNode.Add(newNode);
            openNode.Sort(delegate (NodeIndex a, NodeIndex b)
               {
                   int aCost = Nodes[a.X][a.Y].TotalCost;
                   int bCost = Nodes[b.X][b.Y].TotalCost;

                   if (aCost < bCost) return 1;
                   else if (aCost > bCost) return -1;
                   else return 0;
               });

            return true;
        }

        // 2-1. 이미 추가하려는 노드가 열린 목록에 있다면 Cost from start 새로 계산하여 비교
        Node addedNode = Nodes[newNode.X][newNode.Y];
        // 2-1-a. 기존 값이 더 작다면 그냥 넘어감
        // 2-1-b. 새 값이 더 작다면 해당 노드의 부모 노드를 나 자신으로 설정하고 값들을 재계산 한다.
        if (addedNode.FromStart > fromStart)
        {
            addedNode.FromStart = fromStart;
            addedNode.Parent = parentNode;
        }

        return true;
    }

    private void ResetMap()
    {
        for(int i = 0; i<width; ++i)
        {
            for(int j = 0; j<height; ++j)
            {
                isVisited[i][j] = false;
            }
        }
    }
}