using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
  private bool debugMode = false;
  private DungeonManager dungeonManager;
  // Start is called before the first frame update
  void Start()
  {
      dungeonManager = GameManager.getDungeonManager();
      gameObject.SetActive(debugMode);

  }

  // Update is called once per frame
  void Update()
  {
      if (Input.GetKeyDown(KeyCode.D) && Input.GetKey(KeyCode.RightShift))
      {
          debugMode = !debugMode;
          gameObject.SetActive(debugMode);
      }
  }
}
