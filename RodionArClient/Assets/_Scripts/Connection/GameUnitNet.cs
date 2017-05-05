using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//юнит, синхронизированный по сети
public class GameUnitNet : GameUnit {

    [Header("Net sync")]
    public  bool online = false;

	// Use this for initialization
	void Start () {
        InitUnit();

        //события подключения или отключения
        if (PhotonServer.Instance != null)
        {

            //подключение / отключение
            PhotonServer.Instance.onConnect += delegate
            {
                online = true;
                if (healthText != null)
                {
                    healthText.color = new Color(0f, 1f, 0f, 1f);
                }
            };

            PhotonServer.Instance.onDisconnect += delegate
            {
                online = false;
                if (healthText != null)
                {
                    healthText.color = new Color(1f, 0f, 0f, 1f);
                }
            };

            //атака
            PhotonServer.Instance.onDamage += delegate(string _characterName)
            {
                if (_characterName == CharacterName)
                {
                    SetDamageNet(10f);
                }
            };

            //обновить жизни
            PhotonServer.Instance.onUpdateHealth += delegate (string _characterName, float _health)
            {
                if (_characterName == CharacterName)
                {
                    health = _health;
                }
            };

            //смерть
            PhotonServer.Instance.onDeath += delegate (string _characterName)
            {
                if (_characterName == CharacterName)
                {
                    base.OnDeath();
                }
            };

            //воскрешение
            PhotonServer.Instance.onRespawn += delegate (string _characterName)
            {
                if (_characterName == CharacterName)
                {
                    RespawnNet();
                }
            };
            
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    //нанести удар
    public override void HitTarget()
    {
        if (online == false)
        {
            base.HitTarget();
        }
        else
        {
            PhotonServer.Instance.SetHit(enemyTarget.name);
        }
    }



    //перерождение
    public override void Respawn()
    {
        if (online == false)
        {
            base.Respawn();
        }
    }


    public void RespawnNet()
    {
        base.Respawn();
    }

    //получение урона
    public override void SetDamage(float _damage)
    {
        if (online == false)
        {
            base.SetDamage(_damage);
        }
    }


    public void SetDamageNet(float _damage)
    {
        base.SetDamage(_damage);
    }
    


}
