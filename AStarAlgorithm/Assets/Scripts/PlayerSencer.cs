using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSencer : MonoBehaviour
{
    public TileCreater TileCreater { get; set; }

    public void Awake()
    {
        Debug.Log(TileCreater.GetTileIndex(transform.localPosition));
    }
}
