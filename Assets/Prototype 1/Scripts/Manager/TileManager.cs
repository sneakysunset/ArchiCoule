using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    //Méthode appelé quand le joueur sort du collider d'une tile. Elle appel une fonction changeant le type de la tile en question.
    public void OnPlayerExit(GameObject player, GameObject tile)
    {
        if (char.GetNumericValue(player.name[1]) == 1) GridGenerator.Instance.grid[Mathf.RoundToInt(tile.transform.position.x), Mathf.RoundToInt(tile.transform.position.z)].tileF.ChangeTileType(GridTile.tileTypes.T1);
        else if (char.GetNumericValue(player.name[1]) == 2) GridGenerator.Instance.grid[Mathf.RoundToInt(tile.transform.position.x), Mathf.RoundToInt(tile.transform.position.z)].tileF.ChangeTileType(GridTile.tileTypes.T2);
    }
}
