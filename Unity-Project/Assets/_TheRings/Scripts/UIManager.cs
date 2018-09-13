using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using SgLib;

#if EASY_MOBILE
using EasyMobile;
#endif

public class UIManager : MonoBehaviour
{
    public static bool firstLoad = true;

    [Header("Check to show menu when game starts")]
    public bool showMenuAtStart = false;

    [Header("Object References")]
    public GameManager gameManager;
    public GameObject title;
    public GameObject mainCanvas;
    public GameObject pauseCanvas;
    public GameObject settingsCanvas;
    public Text txtScore;
    public Text txtCombo;
    public Text txtComboText;
    public Text txtBestScore;
    public Text txtChangeRing;
    public Text txtNoMoreMoves;
    public Button btnChangeRing;
    public Button btnPause;
    public Button btnPlay;
    public Button btnReview;
    public Button btnSoundOn;
    public Button btnSoundOff;
    public GameObject gameOverButtons;
    public GameObject blackPanel;

    [Header("Premium Features Buttons")]
    public GameObject btnWatchAd;
    public GameObject leaderboardBtn;
    public GameObject leaderboardBtn2;
    public GameObject shareBtn;
    public GameObject removeAdsBtn;
    public GameObject restorePurchaseBtn;

    [Header("Sharing-Specific")]
    public GameObject shareUI;
    public Image sharedImage;
 
    Animator scoreAnimator;
    bool hasCheckedGameOver = false;
    
    int t = 1;

    void OnEnable()
    {
        ScoreManager.ScoreUpdated += OnScoreUpdated;
    }

    void OnDisable()
    {
        ScoreManager.ScoreUpdated -= OnScoreUpdated;
    }

    // Use this for initialization
    void Start()
    {
        scoreAnimator = txtScore.GetComponent<Animator>();
        pauseCanvas.SetActive(showMenuAtStart);

        // Enable or disable premium stuff
        bool enablePremium = gameManager.enablePremiumFeatures;
        leaderboardBtn.SetActive(enablePremium);
        leaderboardBtn2.SetActive(enablePremium);
        shareBtn.SetActive(enablePremium);
        removeAdsBtn.SetActive(enablePremium);
        restorePurchaseBtn.SetActive(enablePremium);

        // Hidden at start
        shareUI.SetActive(false);
        settingsCanvas.SetActive(false);
        gameOverButtons.SetActive(false);
        btnWatchAd.SetActive(false);

        ShowGameplayUI();
        firstLoad = false;
    }
	
    // Update is called once per frame
    void Update()
    {
        txtScore.text = ScoreManager.Instance.Score.ToString();
        txtBestScore.text = ScoreManager.Instance.HighScore.ToString();
        txtChangeRing.text = CoinManager.Instance.Coins.ToString();

        if (gameManager.combo > 1)
        {
            ShowCombo();
        }
        else
        {
            HideCombo();
        }

        if (gameManager.gameOver && !hasCheckedGameOver)
        {
            hasCheckedGameOver = true;
            Invoke("ShowGameOverUI", 1f);
        }

        UpdateMuteButtons();
    }

    void OnScoreUpdated(int newScore)
    {
        scoreAnimator.Play("NewScore");
    }

    public void HandleRestartButton()
    {
        StartCoroutine(CRRestart(0.2f));
    }

    public void HandlePlayButton()
    {
        if (!firstLoad)
        {
            StartCoroutine(CRRestart());
        }
        else
        {
            ShowGameplayUI();
            firstLoad = false;
        }
    }

    void ShowCombo()
    {
        txtCombo.gameObject.SetActive(true);
        txtCombo.text = "x" + gameManager.combo.ToString();
    }

    void HideCombo()
    {
        txtCombo.gameObject.SetActive(false);
    }


    public void ShowComboText(float waitTime)
    {
        StartCoroutine(ComboText(waitTime));
    }

    IEnumerator ComboText(float waitTime)
    {      
        txtComboText.text = "combo x" + gameManager.combo.ToString();
        txtComboText.gameObject.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        txtComboText.gameObject.SetActive(false);
    }

    public void HandleChangeRingButton()
    {
        if (CoinManager.Instance.Coins > 0 && gameManager.finishMoveRing)
        {
            gameManager.combo = 0;
            gameManager.finishMoveRing = false;
            StartCoroutine(gameManager.ChangeRing());
            CoinManager.Instance.AddCoins(-1);
        }
    }

    public void HandleReviewButton()
    {
        if (!firstLoad)
        {
            t = t * (-1);
            if (t < 0)
            {
                btnPlay.gameObject.SetActive(false);
                gameOverButtons.SetActive(false);
                blackPanel.gameObject.SetActive(false);
                txtNoMoreMoves.gameObject.SetActive(false);
            }
            else
            {
                btnPlay.gameObject.SetActive(true);
                gameOverButtons.SetActive(true);
                blackPanel.gameObject.SetActive(true);
                txtNoMoreMoves.gameObject.SetActive(true);
            }
        } 
    }

    public void HandleSoundButton()
    {
        SoundManager.Instance.ToggleMute();
    }

    public void ShowGameOverUI()
    {
        gameOverButtons.SetActive(true);
        blackPanel.SetActive(true);
        btnPlay.gameObject.SetActive(true);
        btnReview.gameObject.SetActive(true);
        btnWatchAd.gameObject.SetActive(false);
        txtNoMoreMoves.gameObject.SetActive(true);

        btnChangeRing.gameObject.SetActive(false);
        btnPause.gameObject.SetActive(false);
    }

    public void ShowGameplayUI()
    {
        title.SetActive(false);
        gameOverButtons.SetActive(false);
        blackPanel.SetActive(false);
        btnPlay.gameObject.SetActive(false);
        btnReview.gameObject.SetActive(false);
        txtNoMoreMoves.gameObject.SetActive(false);
        btnWatchAd.gameObject.SetActive(false);
        txtComboText.gameObject.SetActive(false);

        txtCombo.gameObject.SetActive(true);
        txtScore.gameObject.SetActive(true);
        btnChangeRing.gameObject.SetActive(true);
        btnPause.gameObject.SetActive(true);
    }

    public void ShowSettingsUI()
    {
        settingsCanvas.SetActive(true);
    }

    public void HideSettingsUI()
    {
        settingsCanvas.SetActive(false);
    }

    public void ShowWatchAdButton()
    {
        btnWatchAd.gameObject.SetActive(true);
        btnChangeRing.gameObject.SetActive(false);
    }

    public void ShowLeaderboardUI()
    {
        #if EASY_MOBILE
        if (GameServices.IsInitialized())
        {
            GameServices.ShowLeaderboardUI();
        }
        else
        {
            #if UNITY_IOS
        NativeUI.Alert("Service Unavailable", "The user is not logged in to Game Center.");
            #elif UNITY_ANDROID
            GameServices.Init();
            #endif
        }
        #endif
    }

    public void PurchaseRemoveAds()
    {
        #if EASY_MOBILE
        InAppPurchaser.Instance.Purchase(InAppPurchaser.Instance.removeAds);
        #endif
    }

    public void RestorePurchase()
    {
        #if EASY_MOBILE
        InAppPurchaser.Instance.RestorePurchase();
        #endif
    }

    public void ShowShareUI()
    {
        #if EASY_MOBILE
        Texture2D texture = ScreenshotSharer.Instance.GetScreenshotTexture();

        if (texture != null)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            Transform imgTf = sharedImage.transform;
            Image imgComp = imgTf.GetComponent<Image>();
            float scaleFactor = imgTf.GetComponent<RectTransform>().rect.width / sprite.rect.width;
            imgComp.sprite = sprite;
            imgComp.SetNativeSize();
            imgTf.localScale = imgTf.localScale * scaleFactor;

            shareUI.SetActive(true);
        }
        #endif
    }

    public void HideShareUI()
    {
        shareUI.SetActive(false);
    }

    public void ShareScreenshot()
    {
        #if EASY_MOBILE
        shareUI.SetActive(false);
        ScreenshotSharer.Instance.ShareScreenshot();
        #endif
    }

    public void RateApp()
    {
        Utilities.RateApp();
    }

    public void OpenTwitterPage()
    {
        Utilities.OpenTwitterPage();
    }

    public void OpenFacebookPage()
    {
        Utilities.OpenFacebookPage();
    }

    public void ContactUs()
    {
        Utilities.ContactUs();
    }

    public void ButtonClickSound()
    {
        Utilities.ButtonClickSound();
    }

    void UpdateMuteButtons()
    {
        if (SoundManager.Instance.IsMuted())
        {
            btnSoundOn.gameObject.SetActive(false);
            btnSoundOff.gameObject.SetActive(true);
        }
        else
        {
            btnSoundOn.gameObject.SetActive(true);
            btnSoundOff.gameObject.SetActive(false);
        }
    }

    IEnumerator CRRestart(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void HandlePlayButtonOnPauseCanvas()
    {
        pauseCanvas.SetActive(false);
        mainCanvas.gameObject.SetActive(true);
        gameManager.isPaused = false;
    }

    public void HandlePauseButton()
    {
        gameManager.isPaused = true;
        mainCanvas.gameObject.SetActive(false);
        pauseCanvas.transform.Find("BestScore").GetComponent<Text>().text = ScoreManager.Instance.HighScore.ToString();
        pauseCanvas.gameObject.SetActive(true);
        
    }

    public void CheckAndShowWatchAdOption()
    {
        if (CoinManager.Instance.Coins == 0 && !btnWatchAd.activeSelf)
        {
            if (gameManager.enablePremiumFeatures)
            {
                StartCoroutine(CRShowWatchAdOption(0.5f));
            }
            else
            {
                ShowChangeRingButton();
            }
        }
        else if (CoinManager.Instance.Coins > 0 && !btnChangeRing.gameObject.activeSelf)
        {
            ShowChangeRingButton();
        }
    }

    public void ShowChangeRingButton()
    {
        btnChangeRing.gameObject.SetActive(true);
        btnWatchAd.SetActive(false);
    }

    IEnumerator CRShowWatchAdOption(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        if (CoinManager.Instance.Coins == 0)
        {
            // Only show "watch ad" button if a rewarded ad is loaded and premium features are enabled
            #if EASY_MOBILE
            if (gameManager.enablePremiumFeatures && AdDisplayer.Instance.CanShowRewardedAd() && AdDisplayer.Instance.watchAdToRefillRingSwap)
            {
                btnWatchAd.SetActive(true);
                btnChangeRing.gameObject.SetActive(false);
            }
            else
            {
                #if !UNITY_EDITOR
                ShowChangeRingButton(); 
                #else
                if (gameManager.enablePremiumFeatures)
                {
                    btnWatchAd.SetActive(true);   // for testing in the editor
                    btnChangeRing.gameObject.SetActive(false);
                }
                #endif
            }
            #elif UNITY_EDITOR
            if (gameManager.enablePremiumFeatures)
            {
                btnWatchAd.SetActive(true);   // for testing in the editor
                btnChangeRing.gameObject.SetActive(false);
            }
            #endif
        }
    }

    public void WatchRewardedAd()
    {
        #if UNITY_EDITOR
        if (gameManager.enablePremiumFeatures)
        {
            // Give the award right away for testing in the editor
            // Show the change ring button
            ShowChangeRingButton();
            RewardAfterWatchAd(5);  // for testing in the editor
        }
        #elif EASY_MOBILE
        // Show the change ring button
        ShowChangeRingButton();

        AdDisplayer.CompleteRewardedAd += OnCompleteRewardedAd;
        AdDisplayer.Instance.ShowRewardedAd();
        #endif
    }

    void OnCompleteRewardedAd()
    {
        #if EASY_MOBILE
        // Unsubscribe
        AdDisplayer.CompleteRewardedAd -= OnCompleteRewardedAd;
        RewardAfterWatchAd(AdDisplayer.Instance.rewardedRingSwaps);
        #endif
    }

    void RewardAfterWatchAd(int amount)
    {
        // Give the award!
        CoinManager.Instance.AddCoins(amount);
    }
}
