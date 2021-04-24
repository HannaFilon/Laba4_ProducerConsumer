using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Laba4_ProducerConsumer
{
    static class Program
    {
        static void Main()
        {
            int producersCount = 2, consumersCount=3;
            Channel<String> channel = Channel.CreateUnbounded<String>();
            Producer producer = new Producer(producersCount, channel.Writer);
            Consumer consumer = new Consumer(consumersCount, channel.Reader);
            Task[] producerTasks = producer.StartProducing();
            Task[] consumerTasks = consumer.StartConsuming();
            Task.WaitAll(producerTasks);
            Task.WaitAll(consumerTasks);
        }
    }
}