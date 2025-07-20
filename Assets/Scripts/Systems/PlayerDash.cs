using UnityEngine;
using RobotGame.States;
public class PlayerDash : MonoBehaviour
{
    [SerializeField] private float dashCooldownTime;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashSpeed;

    private float dashCooldownTimer;
    private PlayerController playerController;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        dashCooldownTimer = 0;
    }
    
    void Update()
    {
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
        else
        {
            dashCooldownTimer = 0;
        }
    }

    private void Dash()
    {
        if (dashCooldownTimer <= 0)
        {
            playerController.TransitionState(new PlayerDashing(playerController, dashSpeed, dashTime));
            dashCooldownTimer = dashCooldownTime;
        }
    }
    private void OnEnable()
    {
        InputManager.onDash += Dash;
    }

    private void OnDisable()
    {
        InputManager.onDash -= Dash;
    }
}
