using UnityEngine;

namespace Game._Script.Gems
{
    [CreateAssetMenu(fileName = "New Gem Config",menuName = "Hidden Gem/ Gem Config")]
    public class GemConfig : ScriptableObject
    {
        [Header("Gem ID")]
        public int gemId;
        [Header("Gem Size")]
        public int width;
        public int height;
    }
}
