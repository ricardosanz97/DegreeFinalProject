using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Transform cameraTarget;
    private float x = 0.0f;
    private float y = 0.0f;

    private int mouseXSpeedMod = 3;
    private int mouseySpeedMod = 3;

    public float maxViewDistance = 25.0f;
    public float minViewDistance = 1.0f;
    public int zoomRate = 30;
    public float lerpRate = 1.0f;
    private float distance = 3.0f;
    private float desiredDistance;
    private float correctedDistance;
    private float currentDistance;

    public float cameraTargetHeight = 1.0f;

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x += angles.x;
        y -= angles.y;

        currentDistance = distance;
        desiredDistance = distance;
        correctedDistance = distance;
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            x += Input.GetAxis("Mouse X") * mouseXSpeedMod;
            y -= Input.GetAxis("Mouse Y") * mouseySpeedMod;
        }

        else if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("Horizontal") != 0)
        {
            float targetRotationAngle = cameraTarget.eulerAngles.y;
            float cameraRotaionAngle = transform.eulerAngles.y;
            x = Mathf.LerpAngle(cameraRotaionAngle, targetRotationAngle, lerpRate * Time.deltaTime);
        }

        y = ClampAngle(y, -50, 50);

        Quaternion rotation = Quaternion.Euler(y, x, 0);


        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        desiredDistance = Mathf.Clamp(desiredDistance, minViewDistance, maxViewDistance);
        correctedDistance = desiredDistance;

        Vector3 position = cameraTarget.position - (rotation * Vector3.forward * desiredDistance);

        RaycastHit collisionHit;
        Vector3 cameraTargetPosition = new Vector3(cameraTarget.position.x, cameraTarget.position.y + cameraTargetHeight, cameraTarget.position.z);

        bool isCorrected = false;
        if (Physics.Linecast(cameraTargetPosition, position, out collisionHit))
        {
            if (collisionHit.transform.GetComponent<Boogie>() == null)
            {
                position = collisionHit.point;
                correctedDistance = Vector3.Distance(cameraTargetPosition, position);
                isCorrected = true;
            }
        }

        currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomRate) : correctedDistance;

        position = cameraTarget.position - (rotation * Vector3.forward * currentDistance + new Vector3(0, -cameraTargetHeight, 0));

        transform.rotation = rotation;
        transform.position = position;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
        {
            angle += 360;
        }
        if (angle > 360)
        {
            angle -= 360;
        }
        return Mathf.Clamp(angle, min, max);
    }
}
