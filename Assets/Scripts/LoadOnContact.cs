using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LoadOnContact : MonoBehaviour
{
    [SerializeField]
    private float m_loadDelay = 3f;

    [SerializeField]
    private Animator m_transitionAnimator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerInput>().enabled = false;
            StartCoroutine(LoadNextScene());
        }
    }

    private IEnumerator LoadNextScene()
    {
        m_transitionAnimator.SetTrigger("Transition");

        yield return new WaitForSeconds(m_loadDelay);

        SceneManager.LoadScene("BossScene");
    }
}