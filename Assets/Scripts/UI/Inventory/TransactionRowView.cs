using TMPro;
using UnityEngine;

public class TransactionRowView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _noText;
    [SerializeField] private TextMeshProUGUI _transactionNoText;
    [SerializeField] private TextMeshProUGUI _qrCodeText;
    [SerializeField] private TextMeshProUGUI _partNameText;
    [SerializeField] private TextMeshProUGUI _categoryText;
    [SerializeField] private TextMeshProUGUI _boxQtyText;
    [SerializeField] private TextMeshProUGUI _binIdText;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private TextMeshProUGUI _dateTimeText;

    public void SetData(int no, TransactionData data)
    {
        _noText.text = no.ToString();
        _transactionNoText.text = data.transactionNo;
        _qrCodeText.text = data.qrCode;
        _partNameText.text = data.partName;
        _categoryText.text = data.category;
        _boxQtyText.text = data.boxQty.ToString();
        _binIdText.text = data.binId;
        _statusText.text = data.status;
        _dateTimeText.text = data.dateTime;
    }
}