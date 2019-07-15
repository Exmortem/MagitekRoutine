using System.Threading.Tasks;

namespace Magitek.Rotations.Ninja
{
    internal static class Pull
    {
        public static async Task<bool> Execute()
        {
            return await Combat.Execute();
        }
    }
}