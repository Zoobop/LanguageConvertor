using Output;

namespace Output
{
    public class FromCSharp : Base, IInterface
    {
        public string StringProperty { get; protected set; }
        public int IntProperty { get; }

        public int number = 0;
        public float weight;

        public void Method()
        {
            
        }

        private static int Add(int[] numbers, int count)
        {
            return 0;
        }

        void Explode(bool sure)
        {

        }

        protected override void Func1(int obj)
        {

        }

        protected override void Func2(string obj)
        {

        }

        public void Func3(object obj)
        {

        }
    }
}