using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BinDetailPopup : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [SerializeField] private TextMeshProUGUI _partNameText;
    [SerializeField] private TextMeshProUGUI _categoryText;
    [SerializeField] private TextMeshProUGUI _totalQtyText;
    [SerializeField] private TextMeshProUGUI _binStatusText;
    [SerializeField] private TextMeshProUGUI _binIdText;
    [SerializeField] private TextMeshProUGUI _binPositionText;
    [SerializeField] private TextMeshProUGUI _shuttleIdText;
    [SerializeField] private TextMeshProUGUI _lastInboundTimeText;

    private RectTransform _rectTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void SetData(string partName, string category, int totalQty,
                    int binStatus, string binId, string binPosition,
                    string shuttleId, string lastInboundTime)
    {
        _partNameText.text = partName;
        _categoryText.text = category;
        _totalQtyText.text = totalQty.ToString();
        _binStatusText.text = binStatus.ToString() + "%";  // 퍼센트로 표시
        _binIdText.text = binId;
        _binPositionText.text = binPosition;
        _shuttleIdText.text = shuttleId;
        _lastInboundTimeText.text = lastInboundTime;
    }

    private Vector2 _dragOffset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragOffset = (Vector2)transform.position - eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position + _dragOffset;
    }

}
