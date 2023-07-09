using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AdventurerController : MonoBehaviour
{
    [SerializeField]
    private float m_speed;

    [SerializeField]
    private bool m_canMove;
    private bool m_lookLocked;

    [SerializeField]
    private Transform m_attackParent;

    private Rigidbody2D m_rigidbody;

    private Vector2 m_lastMoveInput;

    private void Awake()
    {

        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!m_canMove)
        {
            return;
        }

        Vector2 oldPos = m_rigidbody.position;

        Vector2 delta = m_speed * Time.fixedDeltaTime * m_lastMoveInput;

        m_rigidbody.MovePosition(oldPos + delta);
    }

    public void OnMove(InputValue value)
    {
        m_lastMoveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        if (m_lookLocked)
        {
            return;
        }

        if (!m_canMove)
        {
            m_lookLocked = true;
        }

        Vector2 val = value.Get<Vector2>();
        if (val.sqrMagnitude > 0)
        {
            m_attackParent.localPosition = val.normalized * 1.4f;
        }
    }
}