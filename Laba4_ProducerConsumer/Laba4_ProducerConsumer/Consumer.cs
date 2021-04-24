using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Laba4_ProducerConsumer
{
    public class Consumer
    {
        private int ConsumersCount { get; set; }
        private readonly ChannelReader<String> _consumerChannel;

        public Consumer(int cCount, ChannelReader<String> chanR)
        {
            ConsumersCount = cCount;
            _consumerChannel = chanR;
        }

        public Task[] StartConsuming()
        {
            Task[] consumers = new Task[ConsumersCount];
            for (int i = 0; i < ConsumersCount; i++)
            {
                consumers[i] = Task.Factory.StartNew(async () =>
                {
                    do
                    {
                        if (_consumerChannel.TryRead(out var data))
                            Consume(data);
                    } while (await _consumerChannel.WaitToReadAsync());
                });
            }
            return consumers;
        }
        private void Consume(String str)
        {
            HashSet<char> chars = new HashSet<char>();
            foreach (var t in str)
                chars.Add(t);
            Console.WriteLine("The number of unique chars in string \"" + str + "\" is " + chars.Count);
        }
    }
}