using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CloseMatchCmdApp {
	internal class Program {
		/// <summary>
		/// Main program entry.
		/// </summary>
		private static void Main(string[] args) {
			var strings = new List<string>();
			var charsApart = 0;
			var linesApart = 2;
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
					var hitLines = new List<string>();
					var outputs = new List<string>();

					for (var i = 0; i < lines.Length; i++) {
						if (lines[i].IndexOf(strings[0], StringComparison.InvariantCultureIgnoreCase) == -1)
							continue;

						var hit = false;

						if (strings.Count > 1) {
							var sli = i - linesApart < 0 ? 0 : i - linesApart;
							var eli = i + linesApart >= lines.Length ? lines.Length : i + linesApart;
							var sps = new Dictionary<int, List<int>>();

							for (var j = sli; j < eli; j++)
								sps.Add(
									j,
									strings
										.Select(t => lines[j].IndexOf(t, StringComparison.InvariantCultureIgnoreCase))
										.ToList());

							if (!sps.Any())
								sps.Add(
									i,
									strings
										.Select(t => lines[i].IndexOf(t, StringComparison.InvariantCultureIgnoreCase))
										.ToList());

							if (charsApart > 0) {
								var shits = 1;
								var spm = sps[i][0];

								for (var j = 1; j < strings.Count; j++) {
									for (var k = sli; k < eli; k++) {
										var spc = sps[k][j];
										int diff;

										if (spm > spc)
											diff = spm - spc;
										else
											diff = spc - spm;

										if (diff >= charsApart)
											continue;

										shits++;
										break;
									}
								}

								if (shits == strings.Count) {
									hit = true;
									hitLines.Add(string.Format("{0} to {1}", sli + 1, eli + 1));

									if (verbose) {
										for (var j = sli; j < eli; j++)
											ConsoleWriteLineHighlightWords(
												string.Format(
													"[{0}] {1}",
													j,
													lines[j].Trim()), strings);

										Console.WriteLine();
									}
								}
							}
							else {
								var shits = 0;

								for (var j = 0; j < strings.Count; j++) {
									var shit = false;

									for (var k = sli; k < eli; k++) {
										if (sps[k][j] <= -1)
											continue;

										shit = true;
										break;
									}

									if (shit)
										shits++;
								}

								if (shits == strings.Count) {
									hit = true;
									hitLines.Add(string.Format("{0} to {1}", sli + 1, eli + 1));

									if (verbose) {
										for (var j = sli; j < eli; j++)
											ConsoleWriteLineHighlightWords(
												string.Format(
													"[{0}] {1}",
													j,
													lines[j].Trim()), strings);

										Console.WriteLine();
									}
								}
							}
						}
						else {
							hit = true;
							hitLines.Add((i + 1).ToString());

							if (verbose) {
								ConsoleWriteLineHighlightWords(string.Format("[{0}] {1}", i, lines[i].Trim()), strings);
								Console.WriteLine();
							}
						}

						if (!hit)
							continue;
						
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
		}

		/// <summary>
		/// Writes out the line to console with highlight for the given words.
		/// </summary>
		private static void ConsoleWriteLineHighlightWords(string line, List<string> words) {
			var writes = new List<string>();

			while (true) {
				var hit = false;

				foreach (var word in words) {
					if (!line.StartsWith(word, StringComparison.InvariantCultureIgnoreCase))
						continue;

					writes.Add(word);
					line = line.Substring(word.Length);
					hit = true;
					break;
				}

				if (hit)
					continue;

				writes.Add(line.Substring(0, 1));
				line = line.Substring(1);

				if (line.Length == 0)
					break;
			}

			foreach (var write in writes) {
				if (words.Contains(write)) {
					Console.BackgroundColor = ConsoleColor.DarkCyan;
					Console.ForegroundColor = ConsoleColor.White;
				}

				Console.Write(write);

				if (words.Contains(write)) {
					Console.ResetColor();
				}
			}

			Console.Write(Environment.NewLine);
		}
	}
}