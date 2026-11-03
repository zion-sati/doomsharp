# NetWasm render benchmark

A command-line WebAssembly build of DoomSharp's renderer that
[NetWasm](https://github.com/zion-sati/NetWasm) uses to measure rendering and to
check that each of its optimization presets compiles DoomSharp correctly. It
renders the paused first scene of E1M1 from a fresh game, the scene the browser
hosts' benchmark export renders, and prints each batch's time and a checksum of
the rendered pixels.

The engine sources are the repository's own, compiled unchanged. The program
cannot read files, so `Reducer` (desktop .NET) renders the same frames from
`assets/freedoom/freedoom1.wad`, keeps the data of the lumps they read, and
writes that reduced WAD as `WadData.g.cs`, which the benchmark compiles in.
`Reducer/WadTracked.cs` is `Wad.cs` with `Find` recording each lump name it
looks up.

From the repository root, with the .NET 10 SDK and
[Wasmtime](https://wasmtime.dev):

```sh
dotnet run --project tools/NetWasmBench/Reducer -c Release -- assets/freedoom/freedoom1.wad tools/NetWasmBench
dotnet publish tools/NetWasmBench/DoomBench.csproj -c Release -o artifacts/bench \
  -p:NetWasmOptimization=O3 -p:NetWasmGarbageCollector=NetWasm
wasmtime run artifacts/bench/DoomBench.wasm 120 2
```

The arguments are the frames per batch and the number of batches. Each line
reads `batch <n> map E1M1 frames 120 ms <time> checksum <checksum>`; every batch
of 120 frames renders checksum `1326079720`, the reducer's desktop .NET result.
The first batch includes warm-up, so timings report the second.

Freedoom's data is distributed under the terms in
[`assets/freedoom/COPYING.txt`](../../assets/freedoom/COPYING.txt).
