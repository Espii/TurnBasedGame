using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour {
    public Entity entity;
    public static bool PlayAnimation(Animator animator, string animation)
    {
        if (animator.runtimeAnimatorController == null || string.IsNullOrEmpty(animation))
        {
            return false;
        }
        animator.Play(animation);
        return true;
    }
	public void AttackTarget()
    {
        if (entity.projectile != null)
        {
            GameObject go = Instantiate(entity.projectile, entity.GetProjectileSpawnPoint(), Quaternion.identity);
            Projectile projectile = go.GetComponent<Projectile>();
            projectile.Owner = entity;
            projectile.Target = entity.Target;
        }
        else
        {
            entity.Target.ChangeHealthByAmount(-entity.Attack);
        }
    }
}
