using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteInEditMode]
public class GridTileEdit : MonoBehaviour
{

    GridTile tile;
    [HideInInspector] GridTile.tileTypes currentType;
    Renderer meshR;

    [Space(5)] [Header("Materials")] [Space(3)]
    public Material neutral_Mat;
    public Material T1_Mat;
    public Material T2_Mat;
    public Material Disabled_Mat;

#if UNITY_EDITOR

    private void Awake()
    {
        tile = GetComponent<GridTile>();
        meshR = transform.Find("Mesh").GetComponent<Renderer>();
        currentType = tile.tileType;
    }

    void Update()
    {
        TilePositionRestrictions();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print(1);
        }

        if (currentType != tile.tileType)
        {
            currentType = tile.tileType;
            ChangeMaterial();
        }
    }

    private void ChangeMaterial()
    {
        switch (currentType)
        {
            case GridTile.tileTypes.Neutral:
                meshR.sharedMaterial = neutral_Mat;
                break;
            case GridTile.tileTypes.T1:
                meshR.sharedMaterial = T1_Mat;
                break;
            case GridTile.tileTypes.T2:
                meshR.sharedMaterial = T2_Mat;
                break;
            case GridTile.tileTypes.Disabled:
                meshR.sharedMaterial = Disabled_Mat;
                break;
            default:
                return;
        }
    }

    private void TilePositionRestrictions()
    {
        transform.position = new Vector3(
        Mathf.RoundToInt(Mathf.Clamp(transform.position.x, 0, 100)),
        Mathf.RoundToInt(Mathf.Clamp(transform.position.y, -10, 10)),
        Mathf.RoundToInt(Mathf.Clamp(transform.position.z, 0, 100))
        );
    }


#endif
}
