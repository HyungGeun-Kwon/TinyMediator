namespace TinyMediator.Sample.Domain.Items
{
    public sealed record ItemName
    {
        public const int MinLength = 2;
        public const int MaxLength = 50;

        public string Value { get; }

        public ItemName(string value)
        {
            var v = NormalizeAndValidate(value);
            Value = v;
        }
        private static string NormalizeAndValidate(string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var v = value.Trim();
            if (v.Length < MinLength)
                throw new ArgumentOutOfRangeException(nameof(value), $"유저명은 {MinLength}글자 이상이어야 함.");

            if (v.Length > MaxLength)
                throw new ArgumentOutOfRangeException(nameof(value), $"유저명은 {MaxLength}글자 이하여야 함.");

            return v;
        }
        public static bool IsValid(string value, out string? errMsg)
        {
            errMsg = null;
            try
            {
                _ = NormalizeAndValidate(value);
                return true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }

        public override string ToString() => Value;
    }
}
