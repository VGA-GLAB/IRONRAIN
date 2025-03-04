using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class SequenceLoader : MonoBehaviour
    {
        [Header("Debug機能")]
        [SerializeField] private bool _isDebug;
        [SerializeReference, SubclassSelector] private ISequence[] _debugSequenceList;

        public ISequence[] GetSequences()
        {
            return _debugSequenceList;
        }
    }
}
