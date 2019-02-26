using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public enum SELECTION_TYPE
{
    SquadSpawnPosition,
    SquadMovingPosition,
    SpawnBoogiesArea
}
public class SelectorMouseBehavior : MonoBehaviour
{
    public Vector3 clickPosition;
    public Sprite sprite;
    public SELECTION_TYPE selectionType;

    public static SELECTION_TYPE Selection
    {
        get { return FindObjectOfType<SelectorMouseBehavior>().selectionType; }
    }

    public static Vector3 Position
    {
        get { return FindObjectOfType<SelectorMouseBehavior>().clickPosition; }
    }

    public static SelectorMouseBehavior Create(SELECTION_TYPE type)
    {

        GameObject selectorMouse = Instantiate(Resources.Load("Prefabs/UI/SelectorMouse")) as GameObject;
        SelectorMouseBehavior selectorMouseBehavior = selectorMouse.GetComponent<SelectorMouseBehavior>();
        switch (type)
        {
            case SELECTION_TYPE.SquadSpawnPosition:
                selectorMouseBehavior.sprite = Resources.Load<Sprite>("Sprites/selector");
                break;
            case SELECTION_TYPE.SquadMovingPosition:
                selectorMouseBehavior.sprite = Resources.Load<Sprite>("Sprites/selector");
                break;
            case SELECTION_TYPE.SpawnBoogiesArea:
                selectorMouseBehavior.sprite = Resources.Load<Sprite>("Sprites/circle");
                selectorMouseBehavior.GetComponentInChildren<SpriteRenderer>().transform.localScale = Vector3.one;
                selectorMouseBehavior.GetComponentInChildren<SpriteRenderer>().gameObject.AddComponent<CircleCollider2D>();
                BoogiesSpawner.RadiusCircle = selectorMouseBehavior.GetComponentInChildren<CircleCollider2D>().radius;
                break;
        }
        selectorMouseBehavior.GetComponentInChildren<SpriteRenderer>().sprite = selectorMouseBehavior.sprite;
        selectorMouseBehavior.GetComponentInChildren<SpriteRenderer>().enabled = true;
        return selectorMouseBehavior;
    }

    public static void Destroy()
    {
        if (FindObjectOfType<SelectorMouseBehavior>() != null)
        {
            Destroy(FindObjectOfType<SelectorMouseBehavior>().gameObject);
        }
    }

}
