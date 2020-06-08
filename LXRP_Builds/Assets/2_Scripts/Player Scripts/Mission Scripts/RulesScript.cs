using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesScript : MonoBehaviour
{
    private SO_RuleInfo ruleInfo;
    public SO_RuleInfo RuleInfo { get => ruleInfo; set => ruleInfo = value; }

    public static string bookTag = "";

    /*public void OnClick()
    {
        
        ruleInfo.IsSelected = false;
        Debug.Log("Rule Game Object Clicked");
        UIManager.Instance.SetRuleInfo(RuleInfo);

        bookTag = gameObject.tag;
        Debug.Log("Rule tag:" + bookTag);
        
    }*/

    public void CollideWithPlayer()
    {
        ruleInfo.IsSelected = false;
        //Debug.Log("Rule Game Object Clicked");
        UIManager.Instance.SetRuleInfo(RuleInfo);

        bookTag = gameObject.tag;
        //Debug.Log("Rule tag:" + bookTag);
    }

    public void SendMessageToManager()
    {
        throw new System.NotImplementedException();
    }
}
