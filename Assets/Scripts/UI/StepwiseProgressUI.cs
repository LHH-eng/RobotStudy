using UnityEngine;
using UnityEngine.UI;

public class StepwiseProgressUI : MonoBehaviour
{
    [Header("UI 구성 요소")]
    [Tooltip("게이지가 채워질 Image (Filled 타입이어야 합니다)")]
    public Image fillImage;

    [Header("수량 설정")]
    [Tooltip("최소 수량 (예: 0)")]
    public int minAmount = 0;
    [Tooltip("최대 수량 (예: 100)")]
    public int maxAmount = 100;

    [Header("현재 수량")]
    [Tooltip(" 수량 (예: 100)")]
    public int currentAmount = 60;

    [Header("색상 설정")]
    [Tooltip("수량에 따라 변할 색상의 범위 (Inspector에서 편집 가능)")]
    public Gradient colorGradient;
    // Inspector에서 수동으로 지정하려면 아래 public 변수를 사용하세요.
    // public Color emptyColor = Color.gray; // 비었을 때 색상
    // public Color halfColor = Color.blue;  // 중간 색상
    // public Color fullColor = new Color(1f, 0.5f, 0f); // 꽉 찼을 때 색상

    private void Awake()
    {
        if (fillImage == null)
        {
            fillImage = GetComponent<Image>();
        }
    }

    /// <summary>
    /// 현재 수량을 입력하여 UI를 업데이트합니다.
    /// </summary>
    /// <param name="currentAmount">현재 수량</param>
    public void UpdateUIByAmount(int currentAmount)
    {
        // 1. 수량을 0~1 사이의 퍼센트(progress)로 변환합니다.
        // InverseLerp는 min에서 max 사이의 value가 어디쯤 위치하는지를 0~1 값으로 반환합니다.
        float progress = Mathf.InverseLerp(minAmount, maxAmount, currentAmount);

        // 2. 게이지의 Fill Amount를 업데이트합니다.
        fillImage.fillAmount = progress;

        // 3. 색상을 업데이트합니다.
        if (colorGradient != null)
        {
            // Gradient를 사용할 경우: progress 값에 해당하는 색상을 가져옵니다.
            fillImage.color = colorGradient.Evaluate(progress);
        }
        /* Gradient를 사용하지 않고 Color.Lerp를 직접 사용할 경우의 예시:
        else if (progress <= 0.5f)
        {
            // 0 ~ 0.5 구간: emptyColor에서 halfColor로 서서히 변함
            // t 값은 0~0.5를 0~1로 매핑해야 하므로 progress / 0.5f를 사용합니다.
            fillImage.color = Color.Lerp(emptyColor, halfColor, progress / 0.5f);
        }
        else
        {
            // 0.5 ~ 1.0 구간: halfColor에서 fullColor로 서서히 변함
            // t 값은 0.5~1.0을 0~1로 매핑해야 하므로 (progress - 0.5f) / 0.5f를 사용합니다.
            fillImage.color = Color.Lerp(halfColor, fullColor, (progress - 0.5f) / 0.5f);
        }
        */
    }
}