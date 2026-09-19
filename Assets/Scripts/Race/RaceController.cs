using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceController : MonoBehaviour
{
    public CheckPointController[] carsController;
    public static bool racing = false;
    public static int totalLaps = 1;
    public int timer = 3;
    public GameObject endPanel;

    void Start()
    {
        endPanel.SetActive(false);
        //Debug.Log("-----------------------------");
        InvokeRepeating("CountDown", 3, 1);
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
        carsController = new CheckPointController[cars.Length];
        for (int i = 0; i < cars.Length; i++)
        {
            carsController[i] = cars[i].GetComponent<CheckPointController>();
        }
    }

    void LateUpdate()
    {
        int finishedLap = 0;
        foreach (CheckPointController controller in carsController)
        {
            if (controller.lap == totalLaps + 1) 
                finishedLap++;

            if (finishedLap == carsController.Length && racing)
            {
                endPanel.SetActive(true);
                //Debug.Log("FinishRace");
                racing = false;
            }
        }
    }

    void CountDown()
    {
        if (timer != 0)
        {
            Debug.Log("Rozpoczêcie wyœcigu za: " + timer);
            timer--;
        }
        else
        {
            Debug.Log("Start!!!");
            racing = true;
            CancelInvoke("CountDown");
        }
    }
}
