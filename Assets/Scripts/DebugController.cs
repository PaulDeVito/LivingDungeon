using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
  private DungeonManager dungeonManager;
  // Start is called before the first frame update
  void Start()
  {
      dungeonManager = GameManager.getDungeonManager();
  }

  // Update is called once per frame
  void Update()
  {
      
  }
}
