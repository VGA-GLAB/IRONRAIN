using System;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class SequenceManager : MonoBehaviour
    {
        [SerializeField] private SequenceLoader _loader;
        [SerializeField] private SequenceData _sequenceData;

        public ISequence[] GetSequences()
        {
            var sequences = _loader.GetSequences();
            
            for (int i = 0; i < sequences.Length; i++)
            {
                try
                {
                    sequences[i].SetData(_sequenceData);
                }
                catch (Exception e)
                {
                    ExceptionReceiver(e, i);
                }
            }
            return sequences;
        }
        
        private void ExceptionReceiver(Exception e, int index)
        {
            Debug.LogError($"{name}の Element{index}");
            throw e;
        }
    }
}
