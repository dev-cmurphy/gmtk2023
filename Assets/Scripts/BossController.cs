using Assets.Scripts.GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class BossController : MonoBehaviour
{
    [SerializeField]
    private BossMovementSettings m_moveSettings;

    private Rigidbody2D m_rigidbody;

    private Vector2 m_lastMoveInput;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_lastMoveInput = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector2 oldPos = m_rigidbody.position;

        Vector2 delta = m_moveSettings.Speed * Time.fixedDeltaTime * m_lastMoveInput.normalized;

        m_rigidbody.MovePosition(oldPos + delta);
    }

    public void OnMove(InputValue value)
    {
        m_lastMoveInput = value.Get<Vector2>();
    }
}
