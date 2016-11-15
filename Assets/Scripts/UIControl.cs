using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    public static UIControl instance;

    public Text textLevelIndicator, textPointsToPlayer, tEnergy, tPoints, tLength;
    public GameObject endLevelPanel;
    public Image[] health;

    public GameObject panelShop, panelMenu;
    public Text upgrade_cost;
    public GameObject skinPoint;

    public int[] costVariety, costSkins;

    public Color cActivr, cInactive, cLocked;
    public Image[] skinsBtns;

    private bool isPause;

    public Toggle tmus, tsnd;

    public int costEnergy;

    public Text textLog, timeRegen;

    public Animator bt_health, bt_coins;

    ////////////

    public void ToggleSnd(bool ismus)
    {
        if (ismus)
        {
            PlayerPrefsPlus.SetBool("mus", !tmus.isOn);
            GameManager.instance.SetMusicMute();
        }
        else
        {
            PlayerPrefsPlus.SetBool("snd", !tsnd.isOn);
            //if (tsnd) tsnd.isOn = !PlayerPrefsPlus.GetBool("snd");
        }
    }

    public void UpdateVariety()
    {
        int p = PlayerPrefsPlus.GetInt("points");
        int v = PlayerPrefsPlus.GetInt("variety");

        if (v + 1 > costVariety.Length)
            if (p >= costVariety[v + 1])
            {
                PlayerPrefsPlus.SetInt("points", p - costVariety[v + 1]);
                PlayerPrefsPlus.SetInt("variety", v + 1);
            }
    }

    public void BuyEnergy()
    {
        int p = PlayerPrefsPlus.GetInt("points");
        if (p >= costEnergy)
        {
            PlayerPrefsPlus.SetInt("points", p - costEnergy);
            GameManager.instance.EnergyUse(true);
            SetEnergy();
            SetPoints();
        }
    }

    public void SetSkin(int s)
    {
        int p = PlayerPrefsPlus.GetInt("points");
        bool o = PlayerPrefsPlus.GetBool("skin" + s);

        if (o)
        {
            PlayerPrefsPlus.SetInt("skin", s);
            print("set skin " + s);
            UpdateSkinInShop();
        }
        else
        {
            if (p >= costSkins[s])
            {
                PlayerPrefsPlus.SetInt("points", p - costSkins[s]);
                PlayerPrefsPlus.SetInt("skin", s);
                PlayerPrefsPlus.SetBool("skin" + s, true);
                print("set skin " + s);
                UpdateSkinInShop();
                skinsBtns[s].transform.FindChild("Text").GetComponent<Text>().text = " Cool!";
                GPG.instance.UnlockAchievment(GPG.instance.ACH_UNLOCK_BEE);
            }
        }
        SetPoints();
    }

    public void UpdateSkinInShop()
    {
        if (panelShop)
        {
            for (int i = 0; i < skinsBtns.Length; i++)
            {
                skinsBtns[i].color = PlayerPrefsPlus.GetBool("skin" + i) ? (PlayerPrefsPlus.GetInt("skin") == i ? cActivr : cInactive) : cLocked;
                if (!PlayerPrefsPlus.GetBool("skin" + i)) skinsBtns[i].transform.FindChild("Text").GetComponent<Text>().text = "??? (" + costSkins[i] + ")";
            }
        }
    }

    ////////////

    private void Awake()
    {
        if (instance == null) instance = this; else if (instance != this) Destroy(instance.gameObject); instance = this;
    }

    private void Start()
    {
        SetEnergy();
        SetPoints();
        PlayerPrefsPlus.SetBool("skin0", true);
        UpdateSkinInShop();

        if (tmus) tmus.isOn = !PlayerPrefsPlus.GetBool("mus");
        if (tsnd) tsnd.isOn = !PlayerPrefsPlus.GetBool("snd");

        if (textLevelIndicator) textLevelIndicator.text = PlayerPrefsPlus.GetInt("level").ToString();

        Log("Please click ads - you will be rewarded! Thanks! ^_^", 2.5f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void ChangeLevel(int l)
    {
        if (textLevelIndicator != null) textLevelIndicator.text = l.ToString();
    }

    public void OpenPanelEndLevel(string msg)
    {
        isPause = false;
        textLevelIndicator.text = GameManager.instance.isEndless ? "Endless!" : "Level " + GameManager.instance.level + " completed!";
        textPointsToPlayer.text = msg;
        endLevelPanel.SetActive(true);
    }

    public void Pause()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        textLevelIndicator.text = GameManager.instance.isEndless ? "Endless!" : "Level " + GameManager.instance.level;
        if (textPointsToPlayer) textPointsToPlayer.text = "";
        endLevelPanel.SetActive(!endLevelPanel.activeInHierarchy);
        Time.timeScale = endLevelPanel.activeInHierarchy ? 0f : 1f;
        isPause = endLevelPanel.activeInHierarchy;
    }

    public void ToMenu()
    {
        var p = FindObjectOfType<PlayerController>();
        if (p) p.ExitGame();
        Destroy(GameManager.instance.gameObject);
        SceneManager.LoadScene(0);
    }

    public void SetHealth(int hp)
    {
        print("HP " + hp);
        for (int i = 0; i < health.Length; i++)
        {
            health[i].color = i >= hp ? Color.black : Color.white;
        }
    }

    public void PlayButton()
    {
        if (isPause)
        {
            Pause();
        }
        else
        {
            GameManager.instance.LoadGame();
            //SceneManager.LoadScene(1);
        }
    }

    public void SetEnergy()
    {
        if (tEnergy != null) tEnergy.text = PlayerPrefsPlus.GetInt("energy").ToString();
    }

    public void SetPoints()
    {
        if (tPoints != null)
        {
             tPoints.text = PlayerPrefsPlus.GetInt("points").ToString();
        }
    }

    public void SetLength(int l)
    {
        if (tLength) tLength.text = "" + l;
    }

    public void Log(string msg, float time = 2f)
    {
        Debug.Log(msg);
        if (textLog == null) return;
        textLog.gameObject.SetActive(true);
        Invoke("DisableLog", time);
        textLog.text = msg;
    }

    private void DisableLog()
    {
        textLog.gameObject.SetActive(false);
    }

    public void OpenShopHint(bool b)
    {
        if (bt_health) bt_health.enabled = b;
        if (bt_coins) bt_coins.enabled = b;
        panelShop.SetActive(true);
        panelMenu.SetActive(false);
    }

    public void SetTimeReg(string t)
    {
        if (timeRegen) timeRegen.text = t;
    }

    public void OpenMarket()
    {
        Application.OpenURL("https://play.google.com/store/apps/developer?id=mr.zorgman");
    }
}