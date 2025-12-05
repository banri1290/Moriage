using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UI;

public class GuestCtrl : GameSystem
{
    [Header("参照設定")]
    [Tooltip("生成する客のプレハブ")]
    [SerializeField] private GuestBehaviour[] guestPrefabs;
    [Tooltip("客の出現場所")]
    [SerializeField] private Transform spawnSpot;
    [Tooltip("客が注文をする場所")]
    [SerializeField] private Transform orderingSpot;
    [Tooltip("客が料理を待つ場所")]
    [SerializeField] private Transform waitingServeSpot;
    [Tooltip("客が退店する場所")]
    [SerializeField] private Transform exitSpot;

    [Header("客の出現と待機設定")]
    [Tooltip("注文待ちの列における客同士の間隔")]
    [SerializeField] private Vector3 waitingOrderOffset = new Vector3(0, 0, 1);
    [Tooltip("提供待ちの列における客同士の間隔")]
    [SerializeField] private Vector3 waitingServeOffset = new(0, 0, 1);
    [Tooltip("待機中の客が向く方向")]
    [SerializeField] private Vector3 waitingDirection = new(1, 0, 0);
    [Tooltip("客が出現する最短間隔（秒）")]
    [SerializeField] private float SpawnIntervalMin;
    [Tooltip("客が出現する最長間隔（秒）")]
    [SerializeField] private float SpawnIntervalMax;
    [Tooltip("ゲーム中に登場する客の総数")]
    [SerializeField] private int totalGuestNum;
    [Tooltip("店内に同時に存在できる客の最大数")]
    [SerializeField] private int maxGuestNum;

    [Header("客のパラメータ設定")]
    [Tooltip("客の移動速度")]
    [SerializeField] private float speed = 1f;

    List<GuestBehaviour> guestList = new();

    [Header("UI")]
    [SerializeField] private Sprite[] guestSprites;

    [Header("チョビンボタンUI")]
    [SerializeField] private Image chobinUIImage;
    [SerializeField] private Sprite[] chobinSprites;

    private bool isActive = false;
    private float spawnTimer = 0f;
    private bool hasGuestWaitingOrder = false;

    private int guestComeCounter;
    private int guestOrderCounter;
    private int guestExitCounter;

    private UnityEvent hasComeGuestEvent = new();
    private UnityEvent allGuestExitEvent = new();

    public UnityEvent HasComeGuest => hasComeGuestEvent;
    public UnityEvent AllGuestExit => allGuestExitEvent;

    public int WaitingGuestNum => guestOrderCounter - guestExitCounter;
    public bool HasGuestWaitingOrder => hasGuestWaitingOrder;

    void Update()
    {
        if (isActive)
        {
            CountSpawnTimer();
        }
    }

    public override bool CheckSettings()
    {
        if (SpawnIntervalMin < 0)
        {
            Debug.LogWarning("SpawnIntervalMinの値が不正です。0以上の値に修正します。");
            SpawnIntervalMin = 0;
        }
        if (SpawnIntervalMax < SpawnIntervalMin)
        {
            Debug.LogWarning("SpawnIntervalMaxの値が不正です。SpawnIntervalMinと同じ値に修正します。");
            SpawnIntervalMax = SpawnIntervalMin;
        }

        bool AllSettingsAreCorrect = true;
        if (guestPrefabs.Length == 0)
        {
            AllSettingsAreCorrect = false;
            Debug.LogError("guestPrefabsが設定されていません。");
        }
        else
        {
            for (int i = 0; i < guestPrefabs.Length; i++)
            {
                if (guestPrefabs[i] == null)
                {
                    AllSettingsAreCorrect = false;
                    Debug.LogError("guestPrefabs[" + i + "]が設定されていません。");
                }
            }
        }
        if (spawnSpot == null)
        {
            AllSettingsAreCorrect = false;
            Debug.LogError("spawnSpotが設定されていません。");
        }
        if (orderingSpot == null)
        {
            AllSettingsAreCorrect = false;
            Debug.LogError("orderingSpotが設定されていません。");
        }
        if (waitingServeSpot == null)
        {
            AllSettingsAreCorrect = false;
            Debug.LogError("waitingServeSpotが設定されていません。");
        }
        if (exitSpot == null)
        {
            AllSettingsAreCorrect = false;
            Debug.LogError("exitSpotが設定されていません。");
        }

        return AllSettingsAreCorrect;
    }

    public void Init()
    {
        isActive = true;
        guestComeCounter = 0;
        guestOrderCounter = 0;
        guestExitCounter = 0;
        hasGuestWaitingOrder = false;

        UpdateWaitingOrderGuestImage();

        Debug.Log("GuestCtrlの初期化が完了しました。");
    }

    private void CountSpawnTimer()
    {
        spawnTimer -= Time.deltaTime;
        bool timerIsUp = spawnTimer <= 0f;
        bool underTotalGuest = guestComeCounter < totalGuestNum;
        bool underMaxGuest = guestExitCounter - guestComeCounter < maxGuestNum;
        if (timerIsUp && underTotalGuest && underMaxGuest)
        {
            SpawnGuest();
        }
    }

    private void SpawnGuest()
    {
        int prefabIndex = Random.Range(0, guestPrefabs.Length);
        GuestBehaviour newGuest = Instantiate(guestPrefabs[prefabIndex], spawnSpot.position, Quaternion.identity);
        newGuest.PrefabIndex = prefabIndex; // 追加
        guestList.Add(newGuest);
        newGuest.SetSpeed(speed);
        newGuest.GuestEventInstance.RemoveAllListeners();
        newGuest.GuestEventInstance.AddListener(SendGuestMessage);
        newGuest.Init(guestComeCounter);

        int differenceCount = guestComeCounter - guestOrderCounter;
        newGuest.SetDestination(orderingSpot.position + waitingOrderOffset * differenceCount);

        guestComeCounter++;
        spawnTimer = Random.Range(SpawnIntervalMin, SpawnIntervalMax);

        //UpdateWaitingOrderGuestImage(); // 追加
    }

    public void ReceiveOrder()
    {
        guestList[guestOrderCounter].SetState(GuestBehaviour.Status.WaitingDish);
        for (int i = guestExitCounter; i < guestOrderCounter + 1; i++)
        {
            int differenceCount = i - guestExitCounter;
            guestList[i].SetDestination(waitingServeSpot.position + waitingServeOffset * differenceCount);
        }
        for (int i = guestOrderCounter + 1; i < guestComeCounter; i++)
        {
            int differenceCount = i - guestOrderCounter - 1;
            guestList[i].SetDestination(orderingSpot.position + waitingOrderOffset * differenceCount);
        }

        hasGuestWaitingOrder = false;
        guestOrderCounter++;

        //UpdateWaitingOrderGuestImage(); // 追加
           UpdateOrderTextDisplay(); // ← ここ追加
    }

    public void ServeDish()
    {
        guestList[guestExitCounter].SetState(GuestBehaviour.Status.GotDish);
        guestList[guestExitCounter].SetDestination(exitSpot.position);
        for (int i = guestExitCounter + 1; i < guestOrderCounter; i++)
        {
            int differenceCount = i - guestExitCounter - 1;
            guestList[i].SetDestination(waitingServeSpot.position + waitingServeOffset * differenceCount);
        }
        guestExitCounter++;

        //UpdateWaitingOrderGuestImage(); // 追加
        UpdateOrderTextDisplay(); // ← 追加
    }

    public GuestBehaviour GetServedGuest()
    {
        return guestList[guestExitCounter - 1];
    }

    public GuestBehaviour GetOrderingGuest()
    {
        return guestList[guestOrderCounter];
    }

    private void GuestOnCounter(int guestId)
    {
        GuestBehaviour guest = guestList[guestId];
        if (guestId == guestOrderCounter)
        {
            hasGuestWaitingOrder = true;
            hasComeGuestEvent.Invoke();
            guest.SetState(GuestBehaviour.Status.Ordering);
        }
        else if (guest.CurrentStatus == GuestBehaviour.Status.Entering)
        {
            guest.SetState(GuestBehaviour.Status.WaitingOrder);
        }
            UpdateOrderTextDisplay(); // ← 追加！
    }

    private void SendGuestMessage(int guestId)
    {
        GuestBehaviour guest = guestList[guestId];
        guest.SetDirection(waitingDirection);
        GuestBehaviour.Status status = guest.CurrentStatus;
        switch (status)
        {
            case GuestBehaviour.Status.None:
                break;
            case GuestBehaviour.Status.Entering:
                GuestOnCounter(guestId);
                guest.StartWaiting();
                break;
            case GuestBehaviour.Status.WaitingOrder:
                GuestOnCounter(guestId);
                break;
            case GuestBehaviour.Status.Ordering:
                break;
            case GuestBehaviour.Status.WaitingDish:
                break;
            case GuestBehaviour.Status.GotDish:
                if (guestId < guestExitCounter)
                {
                    Destroy(guest.gameObject);
                    guest = null;
                    if (guestExitCounter == totalGuestNum)
                        allGuestExitEvent.Invoke();
                }
                break;
            default:
                break;
        }
         UpdateOrderTextDisplay(); // ← 追加！（常に最新の表示にする）
    }

    /// <summary>
    /// 一番先に来て「注文待ち(WaitingOrder)」の客の画像をUIに表示
    /// </summary>
    private void UpdateWaitingOrderGuestImage()
    {
        Image guestUIImage = null;
        if (guestUIImage == null || guestSprites == null)
            return;

        // guestListの中で、状態がWaitingOrderの最も前の客を探す
        foreach (var guest in guestList)
        {
            if (guest != null && guest.CurrentStatus == GuestBehaviour.Status.WaitingOrder)
            {
                int prefabIndex = guest.PrefabIndex;
                if (prefabIndex >= 0 && prefabIndex < guestSprites.Length)
                {
                    guestUIImage.sprite = guestSprites[prefabIndex];
                }
                return;
            }
        }
        // 見つからなければ画像を消す
        guestUIImage.sprite = null;
    }

    public Sprite GetGuestSprite()
    {
        if(guestSprites == null || guestSprites.Length == 0)return null;
        int prefabIndex=GetOrderingGuest().PrefabIndex;
        return guestSprites[prefabIndex];
    }

    public void OnChobinButtonClicked(int index)
    {
        if (chobinUIImage == null || chobinSprites == null) return;
        if (index >= 0 && index < chobinSprites.Length)
        {
            chobinUIImage.sprite = chobinSprites[index];
        }
    }
/// <summary>
/// 先頭の客だけ注文テキストを表示し、後続は非表示にする
/// </summary>
private void UpdateOrderTextDisplay()
{
    for (int i = 0; i < guestList.Count; i++)
    {
        if (guestList[i] == null) continue;

        if (i == guestOrderCounter) // 注文対象の客（最前）
        {
            guestList[i].ShowOrderText();
        }
        else
        {
            guestList[i].HideOrderText();
        }
    }
}
}
