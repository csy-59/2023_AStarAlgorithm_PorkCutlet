using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Coroutine moveToTargetCoroutine;

    public void MoveToTargetPosition()
    {
        moveToTargetCoroutine = StartCoroutine(CoMoveToTarget());
    }

    private IEnumerator CoMoveToTarget()
    {
        yield return null;
    }
}
