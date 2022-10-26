using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if  UNITY_EDITOR
using UnityEditor;
#endif
public class GridGenerator : MonoBehaviour
{
    #region variables
    public GridTile?[,] grid;
    [SerializeField] public bool instantiateGrid = false;
    public GameObject Tile;
    Transform playerT;
    public static GridGenerator Instance { get; private set; }
    Transform tileHolder;


    [Header("Input Values")]

    [SerializeField] public int raws;
    [SerializeField] public int columns;
    [HideInInspector] public Vector3 ogPosition;
    #endregion

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        instantiateGrid = false;
        
        GridTile[] list = FindObjectsOfType<GridTile>();

        grid = new GridTile[raws + 1, columns + 1];
        for (int i = 0; i < list.Length; i++)
        {
            int x = (int)list[i].transform.position.x / (int)list[i].transform.localScale.x;
            int y = (int)list[i].transform.position.z / (int)list[i].transform.localScale.y;
            grid[x, y] = list[i];
            grid[x, y].name = "tiles " + x + " " + y;
        }

    }
    private void Start()
    {
        foreach (GridTile obj in grid)
        {
            if (obj.ogPos)
            {
                ogPosition = new Vector3(obj.transform.position.x, playerT.position.y, obj.transform.position.z);
                playerT.position = ogPosition;
            }
        }
    }

    public void generateGrid()
    {
        GridTile[] list = FindObjectsOfType<GridTile>();
        if (list.Length != 0)
        {
            grid = new GridTile[raws + 1, columns + 1];
            for (int i = 0; i < list.Length; i++)
            {
                int x = (int)list[i].transform.position.x / (int)list[i].transform.localScale.x;
                int y = (int)list[i].transform.position.z / (int)list[i].transform.localScale.y;
                grid[x, y] = list[i];
                grid[x, y].name = "tiles " + x + " " + y;
            }
        }
    }




    private void OnDrawGizmos()
    {
        if (instantiateGrid)
        {
            tileHolder = GameObject.FindGameObjectWithTag("Grid").transform; 

            GridTile[] list = FindObjectsOfType<GridTile>();
            grid = new GridTile[raws + 1, columns + 1];
            for (int i = 0; i < list.Length; i++)
            {
                if(list[i].transform.position.x > raws || list[i].transform.position.z > columns)
                {
                    DestroyImmediate(list[i].gameObject);
                    return;
                }
                else
                {
                    int x = (int)list[i].transform.position.x / (int)list[i].transform.localScale.x;
                    int y = (int)list[i].transform.position.z / (int)list[i].transform.localScale.y;
                    grid[x, y] = list[i];
                    grid[x, y].name = "tiles " + x + " " + y;
                }
            }


            for (int x = 0; x < raws + 1; x++)
            {
                for (int y = 0; y < columns + 1; y++)
                {
                    if (!grid[x, y])
                    {
#if UNITY_EDITOR
                        Selection.activeObject = PrefabUtility.InstantiatePrefab(Tile);
                        var inst = Selection.activeObject as GameObject;
#endif

#if !UNITY_EDITOR
                        var inst = Instantiate(Tile);
#endif
                        inst.transform.position = new Vector3(x, 0, y);
                        grid[x, y] = inst.GetComponent<GridTile>();
                        grid[x, y].transform.parent = tileHolder;
                        grid[x, y].name = "tiles " + x + " " + y;
                        return;
                    }
                }
            }

            instantiateGrid = false;

        }

    }


}

