using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ChobinSetting : GameSystem
{
    [Header("チョビンの設定")]
    [Tooltip("シーンに存在するすべてのチョビンのリスト")]
    [SerializeField] private ChobinBehaviour[] chobins;
    [Header("チョビンの待機場所")]
    [Tooltip("チョビンが命令を待つ場所")]
    [SerializeField] private Transform WaitingSpot;
    [Tooltip("チョビンが配膳を実行する場所")]
    [SerializeField] private Transform ServingSpot;

    [Header("チョビンのパラメータ")]
    [Tooltip("チョビンの移動速度")]
    [SerializeField] private float chobinSpeed;
    [Tooltip("チョビンの加速度")]
    [SerializeField] private float chobinAcceleration;
    [Tooltip("チョビンが配膳行動等にかける時間（秒）")]
    [SerializeField] private float performingTimeLength = 2f;
    [Tooltip("待機場所に到着したと判定される半径")]
    [SerializeField] private float waitingSpotRadius = 1f;

    private int commandCount = 1;
    public ChobinBehaviour[] Chobins => chobins;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool CheckSettings()
    {
        bool allSettingAreCorrect = true;

        if (chobins == null || chobins.Length == 0)
        {
            allSettingAreCorrect = false;
            Debug.LogError("ChobinBehaviourの配列が空です。ChobinBehaviourをアタッチしてください。");
        }
        else
        {
            for (int i = 0; i < chobins.Length; i++)
            {
                if (chobins[i] == null)
                {
                    allSettingAreCorrect = false;
                    Debug.LogError($"ChobinBehaviourの配列にnullが含まれています。chobin {i} を設定してください。");
                    continue;
                }
                
                if (chobins[i].GetComponent<NavMeshAgent>() == null)
                {
                    allSettingAreCorrect = false;
                    Debug.LogError($"ChobinBehaviourにNavMeshAgentがchobin {i} に設定されていません。NavMeshAgentを追加してください。");
                }
            }
        }

        if (WaitingSpot == null)
        {
            allSettingAreCorrect = false;
            Debug.LogError("待機場所のTransformが設定されていません。");
        }
        if (ServingSpot == null)
        {
            allSettingAreCorrect = false;
            Debug.LogError("配膳を実行できる場所のTransformが設定されていません。");
        }

        return allSettingAreCorrect;
    }

    public void Init()
    {
        if (chobins == null) return;

        for (int i = 0; i < chobins.Length; i++)
        {
            if (chobins[i] != null)
            {
                if (chobins[i].TryGetComponent<NavMeshAgent>(out var agent))
                {
                    agent.speed = chobinSpeed;
                    agent.acceleration = chobinAcceleration;
                }
                chobins[i].SetWaitingSpot(WaitingSpot, waitingSpotRadius);
                chobins[i].SetServingSpot(ServingSpot);
                chobins[i].SetPerformingTimeLength(performingTimeLength);
                chobins[i].Init(i, commandCount);
            }
        }
    }

    public void SetCommandCount(int _commandCount)
    {
        commandCount = _commandCount;
    }
}
