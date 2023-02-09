using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int health = 10;

    public int Health { get => health; set => health = value; } 
    // Start is called before the first frame updat

    // Update is called once per frame
 
    public void TakeDamage(int damage)
    {
        Health -= damage;
       

        if(health <= 0)
        {
            Health = 0;

        }
    }

    
}
