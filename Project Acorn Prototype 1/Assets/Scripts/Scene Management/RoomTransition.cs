using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTransition : MonoBehaviour
{
    public string roomName;
    public float xCoordinate;
    public float yCoordinate;

    private SceneController sc;
    // Start is called before the first frame update
    void Start()
    {
        sc = GameObject.Find("MasterGameController").GetComponent<SceneController>();

        
    }



    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {

        Debug.Log("Contact");

        if (other.CompareTag("Player"))
        {

            
            //other.transform.position = new Vector2(xCoordinate, yCoordinate);
            sc.ChangeRooms(roomName, xCoordinate, yCoordinate);
        }

    }
}
