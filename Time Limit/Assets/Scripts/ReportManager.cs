using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Report
{
    public string name;
    public int startTime;
    public int startCo;
    public int endTime;
    public int endCo;
    public string clearTime;
    public string endingKind;
}

public class ReportManager : MonoBehaviour
{
    private void function()
    {
        string s = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }
}
