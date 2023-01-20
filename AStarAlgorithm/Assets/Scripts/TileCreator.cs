using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCreator : MonoBehaviour
{
    /// <summary>
    /// 노드의 좌표값을 저장하는 struct
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

        // 연산자 오버로딩
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

    // 타일 정보
    private TileNode[][] tiles;
    public TileNode[][] Tiles => tiles;

    /// <summary>
    /// GetTileIndex에서 사용하는 에외
    /// 입력된 position 값의 인덱스 값이 범위를 벗어나면 throw
    /// </summary>
    private readonly IndexOutOfRangeException outOfRangeException = 
        new IndexOutOfRangeException();
    private NodeIndex returnIndex;

    public void Awake()
    {
        // 타일 생성
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

        // 플레이어 생성 및 설정
        GameObject player = Instantiate(playPrefab, 
            new Vector3(tileWidth * widthCount / 2, 0f, tileHeight * heightCount / 2), 
            Quaternion.identity);
        player.GetComponent<PlayerMovement>().TileCreator = this;
        PlayerTransform = player.transform;

        // 플레이어가 처음 밟게 되는 타일은 무조건 벽이 아니여야 한다.
        tiles[widthCount / 2][heightCount / 2].SetToNormalTile();
    }

    /// <summary>
    /// position을 타일 인덱스 값으로 변환하여 반환
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public NodeIndex GetTileIndex(Vector3 position)
    {
        int xPosition = Mathf.RoundToInt(position.x / tileWidth);
        int zPosition = Mathf.RoundToInt(position.z / tileHeight);

        // 인덱스의 범위를 벗어나면 호출
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
    /// 인덱스를 통해 특정 타일의 위치를 반환
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Vector3 GetTilePosition(NodeIndex index)
    {
        return Tiles[index.X][index.Y].transform.position;
    }

    /// <summary>
    /// [디버그용] 해당 인덱스의 타일에 길 표시(빨간색)
    /// </summary>
    /// <param name="node"></param>
    public void PaintRouteTile(NodeIndex node)
    {
        tiles[node.X][node.Y].Renderer.material.color = Color.red;
    }

    /// <summary>
    /// [디버그용] 해당 인덱스의 타일에 계산함을 표시(파랑)
    /// </summary>
    /// <param name="node"></param>
    public void PaintPossibleTile(NodeIndex node)
    {
        tiles[node.X][node.Y].Renderer.material.color = Color.blue;
    }
}
