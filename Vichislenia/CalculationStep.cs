namespace Vichislenia
{
    public class CalculationStep
    {
        public string Name { get; }
        public string Formula { get; }
        public double Value { get; }

        public CalculationStep(string name, string formula, double value)
        {
            Name = name;
            Formula = formula;
            Value = value;
        }

        public override string ToString() => $"{Name}: {Formula} = {ObshieVichislia.Format(Value)}";
    }
}
