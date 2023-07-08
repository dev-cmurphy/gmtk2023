using Pathfinding;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
public class AdventurerAI : MonoBehaviour
{
    private Seeker m_seeker;

    [SerializeField]
    private BossController m_bossController;

    private State m_currentState;

    private void Awake()
    {
        m_seeker = GetComponent<Seeker>();
    }

    private void Start()
    {
        m_currentState = new NeutralState(m_bossController, m_seeker);
    }

    private void Update()
    {
        m_currentState = m_currentState.Update(Time.deltaTime);
    }

    private abstract class State
    {
        protected bool m_hasFoundPath;
        protected BossController m_bossController;
        protected Seeker m_seeker;

        public State(BossController boss, Seeker seeker)
        {
            m_bossController = boss;
            m_seeker = seeker;
        }

        public abstract State Update(float dt);

        protected void TryPathSearch(Vector2 target)
        {
            m_seeker.StartPath(m_seeker.transform.position, target, OnPathComplete);
        }

        protected virtual void OnPathComplete(Path p) { }
    }

    private class NeutralState : State
    {

        public NeutralState(BossController boss, Seeker seeker) : base(boss, seeker)
        {
        }

        private bool ShouldBeAggressive()
        {
            return true;
        }

        public override State Update(float dt)
        {
            if (ShouldBeAggressive())
            {
                return new AggressiveState(m_bossController, m_seeker);
            }
            else
            {

            }
            return this;
        }
    }

    private class AggressiveState : State
    {
        private float m_repathRate = 0.5f;
        private float m_repathTimer;

        public AggressiveState(BossController boss, Seeker seeker) : base(boss, seeker)
        {
            m_seeker.enabled = true;
            m_repathTimer = 0f;
            TryPathSearch(FindTarget());
        }

        private bool CanAttack()
        {
            return true;
        }

        public override State Update(float dt) 
        {
            if (CanAttack())
            {
                m_seeker.enabled = false;
                return new Attacking(m_bossController, m_seeker);
            }

            m_repathTimer += dt;
            if (m_repathTimer > 1f / m_repathRate)
            {
                m_repathTimer = 0f;

                TryPathSearch(FindTarget());
            }

            return this; 
        }


        protected override void OnPathComplete(Path p)
        {
            if (p.error)
            {
                TryPathSearch(FindTarget());
            }
        }

        protected Vector2 FindTarget()
        {
            return m_bossController.transform.position + (Vector3)(2f * Random.insideUnitCircle.normalized);
        }
    }

    private class Attacking : State
    {
        // temp
        private float m_attackTimer;

        public Attacking(BossController boss, Seeker seeker) : base(boss, seeker)
        {
            m_attackTimer = 0f;
        }

        public override State Update(float dt)
        {
            m_attackTimer += dt;
            if (m_attackTimer > 1f)
            {
                return new NeutralState(m_bossController, m_seeker);
            }

            return this;
        }
    }
}