using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialisation
{
    class ListNode
    {
        public ListNode Previous;
        public ListNode Next;
        public ListNode Random; // random element in the list
        public string Data;
    }
    class ListRandom
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(Stream s) // Serializing the data
        {
            Dictionary<ListNode, int> dictionary = new Dictionary<ListNode, int>();
            int id = 0;

            // Transforming nodes into dictionary
            for (ListNode currentNode = Head; currentNode != null; currentNode = currentNode.Next)
            {
                dictionary.Add(currentNode, id);
                id++;
            }

            // Writeng data into file
            using (BinaryWriter writer = new BinaryWriter(s))
            {
                for (ListNode currentNode = Head; currentNode != null; currentNode = currentNode.Next)
                {
                    writer.Write(currentNode.Data);
                    writer.Write(dictionary[currentNode.Random]);
                }
            }

            Console.WriteLine("List serialized");
        }

        public void Deserialize(Stream s) // Deserializing the data
        {
            Dictionary<int, Tuple<String, int>> dictionary = new Dictionary<int, Tuple<String, int>>();
            int counter = 0;

            try
            {
                // Trying to read the file
                using (BinaryReader reader = new BinaryReader(s))
                {
                    while (reader.PeekChar() != -1)
                    {
                        String data = reader.ReadString();
                        int randomId = reader.ReadInt32();
                        dictionary.Add(counter, new Tuple<String, int>(data, randomId));
                        counter++;
                    }
                    Console.WriteLine("File readed");
                }

                // Transforming file into nodes
                Count = counter;
                Head = new ListNode();
                ListNode current = Head;
                for (int i = 0; i < Count; i++)
                {
                    current.Data = dictionary.ElementAt(i).Value.Item1;
                    current.Next = new ListNode();
                    if (i != this.Count - 1)
                    {
                        current.Next.Previous = current;
                        current = current.Next;
                    }
                    else
                    {
                        Tail = current;
                    }
                }
                counter = 0;
                for (ListNode i = Head; i.Next != null; i = i.Next)
                {
                    int count = 0;
                    for (ListNode j = Head; j.Next != null; j = j.Next)
                    {
                        if (counter == dictionary.ElementAt(counter).Value.Item2)
                            i.Random = j;
                        count++;
                    }
                    counter++;
                }
                Console.WriteLine("List deserialized");
            }
            catch (Exception e) // Cathing the errors
            {
                Console.WriteLine("Can't open the file.");
                Console.WriteLine(e.Message);
                Console.WriteLine("Press Enter to exit.");
                Console.Read();
                Environment.Exit(0);
            }
        }
    }
    class Program
    {
        static Random random = new Random();

        static ListNode addNode(ListNode previous) // Creating new node
        {
            ListNode list = new ListNode();
            list.Previous = previous;
            list.Next = null;
            list.Data = random.Next(0, 100).ToString();
            previous.Next = list;
            return list;
        }

        static ListNode randomNode(ListNode _head, int _length) // Creating referense to the random node
        {
            int k = random.Next(0, _length);
            int i = 0;
            ListNode list = _head;
            while (i < k)
            {
                list = list.Next;
                i++;
            }
            return list;
        }

        static void Main(string[] args)
        {
            int length = 10;

            ListNode head = new ListNode(); // Node for the test
            ListNode tail = new ListNode();
            ListNode temp = new ListNode();

            head.Data = random.Next(0, 1000).ToString();

            tail = head;

            for (int i = 1; i < length; i++)
                tail = addNode(tail);

            temp = head;

            for (int i = 0; i < length; i++)
            {
                temp.Random = randomNode(head, length);
                temp = temp.Next;
            }

            ListRandom test1 = new ListRandom();
            ListRandom test2 = new ListRandom();

            test1.Head = head; // Creating the list
            test1.Tail = tail;
            test1.Count = length;

            FileStream file = new FileStream("data.dat", FileMode.OpenOrCreate);
            test1.Serialize(file);

            try
            {
                file = new FileStream("data.dat", FileMode.Open);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press Enter to exit.");
                Console.Read();
                Environment.Exit(0);
            }
            test2.Deserialize(file);

            Console.ReadLine();
        }
    }
}
