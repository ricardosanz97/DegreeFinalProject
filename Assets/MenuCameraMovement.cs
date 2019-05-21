using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraMovement : MonoBehaviour
{
    private Vector3 initialPos;

    // Start is called before the first frame update
    void Start()
    {
        initialPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.forward * 2f * Time.deltaTime);
    }
}
