using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public MenuEntityMovement first;
    public Transform spawnFirst;
    public Transform destinyFirst;
    public MenuEntityMovement second;
    public Transform spawnSecond;
    public Transform destinySecond;
    public MenuDoorBehavior door;

    private void Start()
    {
        SelectEntities();
    }

    public void SelectEntities()
    {
        GameObject firstEntity = Instantiate(Resources.Load<GameObject>("Prefabs/MenuEntities/Entities/" + Random.Range(1, 8)), spawnFirst.transform.position, spawnFirst.transform.rotation);
        first = firstEntity.GetComponent<MenuEntityMovement>();
        first.AssignPoint(spawnFirst, destinyFirst);

        GameObject secondEntity = Instantiate(Resources.Load<GameObject>("Prefabs/MenuEntities/Entities/" + Random.Range(1, 8)), spawnSecond.transform.position, spawnSecond.transform.rotation);
        second = secondEntity.GetComponent<MenuEntityMovement>();
        second.AssignPoint(spawnSecond, destinySecond);
    }

    public void RestartAnimation()
    {
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z - 60);
        door.RestartDoor();
        Destroy(first.gameObject);
        Destroy(second.gameObject);
        SelectEntities();
    }

    public void TriggerFirst()
    {
        first.Trigger();
    }

    public void TriggerSecond()
    {
        second.Trigger();
    }

    public void TriggerDoor()
    {
        door.Trigger();
    }
}
