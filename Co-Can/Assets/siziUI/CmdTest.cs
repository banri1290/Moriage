using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CmdTest : MonoBehaviour
{
    private int CmdAct = 0;
    [SerializeField] private Animator characterAnimator; // キャラクターのAnimator

    // ボタンのOnClickイベントに登録
    public void SetCmdAct()
    {
        CmdAct = 1;
    }

    void Update()
    {
        if (CmdAct == 1)
        {
            if (characterAnimator != null)
            {
                characterAnimator.SetTrigger("PlayAnim"); 
            }
            CmdAct = 0; 
        }
    }
}
