using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCreator : MonoBehaviour
{
    /// <summary>
    /// ����� ��ǥ���� �����ϴ� struct
    /// </summary>
    public struct NodeIndex
    {
        public int X { get; set; }
        public int Y { get; set; }

        public NodeIndex(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        // ������ �����ε�
        public static bool operator==(NodeIndex a, NodeIndex b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator!=(NodeIndex a, NodeIndex b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", X, Y);
        }
    }

    [Header("==��ü Ÿ�� ��==")]
    [SerializeField] private int widthCount = 20;
    [SerializeField] private int heightCount = 20;
    public int Width => widthCount;
    public int Height => heightCount;
    
    [Header("\n==Ÿ�� ����==")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private float tileWidth = 1f;
    [SerializeField] private float tileHeight = 1f;

    [Header("\n==�÷��̾� ����==")]
    [SerializeField] private GameObject playPrefab;
    public Transform PlayerTransform { get; private set; }

    // Ÿ�� ����
    private TileNode[][] tiles;
    public TileNode[][] Tiles => tiles;

    /// <summary>
    /// GetTileIndex���� ����ϴ� ����
    /// �Էµ� position ���� �ε��� ���� ������ ����� throw
    /// </summary>
    private readonly IndexOutOfRangeException outOfRangeException = 
        new IndexOutOfRangeException();
    private NodeIndex returnIndex;

    public void Awake()
    {
        // Ÿ�� ����
        tiles = new TileNode[widthCount][];
        for (int i = 0; i < widthCount; ++i)
        {
            tiles[i] = new TileNode[widthCount];
            for (int j = 0; j < heightCount; ++j)
            {
                GameObject newTile = Instantiate(tilePrefab);
                newTile.transform.localPosition = new Vector3(tileWidth * i, 0f, tileHeight * j);
                tiles[i][j] = newTile.GetComponent<TileNode>();
            }
        }

        // �÷��̾� ���� �� ����
        GameObject player = Instantiate(playPrefab, 
            new Vector3(tileWidth * widthCount / 2, 0f, tileHeight * heightCount / 2), 
            Quaternion.identity);
        player.GetComponent<PlayerMovement>().TileCreator = this;
        PlayerTransform = player.transform;

        // �÷��̾ ó�� ��� �Ǵ� Ÿ���� ������ ���� �ƴϿ��� �Ѵ�.
        tiles[widthCount / 2][heightCount / 2].SetToNormalTile();
    }

    /// <summary>
    /// position�� Ÿ�� �ε��� ������ ��ȯ�Ͽ� ��ȯ
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public NodeIndex GetTileIndex(Vector3 position)
    {
        int xPosition = Mathf.RoundToInt(position.x / tileWidth);
        int zPosition = Mathf.RoundToInt(position.z / tileHeight);

        // �ε����� ������ ����� ȣ��
        if(xPosition >= widthCount || xPosition < 0 ||
            zPosition >= heightCount || zPosition < 0)
        {
            throw outOfRangeException;
        }

        //tiles[xPosition][zPosition].Renderer.material.color = Color.white;

        returnIndex.X = xPosition;
        returnIndex.Y = zPosition;

        return returnIndex;
    }

    /// <summary>
    /// �ε����� ���� Ư�� Ÿ���� ��ġ�� ��ȯ
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Vector3 GetTilePosition(NodeIndex index)
    {
        return Tiles[index.X][index.Y].transform.position;
    }

    /// <summary>
    /// [����׿�] �ش� �ε����� Ÿ�Ͽ� �� ǥ��(������)
    /// </summary>
    /// <param name="node"></param>
    public void PaintRouteTile(NodeIndex node)
    {
        tiles[node.X][node.Y].Renderer.material.color = Color.red;
    }

    /// <summary>
    /// [����׿�] �ش� �ε����� Ÿ�Ͽ� ������� ǥ��(�Ķ�)
    /// </summary>
    /// <param name="node"></param>
    public void PaintPossibleTile(NodeIndex node)
    {
        tiles[node.X][node.Y].Renderer.material.color = Color.blue;
    }
}
