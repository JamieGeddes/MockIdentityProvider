using System;

namespace MockIdentityProvider
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var identityProvider = new MockIdentityProvider();

            identityProvider.Start();

            Console.ReadKey();
        }
    }
}
