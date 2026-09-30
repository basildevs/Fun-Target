using System.Collections.Generic;
using UnityEngine;

public class WheelController : Singleton<WheelController>
{
    public int totalPoints = 10;
    public bool clockwise = true;

    public List<float> points = new List<float>();

    public void StartWheelPoint(int index)
    {
        float targetAngle = points[index];

        if (clockwise)
            targetAngle = -targetAngle;

        Debug.Log($"[WheelController] Rotating to index: {index}, angle: {targetAngle}");

        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }
}
