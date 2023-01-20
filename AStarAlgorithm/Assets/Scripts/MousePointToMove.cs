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

    // ����� �迭��
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
        // ������ ��ư Ŭ�� �� �÷��̾��� ���� ��ġ�� 
        if (Input.GetMouseButtonDown(1))
        {
            try
            {
                Vector3 point = mainCamera.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * cameraHeight);
                NodeIndex targetIndex = tileCreater.GetTileIndex(point);

                if (tileCreater.Tiles[targetIndex.X][targetIndex.Y].IsBlocked)
                {
                    Debug.Log("���� �� �� ����");
                    return;
                }
                NodeIndex startIndex = tileCreater.GetTileIndex(playerTransform.position);

                Debug.Log($"���� ����: {startIndex} ��ǥ ����: {targetIndex}");

                if(!SearchAStar(startIndex, targetIndex))
                {
                    Debug.Log($"���� �� �� ����");
                }
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log($"�� ���� ������");
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
            // *�Ʒ��� ������ ������ �� ������ ������.
            // (1) end ��带 ã��
            // (2) ���� ����� ��ԵǴ� ��� >> �̰�� ��� �� �� ����

            // 0. ���� ��Ͽ��� �ڽ��� ����, ���� ���� �̵�
            NodeIndex curIndex = openList[^1];
            Node curNode = Nodes[curIndex.X][curIndex.Y];
            //tileCreater.PaintPossibleTile(curIndex);
            openList.RemoveAt(openList.Count - 1);
            closedList.Add(curIndex);
            // ���� ��尡 ��ǥ ����� ����
            if (curIndex == end)
            {
                break;
            }

            byte diagonalBits = 0b0000;
            // 1. �� �Ʒ� �翷 �� �˻� => ���̸� �� �ֺ� ��� ������
            int weight = curNode.FromStart + 10;
            // ��
            if (!AddNodeToOpenList(new NodeIndex(curIndex.X - 1, curIndex.Y),
                curIndex, weight, in end, ref openList, in closedList))
            {
                diagonalBits |= 0b1000;
            }
            // �Ʒ�
            if (!AddNodeToOpenList(new NodeIndex(curIndex.X + 1, curIndex.Y),
                curIndex, weight, in end, ref openList, in closedList))
            {
                diagonalBits |= 0b0100;
            }
            //����
            if (!AddNodeToOpenList(new NodeIndex(curIndex.X, curIndex.Y - 1),
                curIndex, weight, in end, ref openList, in closedList))
            {
                diagonalBits |= 0b0010;
            }
            //������
            if (!AddNodeToOpenList(new NodeIndex(curIndex.X, curIndex.Y + 1),
                curIndex, weight, in end, ref openList, in closedList))
            {
                diagonalBits |= 0b0001;
            }

            weight = curNode.FromStart + 14;
            // 2. �� �� �ִ� ���� ���� ��Ͽ� �߰�,
            if ((diagonalBits & 0b1010) == 0) // ���� ��
            {
                AddNodeToOpenList(new NodeIndex(curIndex.X - 1, curIndex.Y - 1),
                curIndex, weight, in end, ref openList, in closedList);
            }
            if ((diagonalBits & 0b1001) == 0) // ������ ��
            {
                AddNodeToOpenList(new NodeIndex(curIndex.X - 1, curIndex.Y + 1),
                curIndex, weight, in end, ref openList, in closedList);
            }
            if ((diagonalBits & 0b0110) == 0) // ���� �Ʒ�
            {
                AddNodeToOpenList(new NodeIndex(curIndex.X + 1, curIndex.Y - 1),
                curIndex, weight, in end, ref openList, in closedList);
            }
            if ((diagonalBits & 0b0101) == 0) // ������ �Ʒ�
            {
                AddNodeToOpenList(new NodeIndex(curIndex.X + 1, curIndex.Y + 1),
                curIndex, weight, in end, ref openList, in closedList);
            }

            // 3. ���� ���� ��Ͽ� �ִ� ��� �� Total cost�� ���� ���� ��带 ���� ���� ����.
            // >> ��� �߰� �� ���� �ϴ� ������ �ذ�
        }

        // �������� ���ϴ� ���
        if(openList.Count <= 0)
        {
            return false;
        }

        // 4. ���� ����� �θ� ��带 �����Ͽ� ã�ư���.
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
        // �ε��� ���� �̻���
        if(newNode.X < 0 || newNode.X >= width ||
            newNode.Y < 0 || newNode.Y >= height)
        {
            return false;
        }

        // ����
        if(tileCreater.Tiles[newNode.X][newNode.Y].IsBlocked)
        {
            return false;
        }

        int fromStart = weight;
        int toEnd = (Mathf.Abs(endNode.X - newNode.X) + Mathf.Abs(endNode.Y - newNode.Y)) * 10;
        
        // 2-1. ���� �߰��� ���� �� ����� �θ� ��带 ���� ���� �����Ѵ�.
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

        // 2-1. �̹� �߰��Ϸ��� ��尡 ���� ��Ͽ� �ִٸ� Cost from start ���� ����Ͽ� ��
        Node addedNode = Nodes[newNode.X][newNode.Y];
        // 2-1-a. ���� ���� �� �۴ٸ� �׳� �Ѿ
        // 2-1-b. �� ���� �� �۴ٸ� �ش� ����� �θ� ��带 �� �ڽ����� �����ϰ� ������ ���� �Ѵ�.
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