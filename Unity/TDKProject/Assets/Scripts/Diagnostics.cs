using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Diagnostics : MonoBehaviour
{
    public string fileName;
    static float lastTic;
    public float timeLimit;

    static private float gameStart;

    struct LogRecord
    {
        public LogRecord(string f, float v, float c)
        {
            flag = f;
            value = v;
            ct = c;
        }

        public string flag;
        public float value;
        public float ct;
    }

    static private List<LogRecord> LogRecords = new List<LogRecord>();

    protected void Start()
    {
        gameStart = Time.realtimeSinceStartup;
        Debug.Log("Diagnostics started");
    }

    protected void OnDestroy()
    {
        StreamWriter writer = new StreamWriter("Assets/"+fileName+".txt", false);
        foreach (var logRec in LogRecords)
        {
            //Debug.Log(logRec.ct);
            if (logRec.ct > timeLimit) break;

            writer.WriteLine(logRec.flag + " " + logRec.value);
        }

        Debug.Log("Diagnostics ended");

        writer.Close();
    }

    public static void Tic()
    {
        lastTic = Time.realtimeSinceStartup;
    }
    public static float Toc()
    {
        float dt = 1000f * (Time.realtimeSinceStartup - lastTic);
        return dt;
    }

    public static void Toc2Log(string flag)
    {
        float time = Time.realtimeSinceStartup;
        float dt = 1000f * (Time.realtimeSinceStartup - lastTic);

        LogRecords.Add(new LogRecord(flag, dt, time - gameStart));

    }


}
