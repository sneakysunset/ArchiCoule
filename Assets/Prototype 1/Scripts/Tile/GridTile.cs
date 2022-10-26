using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    public enum tileTypes { Neutral, T1, T2, Disabled };
    
    public bool ogPos;
    public tileTypes tileType;
    [HideInInspector] public bool walkable = true;
    [HideInInspector] public TileFunctions tileF;
    private void Awake()
    {
        tileF = GetComponent<TileFunctions>();
    }
}


