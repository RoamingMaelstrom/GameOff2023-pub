using System;
using UnityEngine;


[CreateAssetMenu(fileName = "SaveObject", menuName = "GliderFramework/SaveObject", order = 0)]
public class SaveObject : ScriptableObject 
{
    public string saveName;
    public SaveType saveType;

    public int intDefaultValue;
    public float floatDefaultValue;
    public string stringDefaultValue;

    public float rangeLower;
    public float rangeUpper;

    public int length;
    public SaveUpdateRuleNumeric saveUpdateRuleNumeric;
    public SaveUpdateRuleString saveUpdateRuleString;

    public string GetPrefsName() => saveName;


    public float GetValueFloat() => PlayerPrefs.GetFloat(GetPrefsName(), 0);
    public int GetValueInt() => PlayerPrefs.GetInt(GetPrefsName(), 0);
    public string GetValueString() => PlayerPrefs.GetString(GetPrefsName(), "");


    public void OverrideSet(float newValue) 
    {
        if (saveType == SaveType.FLOAT) PlayerPrefs.SetFloat(GetPrefsName(), newValue);
        else PrintWrongTypeSet(typeof(float));
    }
    public void OverrideSet(int newValue) 
    {
        if (saveType == SaveType.INT) PlayerPrefs.SetInt(GetPrefsName(), newValue);
        else if (saveType == SaveType.FLOAT) OverrideSet((float)newValue);
        else PrintWrongTypeSet(typeof(int));
    }
    public void OverrideSet(string newValue) 
    {
        if (saveType == SaveType.STRING) PlayerPrefs.SetString(GetPrefsName(), newValue);
        else PrintWrongTypeSet(typeof(string));
    }


    public bool Set(float newValue)
    {
        if (saveType == SaveType.INT) return Set((int)newValue);
        if (saveType != SaveType.FLOAT) return false;

        float oldValue = PlayerPrefs.GetFloat(GetPrefsName());
        if (!SatifiesNumericRule(saveUpdateRuleNumeric, newValue, oldValue)) return false;

        OverrideSet(newValue);
        return true;
    }

    public bool Set(int newValue)
    {
        if (saveType != SaveType.INT) return false;

        int oldValue = PlayerPrefs.GetInt(GetPrefsName());
        if (!SatifiesNumericRule(saveUpdateRuleNumeric, newValue, oldValue)) return false;

        OverrideSet(newValue);
        return true;
    }

    public bool Set(string newValue)
    {
        if (saveType != SaveType.STRING) return false;

        string oldValue = PlayerPrefs.GetString(GetPrefsName());
        if (!SatifiesStringRule(saveUpdateRuleString, newValue, oldValue)) return false;

        OverrideSet(newValue);
        return true;
    }


    private void PrintWrongTypeSet(Type inputType)
    {
        Debug.Log(string.Format("SaveObject.OverrideSet aborted. Input of type {0} invalid for SaveObject of type {1}", inputType, saveType));
    } 


    private bool SatifiesNumericRule(SaveUpdateRuleNumeric saveUpdateRuleNumeric, float newValue, float oldValue)
    {
        switch (saveUpdateRuleNumeric)
        {
            case SaveUpdateRuleNumeric.NONE: return true;
            case SaveUpdateRuleNumeric.NOT_EQUAL: return newValue != oldValue;
            case SaveUpdateRuleNumeric.GREATER: return newValue > oldValue;
            case SaveUpdateRuleNumeric.LESS_THAN: return newValue < oldValue;
            case SaveUpdateRuleNumeric.IN_RANGE: return newValue >= rangeLower && newValue <= rangeUpper;
            default: return false;
        }
    }

    private static bool SatifiesStringRule(SaveUpdateRuleString saveUpdateRuleString, string newValue, string oldValue)
    {
        switch (saveUpdateRuleString)
        {
            case SaveUpdateRuleString.NONE: return true;
            case SaveUpdateRuleString.LONGEST: return newValue.Length > oldValue.Length;
            case SaveUpdateRuleString.SHORTEST: return newValue.Length < oldValue.Length;
            default: return false;
        }
    }


    public void ResetToDefault()
    {
        switch (saveType)
        {
            case SaveType.FLOAT: OverrideSet(floatDefaultValue); break;
            case SaveType.INT: OverrideSet(intDefaultValue); break;
            case SaveType.STRING: OverrideSet(stringDefaultValue); break;
            default: break;
        }
    }


    public void Delete() => PlayerPrefs.DeleteKey(GetPrefsName());
}

public enum SaveType
{
    INT,
    FLOAT,
    STRING
}

public enum SaveUpdateRuleNumeric
{
    NONE,
    NOT_EQUAL,
    GREATER,
    LESS_THAN,
    IN_RANGE
}

public enum SaveUpdateRuleString
{
    NONE,
    LONGEST,
    SHORTEST
}
