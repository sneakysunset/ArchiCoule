using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFunctions : MonoBehaviour
{
    GridTile t;
    GridTileEdit tileE;
    Renderer meshR;
    Collider col;
    private void Awake()
    {
        t = GetComponent<GridTile>();
        tileE = GetComponent<GridTileEdit>();
        meshR = transform.Find("Mesh").GetComponent<Renderer>();
        col = transform.Find("Collider").GetComponent<Collider>();
    }

    private void Start()
    {
        if (t.tileType == GridTile.tileTypes.Disabled)
        {
            t.walkable = false;
            meshR.enabled = false;
            col.enabled = false;
            meshR.transform.Find("Outline").gameObject.SetActive(false);
        }
    }



    public void ChangeTileType(GridTile.tileTypes tType)
    {
        t.tileType = tType;
        switch (t.tileType)
        {
            case GridTile.tileTypes.Neutral:
                meshR.material = tileE.neutral_Mat;
                break;
            case GridTile.tileTypes.T1:
                meshR.material = tileE.T1_Mat;
                break;
            case GridTile.tileTypes.T2:
                meshR.material = tileE.T2_Mat;
                break;
            case GridTile.tileTypes.Disabled:
                meshR.material = tileE.Disabled_Mat;
                t.walkable = false;
                meshR.enabled = false;
                col.enabled = false;
                break;
            default:
                return;
        }
    }
}
