namespace IdSetConv
{
  public class Variable
  {
    public bool Positive { get; }
    public string Name { get; }

    public Variable(bool positive, string name)
    {
      Positive = positive;
      Name = name;
    }
  }
}
