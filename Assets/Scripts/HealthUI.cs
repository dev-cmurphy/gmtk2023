using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField]
    private Health m_health;

    [SerializeField]
    private Slider m_bar;

    private void Awake()
    {
        m_bar.maxValue = m_health.MaxHealth;
        m_health.OnHealthChange.AddListener((int delta, int total) => { m_bar.value = total; });
    }

    private void Start()
    {
        m_bar.value = m_health.CurrentHealth;
    }
}