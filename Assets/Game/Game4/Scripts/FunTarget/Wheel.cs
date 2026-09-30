using FunTarget;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class Wheel : Singleton<Wheel>
{
    [SerializeField] private float rotationSpeed = 100f;
    public bool canRotate = true;
    [SerializeField] private List<SpriteRenderer> bonuseSpriteRenderers;
    public CollisionHandler collisionHandler;
    public bool isWheelStopped = false;
    public int wheelIndex;
    [SerializeField] private int bonusIndex;
    [SerializeField] private Animator bonusBallAnimator;
    [SerializeField] private SpriteRenderer bounusBallRenderer;
    public Animator pointerAnimator;
    public bool valueReceived;
    public float startTime;
    public Action OnWheelRotationFinished;
    public List<float> rotationAngles;
    public float maxStopWaitTime = 12f;
    private bool stopTimeoutTriggered = false;
    private float stopElapsedTime = 0f;
    public SpriteRenderer wheelSprite;
    public SpriteRenderer initialWheelSprite;
    public int BonusIndex { get => bonusIndex; set => bonusIndex = value; }
    void Start()
    {
        wheelSprite.enabled = false;
        canRotate = false;
        bonusBallAnimator.enabled = false;
        foreach (var sr in bonuseSpriteRenderers)
        {
            sr.enabled = false;
        }
        collisionHandler.OnCollision += OnCollision;
    }
    public void ShowIndex(int index)
    {
        transform.rotation = Quaternion.Euler(0, 0, rotationAngles[index]);
    }
    private void OnDisable()
    {
        collisionHandler.OnCollision -= OnCollision;
    }
    public void SetRotate(bool rotate)
    {
        initialWheelSprite.enabled = false;
        wheelSprite.enabled = true;
        startTime = Time.time;
        canRotate = rotate;
        isWheelStopped = false;
        foreach (var sr in bonuseSpriteRenderers)
        {
            sr.enabled = false;
        }
        bonusBallAnimator.enabled = rotate;
        bounusBallRenderer.enabled = rotate;
    }
    private void OnCollision(Collider2D collider)
    {
        //  Debug.Log(Time.time > startTime + 2f);
        //  if (valueReceived && collider.gameObject.CompareTag(wheelIndex.ToString()) && Time.time > startTime + 2f)
        if (collider.gameObject.CompareTag(wheelIndex.ToString()) && Time.time > startTime + 4f)
        {
            WinLogic();
        }
    }
    private void WinLogic()
    {
        valueReceived = false;
        pointerAnimator.SetTrigger("PointerAnim");
        canRotate = false;
        stopElapsedTime = 0;
        isWheelStopped = true;
        AudioController.Instance.PlayAudio(false, "wheelAudio");
        //this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        if (bonusBallAnimator != null)
        {
            bonusBallAnimator.enabled = false;
        }
        bounusBallRenderer.enabled = false;
        DisplayBonusSprite(bonusIndex);
        //  Debug.Log("Wheel rotation finished");
        OnWheelRotationFinished?.Invoke();
    }
    void Update()
    {
        if (canRotate)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
        if (!isWheelStopped && canRotate && Time.time > startTime + 6f)
        {
            ForcedStopWheel();
        }
        if (!isWheelStopped && !stopTimeoutTriggered && canRotate)
        {
            stopElapsedTime += Time.deltaTime;
            if (stopElapsedTime >= maxStopWaitTime)
            {
                stopTimeoutTriggered = true;
                Debug.LogError("Wheel failed to stop in expected time (by diff).");
                GameController.Instance.SlowNetwork();
                //WebSocketEvent.Instance.OnErrorOccured?.Invoke();
            }
        }
    }
    private void ForcedStopWheel()
    {
        Debug.LogWarning("stop triggered!");
        initialWheelSprite.enabled = true;
        wheelSprite.enabled = false;
        // Stop rotation
        canRotate = false;
        isWheelStopped = true;
        // Snap wheel to its final angle
        WheelController.Instance.StartWheelPoint(wheelIndex);
        // Call normal win logic
        WinLogic();
    }
    public void DisplayBonusSprite(int index)
    {
        if (index >= 0 && index < bonuseSpriteRenderers.Count)
        {
            foreach (var sr in bonuseSpriteRenderers)
            {
                sr.enabled = false;
            }
            bonuseSpriteRenderers[index].enabled = true;
        }
    }
}