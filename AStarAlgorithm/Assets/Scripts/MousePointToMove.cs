using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeIndex = TileCreater.NodeIndex;

public class MousePointToMove : MonoBehaviour
{

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

    // ����� �迭��
    public Node[] Nodes { get; set; }
    private bool[] isVisited;
    private int mapSize;
    private int width, height;

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
        // ������ ��ư Ŭ�� �� �÷��̾��� ���� ��ġ�� 
        if (Input.GetMouseButtonDown(1))
        {
            try
            {
                NodeIndex startIndex = tileCreater.GetTileIndex(playerTransform.position);

                Vector3 point = mainCamera.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * cameraHeight);
                NodeIndex targetIndex = tileCreater.GetTileIndex(point);

                if(tileCreater.Tiles[targetIndex.X][targetIndex.Y].IsBlocked)
                {
                    Debug.Log("���� �� �� ����");
                    return;
                }

                Debug.Log($"���� ����: {startIndex} ��ǥ ����: {targetIndex}");

            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log($"�� ���� ������");
            }
        }
    }

    private bool SearchAStar(int start, int end)
    {
        //ResetMap();
        //Queue<int> queue = new Queue<int>();
        //queue.Enqueue(start);

        //int def = 0;
        //bool foundEnd = false;
        //while(queue.Count > 0)
        //{
        //    int currentIndex = queue.Dequeue();
        //    Nodes[currentIndex].FromStart = def;

        //    // ���� �������� �� �������� �����ߴ��� Ȯ��
        //    foundEnd = currentIndex == end;
        //}

        //if (!foundEnd)
        //    throw endNotReachable;

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