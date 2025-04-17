using System.Collections.Generic;
using Game._Script.Gems;
using UnityEngine;

namespace Game._Script.Stages
{
    [CreateAssetMenu(fileName = "New Stage Config", menuName = "Hidden Gem/ Stage Config")]
    public class StageConfig : ScriptableObject
    {
        public string stageName;   
        public List<GemStageConfig> gemConfig;
        public int rows;
        public int columns;
        public int pickaxeReward;
        public int dynamite;
    }
}
