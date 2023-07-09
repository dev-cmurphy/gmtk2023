using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.UI;

/// <summary>
/// Spawned by player
/// Damages X units. Represents the active attack.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Attack : MonoBehaviour
{
    [SerializeField]
    private float m_activationDelay = 0;

    private float m_delayTimer;

    [SerializeField]
    [Min(0f)]
    private int m_damage;

    [SerializeField]
    [Min(0)]
    private float m_lifetime;

    [SerializeField]
    private LayerMask m_hitLayers;

    private Collider2D m_collider;

    public UnityEvent OnAttackOver;

    [SerializeField]
    private GameObject m_visual;

    private void Awake()
    {
        m_collider = GetComponent<Collider2D>();
        m_collider.isTrigger = true;
        m_delayTimer = 0;
        m_visual.SetActive(false);
        m_collider.enabled = false;
    }

    private void Start()
    {
        StartCoroutine(DestroyAfterTime());
    }

    private void Update()
    {
        m_delayTimer += Time.deltaTime;

        if (m_delayTimer >= m_activationDelay && !m_visual.activeSelf)
        {
            m_collider.enabled = true;
            m_visual.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if ((m_hitLayers.value & (1 << collision.gameObject.layer)) != 0)
            {
                Debug.Log($"Hit! {gameObject.name} on {collision.gameObject}");
                if (collision.gameObject.TryGetComponent<Health>(out Health health))
                {
                    health.Damage(m_damage);
                }
                m_collider.enabled = false;
            }
        }
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(m_lifetime);
        OnAttackOver.Invoke();
        OnAttackOver.RemoveAllListeners();
        Destroy(this.gameObject);
    }
}