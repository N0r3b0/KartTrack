using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera cam;
    public Vector3[] positions;
    int activePosition;
    // Start is called before the first frame update
    void Start()
    {
        if (positions.Length == 0) 
            return;
        cam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = positions[activePosition];
    }

    // Update is called once per frame
    void Update()
    {
        if (positions.Length == 0) 
            return;
        if (Input.GetKeyDown(KeyCode.T))
        {
            activePosition++;
            activePosition = activePosition % positions.Length;
            cam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = positions[activePosition];
        }
    }

    public void SetCameraProperties(GameObject car)
    {
        DrivingScript driver = car.GetComponent<DrivingScript>();
        Transform targetTransform = (driver != null && driver.rb != null) ? driver.rb.transform : car.transform;

        cam.Follow = targetTransform;
        cam.LookAt = targetTransform;
    }
}
