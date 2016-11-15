using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField]
    private float jumpForce, moveSpeed, gravity, longflyCoef, healCD;

    private float lengthDelay, lengthLevel;
    public int points, health;
    public bool isGame, isEndless;
    private float modifierEndless = 1f;
    private Animator anim;
    private AudioSource aud;
    [SerializeField]
    private AudioClip snd_bee, snd_beeSmooth;
    [SerializeField]
    private AudioClip[] snd_kick;
    public AudioSource audConst;
    private int penalty;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        aud = GetComponent<AudioSource>();
        aud.clip = snd_bee;
        
        aud.mute = PlayerPrefsPlus.GetBool("snd");
        audConst.mute = PlayerPrefsPlus.GetBool("snd");
        UIControl.instance.SetLength(0);
    }

    private void OnEnable()
    {
        anim = GetComponent<Animator>();
        transform.position = new Vector3(-15f, 0f, 0f);

        var s = FindObjectOfType<Spawner>();
        lengthDelay = s.lengthDelay;
        lengthLevel = s.lengthLevel - 0.1f;

        health = 3;
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        Invoke("SetNoKinematic", 2f);
        rb.velocity = new Vector2(moveSpeed, 0f);

        FindObjectOfType<CameraControl>().follow = transform;

        isGame = true;
        anim.SetBool("isFly", isGame);

        if (GameManager.instance)
        {
            isEndless = GameManager.instance.isEndless;
            if (!isEndless) moveSpeed += GameManager.instance.level * 0.02f;
        }

        FindObjectOfType<UIControl>().SetHealth(health);
    }

    private void FixedUpdate()
    {
        if ((Input.GetButton("Jump") || Input.GetMouseButton(0)) && Time.timeScale == 1f)
        {
            rb.AddForce(new Vector2(0f, jumpForce * 0.001f * longflyCoef));
            anim.SetBool("isFast", true);
        }
        else

        if (isGame) rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    private void Update()
    {
        float angle = health > 0 ? Vector2.Angle(Vector2.right, rb.velocity.normalized) : 179.5f;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, rb.velocity.y >= 0 ? angle : -angle), 0.5f);

        if (!isGame) return;

        if ((Input.GetButtonUp("Jump") || Input.GetMouseButtonUp(0)))
        {
            aud.clip = snd_beeSmooth;
            aud.loop = false;
            aud.Play();
        }

        if ((Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(0)) && Time.timeScale == 1f)
        {
            aud.clip = snd_bee;
            aud.loop = true;
            aud.Play();
            anim.SetBool("isFast", true);
            rb.velocity = new Vector2(moveSpeed, 0f);
            rb.AddForce(new Vector2(0f, jumpForce));
        }
        else if (!Input.GetButton("Jump") && !Input.GetMouseButton(0))
            anim.SetBool("isFast", false);

        int ppoints = points;
        points = (int)(transform.position.x / lengthDelay);
        if (ppoints != points && points >= 0)
        {
            UIControl.instance.SetLength(points);
            //print(points + " beep at " + transform.position.x);
            
            if (isEndless) moveSpeed += 0.01f;
            if (points % healCD == 0 && points != 0) { if (health < 3)  health++; UIControl.instance.SetHealth(health); }
            modifierEndless = 1 + points * 0.2f;
        }

        if (transform.position.x > lengthLevel)
        {
            rb.isKinematic = true;
            rb.velocity = new Vector2(moveSpeed, 0f);
            isGame = false;
            aud.clip = snd_beeSmooth;
            aud.loop = false;
            aud.Play();
            //anim.SetBool("isFly", isGame);

            if (GameManager.instance)
            {
                int fpoints = (int)(GameManager.instance.level + (points - penalty) * GameManager.instance.level * 0.1f);

                if (UIControl.instance != null)
                {
                    UIControl.instance.OpenPanelEndLevel("Added "+fpoints + " to honey!");
                }

                GameManager.instance.SetEndLevel(true);

                switch (GameManager.instance.level)
                {
                    case 10: GPG.instance.UnlockAchievment(GPG.instance.ACH_LEVEL_10); break;
                    case 50: GPG.instance.UnlockAchievment(GPG.instance.ACH_LEVEL_50); break;
                    case 100: GPG.instance.UnlockAchievment(GPG.instance.ACH_LEVEL_100); break;
                }
                GPG.instance.PostScoreToLeaderboard(GameManager.instance.level, GPG.instance.BOARD_LEVEL);


                GameManager.instance.level++;
                if (PlayerPrefsPlus.GetInt("level") < GameManager.instance.level)
                    PlayerPrefsPlus.SetInt("level", GameManager.instance.level);

                print("Added " + fpoints + " points for you!");
                PlayerPrefsPlus.SetInt("points", PlayerPrefsPlus.GetInt("points") + fpoints);
                GameManager.instance.EnergyUse(true);

                if (GameManager.instance.level % 5 == 0 && GameManager.instance.level != 0) Ads.instance.ShowAds( (1 + (int)(GameManager.instance.level * 0.1f)) * 25 );
            }

            Invoke("DisableCamera", 2f);
            Invoke("DisablePlayerObj", 15f);
            //Invoke("ReloadScene", 5f);
        }
    }

    private void DisableCamera()
    {
        FindObjectOfType<CameraControl>().follow = FindObjectOfType<CameraControl>().transform;
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        health--;
        //print("oucj!");
        penalty +=   isEndless ? (int)(points * 0.1f) :  1 + (int)(GameManager.instance.level * 0.1f);
        print("P " + penalty);

        print("ouch " + points);

        aud.clip = snd_kick[Random.Range(0,snd_kick.Length)];
        aud.loop = false;
        aud.Play();

        UIControl.instance.SetHealth(health);

        if (health == 0)
        {
            rb.velocity = Vector2.zero;
            isGame = false;
            anim.SetBool("isFly", isGame);
            Invoke("DisableCamera", 1f);

            //Invoke("DisablePlayerObj", 2f);
            Invoke("ShowAdsDeath", 3.7f);
            

            if (audConst) audConst.Stop();
           if (health == 0) GameManager.instance.SetEndLevel(false);

            if (GameManager.instance.isEndless)
            {
                GPG.instance.PostScoreToLeaderboard(points, GPG.instance.BOARD_END_LENGTH);
                int fpoints = (int)((points - penalty) * modifierEndless);
                print("score endless = " + points);
                print("Added " + fpoints + " points for you!");
                PlayerPrefs.SetInt("points", PlayerPrefs.GetInt("points") + fpoints);

                if (fpoints >= 1000) GPG.instance.UnlockAchievment(GPG.instance.ACH_ENDLESS_1000);
                if (fpoints >= 8000) GPG.instance.UnlockAchievment(GPG.instance.ACH_ENDLESS_8000);
                if (fpoints >= 12000) GPG.instance.UnlockAchievment(GPG.instance.ACH_ENDLESS_12000);
                GPG.instance.PostScoreToLeaderboard(fpoints, GPG.instance.BOARD_ENDLESS);

                if (UIControl.instance != null)
                    UIControl.instance.OpenPanelEndLevel("Your score: " + fpoints+". \n Its added to your honey!");
            } else
            {
                Invoke("ReloadScene", 5f);
            }
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }
    }

    private void ReloadScene()
    {
        if (GameManager.instance) GameManager.instance.ReloadGame();
    }

    private void DisablePlayerObj()
    {
        gameObject.SetActive(false);
    }

    private void SetNoKinematic()
    {
        rb.isKinematic = false;
    }

    public void ExitGame()
    {
        if (isEndless)
        {
            PlayerPrefs.SetInt("points", PlayerPrefs.GetInt("points") + (int)(points * modifierEndless));
        }
    }

    private void ShowAdsDeath()
    {
        print("show ads");
        Ads.instance.ShowAds((int)(isEndless ? (int)(points * modifierEndless * 0.1f) : GameManager.instance.level * 0.25f));
    }


}