
using Cysharp.Threading.Tasks;

namespace WiDiD.Dialogue
{
    public interface IStringLinePlayer
    {
        public void PlayStringLine(string stringLine);
        public UniTask PlayStringLineTask(string stringLine);
    }
}