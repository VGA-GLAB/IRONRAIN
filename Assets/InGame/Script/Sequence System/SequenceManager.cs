using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class SequenceManager : MonoBehaviour
    {
        [SerializeField] private SequenceLoader _loader;
        [SerializeField] private SequenceData _sequenceData;

        public ISequence[] GetSequences()
        {
            foreach (var seq in _loader.GetSequences())
            {
                seq.SetData(_sequenceData);
            }

            return _loader.GetSequences();
        }
    }
}
