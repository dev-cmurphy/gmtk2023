using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AdventurerController : MonoBehaviour
{
    [SerializeField]
    private float m_speed;

    [SerializeField]
    private Transform m_attackParent;

    private Rigidbody2D m_rigidbody;

    private Vector2 m_lastMoveInput;


    [SerializeField]
    private Animator m_gameAnimator;

    [SerializeField]
    private Animator m_adventurerAnimator;

    [SerializeField]
    private GameObject m_deathSpawn;

    private void Awake()
    {

        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    private Vector2 lastPos;
    private Vector2 speed;
    private void FixedUpdate()
    {
        speed = (Vector2)transform.position - lastPos;
        m_adventurerAnimator.SetFloat("Velocity", speed.magnitude / Time.deltaTime);

        Vector2 oldPos = m_rigidbody.position;

        Vector2 delta = m_speed * Time.fixedDeltaTime * m_lastMoveInput;

        m_rigidbody.MovePosition(oldPos + delta);

        lastPos = transform.position;
    }

    public void OnMove(InputValue value)
    {
        m_lastMoveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        Vector2 val = value.Get<Vector2>();
        if (val.sqrMagnitude > 0)
        {
            m_attackParent.localPosition = val.normalized * 1.4f;
        }
    }
    public void Kill()
    {
        var spawn = Instantiate(m_deathSpawn, transform);
        spawn.transform.localPosition = Vector3.zero;
        spawn.transform.SetParent(null);
        Destroy(this.gameObject);
    }

    public void RestartLevel()
    {
        m_gameAnimator.SetTrigger("Restart");

        StartCoroutine(Restart());
    }

    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("Menu");
    }
}