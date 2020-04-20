using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public delegate void RoomEventHandler(Room room);
    public delegate void SizeEventHandler(int width, int height);
    public event RoomEventHandler BuildRoomEvent;
    public event RoomEventHandler EnterRoomEvent;
    public event SizeEventHandler InitMapSizeEvent;
    private static EventManager eventManager;
    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType (typeof (EventManager)) as EventManager;
                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in the scene");
                }
            }

            return eventManager;
        }
    }

    // ------------- BUILD ROOM EVENT -------------------
    public static void AddBuildRoomListener(RoomEventHandler roomEventHandler)
    {
        instance.BuildRoomEvent += roomEventHandler;
    }

    public static void RemoveBuildRoomListener(RoomEventHandler roomEventHandler)
    {
        if (eventManager == null) return;
        instance.BuildRoomEvent -= roomEventHandler;
    }

    public static void TriggerBuildRoomEvent(Room room)
    {
        instance.BuildRoomEvent(room);
    }
    
    // ------------- ENTER ROOM EVENT -------------------
    public static void AddEnterRoomListener(RoomEventHandler roomEventHandler)
    {
        instance.EnterRoomEvent += roomEventHandler;
    }

    public static void RemoveEnterRoomListener(RoomEventHandler roomEventHandler)
    {
        if (eventManager == null) return;
        instance.EnterRoomEvent -= roomEventHandler;
    }

    public static void TriggerEnterRoomEvent(Room room)
    {
        instance.EnterRoomEvent(room);
    }

    // ------------- INIT MAP EVENT -------------------
    public static void AddInitMapSizeListener(SizeEventHandler sizeEventHandler)
    {
        instance.InitMapSizeEvent += sizeEventHandler;
    }

    public static void RemoveInitMapSizeListener(SizeEventHandler sizeEventHandler)
    {
        if (eventManager == null) return;
        instance.InitMapSizeEvent -= sizeEventHandler;
    }

    public static void TriggerInitMapSizeEvent(int width, int height)
    {
        instance.InitMapSizeEvent(width, height);
    }

}
