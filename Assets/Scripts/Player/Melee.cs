using RobotGame.States;
using UnityEngine;

public class Melee : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private float damage;
    [SerializeField] private float stun;
    [SerializeField] private float knockBack;
    [SerializeField] private float duration;
    [SerializeField] private AudioClip swoosh;
    [SerializeField] private float comboTime;

    private PlayerController playerController;
    private float meleeCooldownTime = 1.0f;
    private float meleeCooldownTimer;
    private bool isDealingDamage = false;
    public int comboCounter = 0;
    private float comboTimer = 0.0f;

    private float currentKnockback;

    private void Awake()
    {
        meleeCooldownTimer = meleeCooldownTime;
    }
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        if(GameManager.Instance.increaseKnockBack)
        {
            currentKnockback = knockBack * 2.0f; 
        }
        else
        {
            currentKnockback = knockBack;
        }

        if(comboCounter > 0 && comboTimer <= 0.0f)
        {
            comboTimer = 0.0f;
            comboCounter = 0;
            meleeCooldownTimer = 0.0f;
        }
        else
        {
            comboTimer -= Time.deltaTime;
        }
        meleeCooldownTimer += Time.deltaTime;
        if(isDealingDamage)
        {
            int layerMask = LayerMask.GetMask("Enemies");

            Collider2D[] targets = Physics2D.OverlapCircleAll(playerController.transform.position, radius, layerMask);
            foreach (Collider2D target in targets)
            {
                if (target.gameObject.GetComponent<EnemyController>() != null)
                {
                    target.gameObject.GetComponent<EnemyController>().Damage(damage, stun, currentKnockback, (target.transform.position - playerController.transform.position).normalized);
                }
            }
        }
    }


    private void FixedUpdate()
    {
        GameManager.Instance.SetMeleeCooldownUI(meleeCooldownTimer/ meleeCooldownTime);
    }

    public void TryAttack()
    {
        if (playerController.GetCurrentState() == "PlayerGrappling" || playerController.GetCurrentState() == "PlayerMelee")
        {
            return;
        }
        Attack();
    }

    public void Attack()
    {
        if (meleeCooldownTimer >= meleeCooldownTime)
        {
            comboCounter++;
            comboTimer = comboTime;
            switch(comboCounter)
            {
                case 1:
                    playerController.TransitionState(new PlayerMelee(playerController, radius, damage, stun, 0, duration));
                    break;
                case 2:
                    playerController.TransitionState(new PlayerMelee(playerController, radius, damage * 2, stun, 0, duration));
                    break;
                case 3:
                    playerController.TransitionState(new PlayerMelee(playerController, radius, damage * 3, stun, knockBack, duration));
                    break;
            }

            GetComponentInParent<AudioSource>().PlayOneShot(swoosh);
            playerController.playerAnimator.SetBool("isMeleeing", true);

            if(comboCounter >= 3)
            {
                comboCounter = 0;
                meleeCooldownTimer = 0;
                comboTimer = 0;
            }
        }
    }

}
