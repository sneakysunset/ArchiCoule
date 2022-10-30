using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public struct InputKey
{
    public enum KeyType { Press, Hold, Release};
    
    public KeyCode inputKey;
    public KeyType keyType;
    public UnityEvent InputEvent;
}

//Events lancés quand le joueur appuie sur un input.
public class InputManager : MonoBehaviour
{
    public List<InputKey> Inputs;

    private void Update()
    {
        foreach(InputKey input in Inputs)
        {
            switch (input.keyType)
            {
                case InputKey.KeyType.Press:
                    if (Input.GetKeyDown(input.inputKey))
                    {
                        input.InputEvent?.Invoke();
                    }
                    break;
                case InputKey.KeyType.Hold:
                    if (Input.GetKey(input.inputKey))
                    {
                        input.InputEvent?.Invoke();
                    }
                    break;
                case InputKey.KeyType.Release:
                    if (Input.GetKeyUp(input.inputKey))
                    {
                        input.InputEvent?.Invoke();
                    }
                    break;
                default:
                    return;
            }
        }
    }
}
