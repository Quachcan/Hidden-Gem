using UnityEngine;

namespace Game._Script.Gems
{
    [CreateAssetMenu(fileName = "New Gem Stage Config", menuName = "Hidden Gem/ Gem Stage Config")]
    public class GemStageConfig : ScriptableObject
    {
        public int gemTypeId;
        public int count;
    }
}
