using System.Collections.Generic;
using System.Linq;
using _DeepChat.Scripts.Data;
using MoreMountains.Tools;

namespace _DeepChat.Scripts.Logic
{
    public class MessageSampler
    {
        private readonly MessageBank _bank;
        private readonly List<int> _pool;

        public MessageSampler(MessageBank bank)
        {
            _bank = bank;
            _pool = new List<int>(bank.messages.Count);
        }

        private void ResetPool()
        {
            _pool.AddRange(Enumerable.Range(0, _bank.messages.Count));
            _pool.MMShuffle();
        }

        public Message Sample()
        {
            if (_pool.Count == 0)
                ResetPool();
            var sample = _pool[^1];
            _pool.RemoveAt(_pool.Count - 1);
            return _bank.messages[sample];
        }
    }
}