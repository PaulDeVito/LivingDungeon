using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;   

public class DebugUIManager : MonoBehaviour
{
    private DungeonManager dungeonManager;
    private Transform floorMap;
    private Canvas debugCanvas;

    public GameObject roomPanel;
    public GameObject doorPanelEast;
    public GameObject doorPanelWest;
    public GameObject doorPanelNorth;
    public GameObject doorPanelSouth;
    public GameObject playerPanel;
    public static float MAP_SCALE = 500f;

    private float mapWidth;
    private float mapHeight;

    private float dWidth;
    private float dHeight;
    private Vector3 scaleVector;
    private GameObject[,] panelMap;


    void Awake()
    {
        debugCanvas = gameObject.GetComponent<Canvas>();
        debugCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        dungeonManager = GameManager.getDungeonManager();
        // boardManager = DungeonManager.boardManager; 
        floorMap = gameObject.transform.Find("Map");
        InitializeMap();
    }

    void OnEnable()
    {
        EventManager.AddBuildRoomListener(AddRoomToMap);
        EventManager.AddEnterRoomListener(UpdatePlayerOnMap);
    }

    void OnDisable()
    {
        EventManager.RemoveBuildRoomListener(AddRoomToMap);
        EventManager.RemoveEnterRoomListener(UpdatePlayerOnMap);
    }

    void InitializeMap()
    {
        dWidth = (float) DungeonManager.mapWidth;
        dHeight = (float) DungeonManager.mapHeight;
        scaleVector = new Vector3(1f / dWidth, 1f / dHeight, 1f);

        float ratio = dWidth / dHeight;
        mapWidth = MAP_SCALE * ratio;
        mapHeight = MAP_SCALE / ratio;
        if (mapHeight >= mapWidth)
        {
            mapHeight = MAP_SCALE;
        }
        else
        {
            mapWidth = MAP_SCALE;
        }
        
        floorMap.GetComponent<RectTransform>().sizeDelta = new Vector2(mapWidth, mapHeight);
        panelMap = new GameObject[(int)dWidth, (int)dHeight];

        playerPanel = Instantiate(playerPanel, floorMap);
        playerPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(mapWidth, mapHeight);
        playerPanel.GetComponent<RectTransform>().localPosition = new Vector3(-1f, -1f, 0f);
        playerPanel.GetComponent<RectTransform>().localScale = 0.9f * scaleVector;

    }

    Vector3 CalculatePanelPosition(Room room)
    {
        float mapScaleX = (float) room.getXCoordinate() / dWidth + (scaleVector.x / 2f);
        float mapScaleY = (float) room.getYCoordinate() / dHeight + (scaleVector.y / 2f);
        return new Vector3(mapScaleX * mapWidth, mapScaleY * mapHeight, 0f);
    }

    void AddRoomToMap(Room room)
    {
        GameObject thisRoomPanel = Instantiate(roomPanel, floorMap);
        thisRoomPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(mapWidth, mapHeight);
        thisRoomPanel.GetComponent<RectTransform>().localPosition = CalculatePanelPosition(room);
        thisRoomPanel.GetComponent<RectTransform>().localScale = 0.9f * scaleVector;
        panelMap[room.getXCoordinate(), room.getYCoordinate()] = thisRoomPanel;
        AddDoorsToRoom(room, thisRoomPanel);
    }

    void AddDoorsToRoom(Room room, GameObject panel)
    {
        foreach (Direction direction in Enum.GetValues(typeof(Direction)))
        {
            if (room.hasDoorAtDirection(direction))
            {
                GameObject door;
                switch(direction)
                {
                    case Direction.East:
                        door = Instantiate(doorPanelEast, panel.transform);
                        break;
                    case Direction.West:
                        door = Instantiate(doorPanelWest, panel.transform);
                        break;
                    case Direction.North:
                        door = Instantiate(doorPanelNorth, panel.transform);
                        break;
                    case Direction.South:
                        door = Instantiate(doorPanelSouth, panel.transform);
                        break;
                    default:
                        door = Instantiate(doorPanelSouth, panel.transform);
                        break;
                }

                door.GetComponent<RectTransform>().sizeDelta = new Vector3(50f, 50f, 0f);
            }
        }
    }

    void UpdatePlayerOnMap(Room room)
    {
        playerPanel.GetComponent<RectTransform>().localPosition = CalculatePanelPosition(room);
        playerPanel.transform.SetAsLastSibling();
    }
}
