using UnityEngine;
using System.Collections;

public class Fight_AI : MonoBehaviour
{

    // 定义玩家类型的枚举
    public enum PlayerType
    {
        HUMAN, AI
    };

    public float MAX_HEALTH = 100f;// 最大生命值
    public float health;// 当前生命值

    public string fighterName;
    public bool enable = false;// 是否启用战士

    public PlayerType player; // 玩家类型
    public Fighter enemy;
    public FighterStates currentState = FighterStates.IDLE;
    public FighterStates PunchState = FighterStates.PUNCH;
    public FighterStates KickState = FighterStates.KICK;

    protected Animator animator;
    private Rigidbody myBody;
    private AudioSource audioPlayer;
    private int countF, countB, countDefend = 0;
    private bool isSquat;

    //for AI
    private float random;
    private float randomSetTime;
    private bool isDeathSet = false;

    void Start()
    {
        myBody = GetComponent<Rigidbody>();// 获取刚体组件
        animator = GetComponent<Animator>();// 获取动画组件
        audioPlayer = GetComponent<AudioSource>();// 获取音频播放器组件
        health = MAX_HEALTH;// 设置当前生命值为最大生命值
    }


    // 更新人类玩家输入的方法
    public void UpdateHumanInput()
    {
        if (countF > 0)
        {
            animator.SetBool("Walk", true);
            countF--;
        }
        else
        {
            animator.SetBool("Walk", false);
            countF = 0;
        }

        if (countB > 0)
        {
            animator.SetBool("WalkBack", true);
            countB--;
        }
        else
        {
            animator.SetBool("WalkBack", false);
            countB = 0;
        }

        if (countDefend > 0)
        {
            animator.SetBool("Defend", true);
            countDefend--;
        }
        else
        {
            countDefend = 0;
            animator.SetBool("Defend", false);
        }

        animator.SetBool("Duck", isSquat);


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.LoadLevel("StartScene");
        }

    }

    public void walkForward()
    {
        countF = 20;
    }

    public void walkBackward()
    {
        countB = 20;
    }

    public void punchRight()
    {
        animator.SetTrigger("PunchRight");
    }

    public void punchLeft()
    {
        animator.SetTrigger("PunchLeft");
    }

    public void kickHit()
    {
        animator.SetTrigger("KickRight");
    }

    public void defend()
    {
        countDefend = 20;
    }

    public void jump()
    {
        isSquat = false;
        animator.SetTrigger("Jump");
    }

    public void squat()
    {
        isSquat = true;
    }

    public void standUp()
    {
        isSquat = false;
    }

    public void smashHit()
    {
        animator.SetTrigger("SmashHit");
    }

    public void UpdateAiInput()
    {

        animator.SetBool("defending", defending);
        animator.SetBool("oponent_attacking", enemy.punching || enemy.kicking);
        animator.SetFloat("distanceToOponent", getDistanceToOponent());

        if (Time.time - randomSetTime > 1)
        {
            random = Random.value;
            randomSetTime = Time.time;
        }
        animator.SetFloat("random", random);
    }

    private float getDistanceToOponent()
    {
        return Mathf.Abs(transform.position.x - enemy.transform.position.x);
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Health", healthPercent);

        if (enemy != null)
        {
            animator.SetFloat("EnemyHealth", enemy.healthPercent);
        }
        else
        {
            animator.SetFloat("EnemyHealth", 1);
        }

        if (enable)
        {
            if (player == PlayerType.HUMAN)
            {
                UpdateHumanInput();
                KeyboardInterrupt();
            }
            else
            {
                UpdateAiInput();
            }
        }

        if (health <= 1 && currentState != FighterStates.DEAD)
        {
            if (!isDeathSet)
                animator.SetTrigger("Dead");
            isDeathSet = true;
        }
    }



    public virtual void hurt(float damage)
    {
        if (!invulnerable)
        {
            if (defending)
            {
                damage *= 0.2f;
            }
            if (health >= damage)
            {
                health -= damage;
            }
            else
            {
                health = 0;
            }

            if (health > 0)
            {
                animator.SetTrigger("TakeHit");
            }
        }
    }

    public void playSound(AudioClip sound)
    {
        GameUtils.playSound(sound, audioPlayer);
    }

    public bool invulnerable
    {
        get
        {
            return currentState == FighterStates.TAKE_HIT
                || currentState == FighterStates.TAKE_HIT_DEFEND
                    || currentState == FighterStates.DEAD;
        }
    }

    public bool defending
    {
        get
        {
            return currentState == FighterStates.DEFEND
                || currentState == FighterStates.TAKE_HIT_DEFEND;
        }
    }

    public bool punching
    {
        get
        {
            return currentState == FighterStates.PUNCH;
        }
    }

    public bool kicking
    {
        get
        {
            return currentState == FighterStates.KICK;
        }
    }

    public float healthPercent
    {
        get
        {
            return health / MAX_HEALTH;
        }
    }

    public Rigidbody body
    {
        get
        {
            return this.myBody;
        }
    }
    //Keyboard inputs
    public void KeyboardInterrupt()
    {
        if (Input.GetAxis("Horizontal") > 0.1)
        {
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
        }

        if (Input.GetAxis("Horizontal") < -0.1)
        {
            animator.SetBool("WalkBack", true);
            animator.SetBool("Defend", false);
        }
        else
        {
            animator.SetBool("WalkBack", false);
            animator.SetBool("Defend", false);
        }

        if (Input.GetAxis("Vertical") < -0.1)
        {
            animator.SetBool("Duck", true);
        }
        else
        {
            animator.SetBool("Duck", false);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            animator.SetTrigger("Jump");
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.SetTrigger("PunchRight");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            animator.SetTrigger("KickRight");
        }

    }
}
