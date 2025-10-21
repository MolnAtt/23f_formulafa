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

			int Mélység()
			{
				int max = -1;
				foreach (Formula gyerek in gyerekei)
				{
					int m = gyerek.Mélység();
					if (max<m)
					{
						max = m; 
					}
				}
				return max + 1;
			}

			HashSet<Formula> Atomi_formulai()
			{
				HashSet<Formula> atoms = new HashSet<Formula>();

				if (gyerekei.Count == 0)
				{
					atoms.Add(this);
					return atoms;
				}

				foreach (Formula gyerek in gyerekei)
				{
					atoms.UnionWith(gyerek.Atomi_formulai());
				}

				return atoms;
			}

			/// <summary>
			/// Megmondja, hogy a megadott műveletből hányat tartalmaz a formula.
			/// </summary>
			/// <param name="operátor"></param>
			/// <returns></returns>
			public int Műveletek_száma(char operátor)
			{
				if (gyerekei.Count == 0)
					return 0;

				int sum = 0;
				foreach (Formula gyerek in gyerekei)
				{
					sum += gyerek.Műveletek_száma(operátor);
				}

				if (művelet == operátor)
				{
					return 1 + sum;
				}
								
				return sum;
			}

			/// <summary>
			/// Visszaadja, hogy hányszor tagadták benne a megadott műveletet. Például
			/// Tagadott('&') azt adja vissza, hogy hányszor volt konjunkció tagadva benne.
			/// Tagadott('V') azt adja vissza, hogy hányszor volt diszjunkció tagadva benne.
			/// </summary>
			/// <param name="operátor"></param>
			/// <returns></returns>
			//int Tagadott(char muvelet)
			//{

			//}

			/// <summary>
			/// Elkészíti a formula kettős tagadások nélküli verzióját.
			/// </summary>
			/// <returns></returns>
			//Formula Kettostagadasok_nelkul()
			//{

			//}

			/// <summary>
			/// Diszjunktív normálformára hoz, azaz megadja azt az ekvivalens átalakítását, amiben csak V műveletek és literálok vannak. 
			/// </summary>
			/// <param name="operátor"></param>
			/// <returns></returns>
			//Formula Diszjunktív_normálforma()
			//{

			//}

			/// <summary>
			/// Konjunktív normálformára hoz.
			/// </summary>
			/// <param name="operátor"></param>
			/// <returns></returns>
			//Formula Konjunktív_normálforma()
			//{

			//}




		}

		static void Main(string[] args)
		{
			Formula p = new Formula('p');
			Formula q = new Formula('q');
			Formula p_es_q = p * q;
			Formula A = ((p * q) + -p) > q;

			Console.WriteLine(A);

			Formula r = new Formula('r');
			
			//rajzunk:
			Formula B = -(-p > (q * r))+p;

			Console.WriteLine(B.Műveletek_száma('¬'));


		}
	}
}
