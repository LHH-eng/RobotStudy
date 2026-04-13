using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/** 
 * @file    InventoryPanelController.cs
 * @brief   스마트 창고 대시보드 UI 컨트롤러
 *          작업현황 / 재고현황 / 입고내역 / 출고내역 View 전환 및 데이터 표시
 * @author  LHH-eng
 * @date    2026-04-10
 * @history 2026-04-10 최초 작성
 *          2026-04-13 필터+정렬 동시 적용 버그 수정, 코드 최적화
 *          2026-04-13 WorkStatusView 추가, 대시보드 토글 기능 추가
 */
public class InventoryPanelController : MonoBehaviour
{
    [SerializeField] private GameObject _workStatusView;

    /** @brief 메뉴 버튼 - View 전환용 */
    [SerializeField] private Button _homeBtn;
    [SerializeField] private Button _inventoryBtn;
    [SerializeField] private Button _inboundBtn;
    [SerializeField] private Button _outboundBtn;

    /** @brief View 오브젝트 - 버튼 클릭 시 SetActive 전환 */
    [SerializeField] private GameObject _inventoryView;
    [SerializeField] private GameObject _inboundView;
    [SerializeField] private GameObject _outboundView;

    /** @brief 재고현황 - Row 프리팹 / Content / 검색 / 필터 */
    [SerializeField] private GameObject _inventoryRowPrefab;
    [SerializeField] private Transform _content;
    [SerializeField] private TMP_InputField _searchInput;
    [SerializeField] private TMP_Dropdown _filterDropdown;

    /** @brief 재고현황 - TableHeader 정렬 버튼 */
    [SerializeField] private Button _colPartNameBtn;
    [SerializeField] private Button _colTotalQtyBtn;
    [SerializeField] private Button _colWeightBtn;
    [SerializeField] private Button _colBinIdBtn;
    [SerializeField] private Button _colQrBtn;
    [SerializeField] private Button _colCategoryBtn;
    [SerializeField] private Button _colBoxCountBtn;
    [SerializeField] private Button _colBoxQtyBtn;
    [SerializeField] private Button _colPositionBtn;

    /** @brief 재고현황 - TableHeader 텍스트 (▲▼ 아이콘 표시용) */
    [SerializeField] private TextMeshProUGUI _colPartNameText;
    [SerializeField] private TextMeshProUGUI _colTotalQtyText;
    [SerializeField] private TextMeshProUGUI _colWeightText;
    [SerializeField] private TextMeshProUGUI _colBinIdText;
    [SerializeField] private TextMeshProUGUI _colQrText;
    [SerializeField] private TextMeshProUGUI _colCategoryText;
    [SerializeField] private TextMeshProUGUI _colBoxCountText;
    [SerializeField] private TextMeshProUGUI _colBoxQtyText;
    [SerializeField] private TextMeshProUGUI _colPositionText;

    /** @brief 입고/출고 공용 - Row 프리팹 / Content / 검색 / 필터 */
    [SerializeField] private GameObject _transactionRowPrefab;
    [SerializeField] private Transform _inboundContent;
    [SerializeField] private Transform _outboundContent;
    [SerializeField] private TMP_InputField _inboundSearchInput;
    [SerializeField] private TMP_InputField _outboundSearchInput;
    [SerializeField] private TMP_Dropdown _inboundFilterDropdown;
    [SerializeField] private TMP_Dropdown _outboundFilterDropdown;

    /** @brief 입고내역 - TableHeader 정렬 버튼 */
    [SerializeField] private Button _inColTransactionNoBtn;
    [SerializeField] private Button _inColQrBtn;
    [SerializeField] private Button _inColPartNameBtn;
    [SerializeField] private Button _inColCategoryBtn;
    [SerializeField] private Button _inColBoxQtyBtn;
    [SerializeField] private Button _inColBinIdBtn;
    [SerializeField] private Button _inColStatusBtn;
    [SerializeField] private Button _inColDateTimeBtn;

    /** @brief 입고내역 - TableHeader 텍스트 */
    [SerializeField] private TextMeshProUGUI _inColTransactionNoText;
    [SerializeField] private TextMeshProUGUI _inColQrText;
    [SerializeField] private TextMeshProUGUI _inColPartNameText;
    [SerializeField] private TextMeshProUGUI _inColCategoryText;
    [SerializeField] private TextMeshProUGUI _inColBoxQtyText;
    [SerializeField] private TextMeshProUGUI _inColBinIdText;
    [SerializeField] private TextMeshProUGUI _inColStatusText;
    [SerializeField] private TextMeshProUGUI _inColDateTimeText;

    /** @brief 출고내역 - TableHeader 정렬 버튼 */
    [SerializeField] private Button _outColTransactionNoBtn;
    [SerializeField] private Button _outColQrBtn;
    [SerializeField] private Button _outColPartNameBtn;
    [SerializeField] private Button _outColCategoryBtn;
    [SerializeField] private Button _outColBoxQtyBtn;
    [SerializeField] private Button _outColBinIdBtn;
    [SerializeField] private Button _outColStatusBtn;
    [SerializeField] private Button _outColDateTimeBtn;

    /** @brief 출고내역 - TableHeader 텍스트 */
    [SerializeField] private TextMeshProUGUI _outColTransactionNoText;
    [SerializeField] private TextMeshProUGUI _outColQrText;
    [SerializeField] private TextMeshProUGUI _outColPartNameText;
    [SerializeField] private TextMeshProUGUI _outColCategoryText;
    [SerializeField] private TextMeshProUGUI _outColBoxQtyText;
    [SerializeField] private TextMeshProUGUI _outColBinIdText;
    [SerializeField] private TextMeshProUGUI _outColStatusText;
    [SerializeField] private TextMeshProUGUI _outColDateTimeText;

    /** @brief 재고현황 더미 데이터 - TODO: Supabase 연동 */
    private List<InventoryData> _dummyData = new List<InventoryData>();

    /** @brief 입고/출고 더미 데이터 - TODO: Supabase 연동 */
    private List<TransactionData> _inboundDummyData = new List<TransactionData>();
    private List<TransactionData> _outboundDummyData = new List<TransactionData>();

    /** @brief 재고현황 정렬 상태 (0=기본, 1=오름차순, 2=내림차순) */
    private string _currentSortColumn = "";
    private int _sortState = 0;

    /** @brief 입고/출고 정렬 상태 */
    private string _inboundSortColumn = "";
    private int _inboundSortState = 0;
    private string _outboundSortColumn = "";
    private int _outboundSortState = 0;

    /** @brief 현재 필터/검색 상태 */
    private string _currentFilter = "전체";
    private string _currentKeyword = "";
    private string _inboundFilter = "전체";
    private string _inboundKeyword = "";
    private string _outboundFilter = "전체";
    private string _outboundKeyword = "";

    [System.Serializable]
    public class InventoryData
    {
        public string binId;
        public string qrCode;
        public string partName;
        public string category;
        public int boxCount;
        public int boxQty;
        public int totalQty;
        public float weight;   // 단위: g
        public string position;
    }

    /** @brief 초기화 - 각 View 초기화 후 WorkStatusView 활성화 */
    void Awake()
    {
        InitMenuButtons();
        InitInventoryView();
        InitInboundView();
        InitOutboundView();

        HideAllViews();
        _workStatusView.SetActive(true);
    }

    /** @brief 메뉴 버튼 클릭 이벤트 등록 */
    private void InitMenuButtons()
    {
        _homeBtn.onClick.AddListener(OnHomeBtnClicked);
        _inventoryBtn.onClick.AddListener(OnInventoryBtnClicked);
        _inboundBtn.onClick.AddListener(OnInboundBtnClicked);
        _outboundBtn.onClick.AddListener(OnOutboundBtnClicked);
    }

    /** @brief 재고현황 View 초기화 - 더미데이터 / 검색 / 필터 / 정렬 */
    private void InitInventoryView()
    {
        // TODO: Supabase parts / inventories / bins 테이블 연동으로 교체
        _dummyData.Add(new InventoryData { binId = "A-1-1", qrCode = "BOX-001", partName = "클립", category = "문구", boxCount = 2, boxQty = 50, totalQty = 100, weight = 0.5f, position = "1-1-1" });
        _dummyData.Add(new InventoryData { binId = "A-1-2", qrCode = "BOX-002", partName = "가위", category = "문구", boxCount = 3, boxQty = 30, totalQty = 90, weight = 1.2f, position = "1-1-2" });
        _dummyData.Add(new InventoryData { binId = "A-2-1", qrCode = "BOX-003", partName = "테이프", category = "생활용품", boxCount = 5, boxQty = 20, totalQty = 100, weight = 0.8f, position = "1-2-1" });
        _dummyData.Add(new InventoryData { binId = "B-1-1", qrCode = "BOX-004", partName = "볼펜", category = "문구", boxCount = 4, boxQty = 100, totalQty = 400, weight = 0.3f, position = "2-1-1" });
        _dummyData.Add(new InventoryData { binId = "B-1-2", qrCode = "BOX-005", partName = "지우개", category = "문구", boxCount = 6, boxQty = 40, totalQty = 240, weight = 0.3f, position = "2-1-2" });
        _dummyData.Add(new InventoryData { binId = "B-2-1", qrCode = "BOX-006", partName = "칫솔", category = "생활용품", boxCount = 3, boxQty = 24, totalQty = 72, weight = 0.4f, position = "2-2-1" });
        _dummyData.Add(new InventoryData { binId = "B-2-2", qrCode = "BOX-007", partName = "샴푸", category = "생활용품", boxCount = 2, boxQty = 12, totalQty = 24, weight = 3.5f, position = "2-2-2" });
        _dummyData.Add(new InventoryData { binId = "C-1-1", qrCode = "BOX-008", partName = "건전지", category = "전자", boxCount = 4, boxQty = 20, totalQty = 80, weight = 1.0f, position = "3-1-1" });
        _dummyData.Add(new InventoryData { binId = "C-1-2", qrCode = "BOX-009", partName = "이어폰", category = "전자", boxCount = 3, boxQty = 10, totalQty = 30, weight = 0.5f, position = "3-1-2" });
        _dummyData.Add(new InventoryData { binId = "C-2-1", qrCode = "BOX-010", partName = "마스크", category = "생활용품", boxCount = 5, boxQty = 50, totalQty = 250, weight = 0.2f, position = "3-2-1" });

        RefreshInventoryRows();

        _searchInput.onValueChanged.AddListener(OnSearchChanged);

        _filterDropdown.ClearOptions();
        _filterDropdown.AddOptions(new List<string> { "전체", "문구", "생활용품", "전자" });
        _filterDropdown.onValueChanged.AddListener(OnFilterChanged);

        _colPartNameBtn.onClick.AddListener(() => SortAndRefresh("partName"));
        _colTotalQtyBtn.onClick.AddListener(() => SortAndRefresh("totalQty"));
        _colWeightBtn.onClick.AddListener(() => SortAndRefresh("weight"));
        _colBinIdBtn.onClick.AddListener(() => SortAndRefresh("binId"));
        _colQrBtn.onClick.AddListener(() => SortAndRefresh("qrCode"));
        _colCategoryBtn.onClick.AddListener(() => SortAndRefresh("category"));
        _colBoxCountBtn.onClick.AddListener(() => SortAndRefresh("boxCount"));
        _colBoxQtyBtn.onClick.AddListener(() => SortAndRefresh("boxQty"));
        _colPositionBtn.onClick.AddListener(() => SortAndRefresh("position"));
    }

    /** @brief 입고내역 View 초기화 - 더미데이터 / 검색 / 필터 / 정렬 */
    private void InitInboundView()
    {
        // TODO: Supabase inbound 테이블 연동으로 교체
        _inboundDummyData.Add(new TransactionData { transactionNo = "IN-001", qrCode = "BOX-001", partName = "클립", category = "문구", boxQty = 50, binId = "A-1-1", status = "완료", dateTime = "2026-04-10 09:00:00" });
        _inboundDummyData.Add(new TransactionData { transactionNo = "IN-002", qrCode = "BOX-002", partName = "가위", category = "문구", boxQty = 30, binId = "A-1-2", status = "완료", dateTime = "2026-04-10 09:30:00" });
        _inboundDummyData.Add(new TransactionData { transactionNo = "IN-003", qrCode = "BOX-003", partName = "테이프", category = "생활용품", boxQty = 20, binId = "A-2-1", status = "진행중", dateTime = "2026-04-10 10:00:00" });
        _inboundDummyData.Add(new TransactionData { transactionNo = "IN-004", qrCode = "BOX-004", partName = "볼펜", category = "문구", boxQty = 100, binId = "B-1-1", status = "완료", dateTime = "2026-04-10 10:30:00" });
        _inboundDummyData.Add(new TransactionData { transactionNo = "IN-005", qrCode = "BOX-005", partName = "지우개", category = "문구", boxQty = 40, binId = "B-1-2", status = "완료", dateTime = "2026-04-10 11:00:00" });

        RefreshTransactionRows(_inboundDummyData, _inboundContent, _inboundFilter, _inboundKeyword);

        _inboundSearchInput.onValueChanged.AddListener(keyword =>
        {
            _inboundKeyword = keyword;
            RefreshTransactionRows(_inboundDummyData, _inboundContent, _inboundFilter, _inboundKeyword);
        });

        _inboundFilterDropdown.ClearOptions();
        _inboundFilterDropdown.AddOptions(new List<string> { "전체", "문구", "생활용품", "전자" });
        _inboundFilterDropdown.onValueChanged.AddListener(index =>
        {
            _inboundFilter = _inboundFilterDropdown.options[index].text;
            RefreshTransactionRows(_inboundDummyData, _inboundContent, _inboundFilter, _inboundKeyword);
        });

        _inColTransactionNoBtn.onClick.AddListener(() => SortTransactionAndRefresh("transactionNo", _inboundDummyData, _inboundContent, ref _inboundSortColumn, ref _inboundSortState, UpdateInboundSortIcons, _inboundFilter, _inboundKeyword));
        _inColQrBtn.onClick.AddListener(() => SortTransactionAndRefresh("qrCode", _inboundDummyData, _inboundContent, ref _inboundSortColumn, ref _inboundSortState, UpdateInboundSortIcons, _inboundFilter, _inboundKeyword));
        _inColPartNameBtn.onClick.AddListener(() => SortTransactionAndRefresh("partName", _inboundDummyData, _inboundContent, ref _inboundSortColumn, ref _inboundSortState, UpdateInboundSortIcons, _inboundFilter, _inboundKeyword));
        _inColCategoryBtn.onClick.AddListener(() => SortTransactionAndRefresh("category", _inboundDummyData, _inboundContent, ref _inboundSortColumn, ref _inboundSortState, UpdateInboundSortIcons, _inboundFilter, _inboundKeyword));
        _inColBoxQtyBtn.onClick.AddListener(() => SortTransactionAndRefresh("boxQty", _inboundDummyData, _inboundContent, ref _inboundSortColumn, ref _inboundSortState, UpdateInboundSortIcons, _inboundFilter, _inboundKeyword));
        _inColBinIdBtn.onClick.AddListener(() => SortTransactionAndRefresh("binId", _inboundDummyData, _inboundContent, ref _inboundSortColumn, ref _inboundSortState, UpdateInboundSortIcons, _inboundFilter, _inboundKeyword));
        _inColStatusBtn.onClick.AddListener(() => SortTransactionAndRefresh("status", _inboundDummyData, _inboundContent, ref _inboundSortColumn, ref _inboundSortState, UpdateInboundSortIcons, _inboundFilter, _inboundKeyword));
        _inColDateTimeBtn.onClick.AddListener(() => SortTransactionAndRefresh("dateTime", _inboundDummyData, _inboundContent, ref _inboundSortColumn, ref _inboundSortState, UpdateInboundSortIcons, _inboundFilter, _inboundKeyword));
    }

    /** @brief 출고내역 View 초기화 - 더미데이터 / 검색 / 필터 / 정렬 */
    private void InitOutboundView()
    {
        // TODO: Supabase outbound 테이블 연동으로 교체
        _outboundDummyData.Add(new TransactionData { transactionNo = "OUT-001", qrCode = "BOX-001", partName = "클립", category = "문구", boxQty = 50, binId = "A-1-1", status = "완료", dateTime = "2026-04-10 13:00:00" });
        _outboundDummyData.Add(new TransactionData { transactionNo = "OUT-002", qrCode = "BOX-003", partName = "테이프", category = "생활용품", boxQty = 20, binId = "A-2-1", status = "진행중", dateTime = "2026-04-10 13:30:00" });
        _outboundDummyData.Add(new TransactionData { transactionNo = "OUT-003", qrCode = "BOX-006", partName = "칫솔", category = "생활용품", boxQty = 24, binId = "B-2-1", status = "완료", dateTime = "2026-04-10 14:00:00" });
        _outboundDummyData.Add(new TransactionData { transactionNo = "OUT-004", qrCode = "BOX-008", partName = "건전지", category = "전자", boxQty = 20, binId = "C-1-1", status = "완료", dateTime = "2026-04-10 14:30:00" });
        _outboundDummyData.Add(new TransactionData { transactionNo = "OUT-005", qrCode = "BOX-009", partName = "이어폰", category = "전자", boxQty = 10, binId = "C-1-2", status = "신규", dateTime = "2026-04-10 15:00:00" });

        RefreshTransactionRows(_outboundDummyData, _outboundContent, _outboundFilter, _outboundKeyword);

        _outboundSearchInput.onValueChanged.AddListener(keyword =>
        {
            _outboundKeyword = keyword;
            RefreshTransactionRows(_outboundDummyData, _outboundContent, _outboundFilter, _outboundKeyword);
        });

        _outboundFilterDropdown.ClearOptions();
        _outboundFilterDropdown.AddOptions(new List<string> { "전체", "문구", "생활용품", "전자" });
        _outboundFilterDropdown.onValueChanged.AddListener(index =>
        {
            _outboundFilter = _outboundFilterDropdown.options[index].text;
            RefreshTransactionRows(_outboundDummyData, _outboundContent, _outboundFilter, _outboundKeyword);
        });

        _outColTransactionNoBtn.onClick.AddListener(() => SortTransactionAndRefresh("transactionNo", _outboundDummyData, _outboundContent, ref _outboundSortColumn, ref _outboundSortState, UpdateOutboundSortIcons, _outboundFilter, _outboundKeyword));
        _outColQrBtn.onClick.AddListener(() => SortTransactionAndRefresh("qrCode", _outboundDummyData, _outboundContent, ref _outboundSortColumn, ref _outboundSortState, UpdateOutboundSortIcons, _outboundFilter, _outboundKeyword));
        _outColPartNameBtn.onClick.AddListener(() => SortTransactionAndRefresh("partName", _outboundDummyData, _outboundContent, ref _outboundSortColumn, ref _outboundSortState, UpdateOutboundSortIcons, _outboundFilter, _outboundKeyword));
        _outColCategoryBtn.onClick.AddListener(() => SortTransactionAndRefresh("category", _outboundDummyData, _outboundContent, ref _outboundSortColumn, ref _outboundSortState, UpdateOutboundSortIcons, _outboundFilter, _outboundKeyword));
        _outColBoxQtyBtn.onClick.AddListener(() => SortTransactionAndRefresh("boxQty", _outboundDummyData, _outboundContent, ref _outboundSortColumn, ref _outboundSortState, UpdateOutboundSortIcons, _outboundFilter, _outboundKeyword));
        _outColBinIdBtn.onClick.AddListener(() => SortTransactionAndRefresh("binId", _outboundDummyData, _outboundContent, ref _outboundSortColumn, ref _outboundSortState, UpdateOutboundSortIcons, _outboundFilter, _outboundKeyword));
        _outColStatusBtn.onClick.AddListener(() => SortTransactionAndRefresh("status", _outboundDummyData, _outboundContent, ref _outboundSortColumn, ref _outboundSortState, UpdateOutboundSortIcons, _outboundFilter, _outboundKeyword));
        _outColDateTimeBtn.onClick.AddListener(() => SortTransactionAndRefresh("dateTime", _outboundDummyData, _outboundContent, ref _outboundSortColumn, ref _outboundSortState, UpdateOutboundSortIcons, _outboundFilter, _outboundKeyword));
    }

    /** @brief 입고/출고 Row 갱신 - 필터 + 검색 동시 적용 */
    private void RefreshTransactionRows(List<TransactionData> data, Transform content, string filter, string keyword)
    {
        SpawnFilteredRows(data, content, _transactionRowPrefab,
            item => (filter == "전체" || item.category == filter) && item.partName.Contains(keyword),
            (row, i, item) => row.GetComponent<TransactionRowView>().SetData(i + 1, item));
    }

    /** @brief 검색/필터 Row 생성 공용 메서드 */
    private void SpawnFilteredRows<T>(List<T> data, Transform content, GameObject prefab, System.Func<T, bool> filter, System.Action<GameObject, int, T> setup)
    {
        foreach (Transform child in content) Destroy(child.gameObject);

        int rowIndex = 0;
        foreach (var item in data)
        {
            if (!filter(item)) continue;
            GameObject row = Instantiate(prefab, content);
            setup(row, rowIndex, item);
            Image rowBg = row.GetComponent<Image>();
            rowBg.color = (rowIndex % 2 == 0) ? new Color32(52, 73, 94, 255) : new Color32(44, 62, 80, 255);
            rowIndex++;
        }
    }

    /** @brief 재고현황 검색 - 부품명 기준 */
    private void OnSearchChanged(string keyword)
    {
        _currentKeyword = keyword;
        RefreshInventoryRows();
    }

    /** @brief 재고현황 필터 - 카테고리 기준 */
    private void OnFilterChanged(int index)
    {
        _currentFilter = _filterDropdown.options[index].text;
        RefreshInventoryRows();
    }

    /** @brief 재고현황 Row 갱신 - 필터 + 검색 동시 적용 */
    private void RefreshInventoryRows()
    {
        SpawnFilteredRows(_dummyData, _content, _inventoryRowPrefab,
            item => (_currentFilter == "전체" || item.category == _currentFilter)
                 && item.partName.Contains(_currentKeyword),
            (row, i, item) => row.GetComponent<InventoryRowView>().SetData(i + 1, item));
    }

    /** @brief 재고현황 정렬 - 컬럼 클릭 시 기본→오름차순→내림차순 순환 */
    private void SortAndRefresh(string column)
    {
        if (_currentSortColumn == column) _sortState = (_sortState + 1) % 3;
        else { _currentSortColumn = column; _sortState = 1; }

        if (_sortState == 0) _dummyData.Sort((a, b) => a.qrCode.CompareTo(b.qrCode));
        else
        {
            bool asc = _sortState == 1;
            switch (column)
            {
                case "partName": _dummyData.Sort((a, b) => asc ? a.partName.CompareTo(b.partName) : b.partName.CompareTo(a.partName)); break;
                case "totalQty": _dummyData.Sort((a, b) => asc ? a.totalQty.CompareTo(b.totalQty) : b.totalQty.CompareTo(a.totalQty)); break;
                case "weight": _dummyData.Sort((a, b) => asc ? a.weight.CompareTo(b.weight) : b.weight.CompareTo(a.weight)); break;
                case "binId": _dummyData.Sort((a, b) => asc ? a.binId.CompareTo(b.binId) : b.binId.CompareTo(a.binId)); break;
                case "qrCode": _dummyData.Sort((a, b) => asc ? a.qrCode.CompareTo(b.qrCode) : b.qrCode.CompareTo(a.qrCode)); break;
                case "category": _dummyData.Sort((a, b) => asc ? a.category.CompareTo(b.category) : b.category.CompareTo(a.category)); break;
                case "boxCount": _dummyData.Sort((a, b) => asc ? a.boxCount.CompareTo(b.boxCount) : b.boxCount.CompareTo(a.boxCount)); break;
                case "boxQty": _dummyData.Sort((a, b) => asc ? a.boxQty.CompareTo(b.boxQty) : b.boxQty.CompareTo(a.boxQty)); break;
                case "position": _dummyData.Sort((a, b) => asc ? a.position.CompareTo(b.position) : b.position.CompareTo(a.position)); break;
            }
        }

        RefreshInventoryRows();
        UpdateSortIcons(column);
    }

    /** @brief 입고/출고 정렬 공용 - 컬럼 클릭 시 기본→오름차순→내림차순 순환 */
    private void SortTransactionAndRefresh(string column, List<TransactionData> data, Transform content, ref string currentCol, ref int sortState, System.Action<string> updateIcons, string filter, string keyword)
    {
        if (currentCol == column) sortState = (sortState + 1) % 3;
        else { currentCol = column; sortState = 1; }

        if (sortState == 0) data.Sort((a, b) => a.transactionNo.CompareTo(b.transactionNo));
        else
        {
            bool asc = sortState == 1;
            switch (column)
            {
                case "transactionNo": data.Sort((a, b) => asc ? a.transactionNo.CompareTo(b.transactionNo) : b.transactionNo.CompareTo(a.transactionNo)); break;
                case "qrCode": data.Sort((a, b) => asc ? a.qrCode.CompareTo(b.qrCode) : b.qrCode.CompareTo(a.qrCode)); break;
                case "partName": data.Sort((a, b) => asc ? a.partName.CompareTo(b.partName) : b.partName.CompareTo(a.partName)); break;
                case "category": data.Sort((a, b) => asc ? a.category.CompareTo(b.category) : b.category.CompareTo(a.category)); break;
                case "boxQty": data.Sort((a, b) => asc ? a.boxQty.CompareTo(b.boxQty) : b.boxQty.CompareTo(a.boxQty)); break;
                case "binId": data.Sort((a, b) => asc ? a.binId.CompareTo(b.binId) : b.binId.CompareTo(a.binId)); break;
                case "status": data.Sort((a, b) => asc ? a.status.CompareTo(b.status) : b.status.CompareTo(a.status)); break;
                case "dateTime": data.Sort((a, b) => asc ? a.dateTime.CompareTo(b.dateTime) : b.dateTime.CompareTo(a.dateTime)); break;
            }
        }

        RefreshTransactionRows(data, content, filter, keyword);
        updateIcons(column);
    }

    /** @brief 재고현황 정렬 아이콘 업데이트 */
    private void UpdateSortIcons(string column)
    {
        _colPartNameText.text = "부품명";
        _colTotalQtyText.text = "제품 총 수량";
        _colWeightText.text = "총 중량(g)";
        _colBinIdText.text = "BinID";
        _colQrText.text = "QR코드";
        _colCategoryText.text = "카테고리";
        _colBoxCountText.text = "BOX 수량";
        _colBoxQtyText.text = "BOX당 제품수량";
        _colPositionText.text = "위치";

        string icon = _sortState == 1 ? " ▲" : _sortState == 2 ? " ▼" : "";
        switch (column)
        {
            case "partName": _colPartNameText.text += icon; break;
            case "totalQty": _colTotalQtyText.text += icon; break;
            case "weight": _colWeightText.text += icon; break;
            case "binId": _colBinIdText.text += icon; break;
            case "qrCode": _colQrText.text += icon; break;
            case "category": _colCategoryText.text += icon; break;
            case "boxCount": _colBoxCountText.text += icon; break;
            case "boxQty": _colBoxQtyText.text += icon; break;
            case "position": _colPositionText.text += icon; break;
        }
    }

    /** @brief 입고내역 정렬 아이콘 업데이트 */
    private void UpdateInboundSortIcons(string column)
    {
        _inColTransactionNoText.text = "입고번호";
        _inColQrText.text = "QR코드";
        _inColPartNameText.text = "부품명";
        _inColCategoryText.text = "카테고리";
        _inColBoxQtyText.text = "박스당 수량";
        _inColBinIdText.text = "BinID";
        _inColStatusText.text = "입고 상태";
        _inColDateTimeText.text = "입고 시각";

        string icon = _inboundSortState == 1 ? " ▲" : _inboundSortState == 2 ? " ▼" : "";
        switch (column)
        {
            case "transactionNo": _inColTransactionNoText.text += icon; break;
            case "qrCode": _inColQrText.text += icon; break;
            case "partName": _inColPartNameText.text += icon; break;
            case "category": _inColCategoryText.text += icon; break;
            case "boxQty": _inColBoxQtyText.text += icon; break;
            case "binId": _inColBinIdText.text += icon; break;
            case "status": _inColStatusText.text += icon; break;
            case "dateTime": _inColDateTimeText.text += icon; break;
        }
    }

    /** @brief 출고내역 정렬 아이콘 업데이트 */
    private void UpdateOutboundSortIcons(string column)
    {
        _outColTransactionNoText.text = "출고번호";
        _outColQrText.text = "QR코드";
        _outColPartNameText.text = "부품명";
        _outColCategoryText.text = "카테고리";
        _outColBoxQtyText.text = "박스당 수량";
        _outColBinIdText.text = "BinID";
        _outColStatusText.text = "출고 상태";
        _outColDateTimeText.text = "출고 시각";

        string icon = _outboundSortState == 1 ? " ▲" : _outboundSortState == 2 ? " ▼" : "";
        switch (column)
        {
            case "transactionNo": _outColTransactionNoText.text += icon; break;
            case "qrCode": _outColQrText.text += icon; break;
            case "partName": _outColPartNameText.text += icon; break;
            case "category": _outColCategoryText.text += icon; break;
            case "boxQty": _outColBoxQtyText.text += icon; break;
            case "binId": _outColBinIdText.text += icon; break;
            case "status": _outColStatusText.text += icon; break;
            case "dateTime": _outColDateTimeText.text += icon; break;
        }
    }

    /** @brief View 전환 - WorkStatusView (홈) */
    private void OnHomeBtnClicked()
    {
        HideAllViews();
        _workStatusView.SetActive(true);
        SetActiveButton(null);
    }

    /** @brief View 전환 - 재고현황 */
    private void OnInventoryBtnClicked() { HideAllViews(); _inventoryView.SetActive(true); SetActiveButton(_inventoryBtn); }

    /** @brief View 전환 - 입고내역 */
    private void OnInboundBtnClicked() { HideAllViews(); _inboundView.SetActive(true); SetActiveButton(_inboundBtn); }

    /** @brief View 전환 - 출고내역 */
    private void OnOutboundBtnClicked() { HideAllViews(); _outboundView.SetActive(true); SetActiveButton(_outboundBtn); }

    /** @brief 모든 View 비활성화 */
    private void HideAllViews()
    {
        _workStatusView.SetActive(false);
        _inventoryView.SetActive(false);
        _inboundView.SetActive(false);
        _outboundView.SetActive(false);
    }

    /** @brief 활성 버튼 색상 변경 */
    private void SetActiveButton(Button activeBtn)
    {
        _inventoryBtn.image.color = new Color32(52, 73, 94, 255);
        _inboundBtn.image.color = new Color32(52, 73, 94, 255);
        _outboundBtn.image.color = new Color32(52, 73, 94, 255);

        if (activeBtn != null)
            activeBtn.image.color = new Color32(26, 37, 53, 255);
    }

    /** @brief 스마트 창고 대시보드 UI 컨트롤러 */
    public void ToggleDashboard()
    {
        bool isActive = gameObject.activeSelf;

        if (isActive)
            gameObject.SetActive(false);
        else
        {
            gameObject.SetActive(true);
            HideAllViews();
            _workStatusView.SetActive(true);
        }
    }
}