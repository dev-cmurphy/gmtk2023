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
    [Min(0f)]
    private int m_damage;

    [SerializeField]
    [Min(0)]
    private float m_lifetime;

    [SerializeField]
    private LayerMask m_hitLayers;

    private Collider2D m_collider;

    public UnityEvent OnAttackOver;

    private void Awake()
    {
        m_collider = GetComponent<Collider2D>();
        m_collider.isTrigger = true;
    }

    private void Start()
    {
        StartCoroutine(DestroyAfterTime());
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