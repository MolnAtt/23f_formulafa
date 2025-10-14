using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _23f_formulafa
{
	internal class Program
	{
		class Formula
		{
			List<Formula> gyerekei;
			char művelet; // nem csak ∧ ∨ ¬ → ↔, de a p, q, r, ... is művelet lesz!
			public Formula(char művelet)
			{
				this.művelet = művelet;
				this.gyerekei = new List<Formula>();
			}

			public Formula(char művelet, List<Formula>gyerekei)
			{
				this.művelet = művelet;
				this.gyerekei = gyerekei;
			}


			public static Formula operator *(Formula A, Formula B) => new Formula('&', new List<Formula> { A, B });
			public static Formula operator +(Formula A, Formula B) => new Formula('V', new List<Formula> { A, B });
			public static Formula operator -(Formula A) => new Formula('¬', new List<Formula> { A });
			public static Formula operator >(Formula A, Formula B) => new Formula('→', new List<Formula> { A, B });
			public static Formula operator <(Formula A, Formula B) => new Formula('←', new List<Formula> { A, B });
			public static Formula operator ==(Formula A, Formula B) => new Formula('↔', new List<Formula> { A, B });
			public static Formula operator !=(Formula A, Formula B) => new Formula('⨂', new List<Formula> { A, B });
			public override string ToString()
			{
				switch (this.gyerekei.Count)
				{
					case 0: // ez egy atomi formula
						return this.művelet.ToString();
					case 1: // ez egy tagadás
						return this.művelet.ToString() + this.gyerekei[0].ToString();
					case 2: // ez egy és, vagy, ...
						return "(" + this.gyerekei[0].ToString() + this.művelet.ToString() + this.gyerekei[1].ToString() + ")";
					default:
						throw new NotImplementedException();
				}
			}  
		}

		static void Main(string[] args)
		{
			Formula p = new Formula('p');
			Formula q = new Formula('q');
			Formula p_es_q = p * q;
			Formula A = ((p * q) + -p) > q;

			Console.WriteLine(A);

		}
	}
}
