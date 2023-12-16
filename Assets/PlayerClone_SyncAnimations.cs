using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClone_SyncAnimations : MonoBehaviour
{
    Animator animator;
    Animator animSync;

    private void Awake()
    {
        animSync = GameObject.Find("Player").GetComponentInChildren<Animator>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        animator.Play(0, -1, animSync.GetCurrentAnimatorStateInfo(0).normalizedTime);


        for (int i = 0; i < animator.layerCount; i++)
        {
            animator.Play(animSync.GetCurrentAnimatorStateInfo(i).fullPathHash, i, animSync.GetCurrentAnimatorStateInfo(i).normalizedTime);
            animator.SetLayerWeight(i, animSync.GetLayerWeight(i));
        }
    }
}
