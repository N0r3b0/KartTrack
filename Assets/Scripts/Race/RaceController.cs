using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceController : MonoBehaviourPunCallbacks
{
    public CheckPointController[] carsController;
    public static bool racing = false;
    public static int totalLaps = 1;
    public int timer = 3;
    public GameObject endPanel;

    public TMP_Text startText;
    AudioSource audioSource;
    public AudioClip countClip;
    public AudioClip startClip;

    public GameObject carPrefab;
    public Transform[] spawnPos;
    public int playerCount;

    public GameObject startRaceButton; // master only
    public GameObject waitingText;

    bool spawned = false;
    void Update()
    {
        if(!spawned && PhotonNetwork.InRoom)
        {
            spawned = true;
            Spawn();
        }
           
    }
    void Spawn()
    {
        audioSource = GetComponent<AudioSource>();
        endPanel.SetActive(false);
        startText.gameObject.SetActive(false);
        waitingText.SetActive(false);

        GameObject playerCar = null;

        if (PhotonNetwork.IsConnected)
        {
            Vector3 startPos = spawnPos[PhotonNetwork.CurrentRoom.PlayerCount-1].position;
            Quaternion startRot = spawnPos[PhotonNetwork.CurrentRoom.PlayerCount-1].rotation;

            if (OnlinePlayer.LocalPlayerInstance == null)
            {
                playerCar = PhotonNetwork.Instantiate(carPrefab.name, startPos, startRot);
            }
            if (PhotonNetwork.IsMasterClient)
            {
                startRaceButton.SetActive(true);
            }
            else
            {
                startRaceButton.SetActive(false);
                waitingText.SetActive(true);
            }
        }

        if (playerCar != null)
        {
            playerCar.GetComponent<PlayerController>().enabled = true;
            playerCar.GetComponent<DrivingScript>().enabled = true;
        }
    }

    void LateUpdate()
    {
        if (!racing || carsController == null || carsController.Length == 0)
            return;

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
        startText.gameObject.SetActive(true);
        if (timer != 0)
        {
            startText.text = timer.ToString();
            audioSource.PlayOneShot(countClip);
            timer--;
        }
        else
        {
            startText.text = "START!!!";
            audioSource.PlayOneShot(startClip);
            racing = true;
            CancelInvoke("CountDown");
            Invoke("HideStartText", 1);
        }
    }

    void HideStartText()
    {
        startText.gameObject.SetActive(false);
    }

    //public void LoadScene(int index)
    //{
    //    SceneManager.LoadScene(index);
    //}


    // network 
    [PunRPC]
    public void StartGame()
    {
        InvokeRepeating("CountDown", 3, 1);
        endPanel.SetActive(false);
        startText.gameObject.SetActive(false);

        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
        carsController = new CheckPointController[cars.Length];
        for (int i = 0; i < cars.Length; i++)
        {
            carsController[i] = cars[i].GetComponent<CheckPointController>();
        }
    }

    public void BeginGame()  // button listener
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("StartGame", RpcTarget.All);
    }
}


