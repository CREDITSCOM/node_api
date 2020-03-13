using System;
using System.Collections.Generic;
using System.Text;

namespace NodeAPIClient.Models
{
    public class Money: ICloneable
    {
        public Int32 Integral { get; set; }
        public UInt64 Fraction { get; set; }

        public static Money FromDouble(double val)
        {
            Money amount = new Money();

            if (val < (double)Int32.MinValue || val > (double)(Int32.MaxValue))
            {
                throw new OverflowException("Amount::Amount(double) overflow)");
            }

            amount.Integral = (Int32)val;
            if (val < 0.0)
            {
                amount.Integral -= 1;
            }

            double frac = val - (double)amount.Integral;

            frac *= FACTOR;

            if (frac < 1.0)
            {
                amount.Fraction = (UInt64)(frac * (double)MULTIPLIER + 0.5);
            }
            else
            {
                amount.Fraction = (UInt64)(frac + 0.5) * MULTIPLIER;
            }
            if (amount.Fraction >= AMOUNT_MAX_FRACTION)
            {
                amount.Fraction -= AMOUNT_MAX_FRACTION;
                amount.Integral += 1;
            }

            return amount;
        }

        public static Money FromParts(Int32 Integral, UInt64 Fraction)
        {
            return new Money
            {
                Integral = Integral,
                Fraction = Fraction
            };
        }

        public static Money FromCommission(UInt16 bits)
        {
            bool negative = (bits & 0x8000) != 0;
            var man = bits & 0x3FF;
            var fra = (bits >> 10) & 0x1F;
            const double v1024 = 1.0 / 1024;
            double num = (negative ? -1.0 : 1.0) * man * v1024 * Math.Pow(10.0, fra - 18);

            return FromDouble(num);
        }

        static UInt64 gen_pow(UInt64 b, UInt64 exp)
        {
            return exp == 0 ? 1 : b * gen_pow(b, exp - 1);
        }

        const UInt64 AMOUNT_MAX_FRACTION = 1_000_000_000_000_000_000UL;
        const double FACTOR = 1e15;
        const UInt64 MULTIPLIER = 1000;
        //const double MULTIPLIER = 1000.0;

        public static string FormatAmount(Money value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            if (value.Fraction == 0)
            {
                return $"{value.Integral}.0";
            }
            string sign = string.Empty;
            if (value.Integral < 0)
            {
                value.Fraction = AMOUNT_MAX_FRACTION - value.Fraction;
                value.Integral += 1;
                sign = "-";
            }
            var frac = value.Fraction.ToString();
            frac = frac.PadLeft(18, '0');

            return sign + $"{Math.Abs(value.Integral)}.{frac.TrimEnd('0')}";
        }

        public static Money operator + (Money lhs, Money rhs)
        {
            Money result = new Money()
            {
                Integral = lhs.Integral + rhs.Integral,
                Fraction = lhs.Fraction + rhs.Fraction
            };
            if(result.Fraction >= AMOUNT_MAX_FRACTION)
            {
                result.Integral += 1;
                result.Fraction -= AMOUNT_MAX_FRACTION;
            }

            return result;
        }

        public static Money operator - (Money from, Money what)
        {
            Money result = new Money()
            {
                Integral = from.Integral - what.Integral
            };
            if (what.Fraction > from.Fraction)
            {
                result.Integral -= 1;
                result.Fraction = from.Fraction + AMOUNT_MAX_FRACTION - what.Fraction;
            }
            else
            {
                result.Fraction = from.Fraction - what.Fraction;
            }
            return result;
        }

        public bool IsZero => (Integral == 0 && Fraction == 0UL);

        public static readonly Money Zero = new Money() { Integral = 0, Fraction = 0 };

        public override string ToString()
        {
            return Money.FormatAmount(this);
        }

        public static Money FromString(string src)
        {
            if (!string.IsNullOrWhiteSpace(src))
            {
                if(src == "null")
                {
                    return null;
                }
                string[] parts = src.Split('.');
                if (parts.Length == 2)
                {
                    Int32 integral = 0;
                    if (Int32.TryParse(parts[0], out integral))
                    {
                        UInt64 fraction = 0;
                        if (UInt64.TryParse(parts[1], out fraction))
                        {
                            return Money.FromParts(integral, fraction);
                        }
                    }
                }
            }
            return new Money() { Integral = 0, Fraction = 0 };
        }

        public object Clone()
        {
            return Money.FromParts(this.Integral, this.Fraction);
        }
    }
}
