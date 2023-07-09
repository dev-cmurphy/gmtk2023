using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AttackController : MonoBehaviour
{
    [Min(0.1f)]
    public float HeavyAttackHoldThreshold;

    [Min(0.1f)]
    public float AttackCooldown;

    [SerializeField]
    private Attack m_lightAttack;

    [SerializeField]
    private Attack m_heavyAttack;

    public UnityEvent OnLightAttackLaunch;
    public UnityEvent OnHeavyAttackLaunch;
    public UnityEvent OnAttackPreparing;
    public UnityEvent OnAnyAttackLaunch;
    public UnityEvent OnHeavyAttackReady;
    public UnityEvent OnAttackCancelled;

    [SerializeField]
    private Transform m_attackParent;

    [SerializeField]
    private BossController m_bossController;

    private abstract class AttackState
    {
        protected AttackController m_controller;

        public AttackState(AttackController controller)
        {
            m_controller = controller;
        }

        public  virtual AttackState UpdateFromInput(InputValue inputValue)
        {
            return this;
        }

        public virtual AttackState Update(float dt)
        {
            return this;
        }
    }

    private class NoAttack : AttackState
    {
        private float m_cooldownTimer;
        private bool m_readyForAttack;

        public NoAttack(AttackController attackController) : base(attackController)
        {
            m_readyForAttack = false;
            m_cooldownTimer = 0;
        }

        public override AttackState Update(float dt)
        {
            m_cooldownTimer += dt;

            m_readyForAttack = m_cooldownTimer > m_controller.AttackCooldown;

            return this;
        }

        public override AttackState UpdateFromInput(InputValue inputValue)
        {
            if (inputValue.isPressed && m_readyForAttack)
            {
                m_controller.OnAttackPreparing.Invoke();
                return new AttackPreparing(m_controller, m_controller.HeavyAttackHoldThreshold);
            }

            return this;
        }
    }

    private class AttackPreparing : AttackState
    {
        private float m_attackTimer;
        private float m_preparationTime;

        private bool m_hasPreparedHeavy;
        private bool m_wasCancelled;

        public AttackPreparing(AttackController controller, float prepTime) : base(controller)
        {
            m_preparationTime = prepTime;
            m_attackTimer = 0;
            m_hasPreparedHeavy = false;
            m_wasCancelled = false;
            
        }

        public void Cancel()
        {
            m_wasCancelled = true;
        }

        public override AttackState Update(float dt)
        {
            m_attackTimer += dt;

            if (m_attackTimer > m_preparationTime && !m_hasPreparedHeavy)
            {
                m_hasPreparedHeavy = true;
                m_controller.OnHeavyAttackReady.Invoke();
            }

            if (m_wasCancelled)
            {
                m_controller.OnAttackCancelled.Invoke();
                return new NoAttack(m_controller);
            }

            return this;
        }

        public override AttackState UpdateFromInput(InputValue inputValue)
        {
            if (!inputValue.isPressed)
            {
                m_controller.OnAnyAttackLaunch.Invoke();
                if (m_attackTimer < m_preparationTime)
                {
                    m_controller.OnLightAttackLaunch.Invoke();
                }
                else
                {
                    m_controller.OnHeavyAttackLaunch.Invoke();
                }
                return new NoAttack(m_controller);
            }

            return this;
        }
    }

    private AttackState m_currentState;

    private void Awake()
    {
        m_currentState = new NoAttack(this);
    }

    public void CancelPreparedAttack()
    {
        if (m_currentState is AttackPreparing prep)
        {
            m_bossController.RemoveActiveAttack();
            prep.Cancel();
        }
    }

    public void LaunchHeavyAttack()
    {
        LaunchAttack(m_heavyAttack);
    }

    public void LaunchLightAttack()
    {
        LaunchAttack(m_lightAttack);
    }

    private void LaunchAttack(Attack attack)
    {
        Attack atk = Instantiate(attack, this.transform);
        atk.OnAttackOver.AddListener(OnAttackOver);
        atk.transform.SetParent(m_attackParent);
        atk.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        atk.transform.SetParent(null);
    }

    private void Update()
    {
        m_currentState = m_currentState.Update(Time.deltaTime);
    }

    public void OnAttack(InputValue value)
    {
        m_currentState = m_currentState.UpdateFromInput(value);
    }

    public void OnAttackOver()
    {
        m_bossController.RemoveActiveAttack();
    }
}