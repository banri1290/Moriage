using UnityEngine;
using UnityEngine.AI;
using System.Linq;

/// <summary>
/// チョビンを登録し、パラメータを管理するクラス
/// </summary>
public class ChobinsSetting : GameSystem
{
    [System.Serializable]
    class Chobin
    {
        public GameObject chobinObject;
        public Sprite icon;
        public Transform waitingSpot;

        public int InstanceID()
        {
            if (chobinObject == null) return 0;
            return chobinObject.GetInstanceID();
        }

        public bool CheckSettings(string instanceName)
        {
            bool settingsAreCorrect = true;
            if (chobinObject == null)
            {
                Debug.LogError(instanceName + ": Chobin is not assigned.");
                settingsAreCorrect = false;
            }
            else
            {
                if (!chobinObject.GetComponent<ChobinBehaviour>())
                {
                    Debug.LogError(instanceName + ": does not have a ChobinBehaviour component.");
                    settingsAreCorrect = false;
                }
                if (!chobinObject.GetComponent<ChobinCooking>())
                {
                    Debug.LogError(instanceName + ": does not have a ChobinCooking component.");
                    settingsAreCorrect = false;
                }
                if (!chobinObject.GetComponent<NavMeshAgent>())
                {
                    Debug.LogError(instanceName + ": does not have a NavMeshAgent component.");
                    settingsAreCorrect = false;
                }
            }
            if (waitingSpot == null)
            {
                Debug.LogError(instanceName + ": Waiting spot is not assigned.");
                settingsAreCorrect = false;
            }

            return settingsAreCorrect;
        }
    }

    [SerializeField] private Chobin[] chobins;
    [SerializeField] private float chobinSpeed;
    [SerializeField] private float chobinAccel;
    [SerializeField] private int cookingMethodLength;
    [SerializeField] private ChobinManager chobinManager;

    [HideInInspector]
    [SerializeField] private int[] chobinsInstanceID;
    [HideInInspector]
    [SerializeField] private ChobinBehaviour[] chobinBehaviours;
    [HideInInspector]
    [SerializeField] private ChobinCooking[] chobinCookings;

    public int ChobinLength => chobins.Length;
    public int CookingMethodLength => cookingMethodLength;

    public Sprite[] ChobinIcons => chobins.Select(x => x.icon).ToArray();

    /// <summary>
    /// Inspectorで設定された値が正しいかチェック
    /// </summary>
    public override bool CheckSettings()
    {
        bool settingsAreCorrect = true;
        if (chobins == null || chobins.Length == 0)
        {
            Debug.LogError("ChobinsSetting: No chobins defined.");
            settingsAreCorrect = false;
        }
        else
        {
            for (int i = 0; i < chobins.Length; i++)
            {
                if (chobins[i] == null)
                {
                    Debug.LogError($"ChobinsSetting: Chobin at index {i} is not assigned.");
                    settingsAreCorrect = false;
                }
                else if (!chobins[i].CheckSettings("chobins[" + i + "]"))
                {
                    settingsAreCorrect = false;
                }
            }
        }

        if (cookingMethodLength <= 0)
        {
            Debug.LogWarning("調理工程数が0以下です。調理工程数は1以上にしてください。");
            cookingMethodLength = 1;
        }

        if (chobinManager == null)
        {
            Debug.LogError("ChobinsSetting: ChobinManager is not assigned.");
            settingsAreCorrect = false;
        }

        return settingsAreCorrect;
    }

    /// <summary>
    /// ChobinManagerの参照を設定
    /// </summary>
    public void SetChobinManager(ChobinManager _chobinManager)
    {
        chobinManager = _chobinManager;
    }

    /// <summary>
    /// チョビンたちの初期化
    /// </summary>
    public void Init(Transform[][] kitchenLines, float[] cookingTime)
    {
        bool needChobinInitialize = false;
        if (chobinsInstanceID.Length != chobins.Length)
        {
            chobinsInstanceID = new int[chobins.Length];
            needChobinInitialize = true;
        }
        for (int i = 0; i < chobins.Length; i++)
        {
            if (chobinsInstanceID[i] != chobins[i].InstanceID())
            {
                chobinsInstanceID[i] = chobins[i].InstanceID();
                needChobinInitialize = true;
            }
        }
        if (needChobinInitialize)
        {
            InitChobinComponents();
        }

        chobinManager.Init(
            chobinBehaviours,
            chobinCookings,
            kitchenLines,
            cookingTime,
            cookingMethodLength
        );
    }

    private void InitChobinComponents()
    {
        Debug.Log("チョビンの初期化を実行します。");
        chobinBehaviours = new ChobinBehaviour[chobins.Length];
        chobinCookings = new ChobinCooking[chobins.Length];
        for (int i = 0; i < chobins.Length; i++)
        {
            chobinBehaviours[i] = chobins[i].chobinObject.GetComponent<ChobinBehaviour>();
            chobinBehaviours[i].SetChobinSettings(chobins[i].waitingSpot, chobinSpeed, chobinAccel);
            chobinCookings[i] = chobins[i].chobinObject.GetComponent<ChobinCooking>();
        }
    }
}
