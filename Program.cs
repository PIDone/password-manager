using System.Text.Json;

namespace password_manager {
	class Program {
		public class Entry {
			public string service { get; set; }
			public string username { get; set; }
			public string password { get; set; }
		}

		public class Data {
			public List<Entry> data { get; set; }
		}
		
		const string PW_CHARACTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=`~[]\\{}|;':\",./<>?";
		
		static string Path = "";

		static void Main(string[] args) {
			string[] executablePath = System.Reflection.Assembly.GetEntryAssembly().Location.Replace("\\", "/").Split("/");
			executablePath[executablePath.Length - 1] = "passwords.json";
			foreach (string s in executablePath)
				Path += $"{s}/";
			Path = Path.Substring(0, Path.Length - 1);
			
			if (!File.Exists(Path)) {
				Console.WriteLine($"File at {Path} does not exist");
				using (StreamWriter writer = new StreamWriter(Path))
					writer.WriteLine("{ }");
				Console.WriteLine("It has been created");
			}

			if (args.Length == 0) {
				Console.WriteLine("You must call passwd-man with arguments. Run passwd-man help for a list of commands");
				return;
			}

			List<Entry> data = Deserialize();
			if (args[0].ToLower() == "get") {
				if (args.Length != 2) {
					Red();
					Console.WriteLine("Invalid argument, type \"help\" for more information");
					White();
				}

				string service = args[1].ToLower();

				bool found = false;
				foreach (Entry e in data) {
					if (e.service == service) {
						found = true;
						Console.WriteLine($"Username: {e.username}");
						Console.WriteLine($"Password: {e.password}");
						break;
					}
				}

				if (!found) {
					Red();
					Console.WriteLine($"Entry {service} not found");
					White();
				}
			}
			else if (args[0].ToLower() == "set") {
				if (args.Length != 4) {
					Red();
					Console.WriteLine("Invalid argument, type \"help\" for more information");
					White();
				}

				string service = args[1].ToLower();
				string username = args[2];
				string password = args[3];
				
				bool found = false;
				for (int i = 0; i < data.Count; i++) {
					if (data[i].service == service) {
						found = true;
						data[i].username = username;
						data[i].password = password;
					}
				}

				if (!found) {
					data.Add(new Entry {
						service = service,
						username = username,
						password = password
					});
				}

				Serialize(data);
				if (found)
					Console.WriteLine($"Modified entry {service}", found ? "Modified" : "Added");
				else
					Console.WriteLine($"Added entry {service}", found ? "Modified" : "Added");
			}
			else if (args[0].ToLower() == "delete") {;
				if (args.Length != 2) {
					Red();
					Console.WriteLine("Invalid argument, type \"help\" for more information");
					White();
				}
				
				string service = args[1].ToLower();

				int index = 0;
				bool found = false;
				for (int i = 0; i < data.Count; i++) {
					if (data[i].service == service) {
						found = true;
						index = i;
						break;
					}
				}
				
				if (!found) {
					Red();
					Console.WriteLine($"Entry {service} not found");
					White();
				}
				
				data.RemoveAt(index);
				
				Serialize(data);
				Console.WriteLine($"Deleted entry {service}");
			}
			else if (args[0].ToLower() == "rename") {
				if (args.Length != 3) {
					Red();
					Console.WriteLine("Invalid argument, type \"help\" for more information");
					White();
				}

				string service = args[1].ToLower();
				string newName = args[2].ToLower();
			
				bool found = false;
				for (int i = 0; i < data.Count; i++) {
					if (data[i].service == service) {
						found = true;
						data[i].service = newName;
						break;
					}
				}
			
				if (!found) {
					Red();
					Console.WriteLine($"Entry {service} not found");
					White();
				}

				Serialize(data);
				Console.WriteLine($"Renamed entry {service} to {newName}");
			}
			else if (args[0].ToLower() == "gen-password") {
				if (args.Length != 2) {
					Red();
					Console.WriteLine("Invalid argument, type \"help\" for more information");
					White();
				}

				int length;
				try {
					length = Convert.ToInt32(args[1]);
				}
				catch (Exception e) {
					Red();
					Console.WriteLine("Length must be integer");
					White();
					return;
				}

				Console.WriteLine(GenPassword(length));
			}
			else if (args[0].ToLower() == "print") { 
				foreach (Entry e in data)
					Console.WriteLine($"{e.service} {e.username} {e.password}");
			}
			else if (args[0].ToLower() == "list") { 
				foreach (Entry e in data)
					Console.WriteLine(e.service);
			}
			else if (args[0].ToLower() == "exit") {
				Console.Clear();
			}
			else if (args[0].ToLower() == "help") {
				Console.WriteLine("get: query username and password of a certain service. get [service]");
				Console.WriteLine("set: add or modify an entry. set [service] [username] [password]");
				Console.WriteLine("delete: delete an entry. remove [service]");
				Console.WriteLine("rename: rename an entry. rename [service] [service-new-name]");
				Console.WriteLine("gen-password: generate a password with random characters. gen-password [length]");
				Console.WriteLine("print: print everything in password file. no arguments");
				Console.WriteLine("list: list all services in password file. no arguments");
				Console.WriteLine("help: print this text. no arguments");
				Console.WriteLine("exit: exit the program. no arguments");
			}
			else {
				Red();
				Console.WriteLine($"Command {args[0].ToLower()} not found, type \"help\" for list of commands");
				White();
			}
		}

		static List<Entry> Deserialize() {
			List<Entry>? data = JsonSerializer.Deserialize<Data>(File.ReadAllText(Path)).data;
			return data == null ? new List<Entry>() : data;
		}
		static void Serialize(List<Entry> data) {
			JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = false };
			string jsonString = JsonSerializer.Serialize(new Data { data = data }, options);
			using (StreamWriter writer = new StreamWriter(Path))
				writer.WriteLine(jsonString);
		}
		
		static string GenPassword(int length) {
			string password = "";
			Random random = new Random();

			int width = Math.Min(20, Console.WindowWidth - 12);
			
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.CursorVisible = false;

			double currentPercent = -1;
			for (int i = 0; i < length; i++) {
				password += PW_CHARACTERS[random.Next(PW_CHARACTERS.Length)];
				string percentStr = ((double) i / length * 100).ToString();
				double percent = Convert.ToDouble(percentStr.Substring(0, Math.Min(percentStr.Length, 8)));
				int progress = (int) Math.Round((double) i / length * width);
				if (percent == currentPercent)
					continue;
				currentPercent = percent;
				
				Console.Write("[");
				for (int j = 0; j < progress; j++)
					Console.Write("=");
				for (int j = 0; j < width-progress; j++)
					Console.Write(" ");
				Console.Write($"] {percent}%");
				for (int j = 0; j < 8-percent.ToString().Length; j++)
					Console.Write(" ");
			
				for (int j = 0; j < width+12; j++)
					Console.Write("\b");
			}
			
			Console.Write("[");
			for (int j = 0; j < width; j++)
				Console.Write("=");
			Console.WriteLine("] 100%	 ");
			
			Console.CursorVisible = true;
			
			return password;
		}

		static void White() {
			Console.ResetColor();
		}
		static void Red() {
			Console.ForegroundColor = ConsoleColor.Red;
		}
	}
}
