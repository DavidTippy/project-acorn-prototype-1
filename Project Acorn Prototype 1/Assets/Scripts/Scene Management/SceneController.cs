using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameObject playerPrefab;
    //private PlayerManager pm;
    // Start is called before the first frame update
    void Start()
    {
        //pm = GameObject.Find("MasterGameController").GetComponent<PlayerManager>();
        StartCoroutine(LoadingLevel("Scene1", 0, 100));
    }

    //SaveSystem.Load().roomName

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeRooms(string roomName, float xCoord, float yCoord)
    {
        StartCoroutine(LoadingLevel(roomName, xCoord, yCoord));

        //pm.InstantiatePlayer(xCoord, yCoord);

    }

    public IEnumerator LoadingLevel(string roomName, float xCoord, float yCoord)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(roomName);


        while (!asyncLoad.isDone)
        {
            yield return null;
            Debug.Log("room has not yet fully loaded");
        }
        Debug.Log("room has fully loaded");
        Instantiate(playerPrefab, new Vector3(xCoord, yCoord, 0), Quaternion.identity);
    }

    public void SaveGame(string RoomName)
    {

    }
}
