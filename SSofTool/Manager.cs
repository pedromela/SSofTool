using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSofTool
{
	class Manager
	{
		private List<Function> functions;
		private Dictionary<string, Function> functions_dict;
        private Dictionary<string,string> registers;
		private Dictionary<string, string> cstuff;
		public Manager()
		{
			cstuff = new Dictionary<string, string>(); 
			functions = new List<Function>();
			functions_dict = new Dictionary<string, Function>();
            registers = new Dictionary<string, string>();
            registers.Add("rdi", "0");
            registers.Add("esi", "0");
			registers.Add("rax", "0");
			registers.Add("rdx", "0");
			registers.Add("rip", "0");
			registers.Add("rbp", "0");

		}

		public void AddFunction(string name, Function f)
		{
			functions_dict.Add(name, f);
			functions.Add(f);
		}

		public void Clear()
		{
			functions.Clear();
			functions_dict.Clear();
		}

		public Function GetFunction(string name) {
			functions_dict.TryGetValue(name, out Function f);
			return f;
		}
		 
		public List<Function> GetFunctions()
		{
			return functions;
		}

		public string RenderCode()
		{
			string res = "";

			foreach (var pair in functions_dict)
			{
				res += pair.Key + ":\n" + pair.Value.Render();
			}
			return res;

		}
		public string GetRegister(string name)
		{
			if(registers.ContainsKey(name))
			{
				return registers[name];
			}
			return null;
		}

		public void SetRip(Instruction instr)
		{
			registers["rip"] = instr.address;

		}
		public string AutoComplete(string str, int size)
		{
			if(str.Length < 8)
			{
				for(int i = 0; i <= 8 - str.Length; i++)
				{
					str = '0' + str; // += '0';
				}
			}
			return str;
		}
		public Dictionary<int, char> Stack()
		{
			Dictionary<int, char> stack = new Dictionary<int, char>();
			int pointer = 0;
			Stack<char> s = new Stack<char>();
			Stack<Frame> frames = new Stack<Frame>();
			//Stack<string> frames
			foreach (Function f in functions)
			{
				foreach (Instruction instr in f.GetInstructions())
				{
					SetRip(instr);
					Console.WriteLine("rbp : " + registers["rbp"]);
					switch (instr.op)
					{
						case "sub":
							if(instr.args.Length == 2)
							{
								if (instr.args[0].ToString().Equals("rsp"))
								{

									//Console.WriteLine("RSP: " + instr.args[1].ToString());
									int intValue = Convert.ToInt32(instr.args[1].ToString(), 16);
                                    //Frame frame = frames.First();
                                    //frame.end = pointer + intValue;
									Frame frame = new Frame(pointer, pointer + intValue);
									frames.Push(frame);
									for (int i = 0; i < intValue; i++)
									{
										//s.Push('0');
										stack.Add(pointer, '0');
										pointer++;
									}
									Console.WriteLine("limites: {0} - {1} ", frame.start, frame.end);
                                    Console.WriteLine(pointer.ToString());
								}
							}
							break;
						case "push":
							
							int l = instr.args.Length;
							if(l == 1)
							{
								string value = instr.args[0].ToString();
								if (value.Equals("rbp"))
								{
                                    Frame frame = new Frame();
                                    frame.start = pointer;
                                    frames.Push(frame);
									int len = instr.address.Length;
									while (len != 8)
									{
										s.Push('0');
										stack.Add(pointer, '0');
										pointer++;
										len++;
									}
									foreach (char c in instr.address)
									{
										s.Push(c);
										stack.Add(pointer,c);
										pointer++;
									} 
								}
							}
							break;
						case "mov":
							if (instr.args.Length > 1)
							{
								
								string arg1 = instr.args[0].ToString();
								string[] tokens = arg1.Split(' ');
								if (tokens.Length == 3)
								{
									if ((tokens[0].Substring(1) + tokens[1]).Equals("WORDPTR"))
									{
										string x = arg1.Substring(10);
										if (x.Contains("rbp"))
										{

											x = x.Trim('[', ']');
											string[] args = x.Split('-');
											if (args.Length == 2)
											{
												int intValue = Convert.ToInt32(args[1], 16);
												if (!string.IsNullOrEmpty(instr.args[1].ToString()))
												{
													Frame frame = frames.First();
													int i = frame.start + intValue - 1;
													//Console.WriteLine("i : " + i);
													foreach (char c in instr.args[1].ToString().Substring(2))
													{
														stack[i] = c;
														i--;
													}
												}
												//Console.WriteLine("WORD PTR " + intValue);
											}
										}
									}
								}
								else
								{

									foreach (var r in registers)
									{
										if (arg1.Equals(r.Key))
										{
											Console.WriteLine("{0} {1}, {2}", instr.op, instr.args[0], instr.args[1]);
											if (instr.args.Length == 3)
											{
												string[] toks = instr.args[2].ToString().Split(' ');
												if (toks.Length == 3)
												{
													instr.args[instr.args.Count() - 1] = toks[1];
													toks[1] = AutoComplete(toks[1], 8);
													Console.WriteLine("obs arg: " + toks[2]);
												
													registers[r.Key] = toks[1];
													cstuff.Add(toks[1], toks[2]);
												}
											}
											break;
										}
									}
								}
								//foreach(string str in tokens)
								//{
								//	Console.WriteLine(str);
								//}
							}
							break;
						case "lea":
                            //Loads address in register
                            
                            if (instr.args.Length == 2)
                            {
                                
                                string registername = instr.args[0].ToString(); //Register we're saving address in
                                string args1 = instr.args[1].ToString(); // Something in shape of [rbp-0x50]
                                string address = args1.Trim('[', ']'); // address = rbp-0x50
                                string[] split = address.Split('-');
                                if (split.Length == 2 && split[0] == "rbp")
                                {
                                    int intValue = Convert.ToInt32(split[1], 16); //Address starting from RBP to save
                                    Frame frame = frames.First();
                                    registers[registername] = (0xFFFFFFFF - (frame.start + intValue - 1)).ToString("X8");
                                    Console.WriteLine("HELLO, REGISTER {0} CHARGED WITH ADDRESS {1}", registername, registers[registername]);
                                    
                                }


                            }

							break;
						case "call":
         //                   if (instr.args.Length < 1)
         //                   {
         //                       Console.WriteLine("This call makes no sense");
         //                   }
         //                   Console.WriteLine("{0} {1} {2}", instr.pos, instr.args[0], instr.args[1]);


         //                   switch (instr.args[0].ToString())
         //                   {
         //                       //case for dangerous function calls
         //                       case "<fgets@plt>":
         //                           Console.WriteLine("{0} {1} {2}", instr.pos, instr.args[0], instr.args[1]);

         //                           //fgets is dangerous, there's 3 arguments  that are put in registers before calling an fgets
         //                           //the buffer (LEA'd into register)
         //                           //the buff_len (mov'd into register)
         //                           //the stdinstream (This is accessed via mov [rip-"code"])
         //                           string buffstart;
         //                           string bufflen;
         //                           string[] split;
         //                           string rip;
         //                           string register;
									//Console.WriteLine("CALL:");
         //                           int instrcounter = instr.pos - 4;
         //                           Instruction instrstdin = f.GetInstruction(instrcounter);

         //                           rip = instrstdin.args[1].ToString();
         //                           split = rip.Split(' ');
         //                           if (split[2].Equals("[rip+0x200b03]")) //ponteiro p stdin q vai ser passado ao fgets, uma call ao fgets faz smpr isto nesta ordem
         //                           {
         //                               Console.WriteLine("{0} {1} {2}", instrstdin.pos, instrstdin.args[0], instrstdin.args[1]);
         //                               instrcounter++;
         //                               Instruction instrlea = f.GetInstruction(instrcounter);
         //                               if (instrlea.op.Equals("lea"))
         //                               {
         //                                   register = instrlea.args[0].ToString();
         //                                   buffstart = instrlea.args[1].ToString(); // ponteiro pa variavel buffer na stack é guardado num registo q vai ser passado ao fgets
         //                                   instrcounter++;
         //                                   Instruction instrbufflen = f.GetInstruction(instrcounter);
         //                                   if (instrbufflen.op.Equals("mov") && instrbufflen.args[0].ToString().Equals("esi"))
         //                                   {
         //                                       bufflen = instrbufflen.args[1].ToString(); // o valor do buff_len do fgets e posto num registo
         //                                       instrcounter++;
         //                                       Instruction instrmov = f.GetInstruction(instrcounter++);
         //                                       if (instrmov.op.Equals("mov") && instrmov.args[1].ToString().Equals(register)) // o ponteiro para o buffer que foi colocado num registo é posto noutro registo conhecido ao fgets
         //                                       {
         //                                           Console.WriteLine("fgets chamou, guardámos o bufflen, agr vamos ver se pode existir acesso outofbound, ou um overflow numa variável da frame");
         //                                           List<Variable> variablelist = f.getVariables();


         //                                       }
         //                                   }
         //                               }
         //                           }
                            //        break;

                            //}
                            break;

                            
						case "leave":

							break;
						case "ret":

							break;
						default:
							break;
					}
				}
			}

			return stack;
		}
		public List<KeyValuePair<string, string>> RenderStack()
		{
			List<KeyValuePair<string, string>> stack = new List<KeyValuePair<string, string>>();



			return stack;
		}
	}
    public class Register
    {
        private string name;
        private int value;

        public Register(string name, int value)
        {
            this.name = name;
            this.value = value;
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}
