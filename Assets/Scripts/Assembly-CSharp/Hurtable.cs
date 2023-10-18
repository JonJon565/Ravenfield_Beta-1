using UnityEngine;

public class Hurtable : MonoBehaviour
{
	public int team;

	public virtual void Damage(float healthDamage, float balanceDamage, Vector3 point, Vector3 impactForce)
	{
	}
}
