using UnityEngine;

public class StatusPanelToggle : MonoBehaviour
{
    [SerializeField] private RectTransform _statusPanel;
    [SerializeField] private float _hiddenPosY = -390f;
    [SerializeField] private float _shownPosY = 0f;
    [SerializeField] private float _speed = 5f;
    private bool _isOpen = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float targetY = _isOpen ? _shownPosY : _hiddenPosY;

        Vector2 currentPos = _statusPanel.anchoredPosition;
        float newY = Mathf.Lerp(currentPos.y, targetY, Time.deltaTime * _speed);
        _statusPanel.anchoredPosition = new Vector2(currentPos.x, newY);
    }

    public void Toggle()
    {
        _isOpen = !_isOpen;  // true ↔ false 전환
    }
}
