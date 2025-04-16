using System.Collections.Generic;
using Game._Script.Gems;
using UnityEngine;

namespace Game._Script.Stages
{
    [CreateAssetMenu(fileName = "New Stage Config", menuName = "Hidden Gem/ Stage Config")]
    public class StageConfig : ScriptableObject
    {
        public List<GemStageConfig> gemConfig;
        public int pickaxe;
        public int dynamite;
    }
}
