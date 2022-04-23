// See https://aka.ms/new-console-template for more information

using SimulationFramework.Desktop;

namespace Phys;

public static class Program
{
    private static void Main()
    {
        new PhysSimulation().RunWindowed("physics", 1280, 720);
    }
}