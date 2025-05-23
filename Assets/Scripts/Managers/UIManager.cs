using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.Properties;
using Photon.Pun;
using Photon.Realtime;

public enum PotionType
{
    HpPotion = 0,
    AttackPotion = 4,
    ShieldPotion = 5,
    RandomPotion = 7
};

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // StartScene 관련
    [Header("About StartScene")]
    [SerializeField]
    public GameObject startBtn;
    [SerializeField]
    public GameObject endBtn;
    [SerializeField]
    public GameObject LoadBtn;
    [SerializeField]
    public GameObject LoadFilePanel;

    // MainScene 관련
    [Header("About MainScene")]
    [SerializeField]
    public GameObject playerInfoBtn;
    [SerializeField]
    public GameObject EnterDungeonBtn;
    [SerializeField]
    public GameObject MainMenuBtn;
    [SerializeField]
    public GameObject SaveBtn;
    [SerializeField]
    public GameObject NetworkBtn;

    [Header("About Intro in MainScene")]
    [SerializeField]
    public GameObject IntroPanel;
    [SerializeField]
    public TextMeshProUGUI IntroText;
    [SerializeField]
    public GameObject IntroInputField;
    [SerializeField]
    public GameObject LastText;
    [SerializeField]
    GameObject PlayerInfoPanel;

    // BattleScene 관련
    [Header("About BattleScene")]
    [SerializeField]
    public GameObject ExitDungeonBtn;
    [SerializeField]
    public GameObject InventoryBtn;
    [SerializeField]
    public GameObject AttackBtn;

    [SerializeField]
    public TextMeshProUGUI BattleContext;
    [SerializeField]
    public GameObject CharacterInfoObject;
    [SerializeField]
    public TextMeshProUGUI PlayerNameText;
    [SerializeField]
    public TextMeshProUGUI EnemyNameText;
    [SerializeField]
    public GameObject PlayerSlider;
    [SerializeField]
    public GameObject EnemySlider;
    [SerializeField]
    public GameObject PlayerExpSlider;
    [SerializeField]
    public GameObject LevelText;

    [SerializeField]
    public GameObject NextChoicePanel;
    [SerializeField]
    public GameObject EndGamePanelPrefab;
    [SerializeField]
    public GameObject EndGamePanel;

    // Lobby Scene 관련
    [Header("About LobbyScene")]
    [SerializeField]
    GameObject Content;
    [SerializeField]
    GameObject JoinRoomBtnPrefab;
    [SerializeField]
    GameObject CreateRoomPanelBtn;
    [SerializeField]
    GameObject ExitLobbyBtn;

    GameObject RoomPanel;
    GameObject CreateRoomPanel;
    GameObject CreateRoomBtn;
    GameObject CancelCreateRoomBtn;
    GameObject RoomNameInputField;
    GameObject JoinRoomContent;
    GameObject CountUserText;
    GameObject PlayerInfoTextInLobby;
    TMP_InputField InputChatField;
    TextMeshProUGUI ChatText;

    TextMeshProUGUI[] userListText;

    GameObject _inventory;
    // Inventory 관련
    public GameObject Inventory { 
        get { return _inventory; }
        private set { _inventory = value; }
    }
    public GameObject ItemButton;
    GameObject _buttonPanel;
    GameObject buttonPrefab;
    Sprite[] potionSprites;

    public bool IsUsedSliderEffect = false;

    public bool IsSceneLoaded = false;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            buttonPrefab = Resources.Load<GameObject>("Prefabs/ItemButton");
            EndGamePanelPrefab = Resources.Load<GameObject>("Prefabs/EndGamePanel");
            JoinRoomBtnPrefab = Resources.Load<GameObject>("Prefabs/JoinRoomButton");
        }
    }
    private void Start()
    {
        PoolingManager.Instance.AddInMap(PoolingType.JoinRoomBtn, JoinRoomBtnPrefab);
        // TODO : 나중에 고칠 것.
        PoolingManager.Instance.AddInMap(PoolingType.InventoryItemBtn, buttonPrefab);

        NetworkManager.Instance.OnChangedPlayerCount += UpdateLobbyUserText;
    }

    public void LoadFilePanelInit()
    {
        Button[] buttons = LoadFilePanel.GetComponentsInChildren<Button>();
        SaveLoadManager.Instance.LoadSaveFiles(buttons);

        // 현재 기준으론  cancel버튼임
        buttons[3].onClick.AddListener(() => { LoadFilePanel.SetActive(false); });

        LoadFilePanel.SetActive(false);
    }

    public void SetStartSceneUI()
    {
        IsSceneLoaded = false;

        startBtn = GameObject.Find("StartBtn");
        endBtn = GameObject.Find("EndBtn");
        LoadBtn = GameObject.Find("LoadBtn");
        LoadFilePanel = GameObject.Find("LoadFilePanel");
        if (startBtn == null || endBtn == null || LoadBtn == null || LoadFilePanel == null)
        {
            Debug.Log("Error Find Buttons in SetStartSceneUI"); return;
        }

        startBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.OnStartButton);
        endBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.OnEndButton);
        LoadBtn.GetComponent<Button>().onClick.AddListener(() => LoadFilePanel.SetActive(true));
        
        LoadFilePanelInit();

        AdjustLineSpacing();
    }

    public void SetMainSceneUI()
    {
        IsSceneLoaded = false;

        playerInfoBtn = GameObject.Find("PlayerInfoBtn");
        EnterDungeonBtn = GameObject.Find("EnterDungeonBtn");
        MainMenuBtn = GameObject.Find("MainMenuBtn");
        SaveBtn = GameObject.Find("SaveBtn");
        NetworkBtn = GameObject.Find("EnterNetwork");

        IntroPanel = GameObject.Find("IntroPanel");
        IntroText = GameObject.Find("IntroText").GetComponent<TextMeshProUGUI>();
        IntroInputField = GameObject.Find("InputField");
        IntroInputField.SetActive(false);
        LastText = GameObject.Find("LastText");
        LastText.SetActive(false);

        if (playerInfoBtn == null || EnterDungeonBtn == null ||
            MainMenuBtn == null || IntroPanel == null ||
            IntroText == null || IntroInputField == null ||
            LastText == null || SaveBtn == null || NetworkBtn == null)
        {
            Debug.Log("Error Find Buttons in SetMainSceneUI"); return;
        }

        playerInfoBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.PrintPlayerInfo);
        EnterDungeonBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.MoveDungeon);
        MainMenuBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.OnMainMenuButton);
        SaveBtn.GetComponent<Button>().onClick.AddListener(SaveLoadManager.Instance.SaveGame);
        NetworkBtn.GetComponent<Button>().onClick.AddListener(() => 
        {
            if (GameManager.Instance.IsPassed)
                NetworkManager.Instance.Connect();
            else
                Debug.Log("먼저 싱글플레이를 통과해야함");
        });
        IntroInputField.GetComponent<TMP_InputField>().onEndEdit.AddListener(CompleteInputName);

        //IntroInputField.SetActive(false);

        if(GameManager.Instance.IsLoadData==false)
        {
            UtilTextManager.Instance.PrintStringByTick(UtilTextManager.IntroMainScene, 0.05f, IntroText,
            () => { IntroInputField.SetActive(true); GameManager.Instance.IsLoadData = true; });
        }
        else
        {
            IntroPanel.SetActive(false);
        }
            

        AdjustLineSpacing();
    }

    void CompleteInputName(string s)
    {
        IntroInputField.SetActive(false);
        GameManager.Instance.OnEndEditIntroInputField(s);
        LastText.SetActive(true);
        UtilTextManager.Instance.PrintStringByTick($"{s}님 환영합니다!", 0.1f,
            LastText.GetComponent<TextMeshProUGUI>(),
            () => { StartCoroutine(FadeInOutCo(IntroPanel, 1f,1,0,false)); });
    }

    public void SetBattleSceneUI()
    {
        IsSceneLoaded = false;

        potionSprites = Resources.LoadAll<Sprite>("Image/Portions");

        GameObject buttonsPanel = GameObject.Find("ButtonsPanel");
        BattleContext = GameObject.Find("BattleContext").GetComponent<TextMeshProUGUI>();
        PlayerNameText = GameObject.Find("PlayerText").GetComponent<TextMeshProUGUI>();
        PlayerNameText.GetComponent<HoverText>().SetPanel(GameManager.Instance.Player);

        EnemyNameText = GameObject.Find("EnemyText").GetComponent<TextMeshProUGUI>();
        EnemyNameText.GetComponent<HoverText>().SetPanel(GameManager.Instance.GetCurMonster());

        CharacterInfoObject = GameObject.Find("CharacterInfo");
        PlayerSlider = GameObject.Find("PlayerSlider");
        EnemySlider = GameObject.Find("EnemySlider");
        PlayerExpSlider = GameObject.Find("PlayerExpSlider");
        LevelText = GameObject.Find("LevelText");

        CharacterInfoObject.SetActive(false);

        if (buttonsPanel == null || BattleContext == null
            || PlayerNameText == null || EnemyNameText == null ||
            PlayerSlider == null || EnemySlider == null ||
            PlayerExpSlider == null || LevelText == null)
        {
            Debug.Log("SetBattleSceneUI Error");
            return;
        }
        
        ExitDungeonBtn = buttonsPanel.transform.Find("ExitDeungeonBtn").gameObject;
        ExitDungeonBtn.GetComponent<Button>().onClick.AddListener(BattleManager.Instance.OnExitButton);

        InventoryBtn = buttonsPanel.transform.Find("InventoryBtn").gameObject;
        InventoryBtn.GetComponent<Button>().onClick.AddListener(BattleManager.Instance.OnInventoryButton);

        AttackBtn = buttonsPanel.transform.Find("AttackBtn").gameObject;
        AttackBtn.GetComponent<Button>().onClick.AddListener(BattleManager.Instance.OnAttackButton);

        //ExitDungeonBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.MoveTown);

        GameObject choicebox = CreateItemUI("Prefabs/ChoiceBox");
        Button[] buttons = choicebox.GetComponentsInChildren<Button>();
        choicebox.SetActive(false);

        buttons[0].onClick.AddListener(() => { choicebox.SetActive(false); GameManager.Instance.PlayDungeon(GameManager.Instance.Player); });
        buttons[1].onClick.AddListener(()=> { choicebox.SetActive(false); GameManager.Instance.MoveTown();  });
        choicebox.transform.SetParent(GameObject.Find("Panels").transform, false);

        UtilTextManager.Instance.PrintStringByTick(UtilTextManager.EnterDungeon, 0.05f, BattleContext, () => { choicebox.SetActive(true); });
        UpdateUI();


        if (_inventory == null) _inventory = CreateInventory();
        ItemManager.Instance.OnUsedItem += CreateInventoryPanel;
        
        AdjustLineSpacing();
    }


    #region About Lobby

    public void OnAddRoomUI()
    {

    }

    public void SetLobbySceneUI()
    {
        IsSceneLoaded = true;

        ExitLobbyBtn = GameObject.Find("ExitLobbyBtn");
        CreateRoomPanelBtn = GameObject.Find("CreateRoomPanelBtn");

        RoomPanel = GameObject.Find("RoomPanel");

        CreateRoomPanel = GameObject.Find("CreateRoomPanel");
        CreateRoomBtn = GameObject.Find("CreateRoomBtn");
        CancelCreateRoomBtn = GameObject.Find("CancelCreateRoomBtn");
        RoomNameInputField = GameObject.Find("RoomNameInputField");

        JoinRoomContent = GameObject.Find("JoinRoomContent");
        CountUserText = GameObject.Find("CountUserText");
        PlayerInfoTextInLobby = GameObject.Find("PlayerInfoText");

        if (ExitLobbyBtn == null || CreateRoomPanelBtn == null ||
            CreateRoomPanel == null || CreateRoomBtn == null ||
            CancelCreateRoomBtn == null || RoomNameInputField == null
            || RoomPanel == null || JoinRoomContent == null ||
            CountUserText == null || PlayerInfoTextInLobby == null)
        {
            Debug.LogError("Null Error in SetLobbySceneUI()");
            return;
        }
        RoomPanel.SetActive(false);
        // 메인메뉴로 돌아가기 버튼
        ExitLobbyBtn.GetComponent<Button>().onClick.AddListener(
            () => { NetworkManager.Instance.DisConnect(); }
            );
        // 방 패널 만들기 버튼
        CreateRoomPanelBtn.GetComponent<Button>().onClick.AddListener(
            () => { 
                CreateRoomPanel.SetActive(true);
                RoomNameInputField.GetComponent<TMP_InputField>().text = "";
            });

        // 방만들기 패널 관련
        CreateRoomPanel.SetActive(false);
        CreateRoomBtn.GetComponent<Button>().onClick.AddListener(OnCreateRoomBtn);
        CancelCreateRoomBtn.GetComponent<Button>().onClick.AddListener(() => { CreateRoomPanel.SetActive(false); });

        PlayerInfoTextInLobby.GetComponent<TextMeshProUGUI>().text = $"Lv{GameManager.Instance.Player.Level} {GameManager.Instance.Player.Name}";
        PlayerInfoTextInLobby.GetComponent<HoverText>().SetPanel(GameManager.Instance.Player);
    }

    public void UpdateLobbyUserText(int count)
    {
        CountUserText.GetComponent<TextMeshProUGUI>().text = $"접속 인원 : {count}명";
        Canvas.ForceUpdateCanvases();
    }

    public void UpdateRoomListUI(List<RoomInfo> roomInfos)
    {
        // 우선 사용할 곳이 여기뿐이니까 
        PoolingManager.Instance.InActiveAllItemByType(PoolingType.JoinRoomBtn);

        foreach(var roomInfo in roomInfos)
        {
            if (roomInfo.RemovedFromList) continue;
            // 여기서 추가하기
            ItemStruct item = PoolingManager.Instance.GetItem(PoolingType.JoinRoomBtn);
            item.gameObject.GetComponentInChildren<TextMeshProUGUI>().text =
                $"{roomInfo.Name}  {roomInfo.PlayerCount}/{roomInfo.MaxPlayers}";

            item.gameObject.GetComponent<Button>().onClick.AddListener(() => NetworkManager.Instance.JoinRoom(roomInfo.Name));
            item.gameObject.transform.SetParent(JoinRoomContent.transform);
            item.gameObject.SetActive(true);
        }
    }

    public void OnCreateRoomBtn()
    {
        NetworkManager.Instance.CreateRoom(RoomNameInputField.GetComponent<TMP_InputField>().text);
        RoomNameInputField.GetComponent<TMP_InputField>().text = "";
        CreateRoomPanel.SetActive(false);
    }

    public void SetJoinRoomUI(string roomName)
    {
        CreateRoomPanel.SetActive(false);
        RoomPanel.SetActive(true);
        GameObject InRoomTitleText = GameObject.Find("InRoomTitleText");
        InRoomTitleText.GetComponent<TextMeshProUGUI>().text = roomName;

        InputChatField = GameObject.Find("InputChatField").GetComponent<TMP_InputField>();
        InputChatField.onSubmit.AddListener((message) => 
        { 
            NetworkManager.Instance.OnChatSubmit(message);
            InputChatField.text = "";
            InputChatField.ActivateInputField();
        });

        ChatText = GameObject.Find("ChatText").GetComponent<TextMeshProUGUI>();
        ChatText.text = "";

        GameObject StartBtn = GameObject.Find("StartButton");
        GameObject ReadyBtn = GameObject.Find("ReadyButton");
        // 방장인가
        if (PhotonNetwork.IsMasterClient)
        {
            ReadyBtn.SetActive(false);
            StartBtn.GetComponent<Button>().onClick.AddListener(() => 
            { 
                Debug.Log("게임을 시작합니다!");
                NetworkManager.Instance.OnStartButton();
            });
            StartBtn.GetComponent<Button>().interactable = false;
            UpdateStartButton();
        }
        else
        {
            StartBtn.SetActive(false);
            ReadyBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (GameManager.Instance.IsReady == false)
                {
                    GameManager.Instance.IsReady = true;
                    NetworkManager.Instance.OnReadyPlayer();
                }
                else
                {
                    GameManager.Instance.IsReady = false;
                    NetworkManager.Instance.OnCancelReadyPlayer();
                }
            });
        }

        
        GameObject exitBtn = GameObject.Find("ExitButton");
        exitBtn.GetComponent<Button>().onClick.AddListener(() => 
        { 
            RoomPanel.SetActive(false);
            NetworkManager.Instance.LeaveRoom();
            ReadyBtn.SetActive(true);
        });

       userListText = GameObject.Find("UserList").GetComponentsInChildren<TextMeshProUGUI>();
        int count = 0;

        foreach (var user in userListText)
        {
            user.text = "";
        }

        foreach(var p in PhotonNetwork.PlayerList)
        {
            if(p.IsMasterClient)
            {
                userListText[count].text = "★";
            }
            userListText[count].text += p.NickName;count++;
        }

        Canvas.ForceUpdateCanvases();
    }

    public void OnUpdatePlayerListInRoom(bool state, Photon.Realtime.Player player)
    {
        // 입장
        if(state == true)
        {
            foreach(var text in userListText)
            {
                if(text.text=="")
                {
                    text.text = player.NickName;
                    break;
                }
            }
        }
        else
        {
            foreach (var text in userListText)
            {
                if(text.text == player.NickName)
                {
                    text.text = "";
                    break;
                }
            }
        }
        if(PhotonNetwork.IsMasterClient)
        {
            UpdateStartButton();
        }
    }

    public void OnReadyPlayer(Photon.Realtime.Player player)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            NetworkManager.Instance.CountReadyPlayer++;
            UpdateStartButton();
        }

        foreach (var text in userListText)
        {
            if (text.text == player.NickName)
            {
                text.color = Color.red;
                break;
            }
        }
    }

    public void OnCancelReadyPlayer(Photon.Realtime.Player player)
    {
        if(PhotonNetwork.IsMasterClient)
            NetworkManager.Instance.CountReadyPlayer--;

        foreach (var text in userListText)
        {
            if (text.text == player.NickName)
            {
                text.color = Color.white;
                break;
            }
        }
    }

    public void UpdateStartButton()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            GameObject StartBtn = GameObject.Find("StartButton");
            if (NetworkManager.Instance.CountReadyPlayer == PhotonNetwork.CurrentRoom.PlayerCount - 1)
                StartBtn.GetComponent<Button>().interactable = true;
            else
                StartBtn.GetComponent<Button>().interactable = false;
        }
    }

    public void OnSubmittedMessage(string m)
    {
        // 대화창 업데이트
        ChatText.text += m+"\n";
    }

    #endregion


    public GameObject CreateItemUI(string address)
    {
        GameObject item = Resources.Load<GameObject>(address);
        GameObject gameObject = Instantiate(item);

        return gameObject;
    }

    public void UpdateUI()
    {
        Monster monster = GameManager.Instance.GetCurMonster();
        PlayerNameText.text = GameManager.Instance.Player.Name;
        PlayerSlider.GetComponentInChildren<Slider>().value = 
            (float)GameManager.Instance.Player.Hp / GameManager.Instance.Player.MaxHp;
        PlayerSlider.GetComponentInChildren<TextMeshProUGUI>().text = GameManager.Instance.Player.Hp.ToString();

        PlayerExpSlider.GetComponentInChildren<Slider>().value =
            (float)GameManager.Instance.Player.Exp / GameManager.Instance.Player.MaxExp;
        PlayerExpSlider.GetComponentInChildren<TextMeshProUGUI>().text = 
            $"{GameManager.Instance.Player.Exp.ToString()}";

        LevelText.GetComponent<TextMeshProUGUI>().text = $"Lv {GameManager.Instance.Player.Level.ToString()}";

        EnemyNameText.text = monster.Name;
        EnemySlider.GetComponentInChildren<Slider>().value =
            (float)monster.Hp / monster.MaxHp;
        EnemySlider.GetComponentInChildren<TextMeshProUGUI>().text = monster.Hp.ToString();


        EnemyNameText.GetComponent<HoverText>().SetPanel(GameManager.Instance.GetCurMonster());

        CharacterInfoObject.SetActive(true);
    }
    
    public void UpdateCanvas(ScrollRect rect = null)
    {
        Canvas.ForceUpdateCanvases();
        if(rect != null)
            rect.verticalNormalizedPosition = 0f;
    }

    public IEnumerator FadeInOutCo(GameObject panel, float duration,int startValue,int targetValue,bool b)
    {
        CanvasGroup i = panel.GetComponent<CanvasGroup>();
        if (i == null)
        {
            Debug.LogError("Null Error received Object In FadeInOutCo");
            yield break;
        }
        float time = 0f;

        while(time < duration)
        {
            time += Time.deltaTime;
            i.alpha = Mathf.Lerp(startValue, targetValue , time / duration);
            yield return null;
        }
        panel.SetActive(b);
    }
    
    // slider 와 text를 동시에 바꿈
    public IEnumerator SliderEffect(int start, int end, int maxValue, GameObject slider,float duration = 1, Action action = null)
    {
        IsUsedSliderEffect = true;
        Slider s = slider.GetComponentInChildren<Slider>();
        TextMeshProUGUI t = slider.GetComponentInChildren<TextMeshProUGUI>();

        if (s == null || t == null)
        {
            yield break;
        }

        float startValue = s.value;
        float elapsedTime = 0f;

        end = (end < maxValue) ? end : maxValue;
        Debug.Log($"In SliderEffect End : {end}");

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            t.text = ((int)Mathf.Lerp(start, end, Mathf.Clamp01(elapsedTime / duration))).ToString();
            
            s.value = Mathf.Lerp(startValue, (float)end / maxValue, Mathf.Clamp01(elapsedTime / duration));
            
            yield return null;
        }
        Canvas.ForceUpdateCanvases();
        action?.Invoke();
        IsUsedSliderEffect = false;
    }

    public GameObject CreateNextChoicePanel()
    {
        if (NextChoicePanel != null) return null;
        GameObject panelPrefab = Resources.Load<GameObject>("Prefabs/NextChoiceBox");
        GameObject panel = Instantiate(panelPrefab);
        panel.transform.SetParent(GameObject.Find("Panels").transform, false);

        NextChoicePanel = panel;

        
        Button[] buttons = panel.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => { GameManager.Instance.OnContinueButton();Destroy(panel); } );
        buttons[1].onClick.AddListener(()=> { GameManager.Instance.OnMoveTownAfterDungeonButton(); Destroy(panel); } );
        buttons[2].onClick.AddListener(()=> { GameManager.Instance.OnExploreButton(); Destroy(panel); } );

        return panel;
    }

    public GameObject CreatePlayerInfoPanel()
    {
        if (PlayerInfoPanel != null) return PlayerInfoPanel;
        GameObject playerInfoPanelPrefab = Resources.Load<GameObject>("Prefabs/PlayerInfoPanel");
        GameObject panel = Instantiate(playerInfoPanelPrefab);
        panel.transform.SetParent(GameObject.Find("Panels").transform,false);
        Player p = GameManager.Instance.Player;
        panel.GetComponentInChildren<Button>().onClick.AddListener(() => { Destroy(panel); });
        TextMeshProUGUI text = panel.GetComponentInChildren<TextMeshProUGUI>();
        text.text += p.TranslateInfoToString();
        // 플레이어정보출력에서 인벤토리 목록도 출력하기
        text.text += "\n 인벤토리 목록\n";
        foreach(var item in ItemManager.Instance.Inventory)
        {
            text.text += $"-{item.Key} : {item.Value.Count}\n";
        }

        text.color = Color.black;
        PlayerInfoPanel = panel;
        return PlayerInfoPanel;
    }

    public void DeactiveChoiceButtons()
    {
        ExitDungeonBtn.GetComponent<Button>().interactable = false;
        InventoryBtn.GetComponent<Button>().interactable = false;
        AttackBtn.GetComponent<Button>().interactable = false;

        Color32 c = AttackBtn.transform.parent.GetComponent<Image>().color;
        c.a = 100;
        AttackBtn.transform.parent.GetComponent<Image>().color = c;
    }

    public void ActiveChoiceButtons()
    {
        ExitDungeonBtn.GetComponent<Button>().interactable = true;
        InventoryBtn.GetComponent<Button>().interactable = true;
        AttackBtn.GetComponent<Button>().interactable = true;

        Color32 c = AttackBtn.transform.parent.GetComponent<Image>().color;
        c.a = 10;
        AttackBtn.transform.parent.GetComponent<Image>().color = c;
    }

    GameObject CreateInventory()
    {
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/InventoryPanel");
        Inventory = Instantiate(Prefab);
        Inventory.SetActive(true);
        Inventory.transform.SetParent(GameObject.Find("Panels").transform, false);

        Button closeBtn = Inventory.GetComponentInChildren<Button>();
        if(closeBtn == null)
        {
            Debug.Log("CreateInventory CloseButton Not Found Error!");return null;
        }

        closeBtn.onClick.AddListener(CloseInventory);


        // 인벤토리 목록 만들기
        //GameObject go = CreateInventoryPanel();
        if (_buttonPanel == null)
            _buttonPanel = GameObject.Find("ButtonPanel");
        Inventory.SetActive(false);
        return Inventory;
    }

    // 나중에 버튼 생성을 더 효율적으로 -> 미리 만들어놓고 사용하는 오브젝트 풀링기법 적용하기
    public void CreateInventoryPanel(Item item)
    {
        // Find 함수는 활성화일때만 찾을수있음

        StartCoroutine(updateInvenPanel());
        //LayoutRebuilder.ForceRebuildLayoutImmediate(Inventory.GetComponent<RectTransform>());
        Debug.Log("UpdateInventory");
        //return ButtonPanel;
    }

    public Sprite GetSpriteByItemKey(string type)
    {
        if (Enum.TryParse(type, out PotionType potionType))
        {
            return potionSprites[(int)potionType]; // 변환 성공 시 해당 인덱스 사용
                                                              //Debug.Log($"Create Button : {item.Key} - {(int)potionType}");
        }
        else
        {
            Debug.LogWarning($"알 수 없는 포션 타입: {type}");
            return null;
        }
    }

    IEnumerator updateInvenPanel()
    {
        Button[] children = _buttonPanel.GetComponentsInChildren<Button>();
        if (children.Length > 0)
        {
            foreach (var item in children) Destroy(item.gameObject);
        }

        yield return null;

        
        foreach (var item in ItemManager.Instance.Inventory)
        {
            GameObject button = Instantiate(buttonPrefab);
            button.SetActive(true);
            button.transform.SetParent(_buttonPanel.transform, false);

            // 우선 버튼 프리팹 특성 상 두개만 존재하기에 자식에 인덱스로 이렇게 접근함.
            Image[] image = button.GetComponentsInChildren<Image>();

            Sprite sprite = GetSpriteByItemKey(item.Key);
            image[1].sprite = sprite;

            button.GetComponentInChildren<TextMeshProUGUI>().text = $"{item.Value[0].Name} : {item.Value.Count}개";
            button.GetComponent<Button>().onClick.AddListener(
                () => { ItemManager.Instance.UsedItem(item.Key, GameManager.Instance.Player); });

            button.GetComponent<HoverText>().SetPanel(item.Value[0]);
        }
        yield return null;
        Inventory.SetActive(true);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(Inventory.GetComponent<RectTransform>());
        //LayoutRebuilder.ForceRebuildLayoutImmediate(_buttonPanel.GetComponent<RectTransform>());
    }

    public void OpenInventory()
    {
        // 이 창만 클릭되게
        // 만약 여러창이 겹쳐서 쌓이면 오류 발생가능성 있을듯
        
        Debug.Log("OpenInventory");
        
        GameObject.Find("Panels").GetComponent<CanvasGroup>().blocksRaycasts = false;
        Inventory.GetComponent<CanvasGroup>().blocksRaycasts = true;
        Inventory.GetComponent<CanvasGroup>().ignoreParentGroups = true;
        Inventory.GetComponent<RectTransform>().anchoredPosition = new Vector2(800,0);
        // TODO : null 고쳐야함
        CreateInventoryPanel(null);
       
    }

    public void CloseInventory()
    {
        Debug.Log("CLoseInventory");
        GameObject.Find("Panels").GetComponent<CanvasGroup>().blocksRaycasts = true;
        //Inventory.GetComponent<CanvasGroup>().blocksRaycasts = false;
        Inventory.SetActive(false);
    }

    void AdjustLineSpacing()
    {
        TextMeshProUGUI[] items = FindObjectsOfType<TextMeshProUGUI>();
        foreach(var item in items)
        {
            item.lineSpacing = UISetting.LineSpace;
        }
    }

    public void EndGameUI()
    {
        EndGamePanel = Instantiate(EndGamePanelPrefab);

        GameObject go = GameObject.Find("Panels");
        EndGamePanel.transform.SetParent(go.transform,false);

        EndGamePanel.GetComponentInChildren<Button>().onClick.AddListener(GameManager.Instance.MoveTown);
    }
}