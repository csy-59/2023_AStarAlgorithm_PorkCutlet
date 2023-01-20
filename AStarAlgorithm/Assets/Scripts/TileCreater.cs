using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCreater : MonoBehaviour
{
    [Header("==전체 타일 맵==")]
    [SerializeField] private int widthCount = 20;
    [SerializeField] private int heightCount = 20;
    
    [Header("\n==타일 정보==")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private float tileWidth = 1f;
    [SerializeField] private float tileHeight = 1f;

    [Header("\n==플레이어 정보==")]
    [SerializeField] private GameObject playPrefab;

    private List<TileNode> tiles = new List<TileNode>();

    public void Awake()
    {
        GameObject Map = new GameObject();
        Map.name = "Map";
        Map.transform.parent = gameObject.transform;

        for (int i = 0; i < widthCount; ++i)
        {
            for (int j = 0; j < heightCount; ++j)
            {
                GameObject newTile = Instantiate(tilePrefab, Map.transform);
                newTile.transform.localPosition = new Vector3(tileWidth * i, 0f, tileHeight * j);
                tiles.Add(newTile.GetComponent<TileNode>());
            }
        }

        GameObject player = Instantiate(playPrefab, 
            new Vector3(tileWidth * widthCount / 2, 0f, tileHeight * heightCount / 2), 
            Quaternion.identity, Map.transform);
        tiles[widthCount / 2 + heightCount / 2].SetToNormalTile();

    }

    public int GetTileIndex(Vector3 position)
    {
        int xPosition = Mathf.RoundToInt(position.x / tileWidth);
        int zPosition = Mathf.RoundToInt(position.z / tileHeight);
        int index = xPosition * widthCount + zPosition;

        return index;
    }
}
