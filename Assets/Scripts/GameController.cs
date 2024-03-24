using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Transform trainGroup;
    public GameObject[] trains = new GameObject[11];
    [SerializeField] Sprite[] nextShows = new Sprite[11];
    [SerializeField] Image nextShow;
    [SerializeField] Text scoreText;
    [SerializeField] Text bestScoreText;
    [SerializeField] GameObject gameOverText;
    [SerializeField] AudioSource mergeSound;
    [SerializeField] AudioSource dropSound;
    [SerializeField] AudioSource gameOverSound;
    private Train holding = null;
    private int nextID = 0;
    private int score = 0;
    private bool isGameOver = false;
    // private bool dropProhibition = false;

    // Start is called before the first frame update
    void Start()
    {

        if (PlayerPrefs.HasKey("BestScore"))
        {
            bestScoreText.text = PlayerPrefs.GetInt("BestScore").ToString();
        }
        else
        {
            PlayerPrefs.SetInt("BestScore", 0);
            PlayerPrefs.Save();
        }

        holding = GenerateTrain(NextCandidate());
        nextID = NextCandidate();
        ShowNextCandidate();
    }

    // Update is called once per frame
    void Update()
    {
        if (holding == null || isGameOver)
        {
            return;
        }

        holding.SetPosition(GetMousePosition(1));

        if (Input.GetMouseButtonDown(0) /*&& !dropProhibition*/)
        {
            //StartCoroutine(DropProhibit());
            DoDrop();
        }
    }

    //private IEnumerator DropProhibit()
    //{
    //    dropProhibition = true;
    //    yield return new WaitForSeconds(0.5f);
    //    dropProhibition = false;
    //}

    public void AddScore(int size)
    {
        score += ((size + 1) * (size + 2)) / 2;
        scoreText.text = score.ToString();
    }

    private void UpdateBestScore()
    {
        int tempScore = PlayerPrefs.GetInt("BestScore");
        if (score > tempScore)
        {
            PlayerPrefs.SetInt("BestScore", score);
            PlayerPrefs.Save();
        }
    }

    public void ResetBestScore()
    {
        PlayerPrefs.SetInt("BestScore", 0);
        PlayerPrefs.Save();
        bestScoreText.text = "0";
    }

    public void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            gameOverSound.Play();
            UpdateBestScore();
            gameOverText.SetActive(true);
            Rigidbody2D[] rbs = trainGroup.GetComponentsInChildren<Rigidbody2D>();

            for (int i = 0; i < rbs.Length; i++)
            {
                rbs[i].simulated = false;
            }
        }
    }

    public void ReloadScene()
    {
        UpdateBestScore();
        // 現在のSceneを取得
        Scene loadScene = SceneManager.GetActiveScene();
        // 現在のシーンを再読み込みする
        SceneManager.LoadScene(loadScene.name);
    }

    public void PlayMergeSound()
    {
        mergeSound.Play();
    }

    private void DoDrop()
    {
        dropSound.Play();
        holding.DoDrop();
        holding = GenerateTrain(nextID);
        nextID = NextCandidate();
        ShowNextCandidate();
    }

    private int NextCandidate()
    {
        return Random.Range(0, 5);
    }

    private void ShowNextCandidate()
    {
        nextShow.sprite = nextShows[nextID];
    }

    private Vector2 GetMousePosition()
    {
        return GetMousePosition(0);
    }

    private Vector2 GetMousePosition(int mode)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.y = 5.75f;
        switch (mode)
        {
            case 1:
                mousePos.x = Mathf.Clamp(mousePos.x, -3.55f + holding.transform.localScale.x / 2, 3.55f - holding.transform.localScale.x / 2); // 落とす対象に合わせた移動制限
                break;
            default:
                mousePos.x = Mathf.Clamp(mousePos.x, -2.85f, 2.85f);
                break;
        }
        return mousePos;
    }

    private Train GenerateTrain(int id)
    {
        return Instantiate(trains[id], GetMousePosition(), Quaternion.identity, trainGroup).GetComponent<Train>();
    }
}
