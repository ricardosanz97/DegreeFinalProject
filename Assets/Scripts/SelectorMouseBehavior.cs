using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using DG.Tweening;

public enum SELECTION_TYPE
{
    SquadSpawnPosition,
    SquadMovingPosition,
    SpawnBoogiesArea,
    SquadCover,
    SelectingSquadWrestlerChange
}
public class SelectorMouseBehavior : MonoBehaviour
{
    public Vector3 clickPosition;
    public Sprite sprite1;
    public Sprite sprite2;
    public SELECTION_TYPE selectionType;

    public Transform inside;
    public Transform outside;

    public static SELECTION_TYPE Selection
    {
        get { return FindObjectOfType<SelectorMouseBehavior>().selectionType; }
    }

    public static Vector3 Position
    {
        get { return FindObjectOfType<SelectorMouseBehavior>().clickPosition; }
    }

    private void Start()
    {
        StartCoroutine(ChildrenRotate());
    }

    IEnumerator ChildrenRotate()
    {
        while (true)
        {
            inside.transform.Rotate(Vector3.forward, 20f * Time.deltaTime);
            outside.transform.Rotate(Vector3.forward, -20f * Time.deltaTime);
            yield return null;
        }
    }

    public static SelectorMouseBehavior Create(SELECTION_TYPE type)
    {
        Debug.Log("creating selectorMouseBehavior");
        GameObject selectorMouse = Instantiate(Resources.Load("Prefabs/UI/SelectorMouseFinal")) as GameObject;
        SelectorMouseBehavior selectorMouseBehavior = selectorMouse.GetComponent<SelectorMouseBehavior>();
        switch (type)
        {
            case SELECTION_TYPE.SquadSpawnPosition:
                selectorMouseBehavior.sprite1 = Resources.Load<Sprite>("Sprites/UIElements/selector_blue");
                selectorMouseBehavior.sprite2 = Resources.Load<Sprite>("Sprites/UIElements/selector_blue");
                break;
            case SELECTION_TYPE.SquadMovingPosition:
                selectorMouseBehavior.sprite1 = Resources.Load<Sprite>("Sprites/UIElements/selector_red");
                selectorMouseBehavior.sprite2 = Resources.Load<Sprite>("Sprites/UIElements/selector_red");
                break;
            case SELECTION_TYPE.SpawnBoogiesArea:
                selectorMouseBehavior.sprite1 = Resources.Load<Sprite>("Sprites/UIElements/circle_blue");
                //selectorMouseBehavior.sprite2 = Resources.Load<Sprite>("Sprites/UIElements/circle_blue");
                selectorMouseBehavior.inside.gameObject.SetActive(false);
                selectorMouseBehavior.GetComponentInChildren<SpriteRenderer>().transform.localScale = Vector3.one;
                selectorMouseBehavior.GetComponentInChildren<SpriteRenderer>().gameObject.AddComponent<CircleCollider2D>();
                BoogiesSpawner.RadiusCircle = selectorMouseBehavior.GetComponentInChildren<CircleCollider2D>().radius;
                break;
            case SELECTION_TYPE.SquadCover:
                selectorMouseBehavior.sprite1 = Resources.Load<Sprite>("Sprites/UIElements/selector_green");
                selectorMouseBehavior.sprite2 = Resources.Load<Sprite>("Sprites/UIElements/selector_green");
                break;
            case SELECTION_TYPE.SelectingSquadWrestlerChange:
                selectorMouseBehavior.sprite1 = Resources.Load<Sprite>("Sprites/UIElements/selector_yellow");
                selectorMouseBehavior.sprite2 = Resources.Load<Sprite>("Sprites/UIElements/selector_yellow");
                break;
        }
        selectorMouseBehavior.outside.GetComponent<SpriteRenderer>().sprite = selectorMouseBehavior.sprite1;
        selectorMouseBehavior.inside.GetComponent<SpriteRenderer>().sprite = selectorMouseBehavior.sprite2;
        //selectorMouseBehavior.GetComponentInChildren<SpriteRenderer>().enabled = true;
        return selectorMouseBehavior;
    }

    public static void Destroy()
    {
        if (FindObjectOfType<SelectorMouseBehavior>() != null)
        {
            Destroy(FindObjectOfType<SelectorMouseBehavior>().gameObject);
        }
    }

    public static bool IsActive()
    {
        if (FindObjectOfType<SelectorMouseBehavior>() != null)
        {
            return (FindObjectOfType<SelectorMouseBehavior>().gameObject != null);
        }
        else
        {
            return false;
        }
    }

}
