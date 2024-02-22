using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyAnimations
{
    public AnimationClip idle;
    public AnimationClip run;
    public AnimationClip rightAttack;
    public AnimationClip upAttack;
    public AnimationClip downAttack;
}

public class EnemyAnimatorController : MonoBehaviour
{
    public EnemyAnimations enemyAnimations;

    protected Animator animator;
    protected AnimatorOverrideController animatorOverrideController;

    public void Start()
    {
        animator = GetComponent<Animator>();

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        animatorOverrideController["Idle"] = enemyAnimations.idle;
        animatorOverrideController["Run"] = enemyAnimations.run;
        animatorOverrideController["Attack_Right"] = enemyAnimations.rightAttack;
        animatorOverrideController["Attack_Up"] = enemyAnimations.upAttack;
        animatorOverrideController["Attack_Down"] = enemyAnimations.downAttack;
    }
}