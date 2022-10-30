using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Scripts avec des events appelés lorsque l'objet à une collision ou sort d'un collider.

[System.Serializable]
public struct Event
{
    public UnityEvent<GameObject, GameObject> OnTriggerEnterEvent;
    public UnityEvent<GameObject, GameObject> OnTriggerExitEvent;
    public string tag;
}

public class TriggerEvent : MonoBehaviour
{
    public List<Event> Events;

    private void OnTriggerEnter(Collider other)
    {
        foreach(Event triggerEvent in Events)
        {
            if(other.tag == triggerEvent.tag)
            {
                triggerEvent.OnTriggerEnterEvent?.Invoke(this.gameObject, other.gameObject);
                break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (Event triggerEvent in Events)
        {
            if (other.tag == triggerEvent.tag)
            {
                triggerEvent.OnTriggerExitEvent?.Invoke(this.gameObject, other.gameObject);
                break;
            }
        }
    }
}
