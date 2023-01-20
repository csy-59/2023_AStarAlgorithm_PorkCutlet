using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCreater : MonoBehaviour
{
    [Header("==��ü Ÿ�� ��==")]
    [SerializeField] private int widthCount = 20;
    [SerializeField] private int heightCount = 20;
    
    [Header("\n==Ÿ�� ����==")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private float tileWidth = 1f;
    [SerializeField] private float tileHeight = 1f;

    [Header("\n==�÷��̾� ����==")]
    [SerializeField] private GameObject playPrefab;

    private List<List<GameObject>> tiles = new List<List<GameObject>>();

    public void Awake()
    {
        GameObject Map = new GameObject();
        Map.name = "Map";

        for (int i = 0; i < widthCount; ++i)
        {
            tiles.Add(new List<GameObject>());

            for (int j = 0; j < heightCount; ++j)
            {
                GameObject newTile = Instantiate(tilePrefab, Map.transform);
                newTile.transform.localPosition = new Vector3(tileWidth * i, 0f, tileHeight * j);
                tiles[i].Add(newTile);
            }
        }

        Instantiate(playPrefab, new Vector3(tileWidth * widthCount / 2, 0f, tileHeight * heightCount / 2), 
            Quaternion.identity, Map.transform);
    }
}
