using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameUnit : GameBody
{

    [Header("Unit")]
    public Animator animator;
    public string CharacterName;
    public GameUnit enemyTarget;
    public TextMesh healthText;

    // Use this for initialization
    void Start()
    {
        InitUnit();
    }



    public void InitUnit()
    {
        if (gameObject.GetComponent<Animator>())
        {
            animator = gameObject.GetComponent<Animator>();
        }
        FindTarget();
    }



    // Update is called once per frame
    void Update()
    {

    }


    //найти любого врага
    public void FindTarget()
    {
        if (enemyTarget == null)
        {
            GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
            for (int i = 0; i < units.Length; i++)
            {
                if (gameObject != units[i])
                {
                    if (units[i].GetComponent<GameBody>() != null)
                    {
                        enemyTarget = units[i].GetComponent<GameUnit>();
                        return;
                    }
                }
            }
        }
    }


    public void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = health.ToString();
        }
    }



    //Клик - удар по врагу
    void OnMouseDown()
    {
       HitTarget();
    }




    public virtual void HitTarget()
    {
        if (enemyTarget == null) return;
        enemyTarget.SetDamage(damage);

        PlayAnimation("Hit");
    }



    //перерождение
    public override void Respawn()
    {
        base.Respawn();
    }


    //получение урона и анимация
    public override void SetDamage(float _damage)
    {
        base.SetDamage(_damage);

        if (bodyState == BodyState.Alive)
        {
            PlayAnimation("Damage");
        }
        UpdateHealthText();
    }


    //изменение состояние "тела"
    public override void ChangeState(BodyState _bodyState)
    {
        base.ChangeState(_bodyState);

        PlayAnimation(_bodyState.ToString());
    }


    //воспроизведение анимаций
    public void PlayAnimation(string _animationName)
    {
        if (animator == null) return;

        switch (_animationName)
        {
            case "Alive":
                animator.SetBool("Alive", true);
                break;
            case "Death":
                animator.SetBool("Alive", false);
                break;
            case "Hit":
                animator.SetBool("Hit", true);
                StartCoroutine( OutAnimation(0.05f) );
                break;
            case "Damage":
                animator.SetBool("Damage", true);
                StartCoroutine( OutAnimation(0.05f) );
                break;
        }
    }


    IEnumerator OutAnimation(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool("Damage", false);
        animator.SetBool("Hit", false);
    }

}
