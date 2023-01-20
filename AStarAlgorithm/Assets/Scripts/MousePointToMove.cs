using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Index = TileCreater.NodeIndex;

public class MousePointToMove : MonoBehaviour
{
    private class EndNotReachable : System.Exception
    {
        private const string massage = "���� ������ ��ǥ ������ �̾����� ����";
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

    // ����� �迭��
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
        // ������ ��ư Ŭ�� �� �÷��̾��� ���� ��ġ�� 
        if (Input.GetMouseButtonDown(1))
        {
            try
            {
                Index startIndex = tileCreater.GetTileIndex(playerTransform.position);

                Vector3 point = mainCamera.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * cameraHeight);
                Index targetIndex = tileCreater.GetTileIndex(point);

                if(tileCreater.Tiles[targetIndex.X][targetIndex.Y].IsBlocked)
                {
                    Debug.Log("���� �� �� ����");
                    return;
                }

                Debug.Log($"���� ����: {startIndex} ��ǥ ����: {targetIndex}");

                SearchAStar(startIndex, targetIndex);

            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log($"�� ���� ������ {e.StackTrace}");
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

        // ���� ���� �������� ã��
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
            // ���� �������� �� �������� �����ߴ��� Ȯ��
            foundEnd = currentIndex == end;

            // ť�� �ֱ�
            foreach(int xAdder in indexAdder)
            {
                foreach(int yAdder in indexAdder)
                {
                    int newX = currentIndex.X + xAdder;
                    int newY = currentIndex.Y + yAdder;

                    // ���� �� �� ���� �ε����� ��� �ѱ��
                    if(newX >= width || newX < 0 || newY >= height || newY < 0)
                    {
                        continue;
                    }

                    // �ش� �ε����� �̹� ������ ��� �ѱ��
                    if(isVisited[newX][newY])
                    {
                        continue;
                    }

                    // �ش� �ε����� Ÿ���� ���� ��� �ѱ��
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

        // ���� �������� �� ������ ã�� �� ���ٸ� ���� �߻�
        if (!foundEnd)
            throw endNotReachable;

        // �� �������� ã��
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
            // ���� �������� �� �������� �����ߴ��� Ȯ��
            foundEnd = currentIndex == end;

            // ť�� �ֱ�
            foreach (int xAdder in indexAdder)
            {
                foreach (int yAdder in indexAdder)
                {
                    int newX = currentIndex.X + xAdder;
                    int newY = currentIndex.Y + yAdder;

                    // ���� �� �� ���� �ε����� ��� �ѱ��
                    if (newX >= width || newX < 0 || newY >= height || newY < 0)
                    {
                        continue;
                    }

                    // �ش� �ε����� �̹� ������ ��� �ѱ��
                    if (isVisited[newX][newY])
                    {
                        continue;
                    }

                    // �ش� �ε����� Ÿ���� ���� ��� �ѱ��
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

                // ���� �� �� ���� �ε����� ��� �ѱ��
                if (newX >= width || newX < 0 || newY >= height || newY < 0)
                {
                    continue;
                }
                // �ش� �ε����� Ÿ���� ���� ��� �ѱ��
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