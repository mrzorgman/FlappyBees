using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GPG : MonoBehaviour
{
    public static GPG instance;

    private bool isLoggedIn;

    public string BOARD_ENDLESS = "CgkItvun2fwCEAIQAQ",
                        BOARD_LEVEL = "CgkItvun2fwCEAIQAg",
                        ACH_LEVEL_10 = "CgkItvun2fwCEAIQAw",
                        ACH_LEVEL_50 = "CgkItvun2fwCEAIQBA",
                        ACH_LEVEL_100 = "CgkItvun2fwCEAIQBQ",
                        ACH_ENDLESS_1000 = "CgkItvun2fwCEAIQBg",
                        ACH_ENDLESS_8000 = "CgkItvun2fwCEAIQBw",
                        ACH_ENDLESS_12000 = "CgkItvun2fwCEAIQCA",
                        ACH_UNLOCK_BEE = "CgkItvun2fwCEAIQCQ",
                        BOARD_END_LENGTH = "CgkItvun2fwCEAIQCg";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null) instance = this; else if (instance != this) Destroy(instance.gameObject); instance = this;
    }

    private void Start()
    {

        Activate();
        //PlayGamesPlatform.Instance.SignOut();
        SignIn();
    }


    


    private void Activate()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
// enables saving game progress.
//.EnableSavedGames()
// registers a callback to handle game invitations received while the game is not running.
//.WithInvitationDelegate(< callback method >)
// registers a callback for turn based match notifications received while the
// game is not running.
//.WithMatchDelegate(< callback method >)
// require access to a player's Google+ social graph (usually not needed)
//.RequireGooglePlus()
.Build();

        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
    }

    public void SignIn()
    {
        //PlayGamesPlatform.Instance.SignOut();
        // authenticate user:
        Social.localUser.Authenticate((bool success) =>
        {
            
            isLoggedIn = success;
             //UIControl.instance.Log( success ? "GPG: Sign in - OK!" : "GPG: Sign in - FAILED!");
            // handle success or failure
        });
    }

    private void PlayerStatistic()
    {
        ((PlayGamesLocalUser)Social.localUser).GetStats((rc, stats) =>
        {
            // -1 means cached stats, 0 is succeess
            // see  CommonStatusCodes for all values.
            if (rc <= 0 && stats.HasDaysSinceLastPlayed())
            {
                UIControl.instance.Log("It has been " + stats.DaysSinceLastPlayed + " days");
            }
        });
    }

    public void UnlockAchievment(string achievment_id)
    {
        // unlock achievement (achievement ID "Cfjewijawiu_QA")
        Social.ReportProgress(achievment_id, 100.0f, (bool success) =>
        {
            //UIControl.instance.Log(success ? "GPG: Achievment - OK!" : "GPG: Achievment - FAIL!");
            // handle success or failure
        });
    }

    private void IncrementAchievment(string achievment_id)
    {
        // increment achievement (achievement ID "Cfjewijawiu_QA") by 5 steps
        PlayGamesPlatform.Instance.IncrementAchievement(
            achievment_id, 5, (bool success) =>
            {
                // handle success or failure
            });
    }

    public void PostScoreToLeaderboard(int score, string board_id)
    {
        // post score 12345 to leaderboard ID "Cfji293fjsie_QA")
        Social.ReportScore(score, board_id, (bool success) =>
        {
            //UIControl.instance.Log(success ? "GPG: Post score - OK!" : "GPG: Post score - FAIL!");
            // handle success or failure
        });
    }

    public void ShowUIAchievment()
    {
        if (!isLoggedIn) SignIn();
        Social.ShowAchievementsUI();
    }

    public void ShowUILeaderboard()
    {
        if (!isLoggedIn) SignIn();
        Social.ShowLeaderboardUI();
    }

    private void GetLeaderboard()
    {
        ILeaderboard lb = PlayGamesPlatform.Instance.CreateLeaderboard();
        lb.id = "MY_LEADERBOARD_ID";
        lb.LoadScores(ok =>
        {
            if (ok)
            {
                LoadUsersAndDisplay(lb);
            }
            else
            {
                UIControl.instance.Log("Error retrieving leaderboardi");
            }
        });
    }

    internal void LoadUsersAndDisplay(ILeaderboard lb)
    {
        // get the user ids
        List<string> userIds = new List<string>();

        foreach (IScore score in lb.scores)
        {
            userIds.Add(score.userID);
        }
        // load the profiles and display (or in this case, log)
        Social.LoadUsers(userIds.ToArray(), (users) =>
        {
            string status = "Leaderboard loading: " + lb.title + " count = " +
                lb.scores.Length;
            foreach (IScore score in lb.scores)
            {
                IUserProfile user = FindUser(users, score.userID);
                status += "\n" + score.formattedValue + " by " +
                    (string)(
                        (user != null) ? user.userName : "**unk_" + score.userID + "**");
            }
            UIControl.instance.Log(status);
        });
    }

    private IUserProfile FindUser(IUserProfile[] users, string userid)
    {
        foreach (IUserProfile user in users)
        {
            if (user.id == userid)
            {
                return user;
            }
        }
        return null;
    }
}