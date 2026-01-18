using System;
using System.Collections.Generic;
using OpenCover.Framework.Model;
using UnityEngine;

/// <summary>
/// 調理法の管理クラス
/// </summary>
public class CookingMethodManager : GameSystem
{
    [Serializable]
    class CookingMethod
    {
        [SerializeField] private string methodName;
        [SerializeField] private Sprite methodIcon;
        [SerializeField] private float cookingTime;
        [SerializeField] private Transform[] kitchenLine;

        public string MethodName => methodName;
        public Sprite MethodIcon => methodIcon;
        public float CookingTime => cookingTime;

        public Transform[] KitchenLine => kitchenLine;
    }

    [SerializeField] private CookingMethod[] cookingMethods;

    public Transform[] KitchenLine(int methodIndex) => cookingMethods[methodIndex].KitchenLine;
    public float CookingTime(int methodIndex) => cookingMethods[methodIndex].CookingTime;
    public int CookingMethodLength => cookingMethods.Length;

    public Transform[][] GetKitchenLines()
    {
        Transform[][] kitchenLines = new Transform[cookingMethods.Length][];
        for (int i = 0; i < cookingMethods.Length; i++)
        {
            kitchenLines[i] = cookingMethods[i].KitchenLine;
        }
        return kitchenLines;
    }

    public float[] GetCookingTime()
    {
        float[] cookingTime = new float[cookingMethods.Length];
        for (int i = 0; i < cookingMethods.Length; i++)
        {
            cookingTime[i] = cookingMethods[i].CookingTime;
        }
        return cookingTime;
    }

    public Sprite[] GetMethodIcons()
    {
        Sprite[] methodIcons=new Sprite[cookingMethods.Length];
        for (int i = 0; i < cookingMethods.Length; i++)
        {
            methodIcons[i] = cookingMethods[i].MethodIcon;
        }
        return methodIcons;
    }

    /// <summary>
    /// Inspectorで設定された値が正しいかチェック
    /// </summary>
    public override bool CheckSettings()
    {
        bool settingsAreCorrect = true;
        if (cookingMethods == null || cookingMethods.Length == 0)
        {
            Debug.LogError("CookingMethodManager: No cooking methods defined.");
            settingsAreCorrect = false;
        }
        else
        {
            for (int i = 0; i < cookingMethods.Length; i++)
            {
                if (string.IsNullOrEmpty(cookingMethods[i].MethodName))
                {
                    Debug.LogError($"CookingMethodManager: Cooking method at index {i} has no name.");
                    settingsAreCorrect = false;
                }
                if (cookingMethods[i].CookingTime <= 0f)
                {
                    Debug.LogError($"CookingMethodManager: Cooking method '{cookingMethods[i].MethodName}' has invalid cooking time.");
                    settingsAreCorrect = false;
                }
            }
        }
        return settingsAreCorrect;
    }

    /// <summary>
    /// 調理法クラスの初期化
    /// </summary>
    public void Init()
    {

    }
}
