using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using Supabase;

public class SupabaseConnectionChecker : MonoBehaviour
{

    [SerializeField] private SupabaseConfig _config;
    [SerializeField] private TextMeshProUGUI _supabaseText;  // 텍스트
    [SerializeField] private Image _isConnectedImage;            // 이미지

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        // 연결 시도 전 초기 상태
        _isConnectedImage.color = Color.yellow;
        _supabaseText.text = "Supabase Connecting...";

        await CheckConnection();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async Task CheckConnection()
    {
        try
        {
            // Supabase 클라이언트 만들기
            var option = new SupabaseOptions { AutoConnectRealtime = false }; // Realtime 자동 연결 끄기
            var client = new Supabase.Client(_config.supabaseUrl, _config.anonKey, option);
            await client.InitializeAsync();

            // 연결 성공
            _isConnectedImage.color = Color.green;
            _supabaseText.text = "Supabase Connected";

        }

        catch
        {
            // 연결 실패
            _isConnectedImage.color = Color.red;
            _supabaseText.text = "Supabase Disconnected";
        }
    }
}
