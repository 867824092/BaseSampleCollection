using BenchmarkDotNet.Attributes;

namespace BenchmarkSamples; 

[ShortRunJob]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
[MemoryDiagnoser]
[MarkdownExporter]
public class SearchBenchmark {
    private List<int> _source;
    [Params(10, 100, 1000, 10000, 100000)]
    public int Length;
    [GlobalSetup]
    public void Setup()
    {
        _source = Enumerable.Range(0, Length).ToList();
    }
    [Benchmark]
    public void ForeachLoopFind()
    {
        int count = 0;
        foreach (int i in _source) {
            if(i % 2 == 0) {
                count += 1;
            }
        }
    }
    [Benchmark]
    public void ForLoopFind()
    {
        int count = 0;
        for(int i = 0; i < Length; i++) {
            int element = _source[i];
            if(element % 2 == 0) {
                count += 1;
            }
        }
    }

    [Benchmark]
    public void LinqWhereFind()
    {
        int count = _source.Where(i => i % 2 == 0).Count();
        
    }
    
    [Benchmark]
    public void LinqCountFind()
    {
        int count = _source.Count(i => i % 2 == 0);
        
    }
}