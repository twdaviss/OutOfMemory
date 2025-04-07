using RobotGame.States;
using UnityEngine;
public class Grapple : MonoBehaviour
{
    [SerializeField] private GameObject targetUI;
    [SerializeField] private float range;
    [SerializeField] private float startingSpeed;
    [SerializeField] private float targetSpeed;
    [SerializeField] private float grappleCooldownTime;
    [SerializeField] private float grappleAimMaxTime;
    [SerializeField] private float targetUIOffset;
    [SerializeField] private AudioClip grappleStart;
    [SerializeField] private AudioClip grappleEnd;

    private PlayerController playerController;
    private GameObject targetObject;

    private bool isAimingGrapple = false;
    private bool canGrapplePull = false;
    private bool targetValid = false;
    private float grappleCooldownTimer;
    private float grappleAimTimer = 0.0f;
    private bool canGrapple = true;
    private bool canGrappleAudio = true;
    private void Awake()
    {
        grappleCooldownTimer = grappleCooldownTime;
        playerController = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        canGrapplePull = GameManager.enableGrapplePull;
        grappleCooldownTimer += Time.deltaTime;
        if (InputManager.playerControls.Gameplay.Grapple.inProgress)
        {
            canGrapple = true;
        }
        else
        {
            grappleAimTimer = 0.0f;
            canGrapple = false;
        }

        if (grappleAimTimer >= grappleAimMaxTime)
        {
            CancelGrapple();
            return;
        }

        if (canGrapple)
        {
            LookForTarget();
        }
        else
        {
            GrappleTarget();
            canGrappleAudio = true;
        }

        if (isAimingGrapple && targetObject != null)
        {
            targetUI.SetActive(true);
            Vector3 position = targetObject.transform.position;
            position.z -= targetUIOffset;
            targetUI.transform.position = position;
        }
        else
        {
            targetUI.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        GameManager.Instance.SetGrappleCooldownUI(grappleCooldownTimer/ grappleCooldownTime);
    }

    public void GrappleTarget()
    {
        if (grappleCooldownTimer < grappleCooldownTime){return;}
        if(isAimingGrapple == false) { return; }
        
        isAimingGrapple = false;
        if (targetValid)
        {
            if (targetObject.GetComponentInParent<EnemyController>() != null)
            {
                targetObject.gameObject.transform.parent.GetComponentInChildren<EnemyHealth>().SetGrappled();
            }

            grappleCooldownTimer = 0.0f;
            playerController.TransitionState(new PlayerGrappling(playerController, targetObject, startingSpeed, targetSpeed));
        }

        playerController.playerAnimator.SetBool("isBuildingUp", false);

        StartCoroutine(GameManager.Instance.ResetTimeScale());
        GameManager.Instance.DisableHighlight();

        isAimingGrapple = false;
        targetObject = null;
    }

    public void PullGrappleTarget()
    {
        if(canGrapple && canGrapplePull)
        {
            if (grappleCooldownTimer < grappleCooldownTime) { return; }
            if (isAimingGrapple == false) { return; }

            if (targetValid)
            {
                if (targetObject.GetComponentInParent<EnemyController>() != null)
                {
                    targetObject.GetComponentInParent<EnemyController>().Pull(gameObject, startingSpeed, targetSpeed);
                }

                grappleCooldownTimer = 0.0f;
            }

            playerController.playerAnimator.SetBool("isBuildingUp", false);

            StartCoroutine(GameManager.Instance.ResetTimeScale());
            GameManager.Instance.DisableHighlight();
            targetObject = null;
            PlayGrappleEnd();
            playerController.playerAnimator.SetBool("isGrappling", true);
            Invoke(nameof(ReEnableGrapple), 0.1f);
        }
    }

    public bool CheckGrappling()
    {
        return isAimingGrapple;
    }

    public void EnableGrapplePull()
    {
        canGrapplePull = true;
    }

    private void ReEnableGrapple()
    {
        isAimingGrapple = false;
        playerController.playerAnimator.SetBool("isGrappling", false);
    }

    public void CancelGrapple()
    {
        StartCoroutine(GameManager.Instance.ResetTimeScale());
        GameManager.Instance.DisableHighlight();
        isAimingGrapple = false;
        targetObject = null;
        targetUI.SetActive(false);
    }

    public void LookForTarget()
    {
        if (grappleCooldownTimer < grappleCooldownTime)
        {
            return;
        }
        if (canGrappleAudio)
        {
            GetComponent<AudioSource>().pitch = 2;
            GetComponent<AudioSource>().PlayOneShot(grappleStart);
            canGrappleAudio = false;
        }
        GameManager.Instance.SetSlowMoTimeScale();
        GameManager.Instance.EnableHighlight();
        isAimingGrapple = true;
        Vector2 mousePosition = InputManager.mouseScreenPosition;
        Vector3 aimDirection = InputManager.GetAimDirection(playerController.transform.position);

        playerController.playerAnimator.SetBool("isBuildingUp", true);

        if (aimDirection.x < 0) { playerController.playerSprite.flipX = false; }
        else { playerController.playerSprite.flipX = true; }

        int layerMask = LayerMask.GetMask("Enemies") | LayerMask.GetMask("Grapple");

        Ray mouseRay = Camera.main.ScreenPointToRay(InputManager.mouseScreenPosition);
        RaycastHit raycast = new RaycastHit();
        Physics.Raycast(mouseRay, out raycast, 120, layerMask);
        //Debug.DrawLine(mouseRay.origin, mouseRay.origin + mouseRay.direction * 30, Color.red);

        if (raycast.point == null || raycast.point == Vector3.zero)
        {
            targetObject = null;
            targetValid = false;
            return;
        }

        if (Vector3.Distance(playerController.transform.position, raycast.point) < range)
        {
            targetObject = raycast.transform.gameObject;
            targetUI.GetComponent<SpriteRenderer>().color = Color.green;
            targetValid = true;
            return;
        }
        else
        {
            targetObject = raycast.transform.gameObject;
            targetValid = false;
            targetUI.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public void PlayGrappleEnd()
    {
        GetComponent<AudioSource>().pitch = 2;
        GetComponent<AudioSource>().PlayOneShot(grappleEnd);
    }

    private void OnEnable()
    {
        InputManager.onScrapShot += PullGrappleTarget;

    }

    private void OnDisable()
    {
        InputManager.onScrapShot -= PullGrappleTarget;

    }
}
