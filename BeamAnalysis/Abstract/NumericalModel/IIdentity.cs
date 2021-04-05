namespace BeamAnalysis.Abstract
{
    public interface IIdentity
    {
       int[,] Matrix { get; }
       int[,] Id { get; }
       int[,] Ip { get; }
    }
}
