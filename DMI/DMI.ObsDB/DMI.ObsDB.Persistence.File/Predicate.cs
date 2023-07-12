namespace DMI.ObsDB.Persistence.File
{
    public enum Operator
    {
        Equal,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual
    }

    public class Predicate
    {
        public string Field { get; }
        public Operator Operator { get; }
        public object Value { get; }

        public Predicate(
            string field,
            Operator @operator,
            object value)
        {
            Field = field;
            Operator = @operator;
            Value = value;
        }
    }
}
