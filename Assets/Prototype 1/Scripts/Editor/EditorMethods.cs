using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

//Méthodes appelés dans l'éditeur en dehors du play mode quand on appuie sur des inputs.
//Ces méthodes permettent de changer les types de tiles en appuyant sur Shift + 1, 2, 3 ou 4

public class EditorMethods : Editor
{
    [MenuItem("My Commands/Tile to Neutral #1")]
    static void tileToNeutral()
    {
        List<GridTile> tiles = GetSelectionTileList();
        foreach(GridTile tile in tiles)
        {
            tile.tileType = GridTile.tileTypes.Neutral;
            EditorUtility.SetDirty(tile);
        }
    }

    [MenuItem("My Commands/Tile to Disabled #2")]
    static void tileToDisabled()
    {
        List<GridTile> tiles = GetSelectionTileList();
        foreach (GridTile tile in tiles)
        {
            tile.tileType = GridTile.tileTypes.Disabled;
            EditorUtility.SetDirty(tile);
        }
    }

    [MenuItem("My Commands/Tile to T1 #3")]
    static void tileToT1()
    {
        List<GridTile> tiles = GetSelectionTileList();
        foreach (GridTile tile in tiles)
        {
            tile.tileType = GridTile.tileTypes.T1;
            EditorUtility.SetDirty(tile);
        }
        
    }

    [MenuItem("My Commands/Tile to T2 #4")]
    static void tileToT2()
    {
        List<GridTile> tiles = GetSelectionTileList();
        foreach (GridTile tile in tiles)
        {
            tile.tileType = GridTile.tileTypes.T2;
            EditorUtility.SetDirty(tile);
        }
    }

    [MenuItem("My Commands/Tile To Original Position #5")]
    static void TileToOgPos()
    {
        if (Selection.activeGameObject.GetComponent<GridTile>())
        {
            var tile = Selection.activeGameObject.GetComponent<GridTile>();
            if (tile.ogPos) tile.ogPos = false;
            else tile.ogPos = true;
            EditorUtility.SetDirty(tile);
            Debug.Log("Command Called");
        }
        else Debug.Log("No Valid Selection");
    }

    [MenuItem("My Commands/Generate Grid #0")]
    static void generateGrid()
    {
        var gridG = FindObjectOfType<GridGenerator>();
        gridG.instantiateGrid = true;
        EditorUtility.SetDirty(gridG);
        Debug.Log("Command Called");
    }

    static List<GridTile> GetSelectionTileList()
    {
        List<GridTile> temp = new List<GridTile>();
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj.GetComponent<GridTile>())
            {
                temp.Add(obj.GetComponent<GridTile>());
            }
        }

        if (temp.Count == 0)
        {
            Debug.Log("No Valid Selection");
        }
        else
        {
            Debug.Log("Command Called");
        }

        return temp;
    }
}
#endif
