using System.Collections.Generic;
using UnityEngine;

public enum DoorPosition
{
    Left,
    Right,
    Top,
    Bottom,
}

public class RoomManager : MonoBehaviour
{
    [SerializeField] private List<Room> rooms;
    [SerializeField] private Room startingRoom;

    private Room currentRoom;
    private DoorPosition previousDoorPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         currentRoom = Instantiate(startingRoom, transform);
    }

    // Update is called once per frame
    void Update()
    {
        //if(currentRoom == null)
        //{
        //    DeleteCurrentRoom();
        //}
    }
    public void DeleteCurrentRoom(DoorPosition direction)
    {
        previousDoorPosition = direction;
        //rooms.Remove(currentRoom);
        Destroy(currentRoom.gameObject);
        SpawnRandomRoom();
    }


    public void SpawnRandomRoom()
    {
        int randIndex = Random.Range(0, rooms.Count);
        currentRoom = Instantiate(rooms[randIndex], transform);
        currentRoom.prevDoorPosition = previousDoorPosition;
    }
}
