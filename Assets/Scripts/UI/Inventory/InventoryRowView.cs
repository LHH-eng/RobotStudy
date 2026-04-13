using TMPro;
using UnityEngine;

public class InventoryRowView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _noText;
    [SerializeField] private TextMeshProUGUI _binIdText;
    [SerializeField] private TextMeshProUGUI _qrCodeText;
    [SerializeField] private TextMeshProUGUI _partNameText;
    [SerializeField] private TextMeshProUGUI _categoryText;
    [SerializeField] private TextMeshProUGUI _boxCountText;
    [SerializeField] private TextMeshProUGUI _boxQtyText;
    [SerializeField] private TextMeshProUGUI _totalQtyText;
    [SerializeField] private TextMeshProUGUI _weightText;
    [SerializeField] private TextMeshProUGUI _positionText;

    public void SetData(int no, InventoryPanelController.InventoryData data)
    {
        _noText.text = no.ToString();
        _binIdText.text = data.binId;
        _qrCodeText.text = data.qrCode;
        _partNameText.text = data.partName;
        _categoryText.text = data.category;
        _boxCountText.text = data.boxCount.ToString();
        _boxQtyText.text = data.boxQty.ToString();
        _totalQtyText.text = data.totalQty.ToString();
        _weightText.text = data.weight.ToString();
        _positionText.text = data.position;
    }
}