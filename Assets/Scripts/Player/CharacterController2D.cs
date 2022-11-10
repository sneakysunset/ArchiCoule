using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CharacterController2D : MonoBehaviour
{
    public enum Team { J1, J2 };
    private Rigidbody rb;
    [HideInInspector] public Color col;
    /*[HideInInspector]*/ public bool groundCheck = false;
    private float ogGravity;
    PlayerCollisionManager collManager;
    string horizontal;
    [HideInInspector] public bool jumping;

    [HideInInspector] public Team playerType;
    [HideInInspector] public float jumpStrength;
    [HideInInspector] public Color colorJ1, colorJ2;
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public float gravityStrength;
    [HideInInspector] public KeyCode jumpKey;
    [HideInInspector] public float ghostInputTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        collManager = GetComponent<PlayerCollisionManager>();
        ogGravity = rb.mass;
        playerTypeChange();
    }

    private void playerTypeChange()
    {
        MeshRenderer pRend = GetComponentInChildren<MeshRenderer>();
        if (playerType == Team.J1)
        {
            pRend.material.color = colorJ1;
            col = colorJ1;
            horizontal = "HorizontalJ1";
        }
        else
        {
            pRend.material.color = colorJ2;
            col = colorJ2;
            horizontal = "HorizontalJ2";
        }
    }

    private void Update()
    {
        if (groundCheck && Input.GetKeyDown(jumpKey))
            jumping = true;
    }

    private void FixedUpdate()
    {
        Move();
        if (jumping) Jump();
        else if (!groundCheck) rb.mass += Time.deltaTime * gravityStrength;

    }

    private void Jump()
    {
        jumping = false;
        rb.mass = ogGravity;
        rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
        groundCheck = false;
        StartCoroutine(WaitForPhysics());
        if (collManager.groundCheckEnum != null)
        {
            StopCoroutine(collManager.groundCheckEnum);
            collManager.groundCheckEnum = null;
        }
    }

    private void Move()
    {
        float horizontalAxis = Input.GetAxis(horizontal);
        rb.velocity = new Vector2(horizontalAxis * moveSpeed * Time.deltaTime * 100, rb.velocity.y) ;
    }

    public IEnumerator WaitForPhysics()
    {
        yield return new WaitForFixedUpdate();
        groundCheck = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "LineCollider" && other.gameObject.layer != LayerMask.NameToLayer("Collider" + playerType.ToString()))
        {
            print(1);
            other.gameObject.layer = LayerMask.NameToLayer("ColliderJ" + 1);
            if (playerType == CharacterController2D.Team.J1)
            {
                other.gameObject.layer = LayerMask.NameToLayer("ColliderJ" + 2);
                print(2);
            }
        }

    }
}

#region editor
#if UNITY_EDITOR

[CustomEditor(typeof(CharacterController2D))/*, ExecuteInEditMode*/]
[System.Serializable]
public class OnGUIEditorHide : Editor
{
    GUIStyle bigTitle, smallTitle, parameter;
    float spaceBetweenTitles = 50;
    float spaceUnderTitle = 15;
    float spaceBetweenParameters = 10;
    public override void OnInspectorGUI()
    {
        Polices();
        GUILayout.Label("Character Controller", bigTitle);
        EditorGUILayout.Space(spaceUnderTitle);

        base.OnInspectorGUI();
        CharacterController2D script = target as CharacterController2D;

        GUILayout.Label("Is this player 1 or 2?", parameter);
        script.playerType = (CharacterController2D.Team)EditorGUILayout.EnumPopup("Player Type", script.playerType);
        EditorGUILayout.Space(spaceBetweenParameters);

        GUILayout.Label("Input that triggers the player jump", parameter);
        script.jumpKey = (KeyCode)EditorGUILayout.EnumPopup("Jump Key", script.jumpKey);
        EditorGUILayout.Space(spaceBetweenParameters);


        bool condition = script.playerType == CharacterController2D.Team.J1;
        GUILayout.Label("The Color of this player material", parameter);
        if (condition) script.colorJ1 = EditorGUILayout.ColorField("Color of Player", script.colorJ1);
        else script.colorJ1 = EditorGUILayout.ColorField("Color of Player", script.colorJ2);
        EditorGUILayout.Space(spaceBetweenParameters);

        GUILayout.Label("The Strength of this player jump", parameter);
        script.jumpStrength = EditorGUILayout.FloatField("Jump Strength", script.jumpStrength);
        EditorGUILayout.Space(spaceBetweenParameters);

        GUILayout.Label("The Movement Speed of the player", parameter);
        script.moveSpeed = EditorGUILayout.FloatField("Move Speed", script.moveSpeed);
        EditorGUILayout.Space(spaceBetweenParameters);

        GUILayout.Label("The higher this value is the faster the player will fall to the ground", parameter);
        script.gravityStrength = EditorGUILayout.FloatField("Gravity Strength", script.gravityStrength);
        EditorGUILayout.Space(spaceBetweenParameters);

        GUILayout.Label("Timer before the player can't jump after leaving the ground", parameter);
        script.ghostInputTimer = EditorGUILayout.FloatField("Ghost Input Timer", script.ghostInputTimer);

        EditorUtility.SetDirty(script); 
    }

    void Polices()
    {
        //These are typo styles used in labels to make the editor more readable
        //They define font size and color but it is also possible to import other fonts onto them

        bigTitle = new GUIStyle(EditorStyles.label);
        bigTitle.normal.textColor = new Color(1, 0.92f, 0.016f, 0.5f);
        bigTitle.fontSize = 20;

        smallTitle = new GUIStyle(EditorStyles.label);
        smallTitle.normal.textColor = Color.white;
        smallTitle.fontSize = 13;

        parameter = new GUIStyle(EditorStyles.label);
        parameter.normal.textColor = Color.grey;
        parameter.fontSize = 12;
    }

}
#endif
#endregion