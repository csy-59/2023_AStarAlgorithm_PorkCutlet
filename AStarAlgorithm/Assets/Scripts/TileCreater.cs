using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCreater : MonoBehaviour
{
    public struct NodeIndex
    {
        public int X { get; set; }
        public int Y { get; set; }

        public NodeIndex(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

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

    [Header("==전체 타일 맵==")]
    [SerializeField] private int widthCount = 20;
    [SerializeField] private int heightCount = 20;
    public int Width => widthCount;
    public int Height => heightCount;
    
    [Header("\n==타일 정보==")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private float tileWidth = 1f;
    [SerializeField] private float tileHeight = 1f;

    [Header("\n==플레이어 정보==")]
    [SerializeField] private GameObject playPrefab;
    public Transform PlayerTransform { get; private set; }

    private TileNode[][] tiles;
    public TileNode[][] Tiles => tiles;
    private readonly IndexOutOfRangeException outOfRangeException = 
        new IndexOutOfRangeException();

    public void Awake()
    {
        tiles = new TileNode[widthCount][];
        for (int i = 0; i < widthCount; ++i)
        {
            tiles[i] = new TileNode[widthCount];
            for (int j = 0; j < heightCount; ++j)
            {
                GameObject newTile = Instantiate(tilePrefab);
                newTile.transform.localPosition = new Vector3(tileWidth * i, 0f, tileHeight * j);
                tiles[i][j] = newTile.GetComponent<TileNode>();

                if (i == widthCount/2 && j == heightCount/2)
                {
                    tiles[i][j].SetToNormalTile();
                }
            }
        }

        GameObject player = Instantiate(playPrefab, 
            new Vector3(tileWidth * widthCount / 2, 0f, tileHeight * heightCount / 2), 
            Quaternion.identity);
        player.GetComponent<PlayerMovement>().TileCreater = this;
        PlayerTransform = player.transform;

    }

    private NodeIndex returnIndex;
    public NodeIndex GetTileIndex(Vector3 position)
    {
        int xPosition = Mathf.RoundToInt(position.x / tileWidth);
        int zPosition = Mathf.RoundToInt(position.z / tileHeight);

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

    public Vector3 GetTilePosition(NodeIndex index)
    {
        return Tiles[index.X][index.Y].transform.position;
    }

    public void PaintRouteTile(NodeIndex node)
    {
        tiles[node.X][node.Y].Renderer.material.color = Color.red;
    }

    public void PaintPossibleTile(NodeIndex node)
    {
        tiles[node.X][node.Y].Renderer.material.color = Color.blue;
    }
}
