using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cother
{
    /// <summary>
    /// Represents a rational number that allows for division without precision loss. It represents the number as a tuple of integers. 
    /// </summary>
    public struct Rational
    {
        /// <summary>
        /// Gets the numerator of the fraction when reduced to lowest terms.
        /// </summary>
        public int Numerator { get; private set; }
        /// <summary>
        /// Gets the denominator of the fraction when reduced to lowest terms, or 1 if it is an integer.
        /// </summary>
        public int Denominator { get; private set; }
        /// <summary>
        /// The number of decimal places conserved when converting from a floating-point value to a rational.
        /// </summary>
        public const int Precision = 5;
        /// <summary>
        /// Reduces the fraction to the lowest possible denominator.
        /// </summary>
        private void simplifySelf()
        {
            int gcd = greatestCommonDivisor(Math.Abs(Numerator), Math.Abs(Denominator));
            bool isNegative = Math.Sign(Numerator) != Math.Sign(Denominator);
            this.Numerator = (isNegative ? -1 : 1) * (Math.Abs(Numerator) / gcd);
            this.Denominator = (Math.Abs(Denominator) / gcd);
        }
        /// <summary>
        /// Compares two rational numbers.
        /// </summary>
        public static bool operator <(Rational operand1, Rational operand2)
        {
            return operand1 <= operand2 &&
                operand1 != operand2;
        }  
        /// <summary>
        /// Compares two rational numbers.
        /// </summary>
        public static bool operator !=(Rational operand1, Rational operand2)
        {
            return !(operand1 == operand2);
        }
        /// <summary>
        /// Compares two rational numbers.
        /// </summary>
        public static bool operator <=(Rational operand1, Rational operand2)
        {
            return operand1.Numerator * operand2.Denominator
                <= operand1.Denominator * operand2.Numerator;
        }
        /// <summary>
        /// Compares two rational numbers.
        /// </summary>
        public static bool operator >(Rational operand1, Rational operand2)
        {
            return operand2 < operand1;
        } 
        /// <summary>
        /// Compares two rational numbers.
        /// </summary>
        public static bool operator >=(Rational operand1, Rational operand2)
        {
            return operand2 <= operand1;
        } 
        /// <summary>
        /// Compares two rational numbers.
        /// </summary>
        public static bool operator ==(Rational operand1, Rational operand2)
        {
            return (operand1.Numerator == operand2.Numerator &&
                operand1.Denominator == operand2.Denominator);
        }
        private static int greatestCommonDivisor(int a, int b)
        {
            return b == 0 ? a : greatestCommonDivisor(b, a % b);
        }
        /// <summary>
        /// First converts the double to a float, then converts that float to a rational number by cutting it at a certain precision beyond the decimal point.
        /// </summary>
        public static explicit operator Rational(double d)
        {
            return (Rational)(float)d;
        }
        /// <summary>
        /// Converts a float to a rational number by cutting it at a certain precision beyond the decimal point. The precision is specified by the "Precision" constant.
        /// </summary>
        public static explicit operator Rational(float f)
        {
            int numerator = (int)(f * 10.Exponentiate(Precision));
            int denominator = 10.Exponentiate(Precision);
            Rational r= new Rational(numerator, denominator);
            r.simplifySelf();
            return r;
        }
        /// <summary>
        /// Implicitly converts an integer to the equivalent (integer/1) rational number.
        /// </summary>
        public static implicit operator Rational(int integer)
        {
            return new Rational(integer, 1);
        }
        /// <summary>
        /// Divides two rational numbres.
        /// </summary>
        /// <exception cref="DivideByZeroException">When the second operand is zero.</exception>
        public static Rational operator /(Rational operand1, Rational operand2)
        {
            return
                operand1 *
                new Rational(operand2.Denominator, operand2.Numerator);
        }
        /// <summary>
        /// Multiplies two rational numbers.
        /// </summary>
        public static Rational operator *(Rational operand1, Rational operand2)
        {
            Rational r = new Rational(operand1.Numerator * operand2.Numerator,
                operand1.Denominator * operand2.Denominator);
            r.checkDenominatorIsNotZero();
            return r;

        }

        private void checkDenominatorIsNotZero()
        {
            if (Denominator == 0)
            {
                throw new DivideByZeroException("Denominator must never be zero. You must also not divide by a rational number with a zero numerator.");
            }
        }

        /// <summary>
        /// Sums two rational numbers.
        /// </summary>
        public static Rational operator +(Rational operand1, Rational operand2)
        {
            return new Rational(
                operand1.Numerator * operand2.Denominator +
                operand2.Numerator * operand1.Denominator,
                operand1.Denominator * operand2.Denominator);
        }
        /// <summary>
        /// Subtracts the second operand from the first.
        /// </summary>
        public static Rational operator -(Rational operand1, Rational operand2)
        {
            return
                (-operand2) + operand1;
        }
        /// <summary>
        /// Returns the negation of the operand.
        /// </summary>
        public static Rational operator -(Rational unaryOperand)
        {
            return new Rational(-unaryOperand.Numerator, unaryOperand.Denominator);
        }
        /// <summary>
        /// Creates a new rational number from the given numerator and denominator. The denominator must not be zero or an exception occurs.
        /// </summary>
        /// <param name="numerator">The numerator.</param>
        /// <param name="denominator">The denominator (it must not be zero).</param>
        public Rational(int numerator, int denominator) : this()
        {
            Numerator = numerator;
            Denominator = denominator;
            checkDenominatorIsNotZero();
            simplifySelf();
        }
        /// <summary>
        /// The hash code is numerator times 36 plus denominator.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return Numerator.GetHashCode() * 36 +
                Denominator.GetHashCode();
        }
        /// <summary>
        /// Returns a value that indicates whether this rational number has the same value as another rational number.
        /// </summary>
        /// <param name="obj">Another Rational number.</param>
        /// <returns>Do they have the same value?</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is Rational)
            {
                return this == (Rational)obj;
            }
            else if (obj is Int32)
            {
                return (int)(obj) == this.Numerator && this.Denominator == 1; 
            }
            else
            {
                return false;
            }
        }
    }
}
