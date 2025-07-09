using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject[] enemyPrefabs;
    private Transform bottomEntrance;
    private Transform leftEntrance;
    private Transform rightEntrance;

    private List<GameObject> enemiesActive;
    private void Awake()
    {
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