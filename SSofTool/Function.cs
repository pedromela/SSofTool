using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace SSofTool
{
	class Function
	{
		public int Ninstructions;
		private List<Variable> variables = new List<Variable>();
		private List<Instruction> instructions = new List<Instruction>();

		public Function()
		{
		}

		public Function(JObject jObject) {
			int.TryParse(jObject["Ninstructions"].ToString(), out Ninstructions);
			var vars = jObject["variables"];
			for (int i = 0; i < vars.Count(); i++)
			{
				Variable var = new Variable();

				if (!int.TryParse(vars[i]["bytes"].ToString(), out var.bytes))
				{
					Console.WriteLine("variable {0} : Coudlt convert integer!", i);
				}
				var.type = vars[i]["type"].ToString();
				var.name = vars[i]["name"].ToString();
				var.address = vars[i]["address"].ToString();
				variables.Add(var);
			}
			var instrs = jObject["instructions"];
			for (int i = 0; i < instrs.Count(); i++)
			{
				Instruction instr = new Instruction();
				instr.op = instrs[i]["op"].ToString();
				if (!int.TryParse(instrs[i]["pos"].ToString(), out instr.pos))
				{
					Console.WriteLine("instruction {0} : Coudlnt convert integer!", i);
				}
				if (instrs[i]["args"] != null)
				{
					int j = 0;
					//var args = instrs[i]["args"];

					var args = instrs[i]["args"].Values();
					instr.args = new object[args.Count()];
					foreach (JToken arg in args)
					{
						//Console.WriteLine("args {0}", arg);
						if (arg.ToString().First() == '#')
						{
							instr.args[args.Count() - 1] = arg;
							continue;
						}
						else
						{
							instr.args[j++] = arg;
						}
					}
				}
				instr.address = instrs[i]["address"].ToString();
				instructions.Add(instr);
			}
		}

		public List<Instruction> GetInstructions()
		{
			return instructions;
		}

        public Instruction GetInstruction(int i)
        {
            return instructions[i];
        }

        public List<Variable> getVariables()
        {
            return variables;
        }

        public string Render()
		{
			string res = "";
			foreach(Instruction instr in instructions)
			{
				res += instr.op;
				if (instr.args != null)
				{
					foreach (var arg in instr.args)
					{
						//if (arg.ToString().First() == '#') continue;
						res += " " + arg;
					}
				}
				res += "\n";
			}

			return res;
		}
	}
}
