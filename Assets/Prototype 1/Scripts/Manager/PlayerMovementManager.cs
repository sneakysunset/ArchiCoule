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
       bool condition1 = x <= rows && x >= 0 && y <= columns && y >= 0;
       if (condition1 && grid[x, y].walkable )
        {
            if (moveCoroutine == null) return true;
            else
            {
                StartCoroutine(QueueForTest(direction));
                return false;
            } 
        }
       else return false;
    }

    void MoveToTile(int targetX, int targetZ)
    {
        var endPos = new Vector3(targetX, playerT.position.y, targetZ);
        endCoroutineEvent.RemoveAllListeners();
        endCoroutineEvent.AddListener(OnEndCoroutine);
        moveCoroutine = Anims.MovementAnim(movementCompletionTime, playerT.position, endPos, playerT, movementAnimationCurve, endCoroutineEvent);
        StartCoroutine(moveCoroutine);
    }

    void OnEndCoroutine()
    {
        moveCoroutine = null;
    }

    IEnumerator QueueForTest(int direction)
    {
        yield return new WaitUntil(() => moveCoroutine == null);
        MovementCall(direction);
    }
}


