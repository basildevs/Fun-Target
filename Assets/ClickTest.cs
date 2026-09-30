using UnityEngine;
using UnityEngine.UI;

//public class ClickTest : MonoBehaviour,ICanvasRaycastFilter
public class ClickTest : MonoBehaviour
{
    //private Image image;
    //private Texture2D mainTexture;
    //private Texture2D maskTexture;
    //public float alphaHit;
    public Button button;
    void Awake()
    {
        //image = GetComponent<Image>();
        //Material material = image.material;
        //mainTexture = material.GetTexture("_MainTex") as Texture2D;
        //maskTexture = material.GetTexture("_MaskTex") as Texture2D;
        button.GetComponent<Image>().alphaHitTestMinimumThreshold = 1.0f;
    }
    //public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    //{
    //    if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(image.rectTransform, screenPoint, eventCamera, out var localPoint))
    //        return false;

    //    Rect rect = image.rectTransform.rect;
    //    Vector2 normalizedPoint = new Vector2(
    //        (localPoint.x - rect.xMin) / rect.width,
    //        (localPoint.y - rect.yMin) / rect.height
    //    );

    //    // Sample alpha values from the main texture and mask texture
    //    Color mainColor = mainTexture.GetPixelBilinear(normalizedPoint.x, normalizedPoint.y);
    //    Color maskColor = maskTexture.GetPixelBilinear(normalizedPoint.x, normalizedPoint.y);

    //    // Check combined alpha
    //    float combinedAlpha = mainColor.a * maskColor.r;
    //    return combinedAlpha >= image.alphaHitTestMinimumThreshold;
    //}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.onClick.AddListener(() => Debug.Log("Clicked"));
    }

    //// Update is called once per frame
    //void Update()
    //{
    //}
}
