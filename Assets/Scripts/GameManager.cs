using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int level = 1;

    public bool isEndless;

    public int energy;
    private int timeReg;

    private const int maxEnergy = 10;
    private const int timeRegen = 1800;

    private AudioSource aud;

    [SerializeField]
    private AudioClip sndEndGood, sndEndBad, musicMenu;
    [SerializeField]
    private AudioClip[] music;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null) instance = this; else if (instance != this) Destroy(instance.gameObject); instance = this;
        level = PlayerPrefsPlus.GetInt("level");
        if (UIControl.instance != null) UIControl.instance.ChangeLevel(level);
        //SetMusicMute();
    }

    private void Start()
    {
        timeReg = PlayerPrefsPlus.GetInt("timeregen");
        if (!PlayerPrefsPlus.HasKey("energy")) PlayerPrefsPlus.SetInt("energy", maxEnergy);
        energy = PlayerPrefsPlus.GetInt("energy");
        Time.timeScale = 1f;
        aud = GetComponent<AudioSource>();
        aud.loop = true;
        aud.clip = musicMenu;
        aud.Play();
        aud.mute = PlayerPrefsPlus.GetBool("mus");
        //SetMusicMute();
    }

    private void Update()
    {
        if (energy < maxEnergy)
        {
            int sremain = Mathf.Abs(((int)(System.DateTime.UtcNow.Ticks / 10000000) - timeReg) - timeRegen);
            //print(sremain / 60 + ":" + sremain % 60);
            int min = sremain / 60;
            int sec = sremain % 60;
            //print(min + " " + sec);
            UIControl.instance.SetTimeReg(   (min < 10 ? "0" : "") + min + ":" + (sec < 10 ? "0" : "") + sec);
            //print(System.DateTime.);
        }

        if (energy < maxEnergy && ((int)(System.DateTime.UtcNow.Ticks/10000000) - timeReg)  >= timeRegen)
        {
            EnergyUse(true);
            timeReg += timeRegen;
            PlayerPrefsPlus.SetInt("timeregen", timeReg);
        }
    }




    public void SetEndLevel(bool isGood)
    {
        aud.loop = false;
        aud.clip = isGood? sndEndGood : sndEndBad;
        aud.Play();
        
    }

    public void SetEndless(bool isEndlesss)
    {
        isEndless = isEndlesss;
        LoadGame();
    }

    public void LoadGame()
    {
        Time.timeScale = 1f;
        if (energy > 0)
        {
            if (energy == maxEnergy)
            {
                PlayerPrefsPlus.SetInt("timeregen", (int)(System.DateTime.UtcNow.Ticks / 10000000));
                timeReg = PlayerPrefsPlus.GetInt("timeregen");
            }
            EnergyUse();
            //PlayerPrefsPlus.SetInt("timeregen", (int)System.DateTime.UtcNow.Ticks);
            //print("" + ((int)System.DateTime.UtcNow.Ticks - (PlayerPrefsPlus.GetInt("timeregen"))) );

            SceneManager.LoadScene(1);

            aud.loop = true;
            aud.clip = music[Random.Range(0, music.Length)]; 
            aud.Play();
        }
        else
        {
            UIControl.instance.OpenShopHint(true);
        }
    }

    public void EnergyUse(bool isAdd = false)
    {
        energy += isAdd ? 1 : -1;
        PlayerPrefsPlus.SetInt("energy", energy);
        UIControl.instance.SetEnergy();
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        aud.loop = true;
        aud.clip = musicMenu;
        aud.Play();
    }

    public void ChangeLevel(bool increase)
    {
        if (increase)
        {
            if (level < PlayerPrefsPlus.GetInt("level")) level++;
        }
        else if (level > 1) level--;
        if (UIControl.instance != null) UIControl.instance.ChangeLevel(level);
    }

    public void ReloadGame()
    {
        Time.timeScale = 1f;
        if (energy > 0)
        {
            EnergyUse();
            SceneManager.LoadScene(1);
            aud.loop = true;
            aud.clip = music[Random.Range(0, music.Length)];
            aud.Play();
        }
        else
        {
            SceneManager.LoadScene(0);
            aud.loop = true;
            aud.clip = musicMenu;
            aud.Play();
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void SetMusicMute()
    {
        aud.mute = PlayerPrefsPlus.GetBool("mus");
    }
}