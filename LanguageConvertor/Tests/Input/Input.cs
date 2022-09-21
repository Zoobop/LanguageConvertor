using Abstraction;

namespace Output
{
    public class FromCSharp
    {
        public string StringProperty { get; set; }
        public int IntProperty { get; }
        public IAnimal Animal { get; private set; }

        public int number = 0;
        public float weight;

        public void Method()
        {
            Console.WriteLine("Hello World");
        }

        private static int Add(int[] numbers, int count)
        {
            var sum = 0;
            for (var i = 0; i < count; i++)
            {
                sum += numbers[i];
            }
            return sum;
        }

        void Explode(bool sure)
        {

        }

        protected static void Func(string obj)
        {

        }

        public static void FindAnimal(IAnimal animal)
        {

        }
    }
}