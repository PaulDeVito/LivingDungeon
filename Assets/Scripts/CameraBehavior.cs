using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{

    GameObject player;
    Camera camera;
    private float permanentZ;
    private float screenHeight;
    private float screenWidth;
    private bool showDebug = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        camera = gameObject.GetComponent<Camera>();
        screenHeight = camera.orthographicSize;
        screenWidth = screenHeight * camera.aspect;
        permanentZ = gameObject.transform.position.z;
    }

    void Update()
    {
      TrackPlayer();

      if (Input.GetKeyDown(KeyCode.O) && Input.GetKey(KeyCode.RightShift))
      {
          showDebug = !showDebug;
          if (showDebug)
          {
              camera.cullingMask |= 1 << LayerMask.NameToLayer("DebuggingUI");
          }
          else 
          {
              camera.cullingMask &= ~(1 << LayerMask.NameToLayer("DebuggingUI"));
          }
      }
    }

    void TrackPlayer()
    {
        float x = player.transform.position.x;
        float y = player.transform.position.y;

        Room currentRoom = GameManager.getDungeonManager().getCurrentRoom();
        float boardHeight = currentRoom.getHeight();
        float boardWidth = currentRoom.getWidth();

        if (y < screenHeight) y = screenHeight;
        if (y > boardHeight - screenHeight) y = boardHeight - screenHeight;
        if (screenHeight*2 > boardHeight) y = boardHeight/2f;

        if (x < screenWidth) x = screenWidth;
        if (x > boardWidth - screenWidth) x = boardWidth - screenWidth;
        if (screenWidth*2 > boardWidth) x = boardWidth/2f;

        gameObject.transform.position = new Vector3(x-.5f,y-.5f,permanentZ);
    }

}