using UnityEngine;
using Supabase;
using System.Threading.Tasks;

public class InventoryFetcher : MonoBehaviour
{
    [SerializeField] private SupabaseConfig _config;
    private Supabase.Client _client;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        // Supabase 클라이언트 만들기
        var option = new SupabaseOptions { AutoConnectRealtime = false };
        _client = new Supabase.Client(_config.supabaseUrl, _config.anonKey, option);
        await _client.InitializeAsync();
        //await FetchInventories(); //
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private async Task FetchInventories()
    //{
    //    var result = await _client
    //    .From<InventoryRecord>()
    //    .Get();

    //    foreach (var item in result.Models)
    //    {
    //        Debug.Log($"bin_id: {item.BinId}, qty: {item.QtyCount}");
    //    }
    //}
}
