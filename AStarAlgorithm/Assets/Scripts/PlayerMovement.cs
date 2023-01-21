using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeIndex = TileCreator.NodeIndex;

public class PlayerMovement : MonoBehaviour
{
    public MousePointToMove MouseToMove { get; set; }
    public TileCreator TileCreator { get; set; }
    public bool IsMoving { get; private set; }
    private Coroutine moveToTile;

    [SerializeField] private float speed = 5f;

    public void StartMoving()
    {
        IsMoving = true;
        moveToTile = StartCoroutine(CoMoveToTile());
    }



    private IEnumerator CoMoveToTile()
    {
        NodeIndex nextTileIndex = MouseToMove.Route.Pop();
        Vector3 nextTilePosition = TileCreator.GetTilePosition(nextTileIndex);

        Vector3 originalPosition = transform.position;
        Vector3 offsetPosition = originalPosition;

        while (true)
        {
            offsetPosition = Time.deltaTime * speed * (nextTilePosition - originalPosition).normalized;

            if ((nextTilePosition - offsetPosition - transform.position).sqrMagnitude < 0.001f)
            {
                transform.position = nextTilePosition;
                break;
            }
            transform.Translate(offsetPosition);

            yield return null;
        }

        // 이동이 끝남
        if(MouseToMove.Route.Count <= 0)
        {
            IsMoving = false;
            yield break;
        }

        moveToTile = StartCoroutine(CoMoveToTile());
    }

    public void StopMoving()
    {
        if(moveToTile != null)
        {
            StopCoroutine(moveToTile);
            IsMoving = false;
        }
    }
}
