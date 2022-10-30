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
    public Transform playerT;
    public static GridGenerator Instance { get; private set; }
    Transform tileHolder;


    [Header("Input Values")]

    [SerializeField] public int raws;
    [SerializeField] public int columns;
    [HideInInspector] public Vector3 ogPosition;
    #endregion

    void Awake()
    {
        //Un truc pour empêcher d'avoir plusieurs singleton dans une scene.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        instantiateGrid = false;

        generateGrid();

    }
    private void Start()
    {
        //Place le joueur sur la tile avec le bool ogPos activé.
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
        //Trouve toutes les tiles existant dans la scène et les met dans un tableau temporaire et pas classé.
        GridTile[] tempGrid = FindObjectsOfType<GridTile>();

        //Ordonne dans un nouveau tableau toutes les tiles en fonction de leur position et les renomme.
        if(tempGrid.Length > 0)
        {
            grid = new GridTile[raws + 1, columns + 1];
            for (int i = 0; i < tempGrid.Length; i++)
            {
                int x = (int)tempGrid[i].transform.position.x / (int)tempGrid[i].transform.localScale.x;
                int y = (int)tempGrid[i].transform.position.z / (int)tempGrid[i].transform.localScale.y;
                grid[x, y] = tempGrid[i];
                grid[x, y].name = "tiles " + x + " " + y;
            }
        }
    }




    private void OnDrawGizmos()
    {
        //Fonction called en inspector quand le booléen instantiateGrid est appelé.
        if (instantiateGrid)
        {
            tileHolder = GameObject.FindGameObjectWithTag("Grid").transform; 

            GridTile[] tempGrid = FindObjectsOfType<GridTile>();
            grid = new GridTile[raws + 1, columns + 1];
            for (int i = 0; i < tempGrid.Length; i++)
            {
                //Si une tile est en dehors des dimension de la grille la détruit.
                if(tempGrid[i].transform.position.x > raws || tempGrid[i].transform.position.z > columns)
                {
                    DestroyImmediate(tempGrid[i].gameObject);
                    return;
                }
                else
                {
                    int x = (int)tempGrid[i].transform.position.x / (int)tempGrid[i].transform.localScale.x;
                    int y = (int)tempGrid[i].transform.position.z / (int)tempGrid[i].transform.localScale.y;
                    grid[x, y] = tempGrid[i];
                    grid[x, y].name = "tiles " + x + " " + y;
                }
            }


            //Si une tile n'existe pas dans les dimension de la grille l'instantie.
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
            //Met fin à la génération de grille.
            instantiateGrid = false;

        }

    }


}

