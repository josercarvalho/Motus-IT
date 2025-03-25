namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Rating
{
    public double Rate { get; set; }
    public int Count { get; set; }

    public void IncrementCount()
    {
        Count++;
    }

    public void DecrementCount()
    {
        Count--;
    }

    public void ResetCount()
    {
        Count = 1;
    }
}