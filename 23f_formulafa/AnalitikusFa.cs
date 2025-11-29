using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _23f_formulafa
{
	class Analitikus_fa
	{
		public bool kielégíthető;
		public List<Analitikus_fa> gyerekei;
		public Stack<Formula> gyökér;
		public override string ToString()
		{
			string s = "";
			string gyokerstr = string.Join("\\n", gyökér);

			foreach (Analitikus_fa gyerek in gyerekei)
			{
				s += gyokerstr + " -> " + gyerek.ToString() + ";\n";
			}

			s += kielégíthető ? "O" : "*";

			return s;
		}

		public Analitikus_fa(Stack<Formula> formulahalmaz, HashSet<Formula> literálok)
		{
			this.gyökér = formulahalmaz; // kell-e?
			this.gyerekei = new List<Analitikus_fa> { };
			HashSet<Formula> továbbadott_literálok = new HashSet<Formula>(literálok);

			if (formulahalmaz.Count == 0)
			{
				this.kielégíthető = false;
				return;
			}

			Formula teteje = formulahalmaz.Pop();

			// összes lehetőség, tekintve, hogy ide csak akkor jutunk, ha már &-re és tagadásra átírtuk az egész formulát!
			// - atomi formulával van dolgunk
			// - tagadott valamilyen formulával van dolgunk
			//   - tagadott atomi formula
			//   - duplán tagadott formula
			//   - tagadott &-es formula
			// - &-es formula

			if (teteje.Atomi())
			{
				// ha atomi formulával állunk szemben

				if (literálok.Contains(-teteje)) // ha találunk ellentmondást, akkor vége a kisebb fákra való szétbontásnak.
				{
					this.kielégíthető = false;
				}
				else
				{
					Stack<Formula> kov_formulahalmaz = new Stack<Formula>(formulahalmaz);
					továbbadott_literálok.Add(teteje);
					this.gyerekei.Add(new Analitikus_fa(kov_formulahalmaz, továbbadott_literálok));
					this.kielégíthető = this.gyerekei[0].kielégíthető;
				}

			}
			else if (teteje.művelet == '¬')
			{
				// ha tagadott valamilyen formulával állunk szemben... lásd lejjebb
				Formula gyerek = teteje.gyerekei[0];

				if (gyerek.Atomi())
				{
					// ha tagadott atomi formulával állunk szemben: "-p"
					if (literálok.Contains(gyerek))  // ezt most nem kell tagadni, mert teteje az, ami tagadott, tehát ha annak a gyereke ott van a literálhalmazban, akkor a teteje ellentmond neki!
					{
						this.kielégíthető = false;
					}
					else
					{
						Stack<Formula> kov_formulahalmaz = new Stack<Formula>(formulahalmaz);
						továbbadott_literálok.Add(teteje);
						this.gyerekei.Add(new Analitikus_fa(kov_formulahalmaz, továbbadott_literálok));
						this.kielégíthető = this.gyerekei[0].kielégíthető;
					}
				}
				else if (gyerek.művelet == '¬')
				{
					// ha a tagadáson belül tagadás van: "--p" származékai
					Formula unoka = gyerek.gyerekei[0];
					Stack<Formula> kov_formulahalmaz = new Stack<Formula>(formulahalmaz);
					kov_formulahalmaz.Push(unoka);
					this.gyerekei.Add(new Analitikus_fa(kov_formulahalmaz, továbbadott_literálok));
					this.kielégíthető = this.gyerekei[0].kielégíthető;
				}
				else if (gyerek.művelet == '&')
				{
					// ha a tagadáson belül & van: "-(p&q)" származékai
					Formula balunoka = -gyerek.gyerekei[0];
					Formula jobbunoka = -gyerek.gyerekei[1];

					Stack<Formula> bal_formulahalmaz = new Stack<Formula>(formulahalmaz);
					bal_formulahalmaz.Push(balunoka);

					Stack<Formula> jobb_formulahalmaz = new Stack<Formula>(formulahalmaz);
					jobb_formulahalmaz.Push(jobbunoka);

					this.gyerekei.Add(new Analitikus_fa(bal_formulahalmaz, továbbadott_literálok));
					this.gyerekei.Add(new Analitikus_fa(jobb_formulahalmaz, továbbadott_literálok));
					this.kielégíthető = this.gyerekei[0].kielégíthető || this.gyerekei[1].kielégíthető;
				}

			}
			else if (teteje.művelet == '&')
			{
				// Ha konjunkcióval állunk szemben
				Formula bal = teteje.gyerekei[0];
				Formula jobb = teteje.gyerekei[1];

				Stack<Formula> kov_formulahalmaz = new Stack<Formula>(formulahalmaz);
				kov_formulahalmaz.Push(bal);
				kov_formulahalmaz.Push(jobb);

				this.gyerekei.Add(new Analitikus_fa(kov_formulahalmaz, továbbadott_literálok));

				this.kielégíthető = this.gyerekei[0].kielégíthető;
			}
			// több lehetőség nincs, mert ez az analitikus fa úgy hívódik majd meg, hogy a formula csak tagadást és konjunkciót tartalmaz majd.

		}
	}
}
