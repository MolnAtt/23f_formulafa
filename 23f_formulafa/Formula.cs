using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _23f_formulafa
{
	class Formula
	{
		public List<Formula> gyerekei;
		public char művelet; // nem csak ∧ ∨ ¬ → ↔, de a p, q, r, ... is művelet lesz!
		public Formula(char művelet)
		{
			this.művelet = művelet;
			this.gyerekei = new List<Formula>();
		}

		public Formula(char művelet, List<Formula> gyerekei)
		{
			this.művelet = művelet;
			this.gyerekei = gyerekei;
		}

		public Formula bal => this.gyerekei[0];
		public Formula jobb => this.gyerekei[1];

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
				if (max < m)
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

		public bool Atomi() => this.gyerekei.Count == 0;

		public Formula NemÉs()
		{
			if (this.Atomi())
				return this;
			if (this.művelet == '¬')
				return -this.gyerekei[0].NemÉs();
			if (this.művelet == '&')
				return this.gyerekei[0].NemÉs() * this.gyerekei[1].NemÉs();
			if (this.művelet == 'V')
				return -(-this.gyerekei[0].NemÉs() * -this.gyerekei[1].NemÉs());
			if (this.művelet == '→')
				return -(this.gyerekei[0].NemÉs() * -this.gyerekei[1].NemÉs());
			if (this.művelet == '↔')
				return this.gyerekei[0].NemÉs() > this.gyerekei[1].NemÉs() * this.gyerekei[1].NemÉs() > this.gyerekei[0].NemÉs();
			if (this.művelet == '⨂')
				return -(this.gyerekei[0].NemÉs() == this.gyerekei[1].NemÉs());
			return null;
		}

		public bool Negatív_literál() => művelet == '¬' && this.gyerekei[0].Atomi();

		/// <summary>
		/// Literálnak nevezzük az atomi formulákat vagy azok tagadásait.
		/// </summary>
		/// <returns></returns>
		public bool Literál() => Atomi() || (művelet == '¬' && this.gyerekei[0].Atomi());

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

		public static (bool, HashSet<Dictionary<Formula, bool>>) Kielégíthető(HashSet<Formula> formulahalmaz)
		{
			Popper<Formula> formulák = new Popper<Formula>();
			foreach (Formula formula in formulahalmaz)
				formulák.Push(formula);

			HashSet<Dictionary<Formula, bool>> Modellek = new HashSet<Dictionary<Formula, bool>>();

			bool nyitott = SemanticTableaux(
				formulák,
				new HashSet<Formula>(),
				new HashSet<Formula>(),
				Modellek);

			return (nyitott, Modellek);
		}

		static Dictionary<Formula, bool> Modell_építése_literálokból(HashSet<Formula> pozitív_literálok, HashSet<Formula> negatív_literálok)
		{
			Dictionary<Formula, bool> értékelés = new Dictionary<Formula, bool>();
			foreach (Formula atom in pozitív_literálok)
				értékelés[atom] = true;
			foreach (Formula negatív_literál in negatív_literálok)
				értékelés[negatív_literál.gyerekei[0]] = false;
			return értékelés;
		}

		static bool SemanticTableaux(
			Popper<Formula> formulák,
			HashSet<Formula> pozitiv_literalok,
			HashSet<Formula> negativ_literalok,
			HashSet<Dictionary<Formula, bool>> modellek)
		{
			// Ha üres a verem
			if (formulák.Count == 0)
			{
				modellek.Add(Modell_építése_literálokból(pozitiv_literalok, negativ_literalok));
				return true;
			}

			Formula f = formulák.Pop();

			if (f.Atomi())
			{
				if (negativ_literalok.Contains(-f))
					return false;
				pozitiv_literalok.Add(f);
				return SemanticTableaux(
					new Popper<Formula>(formulák),
					new HashSet<Formula>(pozitiv_literalok),
					new HashSet<Formula>(negativ_literalok),
					modellek
					);
			}
			if (f.Negatív_literál())
			{
				if (pozitiv_literalok.Contains(f.gyerekei[0]))
					return false;
				negativ_literalok.Add(f);
				return SemanticTableaux(
					new Popper<Formula>(formulák),
					new HashSet<Formula>(pozitiv_literalok),
					new HashSet<Formula>(negativ_literalok),
					modellek
					);
			}

			// ... a formula nem tagadott

			if (f.művelet == '&')
				return SemanticTableaux((f.bal, f.jobb) + formulák, new HashSet<Formula>(pozitiv_literalok), new HashSet<Formula>(negativ_literalok), modellek);
			if (f.művelet == 'V')
				return 
					SemanticTableaux(f.bal + formulák, new HashSet<Formula>(pozitiv_literalok), new HashSet<Formula>(negativ_literalok), modellek)
					|
					SemanticTableaux(f.jobb + formulák, new HashSet<Formula>(pozitiv_literalok), new HashSet<Formula>(negativ_literalok), modellek);
			if (f.művelet == '>')
				return
					SemanticTableaux(-f.bal + formulák, new HashSet<Formula>(pozitiv_literalok), new HashSet<Formula>(negativ_literalok), modellek)
					|
					SemanticTableaux(f.jobb + formulák, new HashSet<Formula>(pozitiv_literalok), new HashSet<Formula>(negativ_literalok), modellek);
			if (f.művelet == '=')
				return
					SemanticTableaux((f.bal, f.jobb) + formulák, new HashSet<Formula>(pozitiv_literalok), new HashSet<Formula>(negativ_literalok), modellek)
					|
					SemanticTableaux((-f.bal, -f.jobb) + formulák, new HashSet<Formula>(pozitiv_literalok), new HashSet<Formula>(negativ_literalok), modellek);
			if (f.művelet == '⨂')
				return
					SemanticTableaux((f.bal, -f.jobb) + formulák, new HashSet<Formula>(pozitiv_literalok), new HashSet<Formula>(negativ_literalok), modellek)
					|
					SemanticTableaux((-f.bal, f.jobb) + formulák, new HashSet<Formula>(pozitiv_literalok), new HashSet<Formula>(negativ_literalok), modellek);
			// ... a formula tagadott
			if (f.művelet == '¬')
			{
				Formula tf = f.gyerekei[0];
				if (tf.művelet == '¬')
					return SemanticTableaux(tf.bal+formulák, new HashSet<Formula>(pozitiv_literalok), new HashSet<Formula>(negativ_literalok), modellek);
                if (tf.művelet == '&')
					return
						SemanticTableaux(-tf.bal + formulák, new HashSet<Formula>(pozitiv_literalok), new HashSet<Formula>(negativ_literalok), modellek)
						|
						SemanticTableaux(-tf.jobb + formulák, new HashSet<Formula>(pozitiv_literalok), new HashSet<Formula>(negativ_literalok), modellek);
				if (tf.művelet == 'V')
					return SemanticTableaux((-f.bal, -f.jobb) + formulák, new HashSet<Formula>(pozitiv_literalok), new HashSet<Formula>(negativ_literalok), modellek);
				if (tf.művelet == '>')
					return SemanticTableaux((f.bal, -f.jobb) + formulák, new HashSet<Formula>(pozitiv_literalok), new HashSet<Formula>(negativ_literalok), modellek);
				if (tf.művelet == '=')
					return
						SemanticTableaux((f.bal, -f.jobb) + formulák, new HashSet<Formula>(pozitiv_literalok), new HashSet<Formula>(negativ_literalok), modellek)
						|
						SemanticTableaux((-f.bal, f.jobb) + formulák, new HashSet<Formula>(pozitiv_literalok), new HashSet<Formula>(negativ_literalok), modellek);
				if (tf.művelet == '⨂')
					return
						SemanticTableaux((f.bal, f.jobb) + formulák, new HashSet<Formula>(pozitiv_literalok), new HashSet<Formula>(negativ_literalok), modellek)
						|
						SemanticTableaux((-f.bal,-f.jobb) + formulák, new HashSet<Formula>(pozitiv_literalok), new HashSet<Formula>(negativ_literalok), modellek);
			}
			throw new NotImplementedException($"Ez a konnektívum nincs implementálva: {f.művelet}");
		}

		public static bool Ellentmondásos(HashSet<Formula> formulahalmaz) => !Kielégíthető(formulahalmaz).Item1;

		public static bool Logikai_igazság(Formula formula) => Ellentmondásos(new HashSet<Formula> { -formula });

	}
}


