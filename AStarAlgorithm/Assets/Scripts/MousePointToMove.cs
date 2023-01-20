using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePointToMove : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TileCreater tileCreater;
    private Transform playerTransform;
    private float cameraHeight;

    public void Start()
    {
        playerTransform = tileCreater.PlayerTransform;
        cameraHeight = mainCamera.transform.position.y;
    }
    public void Update()
    {
        // ������ ��ư Ŭ�� �� �÷��̾��� ���� ��ġ�� 
        if (Input.GetMouseButtonDown(1))
        {
            try
            {
                int startIndex = tileCreater.GetTileIndex(playerTransform.position);

                Vector3 point = mainCamera.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * cameraHeight);
                int targetIndex = tileCreater.GetTileIndex(point);

                Debug.Log($"���� ����: {startIndex} ��ǥ ����: {targetIndex}");
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log($"�� ���� ������");
            }
        }
    }
}
