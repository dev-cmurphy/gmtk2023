using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(Seeker))]
public class AdventurerAI : MonoBehaviour
{
    private Seeker m_seeker;

    [SerializeField]
    private BossController m_bossController;

    [SerializeField]
    private Health m_adventurerHealth;

    public AdventurerAttackController AttackController;

    private State m_currentState;

    [SerializeField]
    private Animator m_adventurerAnimator;

    private Rigidbody2D m_rigidbody;

    private void Awake()
    {
        m_seeker = GetComponent<Seeker>();
        m_adventurerHealth = GetComponent<Health>();
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        m_currentState = new NeutralState(m_bossController, m_seeker, m_adventurerHealth, this);
    }

    private Vector2 lastPos;
    private Vector2 speed;

    private void Update()
    {
        speed = (Vector2)transform.position - lastPos;

        m_adventurerAnimator.SetFloat("Velocity", speed.magnitude / Time.deltaTime);
        m_currentState = m_currentState.Update(Time.deltaTime);
        lastPos = transform.position;
    }

    private abstract class State
    {
        protected bool m_hasFoundPath;
        protected BossController m_bossController;
        protected Seeker m_seeker;
        protected Health m_health;
        protected AdventurerAI m_controller;

        public State(BossController boss, Seeker seeker, Health health, AdventurerAI controller)
        {
            m_bossController = boss;
            m_seeker = seeker;
            m_health = health;
            m_controller = controller;
            m_controller = controller;
        }

        public abstract State Update(float dt);
    }

    private class NeutralState : State
    {
        private static float m_desperation;
        private float m_reflexionTimer;

        public NeutralState(BossController boss, Seeker seeker, Health health, AdventurerAI adventurerAI) : base(boss, seeker, health, adventurerAI)
        {
        }

        private bool ShouldBeAggressive()
        {
            return (m_health.CurrentHealth > (m_health.MaxHealth * 0.5f)) || m_desperation > Random.Range(0.1f, 1f);
        }

        public override State Update(float dt)
        {
            m_desperation += dt * 1f / 100f;
            m_reflexionTimer += dt;

            if (m_reflexionTimer > 1f)
            {
                if (ShouldBeAggressive())
                {
                    return new AggressiveState(m_bossController, m_seeker, m_health, m_controller);
                }
                else
                {
                    return new EvasiveState(m_bossController, m_seeker, m_health, m_controller);
                }
            }

            return this;
        }
    }

    private abstract class PathfindingState : State
    {
        private float m_repathRate = 2f;
        private float m_repathTimer;

        public PathfindingState(BossController boss, Seeker seeker, Health health, AdventurerAI adventurerAI) : base(boss, seeker, health, adventurerAI)
        {
            m_seeker.enabled = true;
            m_repathTimer = 0f;
            TryPathSearch(FindTarget());
        }

        protected void PathChecking(float dt, bool forceRepath = false)
        {
            m_repathTimer += dt;
            if (m_repathTimer > 1f / m_repathRate || forceRepath)
            {
                m_repathTimer = 0f;

                TryPathSearch(FindTarget());
            }
        }


        protected bool IsInMeleeRange()
        {
            return Vector2.Distance(m_bossController.transform.position, m_seeker.transform.position) < 2.5f;
        }

        protected void TryPathSearch(Vector2 target)
        {
            NNInfo nearest = AstarPath.active.GetNearest((Vector3)target, NNConstraint.Default);

            target = nearest.position;

            m_seeker.StartPath(m_seeker.transform.position, target, OnPathComplete);
        }

        protected void OnPathComplete(Path p)
        {
            if (p.error)
            {
                TryPathSearch(FindTarget());
            }
        }

        protected virtual Vector2 FindTarget()
        {
            return m_bossController.transform.position + (Vector3)(2.5f * Random.insideUnitCircle.normalized);
        }
    }

    private class AggressiveState : PathfindingState
    {
        public AggressiveState(BossController boss, Seeker seeker, Health health, AdventurerAI adventurerAI) : base(boss, seeker, health, adventurerAI)
        {
            TryPathSearch(FindTarget());
        }

        public override State Update(float dt) 
        {
            if (IsInMeleeRange())
            {
                m_seeker.enabled = false;
                return new Attacking(m_bossController, m_seeker, m_health, m_controller);
            }
            else
            {
                PathChecking(dt);
            }

            return this; 
        }
    }

    private class EvasiveState : PathfindingState
    {
        private float m_safeTimer;

        public EvasiveState(BossController boss, Seeker seeker, Health health, AdventurerAI adventurer) : base(boss, seeker, health, adventurer)
        {
            m_safeTimer = 0;
            TryPathSearch(FindTarget());
        }

        public override State Update(float dt)
        {
            if (IsInMeleeRange())
            {
                m_safeTimer = 0;
                PathChecking(dt);
            }
            else
            {
                m_safeTimer += dt;
            }

            float safeTime = 3f;
            if (m_safeTimer > safeTime)
            {
                return new NeutralState(m_bossController, m_seeker, m_health, m_controller);
            }


            return this;
        }

        protected override Vector2 FindTarget()
        {
            Vector2 bossDir = m_bossController.transform.position - m_seeker.transform.position;
            bossDir.Normalize();
            Vector2 perp = new(-bossDir.y, bossDir.x);

            float dir = Random.Range(0, 1f) > 0.5f ? 1 : -1;

            perp *= dir;

            float dist = 8f;
            perp *= dist;

            Vector2 targetSafePoint = (0.5f * dist * -bossDir) +  perp + (0.5f * dist * Random.insideUnitCircle);

            return targetSafePoint;
        }
    }



    private class Attacking : State
    {
        private float m_attackTimer;

        public Attacking(BossController boss, Seeker seeker, Health health, AdventurerAI adventurer) : base(boss, seeker, health, adventurer)
        {
            m_attackTimer = 0;
            m_seeker.enabled = false;

            adventurer.AttackController.Attack(m_bossController.transform.position);
        }

        public override State Update(float dt)
        {
            m_attackTimer += dt;

            if (m_attackTimer > 0.6f)
            {
                return new NeutralState(m_bossController, m_seeker, m_health, m_controller);
            }

            return this;
        }
    }


    public void Kill()
    {
        Destroy(this.gameObject);
    }
}