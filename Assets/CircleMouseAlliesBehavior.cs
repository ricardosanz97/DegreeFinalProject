using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMouseAlliesBehavior : MonoBehaviour
{
    public float radiusCircle;
    private CircleCollider2D _circleCollider;
    private void Awake()
    {
        _circleCollider = GetComponentInChildren<CircleCollider2D>();
        radiusCircle = _circleCollider.radius;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            this.transform.position = hit.point;
            if (Input.GetMouseButtonDown(0) && FindObjectOfType<BoogiesSpawner>().selectingAlliesObjective)
            {
                Debug.Log("clicking a position");
                FindObjectOfType<BoogiesSpawner>().HandleAlliesSelect();
                FindObjectOfType<BoogiesSpawner>().SpawnAllBoogiesSelected(hit.point);
            }
        }
    }

    public static float RadiusCircle
    {
        get { return FindObjectOfType<CircleMouseAlliesBehavior>().radiusCircle; }
        set { FindObjectOfType<CircleMouseAlliesBehavior>().radiusCircle = value; }
    }
}
