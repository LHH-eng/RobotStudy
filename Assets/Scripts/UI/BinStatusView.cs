using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BinStatusView : MonoBehaviour
{
    [SerializeField] private Image _emptyImage;
    [SerializeField] private Image _basicImage;
    [SerializeField] private Image _filledImage;
    [SerializeField] private TextMeshProUGUI _partNameText;

    [SerializeField] private bool _hasBin;  // Bin 있으면 true
    [SerializeField] private int _qty;
    [SerializeField] private int _maxQty = 100;

    [SerializeField] private BinDetailPopup _popup;  // 팝업 참조

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateView();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateView();
    }

    public void UpdateView()
    {
        if (!_hasBin)
        {
            // Bin 없음
            _emptyImage.gameObject.SetActive(true);
            _basicImage.gameObject.SetActive(false);
            _filledImage.gameObject.SetActive(false);
            _partNameText.gameObject.SetActive(false);
        }
        else
        {
            // Bin 있음 — Basic이 Empty를 가려주니까 Empty는 그냥 둬도 됨
            _basicImage.gameObject.SetActive(true);
            _filledImage.gameObject.SetActive(true);
            _partNameText.gameObject.SetActive(true);

            // filled
            float t = (float)_qty / _maxQty;  // 0~1 사이 비율
            float fillAmount = Mathf.Lerp(0.1f, 0.8f, t);
            _filledImage.fillAmount = fillAmount;

            // color
            Color startColor = new Color(70 / 255f, 120 / 255f, 190 / 255f, 240 / 255f);  // 수량 0일 때 색상
            Color endColor = new Color(200 / 255f, 80 / 255f, 80 / 255f, 255 / 255f);    // 수량 MAX일 때 색상
            _filledImage.color = Color.Lerp(startColor, endColor, t);
        }
    }

    // 버튼 클릭 시 호출
    public void OnClick()
    {
        _popup.SetData(
            "Bolt",       // 나중에 실제 데이터로 교체
            "bolt",
            _qty,         // 실제 수량
            75,
            "BIN-001",
            "X:0, Y:0, Z:0",
            "SHUTTLE-01",
            "2026-04-06 10:30"
        );
        _popup.Open();
    }
}
