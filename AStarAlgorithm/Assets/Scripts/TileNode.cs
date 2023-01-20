using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNode : MonoBehaviour
{
    [SerializeField] private MeshRenderer myRenderer;

    [Header("==메테리얼 색 변환==")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material blockMaterail;

    public bool IsBlocked { get; private set; }

    public void Awake()
    {
        IsBlocked = Random.Range(0, 2) % 2 == 0;
        Debug.Log(IsBlocked);
        if (IsBlocked)
        {
            myRenderer.material.color = blockMaterail.color;
        }
        else
        {
            myRenderer.material.color = normalMaterial.color;
        }
    }
}
