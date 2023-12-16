using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerAttacks : MonoBehaviour
{
    [SerializeField] private NormalAttackCollider attackCollider_normal = null;
    [SerializeField] private UrbanAttackCollider attackCollider_urban = null;

    [SerializeField] private float normalAttackColDuration = 0.2f;
    [SerializeField] private float urbanAttackColDuration = 0.2f;

    [SerializeField] private float kickPushForce = 1.0f;

    private ThirdPersonController _controller;
    private Animator _animator;
    private CharacterController _charController;
    private StarterAssetsInputs _input;
    private GameObject _mainCamera;

    private bool attacking = false;

    void Start()
    {
        _controller = GetComponent<ThirdPersonController>();

        _animator = GetComponentInChildren<Animator>();
        _charController = GetComponent<CharacterController>();
        _input = GetComponent<StarterAssetsInputs>();
    }

    
    public void UpdateAttacks()
    {
        if (attacking)
            return;

        if (_input.attack)
        {
            if (_controller.Mode == PlayerModes.NORMAL)
            {
                attackCollider_normal.gameObject.SetActive(true);
                attackCollider_normal.InitAttack();
                attacking = true;
                _animator.Play("attack_normal");
                AudioManager.instance.PlayClip(audios.PLAYER_SPIN, transform.position);
                StartCoroutine(NormalAttack());
            }
            else if (_controller.Mode == PlayerModes.URBAN)
            {
                attackCollider_urban.gameObject.SetActive(true);
                attackCollider_urban.InitAttack(kickPushForce);
                attacking = true;
                _animator.Play("attack_urban");
                AudioManager.instance.PlayClip(audios.PLAYER_SPIN, transform.position);
                StartCoroutine(UrbanAttack());
            }
            _input.attack = false;
        }
    }

    IEnumerator NormalAttack()
    {
        yield return new WaitForSeconds(normalAttackColDuration);
        attacking = false;
        attackCollider_normal.StopAttack();
        attackCollider_normal.gameObject.SetActive(false);
    }
    IEnumerator UrbanAttack()
    {
        yield return new WaitForSeconds(urbanAttackColDuration);
        attacking = false;
        attackCollider_urban.StopAttack();
        attackCollider_urban.gameObject.SetActive(false);
    }
    IEnumerator ZoneAttack()
    {
        yield return null;
    }


}