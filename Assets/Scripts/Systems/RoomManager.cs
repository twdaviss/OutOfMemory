using System.Collections.Generic;
using UnityEngine;

public enum DoorPosition
{
    Left,
    Right,
    Top,
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
        //int randIndex = Random.Range(0, rooms.Count);
        //currentRoom = rooms[randIndex];
        //rooms.RemoveAt(randIndex);

        currentRoom = Instantiate(startingRoom);
    }

    // Update is called once per frame
    void Update()
    {
        //    if (!currentRoom.activeSelf)
        //    {
        //        currentRoom.SetActive(true);
        //        currentRoom.
        //    }
        //
    }
}
