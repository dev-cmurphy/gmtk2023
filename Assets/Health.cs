using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField]
    [Min(1)]
    private int m_maxHealth;

    public int MaxHealth { get { return m_maxHealth; } }
    public int CurrentHealth { get; private set; }

    // delta, total
    public UnityEvent<int, int> OnHealthChange;
    public UnityEvent OnDeath;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public void Damage(int damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

        OnHealthChange.Invoke(damage, CurrentHealth);

        if (CurrentHealth == 0)
        {
            OnDeath?.Invoke();
        }
    }
}
