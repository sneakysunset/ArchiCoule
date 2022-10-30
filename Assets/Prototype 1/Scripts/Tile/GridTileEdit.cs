using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//Ce script ne s'execute que dans l'éditeur en dehors du play mode
[ExecuteInEditMode]
public class GridTileEdit : MonoBehaviour
{

    GridTile tile;
    [HideInInspector] GridTile.tileTypes currentType;
    [HideInInspector] bool currentOgPos;
    Renderer meshR;

    [Space(5)] [Header("Materials")] [Space(3)]
    public Material neutral_Mat;
    public Material T1_Mat;
    public Material T2_Mat;
    public Material Disabled_Mat;
    public GameObject ogPosItem;

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

        //Check si le type de tile est le même qu'à la frame précédente
        if (currentType != tile.tileType)
        {
            currentType = tile.tileType;
            //Si le type de tile à été changé depuis la dernière frame change les matériels.
            ChangeMaterial();
        }

        if(currentOgPos != tile.ogPos)
        {
            currentOgPos = tile.ogPos;
            gridTileParameterChanges(ogPosItem, "Original Position", .51f);
        }
    }


    private void gridTileParameterChanges(GameObject obj, string name, float yPos)
    {
        if (transform.Find(name))
        {
            DestroyImmediate(transform.Find(name).gameObject);
        }
        else
        {
            var inst = Instantiate(obj, transform.position + new Vector3(0, yPos, 0), Quaternion.identity, transform);
            inst.name = name;
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

    //Les tiles ne peuvent pas etre positionnée en position flotantes (que des positions à valeurs entieres)
    //Elles ne peuvent pas aussi avoir de position négative ou être trop éloignée du centre.
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
