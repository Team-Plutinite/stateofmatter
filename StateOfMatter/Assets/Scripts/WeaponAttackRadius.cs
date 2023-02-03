using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class WeaponAttackRadius : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
       //This function will handle collison with various objects.
       //if (other.TryGetComponenet<Enemy>(out Enemy enemy){
       //
       //
       //}
    }
}
