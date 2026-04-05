using UnityEngine;

[CreateAssetMenu(fileName = "SupabaseConfig", menuName = "SmartSort/Supabase Config")]
public class SupabaseConfig : ScriptableObject
{
    public string supabaseUrl;
    public string anonKey;
}