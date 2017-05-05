using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//базовое тело юнита
public class GameBody : MonoBehaviour {

	public enum BodyState
	{
		Alive, Death
	}
    
    [Header ("Body")]
	public float health = 100f;
	public float maxHealth = 100f;
	public float damage = 10f;
    public BodyState bodyState = BodyState.Alive;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public virtual void SetDamage(float _damage)
	{
		health -= _damage;

		if (health <= 0f)
		{
			health = 0f;
			if (bodyState != BodyState.Death)
			{
				OnDeath();
			}
		}

		Debug.Log("On Get Damage");
	}

	public virtual void OnDeath()
	{
		ChangeState(BodyState.Death);
	}
    

	public virtual void Respawn()
	{
		if (bodyState == BodyState.Death)
		{
			ChangeState(BodyState.Alive);
			health = maxHealth;
		}
	}

	public virtual void ChangeState(BodyState _bodyState)
	{
		bodyState = _bodyState;
	}
}
