using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Laba4_ProducerConsumer
{
    public class Producer
    {
        private int ProducerCount { get; set; }
        private ChannelWriter<String> ProducerChannel { get; set; }

        public Producer(int pCount, ChannelWriter<String> chanW)
        {
            ProducerCount = pCount;
            ProducerChannel = chanW;
        }

        public Task[] StartProducing()
        {
            Task[] producers = new Task[ProducerCount];
            for (int i = 0; i < ProducerCount; i++)
            {
                producers[i] = Task.Factory.StartNew(async () =>
                {
                    do
                    {
                        String str = Produce();
                        if (!ProducerChannel.TryWrite(str))
                            continue;
                        Console.WriteLine("Write string: \"" + str + "\" by task number " + Task.CurrentId);
                        Thread.Sleep(1000);
                    } while (await ProducerChannel.WaitToWriteAsync());
                    ProducerChannel.Complete();
                });
            }
            return producers;
        }
        
        private String Produce()
        {
            Random rand = new Random();
            int stringLength = rand.Next(1, 100);
            string currentString = "";
            for (int j = 0; j < stringLength; j++)
            {
                int symbolCode = rand.Next(48, 126);
                currentString += Convert.ToChar(symbolCode);
            }
            return currentString;
        }
    }
}