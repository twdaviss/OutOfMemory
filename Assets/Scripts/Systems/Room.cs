using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] private bool startingRoom;
    [SerializeField] private Transform topEntrance;
    [SerializeField] private Transform bottomEntrance;
    [SerializeField] private Transform leftEntrance;
    [SerializeField] private Transform rightEntrance;
    [SerializeField] private RoomTrigger exitTrigger;

    private List<GameObject> enemiesActive;

    public DoorPosition prevDoorPosition;

    private void Start()
    {
        GameObject player = GameObject.Find("Player");

        if(startingRoom)
        {
            prevDoorPosition = DoorPosition.Right;
            exitTrigger.transform.position = rightEntrance.position;
            return;
        }

        SetPlayerPosition(prevDoorPosition, player);
        int rand = Random.Range(0, 1);
        Vector3 triggerPos = topEntrance.transform.position;
        switch (prevDoorPosition)
        {
            case DoorPosition.Left:
                if (rand == 0)
                {
                    triggerPos = topEntrance.position;
                    exitTrigger.GetComponent<RoomTrigger>().doorDirection = DoorPosition.Top;
                }
                else
                {
                    triggerPos = leftEntrance.position;
                    exitTrigger.GetComponent<RoomTrigger>().doorDirection = DoorPosition.Left;
                }
                break;
            case DoorPosition.Right:
                if (rand == 0)
                {
                    triggerPos = topEntrance.position;
                    exitTrigger.GetComponent<RoomTrigger>().doorDirection = DoorPosition.Top;
                }
                else
                {
                    triggerPos = rightEntrance.position;
                    exitTrigger.GetComponent<RoomTrigger>().doorDirection = DoorPosition.Right;
                }
                break;
                case DoorPosition.Top:
                rand = Random.Range(0, 2);
                if(rand == 0)
                {
                    triggerPos = leftEntrance.position;
                    exitTrigger.GetComponent<RoomTrigger>().doorDirection = DoorPosition.Left;
                }
                else if (rand == 1)
                {
                    triggerPos = rightEntrance.position;
                    exitTrigger.GetComponent<RoomTrigger>().doorDirection = DoorPosition.Right;
                }
                else
                {
                    triggerPos = topEntrance.position;
                    exitTrigger.GetComponent<RoomTrigger>().doorDirection = DoorPosition.Top;
                }
                break;
            default:
                triggerPos = topEntrance.position;
                break;
        }
        exitTrigger.transform.position = triggerPos;
    }

    private void SetPlayerPosition(DoorPosition position, GameObject player)
    {
        switch (position)
        {
            case DoorPosition.Left:
                player.transform.position = rightEntrance.position;
                break;
            case DoorPosition.Right:
                player.transform.position = leftEntrance.position;
                break;
            case DoorPosition.Top:
                player.transform.position = bottomEntrance.position;
                break;
            default:
                player.transform.position = bottomEntrance.position;
                break;
        }
    }

    private void SpawnEnemy()
    {
        List<PathNode> walkableNodes = GameManager.Instance.gridMap.GetWalkableNodes();

        int randNodeIndex = Random.Range(0, walkableNodes.Count);
        int randEnemyIndex = Random.Range(0, enemyPrefabs.Length);

        enemiesActive.Add(Instantiate(enemyPrefabs[randEnemyIndex], walkableNodes[randNodeIndex].GetWorldCoords(), Quaternion.identity));
    }

}