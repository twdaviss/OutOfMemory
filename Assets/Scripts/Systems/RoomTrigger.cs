using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField] private GameObject prompt;
    Room room;
    private bool isInRange = false;
    public DoorPosition doorDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        room = GetComponentInParent<Room>();
    }
    private void Update()
    {
        prompt.SetActive(isInRange);
    }
    private void Interact()
    {
        if (isInRange)
        {
            GetComponentInParent<RoomManager>().DeleteCurrentRoom(doorDirection);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
        }
    }
    private void OnEnable()
    {
        InputManager.onInteract += Interact;
    }

    private void OnDestroy()
    {
        InputManager.onInteract -= Interact;
    }
}
