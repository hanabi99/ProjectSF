using ScriptableDictionaries;
using UnityEngine;

namespace OnlyNew.CharacterBoom
{
    [CreateAssetMenu(fileName = "CharacterBoomColorPreset", menuName = "CharacterBoomColorPreset", order = 1)]
    public class CharacterBoomColorPreset : ScriptableDictionary<string, ColorSets>
    {
    }
}