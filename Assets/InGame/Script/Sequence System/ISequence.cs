using System.Threading;
using Cysharp.Threading.Tasks;

namespace IronRain.SequenceSystem
{
    public interface ISequence
    {
        public void SetData(SequenceData data);
        
        public UniTask PlayAsync(CancellationToken ct);

        public void Skip();
    }   
}