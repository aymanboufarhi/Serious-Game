using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Diagnostics;

public class PrizeSceneController : MonoBehaviour
{
    private string Player;
    private string iScore;
    private int rank;
    public Text PlayerText;
    public Text ScoreText;
    public Text PuzzleNumber;
    public GameObject GirlBoyIcon;
    public Sprite girlIcon;
    public GameObject[] RankProgressBarFill = new GameObject[12];
    public GameObject puzzlePiece;
    public GameObject[] puzzles;
    public int[][] ordre = new int[6][];
    public int[] ordre1;
    public int[] ordre2;
    public int[] ordre3;
    public int[] ordre4;
    public int[] ordre5;
    public int[] ordre6;
    private Vector3[][] originalPositions;
    private int lastShuffleScore;
    private int previousScore;
    private bool isFirstTime = true;

    private string previousScoreKey;

    void Start()
    {
        UpdatePlayerData();
        InitializePlayerData();
        previousScoreKey = "PreviousScore_" + Player;
        ApplyShuffle();
        
        if (PlayerPrefs.GetInt("Player" + Player + "RewardCounter") >= 1)
        {
            int x = PlayerPrefs.GetInt("Player" + Player + "RewardCounter");
            int y = new int();
            if (PlayerPrefs.GetInt("Player" + Player + "RewardCounter") != 0)
            {
                if (x >= 1 && x <= 9)
                { y = 0; }
                else if (x >= 10 && x <= 18)
                { y = 1; x = x - 9; }
                else if (x >= 19 && x <= 27)
                { y = 2; x = x - 18; }
                else if (x >= 28 && x <= 36)
                { y = 3; x = x - 27; }
                else if (x >= 37 && x <= 45)
                { y = 4; x = x - 36; }
                else if (x >= 46 && x <= 54)
                { y = 5; x = x - 45; }
                else { return; }
            }

            if (PlayerPrefs.GetInt("Player" + Player + "puzzle" + y + "piece" + x) == 0)
            {
                PlayerPrefs.SetInt("Player" + Player + "puzzle" + y + "piece" + x, 1);
                puzzlePiece.transform.GetChild(0).GetComponent<Image>().sprite = puzzles[y].transform.GetChild(ordre[y][x - 1]).GetComponent<Image>().sprite;
                puzzlePiece.SetActive(true);
                StartCoroutine("hidePiece");
            }
        }
    }

    void Update()
    {
        rank = PlayerPrefs.GetInt("Player" + PlayerPrefs.GetString("Player") + "Rank");
        UpdatePlayerData();
        ApplyShuffle();
        RankProgressBar();
        ShowPuzzle();
        UpdatePuzzleNumber();
        ResetLevelFour();
    }

    IEnumerator hidePiece()
    {
        yield return new WaitForSeconds(3);
        puzzlePiece.SetActive(false);
    }

    private void InitializePlayerData()
    {
        Player = PlayerPrefs.GetString("Player");
        lastShuffleScore = PlayerPrefs.GetInt("Player" + Player + "scoreTotal");
        previousScore = PlayerPrefs.GetInt("PreviousScore", lastShuffleScore);

        originalPositions = new Vector3[puzzles.Length][];

        for (int i = 0; i < puzzles.Length; i++)
        {
            originalPositions[i] = new Vector3[9];

            for (int j = 0; j < 9; j++)
            {
                originalPositions[i][j] = puzzles[i].transform.GetChild(j).position;
            }
        }
    }

    private void ResetLevelFour()
    {
        int LevelFourScore = 0;

        for (int i = 1; i <= 9; i++)
        {
            LevelFourScore += PlayerPrefs.GetInt("Player" + PlayerPrefs.GetString("Player") + "Level" + 4 + "Table" + i + "Score");
        }

        if (LevelFourScore == 90)
        {
            for (int i = 1; i <= 9; i++)
            {
                PlayerPrefs.SetInt("Player" + PlayerPrefs.GetString("Player") + "Level" + 4 + "Table" + i + "Score", 0);
            }
        }
    }

    private void UpdatePlayerData()
    {
        Player = PlayerPrefs.GetString("Player");
        iScore = PlayerPrefs.GetInt("Player" + Player + "RewardScore").ToString();
        PlayerText.text = PlayerPrefs.GetString("name:" + Player);
        ScoreText.text = iScore;

        if (PlayerPrefs.GetString("sex" + Player) == "femme")
            GirlBoyIcon.GetComponent<Image>().sprite = girlIcon;

        ordre[0] = ordre1;
        ordre[1] = ordre2;
        ordre[2] = ordre3;
        ordre[3] = ordre4;
        ordre[4] = ordre5;
        ordre[5] = ordre6;
    }

    private void ApplyShuffle()
    {
        int totalScore = PlayerPrefs.GetInt("Player" + Player + "scoreTotal");

        if (isFirstTime || totalScore != PlayerPrefs.GetInt(previousScoreKey, totalScore))
        {
            ShufflePuzzles();
            isFirstTime = false;
            PlayerPrefs.SetInt(previousScoreKey, totalScore);
            PlayerPrefs.Save();
        }
    }

    private void ShufflePuzzles()
    {
        int rewardCounter = PlayerPrefs.GetInt("Player" + Player + "RewardCounter");
        if (rewardCounter > 0)
        {
            for (int i = 0; i < puzzles.Length; i++)
            {
                if (rewardCounter < (i + 1) * 9)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        int randomIndex = Random.Range(j, 9);
                        Transform puzzlePiece = puzzles[i].transform.GetChild(j);
                        Transform randomPiece = puzzles[i].transform.GetChild(randomIndex);
                        Vector3 tempPosition = randomPiece.position;

                        randomPiece.position = puzzlePiece.position;
                        puzzlePiece.position = tempPosition;
                    }
                }
                else
                {
                    for (int j = 0; j < 9; j++)
                    {
                        puzzles[i].transform.GetChild(j).position = originalPositions[i][j];
                    }
                }
            }
        }
    }

    private void RankProgressBar()
    {
        for (int i = 0; i < RankProgressBarFill.Length; i++)
        {
            if (rank != 0 && i < (rank * 2) - 1)
            {
                RankProgressBarFill[i].SetActive(true);
                RankProgressBarFill[i + 1].SetActive(true);
                i++;
            }
            else
            {
                RankProgressBarFill[i].SetActive(false);
            }
        }
    }

    public void ShowPuzzle()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                puzzles[i].transform.GetChild(j).gameObject.SetActive(false);
            }
        }
        CheckPuzzle();
    }
    
    public void CheckPuzzle()
    {
        int x = PlayerPrefs.GetInt("Player" + PlayerPrefs.GetString("Player") + "RewardCounter");

        for (int i = 0; i < Mathf.Min(x, 54); i++)
        {
            int puzzleIndex = i / 9;
            int childIndex = ordre[puzzleIndex][i % 9];
            puzzles[puzzleIndex].transform.GetChild(childIndex).gameObject.SetActive(true);
        }
    }

    public void NextPuzzleButton()
    {
        for (int i = 0; i < puzzles.Length; i++)
        {
            if (puzzles[i].activeSelf)
            {
                puzzles[i].SetActive(false);

                if (i != 5)
                {
                    puzzles[i + 1].SetActive(true);
                }
                else
                {
                    puzzles[0].SetActive(true);
                }
                break;
            }
        }
    }

    public void PreviousPuzzleButton()
    {
        for (int i = 0; i < puzzles.Length; i++)
        {
            if (puzzles[i].activeSelf)
            {
                puzzles[i].SetActive(false);

                if (i != 0)
                {
                    puzzles[i - 1].SetActive(true);
                }
                else
                {
                    puzzles[5].SetActive(true);
                }
                break;
            }
        }
    }

    public void UpdatePuzzleNumber()
    {
        for (int i = 0; i < puzzles.Length; i++)
        {
            if (puzzles[i].activeSelf)
            {
                PuzzleNumber.text = (i + 1).ToString();
            }
        }
    }

    public void ShareDataFile()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            // Get the path to the data file
            string filePath = Path.Combine(Application.persistentDataPath, "data.csv");

            // Make sure that the file exists
            if (!File.Exists(filePath))
            {
                UnityEngine.Debug.LogError("File does not exist: " + filePath);
                return;
            }

            // Create a new intent to share the file
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            intentObject.Call<AndroidJavaObject>("setType", "text/csv");

            // Get the URI for the file
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + filePath);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);

            // Start the activity to share the file
            AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share File");
            currentActivity.Call("startActivity", chooser);
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            Process.Start("explorer.exe", "/select," + "stats.csv");
        }
    }
}