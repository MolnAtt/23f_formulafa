using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace _23f_formulafa
{
	internal class Program
	{
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

			Console.WriteLine("B:");
			Console.WriteLine(B);
			Console.WriteLine("csak éssel és negációval kifejezve:");

			
			Console.WriteLine(B.NemÉs());

			Console.WriteLine(Formula.Kielégíthető(new HashSet<Formula> { B, r, p_es_q }));

		}
	}
}
