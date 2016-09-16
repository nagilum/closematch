using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloseMatchCmdApp {
	internal class Program {
		private static void Main(string[] args) {
			var strings = new List<string>();
			var charsApart = 0;
			var linesApart = 5;
			var path = string.Empty; // TODO: Figure out own path and set as default
			var recursive = true;

			/*
			 * 
			 * Usage:
			 * 
			 * cm -s "text-1" -s "text-2" -s "text-3" -c 10 -l 5 -p "c:\code\solution" -r
			 * 
			 * -s = Text to search for, accepts multiples.
			 * -c = How many chars apart,on same line, each text can be. Defaults to 0 which disables this check.
			 * -l = How many lines apart each text can be. Defaults to 5.
			 * -p = Path to search. Defaults to app-folder.
			 * -r = Disable recursive. Defaults to true.
			 * 
			 */
		}
	}
}