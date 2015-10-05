﻿using UnityEngine;
using System.Collections;

public class MeleeAttackControl : MonoBehaviour {

	private Rigidbody rb;
	public  float meleeTimeout;
	public float attackSpeed;

	void Start(){
		Destroy(gameObject, meleeTimeout);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Door") 
		    || other.gameObject.CompareTag ("Wall")
		    || other.gameObject.CompareTag ("Enemy")
		    || other.gameObject.CompareTag ("Boss")) {
			Destroy(gameObject);

			HealthControl.dealDamageToEnemy(other.gameObject);
		} 
	}
}