using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CloseMatchCmdApp {
	internal class Program {
		private static void Main(string[] args) {
			var strings = new List<string>();
			var charsApart = 0;
			var linesApart = 5;
			var path = Environment.CurrentDirectory;
			var extensions = "*.*";
			var recursive = false;
			var verbose = false;
			var showErrors = false;
			var displayHelp = false;

			for (var i = 0; i < args.Length; i++) {
				switch (args[i]) {
					case "-s":
						if (i < args.Length) strings.Add(args[i + 1]);
						break;

					case "-c":
						if (i < args.Length) int.TryParse(args[i + 1], out charsApart);
						break;

					case "-l":
						if (i < args.Length) int.TryParse(args[i + 1], out linesApart);
						break;

					case "-p":
						if (i < args.Length) path = args[i + 1];
						break;

					case "-e":
						if (i < args.Length) extensions = args[i + 1].Replace(",", ";");
						break;

					case "-r":
						recursive = true;
						break;

					case "-v":
						verbose = true;
						break;

					case "-x":
						showErrors = true;
						break;

					case "-h":
						displayHelp = true;
						break;
				}
			}

			Console.WriteLine("CloseMatch v0.1");

			if (displayHelp ||
				!strings.Any()) {
				Console.WriteLine("Search for multiple strings in close proximity to each other.");
				Console.WriteLine();
				Console.WriteLine("cm -s text1 -s text2 [-c 1] [-l 2] [-p path] [-e *.cs] [-r] [-v] [-x]");
				Console.WriteLine();
				Console.WriteLine("-s Add texts to search for. Require at least one.");
				Console.WriteLine("-c Make sure each string match is no further than n chars away. Defaults to 0.");
				Console.WriteLine("-l Make sure each string match is no further than n lines away. Defaults to 5.");
				Console.WriteLine("-p Set path to search in. Defaults to current working directory.");
				Console.WriteLine("-e Set extensions to search for. Defaults to *.*");
				Console.WriteLine("-r Do a recursive scan. Defaults to off.");
				Console.WriteLine("-v Output not only the files with hits and the lines, but also the actual text. Defaults to off.");
				Console.WriteLine("-x Shows errors, if any. Defatults to off.");

				return;
			}

			Console.WriteLine();

			var folders = new List<string> {path};
			var findex = 0;
			var hitMatches = 0;
			var fileMatches = 0;

			while (true) {
				if (recursive) {
					try {
						folders.AddRange(
							Directory.GetDirectories(
								folders[findex],
								"*",
								SearchOption.TopDirectoryOnly));
					}
					catch (Exception ex) {
						if (showErrors) {
							Console.WriteLine("Error while getting subfolders from folder: {0}", folders[findex]);
							Console.WriteLine(ex.Message);
						}
					}
				}

				var files = new List<string>();

				foreach (var ext in extensions.Split(';')) {
					try {
						files.AddRange(
							Directory.GetFiles(
								folders[findex],
								ext,
								SearchOption.TopDirectoryOnly));
					}
					catch (Exception ex) {
						if (!showErrors)
							continue;

						Console.WriteLine("Error while getting files from: {0}", folders[findex]);
						Console.WriteLine(ex.Message);
					}
				}

				foreach (var file in files) {
					string[] lines = {};

					try {
						lines = File.ReadAllLines(file);
					}
					catch (Exception ex) {
						if (!showErrors)
							continue;

						Console.WriteLine("Error while reading file: {0}", file);
						Console.WriteLine(ex.Message);
					}

					if (lines.Length == 0)
						continue;

					var hits = 0;
					var hitLines = new List<int>();
					var outputs = new List<string>();

					for (var i = 0; i < lines.Length; i++) {
						var lineSps = new int[strings.Count];

						lineSps[0] = lines[i].IndexOf(strings[0], StringComparison.InvariantCultureIgnoreCase);

						if (lineSps[0] == -1)
							continue;

						var hit = false;

						if (strings.Count > 1) {}
						else {
							hit = true;
						}

						if (!hit)
							continue;

						hitLines.Add(i + 1);
						hits++;
					}

					if (hits == 0)
						continue;

					hitMatches += hits;
					fileMatches++;

					outputs.Insert(0, "Lines: " + string.Join(", ", hitLines));
					outputs.Insert(0, "Hits: " + hits);
					outputs.Insert(0, "File: " + file);

					foreach (var str in outputs) {
						Console.WriteLine(str);
					}

					Console.WriteLine();
				}

				findex++;

				if (findex == folders.Count)
					break;
			}

			Console.WriteLine(
				"Found {0} hits in {1} files matching criteria.",
				hitMatches,
				fileMatches);

			// TODO: Remove!!!
			Console.ReadKey();
		}
	}
}