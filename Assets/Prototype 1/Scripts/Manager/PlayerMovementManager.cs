using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlayerMovementManager : MonoBehaviour
{
    GridTile[,] grid;
    int rows, columns;
    Transform playerT;
    IEnumerator moveCoroutine;
    [HideInInspector] public UnityEvent endCoroutineEvent;

    public float movementCompletionTime = 1;
    public AnimationCurve movementAnimationCurve;

    private void Start()
    {
        grid = GridGenerator.Instance.grid;
        rows = GridGenerator.Instance.raws;
        columns = GridGenerator.Instance.columns;
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void MovementCall(int direction)
    {
        int x = Mathf.RoundToInt(playerT.position.x);
        int y = Mathf.RoundToInt(playerT.position.z);
        switch (direction)
        {
            //Test dans la direction de l'input si une case est disponible (TestDirection).
            //Si la case est disponible lance le mouvement du joueur vers cette dernière.
            case 0:
                if (TestDirection(x, y + 1, x, y, direction)) MoveToTile(x, y + 1);
                break;
            case 1:
                if (TestDirection(x, y - 1, x, y, direction)) MoveToTile(x, y - 1);
                break;
            case 2:
                if (TestDirection(x + 1, y, x, y, direction)) MoveToTile(x + 1, y);
                break;
            case 3:
                if (TestDirection(x - 1, y, x, y, direction)) MoveToTile(x - 1, y);
                break;
            default:
                Debug.LogError("Error");
                return;
        }
    }

    public bool TestDirection(int x, int y, int prevX, int prevY, int direction)
    {
        //Test si la case est dans les dimensions de la grille et si elle n'est pas désactivée.
       bool condition1 = x <= rows && x >= 0 && y <= columns && y >= 0;
       if (condition1 && grid[x, y].walkable )
        {
            //Si un déplacement n'est pas en cours lance l'animation de déplacement.
            if (moveCoroutine == null) return true;
            else
            {
                //Si un déplacement est en court met le call de déplacement en pause jusqu'à ce que le déplacement précédent soit fini.
                StartCoroutine(QueueForTest(direction));
                return false;
            } 
        }
       else return false;
    }

    void MoveToTile(int targetX, int targetZ)
    {
        //Lance animation de déplacement.
        var endPos = new Vector3(targetX, playerT.position.y, targetZ);
        endCoroutineEvent.RemoveAllListeners();
        endCoroutineEvent.AddListener(OnEndCoroutine);
        moveCoroutine = Anims.MovementAnim(movementCompletionTime, playerT.position, endPos, playerT, movementAnimationCurve, endCoroutineEvent);
        StartCoroutine(moveCoroutine);
    }

    void OnEndCoroutine()
    {
        //Méthode appelée à la fin de la coroutine de déplacement. Permet de lancer les animations déplacement en attente.
        moveCoroutine = null;
    }

    IEnumerator QueueForTest(int direction)
    {
        //Attend que le déplacement input précédement soit finit pour submit celui rentrant dans la coroutine.
        yield return new WaitUntil(() => moveCoroutine == null);
        MovementCall(direction);
    }
}


