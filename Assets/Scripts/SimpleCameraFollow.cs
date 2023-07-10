using System.Collections;
using UnityEngine;


public class SimpleCameraFollow : MonoBehaviour
{

    [SerializeField]
    private Transform m_followed;

    [SerializeField]
    private float m_speed;

    private float m_initialZ;

    private void Awake()
    {
        m_initialZ = transform.position.z;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_followed != null)
        {
            Vector2 dir = m_followed.position - transform.position;

            transform.position += Time.deltaTime * m_speed * Mathf.Clamp(dir.magnitude, 0, 1) * (Vector3)dir.normalized;
        }
    }
}