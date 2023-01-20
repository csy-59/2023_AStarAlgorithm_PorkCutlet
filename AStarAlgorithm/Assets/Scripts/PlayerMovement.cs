using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeIndex = TileCreater.NodeIndex;

public class PlayerMovement : MonoBehaviour
{
    public MousePointToMove MouseToMove { get; set; }
    public TileCreater TileCreater { get; set; }
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
        Vector3 nextTilePosition = TileCreater.GetTilePosition(nextTileIndex);

        float elapsedTime = 0f;
        Vector3 originalPosition = transform.position;
        Vector3 currentPosition = originalPosition;

        while (true)
        {
            elapsedTime += Time.deltaTime * speed;
            currentPosition = Vector3.Lerp(originalPosition, nextTilePosition, elapsedTime);

            if ((nextTilePosition - currentPosition).sqrMagnitude < 0.001f)
            {
                transform.Translate(nextTilePosition - transform.position);
                break;
            }
            transform.Translate(currentPosition - transform.position);

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
