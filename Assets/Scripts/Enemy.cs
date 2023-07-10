using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int m_damage = 3;

    [SerializeField]
    private Transform m_playerTarget;

    private bool m_canAttack;

    private Seeker m_seeker;

    private float m_cooldown;

    [SerializeField]
    private GameObject m_deathSpawn;

    private void Awake()
    {
        m_canAttack = true;
        m_cooldown = 0;
        m_seeker = GetComponent<Seeker>();
    }

    // Update is called once per frame
    void Update()
    {
        m_cooldown += Time.deltaTime;

        if (Time.frameCount % 100 == 0 && m_canAttack)
        {
            m_seeker.StartPath(transform.position, m_playerTarget.position);
        }

        if (m_cooldown > 5f)
        {
            m_canAttack = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Health>(out Health h) && collision.gameObject.CompareTag("Player"))
        {
            m_cooldown = 0;
            m_canAttack = false;
            m_seeker.StartPath(transform.position, (Vector2)m_playerTarget.position + (3f * Random.insideUnitCircle.normalized));
            h.Damage(m_damage);
        }
    }

    public void Kill()
    {
        var spawn = Instantiate(m_deathSpawn, transform);
        spawn.transform.localPosition = Vector3.zero;
        spawn.transform.SetParent(null);
        Destroy(this.gameObject);
    }
}
