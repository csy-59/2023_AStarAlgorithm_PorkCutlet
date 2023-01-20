using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSencer : MonoBehaviour
{
    private TileCreater tileCreater;

    public void Awake()
    {
        tileCreater = GetComponentInParent<TileCreater>();
    }
    public void Start()
    {

        //Debug.Log(tileCreater.GetTileIndex(transform.localPosition));
    }
}
