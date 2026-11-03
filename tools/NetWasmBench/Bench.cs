using System.Diagnostics;
using DoomSharp;

// Mirrors the browser hosts' benchmark export: a fresh game and renderer,
// timed for consecutive renders of the paused initial scene.
// The reduced E1M1 WAD is compiled in (WadData.g.cs), so the module needs no files.
int frames = args.Length > 0 ? int.Parse(args[0]) : 120;
int batches = args.Length > 1 ? int.Parse(args[1]) : 1;
Wad wad = new(WadData.Bytes());
string map = wad.Maps.First();
Game game = new(wad, map);
Renderer renderer = new();
for (int batch = 0; batch < batches; batch++)
{
    int checksum = 0;
    long start = Stopwatch.GetTimestamp();
    for (int i = 0; i < frames; i++)
    {
        renderer.Render(game);
        checksum = unchecked(checksum * 31 + renderer.Pixels[(i * 7919) % renderer.Pixels.Length]);
    }
    double milliseconds = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
    Console.WriteLine($"batch {batch} map {map} frames {frames} ms {milliseconds:F1} checksum {checksum}");
}
