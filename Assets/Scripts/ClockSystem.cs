using System;
using Unity.VisualScripting;
using UnityEngine;

public class ClockSystem : MonoBehaviour
{
    public Transform hour;
    public Transform min;
    public Transform sec;
    Quaternion startQ_Hour;
    Quaternion startQ_Min;
    Quaternion startQ_Sec;
    float startAngle_Hour;
    float startAngle_Min;
    float startAngle_Sec;
    float time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DateTime now = DateTime.Now;

        print(now.Hour);
        print(now.Minute);
        print(now.Second);

        startAngle_Hour = (now.Hour % 12) / 12f * 360f;
        startAngle_Min = now.Minute / 60f * 360f;
        startAngle_Sec = now.Second / 60f * 360f;

        startQ_Hour = Quaternion.AngleAxis(startAngle_Hour, Vector3.up);
        startQ_Min = Quaternion.AngleAxis(startAngle_Min, Vector3.up);
        startQ_Sec = Quaternion.AngleAxis(startAngle_Sec, Vector3.up);

        hour.localRotation = startQ_Hour;
        min.localRotation = startQ_Min;
        sec.localRotation = startQ_Sec;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        // 초침: 독립적으로 회전
        float timeSec = startAngle_Sec + (time / 60f) * 360f;

        // 분침: 초침 각도를 60으로 나눠서 참조
        float timeMin = startAngle_Min + timeSec / 60f;

        // 시침: 분침 각도를 12로 나눠서 참조
        float timeHour = startAngle_Hour + timeMin / 12f;

        sec.localRotation = Quaternion.AngleAxis(timeSec, Vector3.up);
        min.localRotation = Quaternion.AngleAxis(timeMin, Vector3.up);
        hour.localRotation = Quaternion.AngleAxis(timeHour, Vector3.up);
    }
}
