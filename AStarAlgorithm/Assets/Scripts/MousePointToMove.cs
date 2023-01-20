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
        // 오른쪽 버튼 클릭 시 플레이어의 현제 위치와 
        if (Input.GetMouseButtonDown(1))
        {
            try
            {
                int startIndex = tileCreater.GetTileIndex(playerTransform.position);

                Vector3 point = mainCamera.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * cameraHeight);
                int targetIndex = tileCreater.GetTileIndex(point);

                Debug.Log($"시작 지점: {startIndex} 목표 지점: {targetIndex}");
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log($"맵 밖을 선택함");
            }
        }
    }
}
