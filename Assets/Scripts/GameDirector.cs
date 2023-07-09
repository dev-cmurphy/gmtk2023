using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    [SerializeField]
    private PlayerInput m_playerInput;

    [SerializeField]
    private Health m_bossHealth, m_enemyHealth;

    [SerializeField]
    private AdventurerAI m_adventurerAI;

    [SerializeField]
    private AudioSource m_audioSource;

    [SerializeField]
    private AudioClip m_introAudioClip, m_victoryAudioClip, m_mainTrackAudioClip, m_defeatAudioClip;  

    [SerializeField]
    private Animator m_animator;

    [SerializeField]
    private TextMeshProUGUI m_resultTextMesh, m_hiddenTextMesh;

    [SerializeField]
    private Button m_playAgainButton;

    [SerializeField]
    [Min(1f)]
    private float m_introDuration;

    private void Start()
    {
        m_bossHealth.OnDeath.AddListener(OnBossDeath);

        m_enemyHealth.OnDeath.AddListener(OnEnemyDeath);

        StartCoroutine(GameIntroCoroutine());
    }

    private void OnBossDeath()
    {
        StartCoroutine(GameOverCoroutine(false));
    }

    private void OnEnemyDeath()
    {
        StartCoroutine(GameOverCoroutine(true));
    }

    private IEnumerator GameIntroCoroutine()
    {
        m_audioSource.PlayOneShot(m_introAudioClip);

        m_animator.SetTrigger("IntroStarts");

        yield return new WaitForSeconds(m_introDuration);


        m_playerInput.enabled = true;
        m_adventurerAI.enabled = true;
    }

    private IEnumerator GameOverCoroutine(bool victory)
    {

        m_playerInput.enabled = false;
        m_adventurerAI.enabled = false;

        if (victory)
        {
            m_resultTextMesh.text = "Victory";
            m_hiddenTextMesh.text = "Defeat...";
            m_audioSource.PlayOneShot(m_victoryAudioClip);
        }
        else
        {
            m_resultTextMesh.text = "Defeat";
            m_hiddenTextMesh.text = "Victory!";
            m_audioSource.PlayOneShot(m_defeatAudioClip);
        }

        m_animator.SetTrigger("GameOver");

        yield return new WaitForSeconds(1f);

        m_playAgainButton.Select();
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("DungeonScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}