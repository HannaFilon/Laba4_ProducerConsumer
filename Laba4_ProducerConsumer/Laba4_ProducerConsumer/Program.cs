using System;
using System.Collections.Generic;
using System.Threading;

namespace Laba4_ProducerConsumer
{
    class BlockingQueue<T>
    {
        readonly int size = 0;
        readonly Queue<T> queue = new Queue<T>();
        readonly object lockObj = new object();
        bool quit = false;

        public BlockingQueue(int Size)
        {
            size = Size;
        }

        public int GetSize()
        {
            return size;
        }

        public void Quit()
        {
            lock (lockObj)
            {
                quit = true;
                Monitor.PulseAll(lockObj);
            }
        }

        public bool Enqueue(T t)
        {
            lock (lockObj)
            {
                while (!quit && queue.Count >= size)
                    Monitor.Wait(lockObj);

                if (quit)
                    return false;

                queue.Enqueue(t);
                Monitor.PulseAll(lockObj);
            }

            return true;
        }

        public bool Dequeue(out T t)
        {
            t = default(T);

            lock (lockObj)
            {
                while (!quit && queue.Count == 0) Monitor.Wait(lockObj);

                if (queue.Count == 0) return false;

                t = queue.Dequeue();

                Monitor.PulseAll(lockObj);
            }

            return true;
        }
    }

    class Program
    {
        static int stringsNumb = 20;
        static BlockingQueue<string> strings = new BlockingQueue<string>(stringsNumb);
        static Random rand = new Random();
        static int pCount = 2, cCount = 3;

        static void Produce(int sec, int producerID)
        {
            for (; ; )
            {
                int stringLength = rand.Next(1, 100);
                string currentString = "";
                for (int j = 0; j < stringLength; j++)
                {
                    int symbolCode = rand.Next(48, 126);
                    currentString += Convert.ToChar(symbolCode);
                }
                if (!strings.Enqueue(currentString))
                    break;
                Thread.Sleep(sec * 1000);
            }
            Console.WriteLine("Producer #" + producerID + " quitting.");
        }

        static void Consume(int consumerID)
        {
            string str = null;
            for (; ; )
            {
                if (!strings.Dequeue(out str))
                    break;

                HashSet<char> chars = new HashSet<char>();
                for (int i = 0; i < str.Length; i++)
                {
                    chars.Add(str[i]);
                }

                Console.WriteLine("The number of unique chars in string \""
                    + str + "\" is " + chars.Count);
            }

            Console.WriteLine("Producer #" + consumerID + " quitting.");
        }

        static void Main()
        {
            Thread[] producingThreads = new Thread[pCount];
            for (int i = 0; i < pCount; i++)
            {
                int n = rand.Next(5);
                producingThreads[i] = new Thread(() => Produce(n, i));
                producingThreads[i].Start();
            }

            Thread[] consumingThreads = new Thread[cCount];
            for (int i = 0; i < cCount; i++)
            {
                consumingThreads[i] = new Thread(() => Consume(i));
                consumingThreads[i].Start();
            }

            for (int i = 0; i < pCount; i++)
                producingThreads[i].Join();
            for (int i = 0; i < cCount; i++)
                consumingThreads[i].Join();
        }
    }
}