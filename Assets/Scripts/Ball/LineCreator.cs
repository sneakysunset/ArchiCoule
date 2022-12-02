using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineCreator : MonoBehaviour
{
    #region Variables
    private float[] pointArray;
    public List<Vector2> pointList = new List<Vector2>();
    private Color col;
    private CharacterController2D charC;
    Transform lineFolder;
    [HideInInspector] public LineRenderer lineR;
    [HideInInspector] public EdgeCollider2D edgeC;
    [HideInInspector] public Transform lineT;

    private CharacterController2D.Team pType;
    Collider coll;
    [Header("Components")]
    [Space(5)]
    public GameObject linePrefab;
    Vector2 ogPos;
    public GameObject ballPrefab;



    [Space(10)]
    [Header("Line Variables")]
    [Space(5)]
    [Range(.001f, 1)] public float lineResolution = .5f;
    public float lineBeginningX = -13f;
    public float lineEndX = 13f;
    public float width = .3f;
    public float lineYOffSet = 0;

    [Space(10)]
    [Header("Refresh Variables")]
    [Space(5)]
    public float threshHoldToUpdatePoints;
    public float updateSoundFrequency;
    public AnimationCurve updateAnim;
    public float updateAnimSpeed;
    #endregion

    private void Start()
    {
        prevPos = transform.position;
        lineFolder = GameObject.FindGameObjectWithTag("LineFolder").transform;
        pointArray = Utils_Points.GeneratePointArray(pointArray, lineBeginningX, lineEndX, lineResolution);
        if (GetComponent<CharacterController2D>())
        {
            charC = GetComponent<CharacterController2D>();
            col = charC.col;
            pType = charC.playerType;
        }
        else pType = CharacterController2D.Team.Ball;
        ogPos = transform.position;
        var firstPoint = new Vector2(Utils_Points.closestPoint(pointArray, transform.position.x), transform.position.y);
        pointList.Add(firstPoint);
        InstantiateLine();
    }

    public void LineUpdater()
    {
        var list = pointList;
        list = pointList.OrderBy(v => v.x).ToList();

        if (!UpdatePointList()) return;

        if (list.Count < 4 || edgeC.gameObject.layer == 10) return;

        lineR.positionCount = list.Count;
        Vector3[] vector3s = new Vector3[list.Count];
        for (int i = 0; i < vector3s.Length; i++)
        {
            vector3s[i] = list[i];
        }
        lineR.SetPositions(vector3s);
        StartCoroutine(afterPhysics(list));
    }

    IEnumerator afterPhysics(List<Vector2> list)
    {
        yield return new WaitForFixedUpdate();
        edgeC.SetPoints(list);
        prevPos = transform.position;
    }

    public bool UpdatePointList()
    {
        int closestVertexIndex = Utils_Points.ClosestPointInList(pointList, transform.position.x, pointArray);
        float closestVertexX = pointList[closestVertexIndex].x;

        BandAidRemovePoints();

        closestVertexIndex = Utils_Points.ClosestPointInList(pointList, transform.position.x, pointArray);
        closestVertexX = pointList[closestVertexIndex].x;

        bool condition2 = false;
        bool condition1 = Mathf.Abs(transform.position.x - closestVertexX) > lineResolution;
        if (pointList.Count > 4)
        {
            condition2 = Mathf.Abs(transform.position.y - pointList[closestVertexIndex].y) > threshHoldToUpdatePoints;
        }


        if (condition1)
        {
            print(1);
            int numOfPointAdded = Utils_Points.AddPoints(pointArray, pointList, closestVertexX, transform.position - Vector3.up * lineYOffSet, lineResolution, lineYOffSet);
            if (pType != CharacterController2D.Team.Ball)
                charC.transform.localScale -= Vector3.one * charC.movementScaler / 100 * numOfPointAdded;
            FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCorde/TirageCorde");
            return true;
        }
        else if (!condition1 && condition2)
        {
            print(2);
            //var startPos = pointList[closestVertexIndex];
            //var endPos = (Vector2)transform.position - (Vector2.up * lineYOffSet);
            Utils_Points.UpdatePoints(pointArray, pointList, closestVertexX, transform.position - Vector3.up * lineYOffSet, lineResolution, lineYOffSet, prevPos);
            //StartCoroutine(Utils_Anim.AnimationLerp(startPos, endPos, endPos, updateAnim, updateAnimSpeed, returnValue => { pointList[closestVertexIndex] = returnValue; }));
            return true;
        }
        else
        {
            return false;
        }
    }

    private Vector2 prevPos;
    private void InstantiateLine()
    {
        lineT = Instantiate(linePrefab, lineFolder).transform;
        lineR = lineT.GetComponentInChildren<LineRenderer>();
        edgeC = lineT.GetComponentInChildren<EdgeCollider2D>();
        lineR.material.color = col;
        lineT.name = "Mesh " + pType.ToString() + " Off";
        edgeC.gameObject.layer = 6;
        if (pType != CharacterController2D.Team.Ball)
            charC.meshObj = lineT.gameObject;
    }

    //Enleve les doublons, il faut trouver une alternative
    void BandAidRemovePoints()
    {
        for (int i = 1; i < pointList.Count; i++)
        {
            if (Mathf.Abs(pointList[i].x - pointList[i - 1].x) < lineResolution * .9f)
            {
                pointList.RemoveAt(i);
                if (pType != CharacterController2D.Team.Ball)
                    charC.transform.localScale += Vector3.one * charC.movementScaler / 100;
                return;
            }
        }
    }

    private void OnDestroy()
    {
        //if(lineT)
        //Destroy(lineT.gameObject);

       // Instantiate(ballPrefab, ogPos, Quaternion.identity);
    }
}
