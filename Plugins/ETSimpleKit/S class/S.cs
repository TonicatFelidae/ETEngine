using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ET.SupportKit;
using TMPro;
using UnityEngine.UI;
using ETSimpleKit;
using ET.SupportKit.ETRegion;
using ET;
using UIType = ETSimpleKit.UIType;
using ET.UIKit;
using ETSimpleKit.EffectSystem;
using ETSimpleKit.SoundSystem;

public class S : MonoBehaviour
{
    #region S singleton
    private static S _instance;
    public static S Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<S>();

                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<S>();
                }
            }
            return _instance;
        }
    }
    private void InstanceInitiationProtocol()
    {
        int count = 0;
        if (_instance != null && _instance != this)
        {
            count += 1;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        count += 1;
        if (count > 1)
        {
            this.LogError($"Instances = {count}. Should only one S instance exist");
        }
    }
    #endregion
    #region General classes
    [Header("Audio")]
    private SoundGeneral _soundGeneral;
    public SoundGeneral SoundGeneral
    {
        get
        {
            if (_soundGeneral == null) _soundGeneral = GameObject.FindAnyObjectByType<SoundGeneral>(FindObjectsInactive.Include);
            return _soundGeneral;
        }
    } 
    [Header("Effect")]
    private EffectGeneral _effectManager;
    public EffectGeneral EffectManager
    {
        get
        {
            if (_effectManager == null) _effectManager = GameObject.FindAnyObjectByType<EffectGeneral>(FindObjectsInactive.Include);
            return _effectManager;
        }
    }
    #endregion
    [Header("GameEffects")]
    public ParticleSystem ef0;
    public ParticleSystem ef1;
    public ParticleSystem ef2;
    public SpriteRenderer cardSpawnBox;
    public SpriteRenderer winBox;
    public Color[] playerCardTextColor = new Color[2];
    public TextMeshProUGUI textMeshR;
    public Image iconR;
    public float zoffset = 0;

    public bool IsDragging = false;
    [Header("Win")]
    public GameObject winPopup;
    public GameObject[] winTexts;
    // public DBHBase trashBin;
    // public DBHBase walletBin;
    public GameObject world;
    public ToggleGroup toggleGroup0;
    public ToggleGroup toggleGroup1;
    public TextMeshProUGUI textScore0;
    public TextMeshProUGUI textScore1;
    //public CounterUIAnimationController counter;
    public ScoreMachine scoreMachine = new();
    //GamePlay
    #region Counter
    public TextMeshProUGUI txCount;
    private int _countNum;
    public int CountNum
    {
        get => _countNum;
        set
        {
            _countNum = value;
            txCount.text = _countNum.ToString();
            BestCountNum = _countNum;
        }
    }
    public TextMeshProUGUI txBestCount;
    private int _bestCountNum;
    public int BestCountNum
    {
        get => _bestCountNum;
        set
        {
            if (value> _bestCountNum)
            {
                _bestCountNum = value;
                txBestCount.text = _bestCountNum.ToString();
            }
        }
    }

    #endregion

    public TextMeshProUGUI txTime;
    private int counterState = 0;
    public ETimeData timeData;
    public ETime eTime;

    public int levelID = 0;

    public TextMeshProUGUI playTimeText;
    public TextMeshProUGUI resultTimeText;


    [SerializeField] private SpriteRenderer background;
    [SerializeField] private Sprite[] BGs;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        InstanceInitiationProtocol();
    }
    private void Start()
    {
        timeData.GenerateMinuteSecTickTF();
        eTime = new ETime(timeData);
        eTime.AddCounter(new ETimeCounterData(300).AddEvent(End));
        S.Instance.SoundGeneral.PlayBG0(); 

    }
    private void Update()
    {
        //eTime.Flow();
        //if (eTime.tick)
        //{
        //    txTime.text = eTime.GetTimeString(ETime.ETimeStringType.Counter);
        //}
        //TimeRender();

    }
    public void Play(int levelIndex,int levelData)
    {
        levelID = levelIndex;
        D.Log($"Play with {levelID} and {levelData}");
        EffectManager.ShowBPE(1, BPEShowType.Single);
        EffectManager.ShowBPE(2, BPEShowType.Additional);
        txCount.gameObject.SetActive(false);
        CountNum = 0;
        counterState = 0;
        //counter.PlayCounterAnimation();
        //eTime.StartAt(levelData*60);
        eTime.ResetCounter(0);
        eTime.Pause();
        txTime.text = eTime.GetTimeString(levelData * 60);
        _bestCountNum = PlayerPrefs.GetInt(levelID.ToString());
        txBestCount.text = _bestCountNum.ToString();
    }
    public void StartGamingCounterGo()
    {
        counterState = 1;

    }

    public void StartGamingCounterEnd()
    {
        counterState = 2;
        txCount.gameObject.SetActive(true);
        eTime.Resume();
        //for (int i = 0; i < cardList0.Count; i++)
        //{
        //    var item = cardList0[i];
        //
        //    if (item != null && item.gameObject != null)
        //    {
        //        item.FlipUp();
        //        item.isClickable = true;
        //    }
        //}
        //for (int i = 0; i < cardList1.Count; i++)
        //{
        //    var item = cardList1[i];
        //
        //    if (item != null && item.gameObject != null)
        //    {
        //        item.FlipUp();
        //        item.isClickable = true;
        //    }
        //}
    }

    public void End()
    {
        counterState = 0; 
        PlayerPrefs.SetInt(levelID.ToString(), BestCountNum);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////// GAME LOGIC /////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnTap()
    {
        TapEffect();
        switch (counterState)
        {
            case 0:
                break;
            case 1:
                //counter.ForceCounterEnd();
                TapCount();
                break;
            case 2:
                TapCount();
                break;
        }
    }
    void TapEffect()
    {
        EffectManager.GetBPE(2).Emit(1);
        SoundGeneral.PlayEF3D(0, Random.Range(0.9f, 1.1f));

    }
    void TapCount()
    {
        CountNum += 1;

    }

    public void SetBG(int index)
    {
        background.sprite = BGs[index];
    }
    

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////// GAME LOGIC END ////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
