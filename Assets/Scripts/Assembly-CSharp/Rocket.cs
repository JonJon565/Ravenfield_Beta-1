using UnityEngine;

public class Rocket : ExplodingProjectile
{
	protected override void Hit(RaycastHit hitInfo)
	{
		GetComponent<Light>().enabled = false;
		base.Hit(hitInfo);
	}
}
