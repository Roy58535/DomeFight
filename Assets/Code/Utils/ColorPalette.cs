using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DomeFight/Color Palette", fileName = "NewColorPalette")]
public class ColorPalette : ScriptableObject
{
    [Serializable]
    public class ColorEntry
    {
        public string Name;
        public Color Color;
    }

    [SerializeField] private List<ColorEntry> _entries = new List<ColorEntry>();

    // Lazy cache for name -> index lookups (case-insensitive)
    private Dictionary<string, int> _nameToIndex;

    private void OnEnable() => BuildCache();
    private void OnValidate() => BuildCache();

    private void BuildCache()
    {
        if (_nameToIndex == null)
            _nameToIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        else
            _nameToIndex.Clear();

        for (int i = 0; i < _entries.Count; i++)
        {
            string name = _entries[i].Name ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
                continue;

            // Keep first occurrence; ignore duplicates but log in editor
            if (!_nameToIndex.ContainsKey(name))
            {
                _nameToIndex[name] = i;
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning($"ColorPalette '{name}' appears more than once. First occurrence will be used.", this);
#endif
            }
        }
    }

    // Access by index
    public bool TryGetColor(int index, out Color color)
    {
        if (index >= 0 && index < _entries.Count)
        {
            color = _entries[index].Color;
            return true;
        }

        color = default;
        return false;
    }

    public Color GetColor(int index, Color fallback = default)
    {
        return TryGetColor(index, out var c) ? c : fallback;
    }

    // Access by name (case-insensitive)
    public bool TryGetColor(string name, out Color color)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            color = default;
            return false;
        }

        if (_nameToIndex == null)
            BuildCache();

        if (_nameToIndex.TryGetValue(name, out int idx))
        {
            color = _entries[idx].Color;
            return true;
        }

        color = default;
        return false;
    }

    public Color GetColor(string name, Color fallback = default)
    {
        return TryGetColor(name, out var c) ? c : fallback;
    }

    // Helpers
    public int Count => _entries?.Count ?? 0;
    public IReadOnlyList<ColorEntry> Entries => _entries;
    public int IndexOf(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return -1;

        if (_nameToIndex == null)
            BuildCache();

        return _nameToIndex.TryGetValue(name, out var idx) ? idx : -1;
    }
}
