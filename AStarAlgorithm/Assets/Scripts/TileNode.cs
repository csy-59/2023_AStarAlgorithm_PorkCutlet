using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNode : MonoBehaviour
{
    [SerializeField] private MeshRenderer myRenderer;
    [Header("==확률==")]
    [SerializeField] [Range(0, 100)] private float blockRate = 20;

    [Header("==메테리얼 색 변환==")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material blockMaterail;

    public bool IsBlocked { get; private set; }

    public void SetToNormalTile()
    {
        if(IsBlocked)
        {
            myRenderer.material.color = normalMaterial.color;
        }
    }

    public void Awake()
    {
        IsBlocked = Random.Range(0, 100) < blockRate;
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
