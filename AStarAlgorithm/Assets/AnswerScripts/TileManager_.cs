using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager_ : MonoBehaviour
{
    [SerializeField] Tile_ tilePrefab;

    private List<Tile_> tiles = new List<Tile_>(TILE_X * TILE_Y);

    const float TILE_SIZE = 1.0f;
    const int TILE_X = 20;
    const int TILE_Y = 20;
    const int CENTER_X = 10;
    const int CENTER_Y = 10;
    const float CAM_HEIGHT = 20f;

    private void Start()
    {
        for (int i = 0; i < TILE_X; ++i)
        {
            for (int j = 0; j < TILE_Y; ++j)
            {
                Tile_ tile = Instantiate(tilePrefab);
                tile.transform.position = new Vector3(i, 0, j);
                tile.Index = new Vector2Int(i, j);
                tiles.Add(tile);

                if(Random.Range(0, 10) < 2)
                {
                    if (i == CENTER_X && j == CENTER_Y)
                        continue;

                    tile.IsObstacle = true;
                    tile.Refresh();
                }
            }
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = CAM_HEIGHT;
            Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
            Vector2Int index = Vector2Int.zero;

            index.x = Mathf.RoundToInt(pos.x / TILE_SIZE);
            index.y = Mathf.RoundToInt(pos.z / TILE_SIZE);

            Tile_ tile  = tiles.Find(_ => _.Index == index);

            FindPath(new Vector2Int() { x = CENTER_X, y = CENTER_Y },
                tile.Index);

            if (tile == null) return;

            tile.IsOpen = true;
            tile.Refresh();
        }
    }

    private List<Tile_> FindPath(Vector2Int start, Vector2Int end)
    {
        List<Tile_> result = new List<Tile_>();

        Tile_ startTile = tiles.Find(_ => _.Index == start);
        Tile_ endTile = tiles.Find(_ => _.Index == end);

        List<Tile_> openList = new List<Tile_>();

        startTile.Set(0, GetH(startTile.Index, endTile.Index));

        do
        {
            startTile.IsClose = true;
            startTile.IsOpen = false;

            startTile.Refresh();

            if (startTile.Index == endTile.Index)
                break;

            // 주변 탐색 시작

            bool isLT = true;
            bool isRT = true;
            bool isLB = true;
            bool isRB = true;

            // 위
            if(!SetTile(startTile, tiles.Find(_ =>
            _.Index == new Vector2Int(startTile.Index.x, startTile.Index.y + 1)), end))
            {
                isLT = false;
                isRT = false;
            }
            // 아래
            if(!SetTile(startTile, tiles.Find(_ =>
            _.Index == new Vector2Int(startTile.Index.x, startTile.Index.y - 1)), end))
            {
                isLB = false;
                isRB = false;
            }
            // 왼쪽
            if(!SetTile(startTile, tiles.Find(_ =>
            _.Index == new Vector2Int(startTile.Index.x - 1, startTile.Index.y)), end))
            {
                isLT = false;
                isLB = false;
            }
            // 오른쪽
            if(!SetTile(startTile, tiles.Find(_ =>
            _.Index == new Vector2Int(startTile.Index.x + 1, startTile.Index.y)), end))
            {
                isRT = false;
                isRB = false;
            }

            // 대각선 연산... 추가바람...
            if(isLT)
            {
                SetTile(startTile, tiles.Find(_ => _.Index == new Vector2Int(startTile.Index.x - 1, startTile.Index.y - 1)), end);
            }
            if(isRT)
            {
                SetTile(startTile, tiles.Find(_ => _.Index == new Vector2Int(startTile.Index.x + 1, startTile.Index.y - 1)), end);
            }
            if(isLB)
            {
                SetTile(startTile, tiles.Find(_ => _.Index == new Vector2Int(startTile.Index.x - 1, startTile.Index.y + 1)), end);
            }
            if(isRB)
            {
                SetTile(startTile, tiles.Find(_ => _.Index == new Vector2Int(startTile.Index.x + 1, startTile.Index.y + 1)), end);
            }

            // 일케 쓰면 안됨.
            var openTiles = tiles.Where(_ => _.IsOpen);
            if (openTiles.Count() > 0)
            {
                int f = openTiles.Min(_ => _.F); // 정렬하는 것
                //openTiles.Find(_ => _.F);
            }
            startTile = tiles.Find(_ => _.IsOpen == true); // 오픈된 노드가 없다면 null이 들어감

        } while (startTile != null);



        return result;
    }

    private int GetH(in Vector2Int index, in Vector2Int end)
    {
        return Mathf.Abs(index.x - end.x) + Mathf.Abs(index.y - end.y);
    }

    private int GetG(in Vector2Int parent, in Vector2Int child)
    {
        int result = Mathf.Abs(parent.x - child.x) + Mathf.Abs(parent.y - child.y);
        if(result > 1)
        {
            return 14; //대각선 이동
        }
        else
        {
            return 10; // 사방 이동
        }
    }

    // 값 복사를 방지하기 위해 in 사용
    private bool SetTile(Tile_ parent, Tile_ child, in Vector2Int end)
    {
        if (child == null) return false;
        if (child.IsObstacle) return false;
        if (child.IsClose) return false;

        child.Set(GetG(parent.Index, child.Index),
            GetH(child.Index, end));

        return true;
    }
}
