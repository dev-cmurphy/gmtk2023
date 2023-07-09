using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AdventurerAttackController : MonoBehaviour
{
    [SerializeField]
    private Attack m_lightAttack;

    [SerializeField]
    private Transform m_attackParent;

    private float m_attackCooldown;

    [SerializeField]
    private Animator m_adventurerAnimator;

    [SerializeField]
    private AudioClip m_attackClip;

    [SerializeField]
    private AudioSource m_audioSource;

    private void Update()
    {
        m_attackCooldown += Time.deltaTime;
    }

    public void PlayAttackClip()
    {
        m_audioSource.PlayOneShot(m_attackClip);
    }

    public void PlayRandomFootStep()
    {
        // todo
    }

    public void OnAttack(InputValue value)
    {
        if (m_attackCooldown > 0.5f)
        {
            m_attackCooldown = 0;
            StartCoroutine(AttackNextFrame());
        }
    }

    private IEnumerator AttackNextFrame()
    {
        yield return new WaitForEndOfFrame();
        Attack(m_attackParent.position);
    }

    public void Attack(Vector2 target)
    {
        m_adventurerAnimator.SetTrigger("Attack");
        Attack atk = Instantiate(m_lightAttack, this.transform);

        Vector2 dir = target - (Vector2)transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x);
        var rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);

        atk.transform.SetPositionAndRotation(transform.position, rotation);

        //atk.OnAttackOver.AddListener(OnAttackOver);
        //atk.transform.SetParent(m_attackParent);
        //atk.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        //atk.transform.SetParent(null);
    }
}