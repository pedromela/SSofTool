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

		private int DWORD = 4;


		private int QWORD = 8;

		public int n_instructions = 0;
		
		/*auxliary stuff*/
		private Dictionary<int, char> stack;
		private int pointer;
		private Stack<Frame> frames;

		public Manager()
		{
			cstuff = new Dictionary<string, string>(); // <address , value>
			functions = new List<Function>();
			functions_dict = new Dictionary<string, Function>();
            registers = new Dictionary<string, string>();
			stack = new Dictionary<int, char>();
			frames = new Stack<Frame>();
			pointer = 0;
			InitializeRegisters();
		}

		public void InitializeRegisters()
		{
			registers.Add("rdi", "0000000000000000");
			registers.Add("rsi", "0000000000000000");
			registers.Add("esi", "00000000");
			registers.Add("eax", "00000000");
			registers.Add("rax", "0000000000000000");
			registers.Add("rdx", "0000000000000000");
			registers.Add("rip", "0000000000000000");
			registers.Add("rbp", "FFFFFFFFFFFFFFFF");
			registers.Add("rsp", "FFFFFFFFFFFFFFFF");
		}

		private static Random random = new Random();
		public string RandomString(int length)
		{
			const string chars = "0123456789ABCDEF";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}

		public string InputString(int length)
		{
			const string RawInput = @"Lorem ipsum dolor sit amet, vis et nulla suscipiantur, no sea etiam alterum percipit. Mutat placerat fabellas ne eum, ex per putent deleniti, no accusata atomorum eos. Et quo convenire rationibus, per ne impetus volumus appetere, at eos quis delectus ullamcorper. Tollit legere aliquam ea eam.
Hinc error volutpat pri ea, sit ei persius propriae salutatus.Debet impetus civibus id sea, qui ut quod inciderint. Labore dictas delicata eu sea, nusquam contentiones mei ad, natum tamquam nec cu.Augue falli ceteros ex usu.Ex duo dolor comprehensam. Nam impetus periculis at, usu no legendos atomorum, id justo disputando sea.
Placerat accusamus te nam, usu accusata scripserit cu, cu usu putant intellegat. Ut choro sensibus oportere pro, nobis putant prompta ad eos.An aeterno nominavi ullamcorper per, appareat voluptaria qui cu. Ex dico dicunt deleniti nec, nullam platonem vituperata no vel, cu diam facilisis nam. Impetus tritani vix an.
Saperet fabellas ea est, ad postea sapientem consequat mel, nam id nibh assum.Ad eum iudicabit mediocritatem, no dicunt prodesset sed, quo cu mazim soleat. Te per viris percipit complectitur.Affert putent aliquid qui ad, solet maiorum principes an pro, ius no albucius dissentiunt. In est possit dolorum gubergren.Vel insolens persecuti definiebas ea, soleat convenire nec et.
Usu ex temporibus signiferumque. Te suas lucilius cum, mea dolores albucius at.Eum ad deserunt disputationi, eum ex adhuc scripserit, te alii laudem graeci quo.Ius ne everti fabulas consectetuer, qui quot dolore animal eu.Cum eu eros postea, ea pri tota commune.Usu dicta error ea.
Sed assum dolore delectus eu, nam eu esse periculis voluptaria, usu probo scribentur persequeris no.Utamur suscipit tincidunt eos no.Vel ad ancillae tacimates, sea veri invenire et.Platonem erroribus ne est, in eum ceteros commune.
Ne vis esse eirmod blandit, ei per numquam recusabo. Eu nam tantas repudiare, prompta impedit tractatos eu sed. Sit ex dicant moderatius, eirmod habemus nec at.Probo aperiri accusata ad vel, ei iusto affert theophrastus eam, debet gloriatur eloquentiam ei vim.Dicunt patrioque vituperatoribus ea cum, ea novum exerci assueverit duo.Purto magna te his. Vero salutandi per ea, delicata argumentum pri at.
Eum summo labitur no, vim meis qualisque eloquentiam ex. Solet argumentum interesset eu sea.Aperiri scaevola est no. Utroque epicurei consequat no pri, qui nisl brute homero id.
Pro at saepe primis, nam illud dicta albucius cu, animal labores eam an.Pro quod essent laoreet eu, vis movet praesent cu. Ut qui solum possim convenire, mel atqui tantas tempor cu.Eu pri congue nominati accusamus.Vim ex prima dolor, omnesque intellegam in duo, accusam interpretaris eam cu. Tollit voluptatum theophrastus vis ad.
Ut semper labitur eos, pri sonet eligendi expetenda id, no sonet vivendo accusamus mea. Partem intellegam in vim.Soluta numquam invenire eu pro, tantas adversarium mea no. Rebum hendrerit cotidieque ne ius, wisi veritus mediocrem eu usu.Option inimicus eos in, at erant accusam assentior mel.";
			return RawInput.Substring(0, length);
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
			cstuff.Clear();
			registers.Clear();
			stack.Clear();
			frames.Clear();
			pointer = 0;
			n_instructions = 0;
			InitializeRegisters();
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
			registers["rip"] = AutoComplete(instr.address, 8);
			//n_instructions++;
		}
		public static string AutoComplete(string str, int size)
		{
			int len = str.Length;
			if(len < size)
			{
				for(int i = 0; i < size - len; i++)
				{

					str = '0' + str; // += '0';
				}
			}
			return str;
		}


		private string GetString(string addr, int n)
		{
			Console.WriteLine("pointer : " + pointer);
			Console.WriteLine("addr : " + addr);

			ulong addr_val = ParseToPointer(addr);
			Console.WriteLine("addr_Val : " + addr_val);
			string str = "";
			while (n > 0)
			{
				n--;
				str += stack[(int) addr_val];
				addr_val--;
			}
			return str;
		}

		private string GetString(string addr)
		{
			//Console.WriteLine("addr_Val : " + addr);
			ulong addr_val = ParseToPointer(addr) - 1;
			//Console.WriteLine("addr_Val : " + addr_val);
			string str = "";
			while(stack.ContainsKey((int) addr_val)) // refatorizar tudo pra long , cast pra int temporario
			{
				if(stack[(int)addr_val] != '#')
				{
					str += stack[(int )addr_val];
				}
				else
				{
					str += stack[(int)addr_val];
					break;
				}
				addr_val--;
			}
			return str;
		}

		public ulong ParseToPointer(string addr)
		{

			int len = addr.Length;
			if (len == 8)
			{
				return 0xFFFFFFFF - Convert.ToUInt32(addr, 16);

			}
			else if(len == 16)
			{
				return 0xFFFFFFFFFFFFFFFF - Convert.ToUInt64(addr, 16);
			}
			//else if(len == 16) {
			//	return Convert.ToInt64(addr, 16);
			//}
			return 0;
		}

		public int ParseAddr(string addr)
		{
			int len = addr.Length;
			if (len == 8)
			{
			}
			return Convert.ToInt32(addr, 16);

			//else if(len == 16) {
			//	return Convert.ToInt64(addr, 16);
			//}
			return 0;
		}

		public string ParseHex(string str, int size)
		{
			str = str.Trim();
			if (str.StartsWith("0x"))
			{
				string hex = str.Substring(2);
				return AutoComplete(hex, size);
			}
			else
			{
				return null;
			}
		}

		public static string HexToASCII(String hexString)
		{
			try
			{
				string ascii = string.Empty;
				int offset = 255;
				for (int i = 0; i < hexString.Length; i += 2)
				{
					String hs = string.Empty;

					hs = hexString.Substring(i, 2);
					uint decval = Convert.ToUInt32(hs, 16);
					char character = (char) (decval + offset);
					//Console.WriteLine("char : " + character);
					ascii += character;

				}

				return ascii;
			}
			catch (Exception ex) { Console.WriteLine("HexToString exception : " + ex.Message); }

			return string.Empty;
		}

		public static string ASCIIToHex(String String)
		{
			try
			{
				int offset = 255;

				string hex = string.Empty;

				for (int i = 0; i < String.Length; i++)
				{
					String hs = string.Empty;

					int c = (int)String[i] - offset;

					hex += c.ToString("X2");

				}

				return hex;
			}
			catch (Exception ex) { Console.WriteLine("HexToString exception : " + ex.Message); }

			return string.Empty;
		}

		public string SubReg(string reg, int value)
		{
			return (Convert.ToInt32(registers[reg],16) - value).ToString("X8");
		}

		public string SubReg2(string reg, int value)
		{
			string addr_val = registers[reg];
			//Console.WriteLine("SubReg2 : {0}", addr_val.Length);
			if (addr_val.Length == 8)
			{
				return (Convert.ToInt32(addr_val, 16) - value).ToString("X8");
			}
			else if (addr_val.Length == 16)
			{
				//Console.WriteLine("SubReg CASE 16 : {0}", addr_val.Length);

				return (Convert.ToInt64(addr_val, 16) - value).ToString("X16");
			}
			else if (addr_val.Length == 16)
			{
				return (Convert.ToInt16(addr_val, 16) - value).ToString("X4");
			}
			//Console.WriteLine("SubReg2 : {0}", new String('0', addr_val.Length));
			return new String('0', addr_val.Length);
		}

		public Dictionary<int, char> Stack(string name)
		{
			Function f = functions_dict[name];
			Frame frame;

			foreach (Instruction instr in f.GetInstructions())
			{
				SetRip(instr);
				Console.WriteLine("{0}", instr.op);
				Console.WriteLine("rdx : " + registers["rdx"]);
				//Console.WriteLine("rdi : " + registers["rdi"]);
				Console.WriteLine("rbp : " + registers["rbp"]);
				//Console.WriteLine("rsp : " + registers["rsp"]);
				Console.WriteLine("POINTER : " + pointer);

				switch (instr.op)
				{
					case "sub":
						if(instr.args.Length == 2)
						{
							string reg = instr.args[0].ToString();
							//if (reg.Equals("rsp"))
							//{
								int intValue = Convert.ToInt32(instr.args[1].ToString(), 16);
								frame = new Frame(pointer, pointer + intValue - 1);
								frames.Push(frame);
								for (int i = 0; i < intValue; i++)
								{
									//s.Push('0');
									stack.Add(pointer, '0');
									pointer++;
								}
								//registers["rsp"] = (Convert.ToInt32(registers["rbp"], 16) - intValue).ToString("X8");
								registers[reg] = SubReg2(reg, intValue);
							Console.WriteLine("SUB: {0} ", registers[reg]);

							//Console.WriteLine("limites: {0} - {1} ", frame.start, frame.end);
							//Console.WriteLine((Convert.ToInt32(registers["rbp"],16) - intValue).ToString("X8"));
							//}
						}
						break;
					case "push":
							
						int l = instr.args.Length;
						if(l == 1)
						{
							string reg = instr.args[0].ToString();
							//if (value.Equals("rbp"))
							//{
							string ToWrite = HexToASCII(registers[reg]);
							int len = ToWrite.Length;
							//while (len != 8)
							//{
							//	stack.Add(pointer, '0');
							//	pointer++;
							//	len++;
							//}
							int i = pointer + len -	1;
							foreach (char c in ToWrite)
							{
								if (stack.ContainsKey(i))
								{
									Console.WriteLine("bad pointer : " + pointer + " , c : " + c);
								}else
								{
									//Console.WriteLine("pointer : " + pointer + " , c : " + c);

									stack.Add(i, c);
									pointer++;
									i--;

								}
							}
							//registers[reg] = (0xFFFFFFFF - pointer).ToString("X8");
							//Console.WriteLine("PUSH: {0} , length: {1}", registers["rsp"], registers["rsp"].Length);

							registers["rsp"] = SubReg2("rsp" , len); // actualizar rsp (stack pointer)
							//Console.WriteLine("PUSH: {0} ", SubReg2("rsp", registers["rsp"].Length));

							//}
						}
						break;
					case "mov":
						if (instr.args.Length > 1)
						{
								
							string arg1 = instr.args[0].ToString();
							string[] tokens = arg1.Split(' ');
							if (tokens.Length == 3)
							{
								if ((tokens[0].Substring(1) + tokens[1]).Equals("WORDPTR")) // caso em que o primeiro argumento é do tipo WORD PTR [addr]
								{
									string x = arg1.Substring(10);
									if (x.Contains("rbp"))
									{
										string addr = x;
										x = x.Trim('[', ']');
										string[] args = x.Split('-');
										if (args.Length == 2) // addr do tipo rbp - x
										{
											//Console.WriteLine("buff1 addr: " + args[1]);

											int intValue = Convert.ToInt32(args[1], 16);
											string value = instr.args[1].ToString();
											if (!string.IsNullOrEmpty(value))
											{
												string reg = SubReg2("rbp", intValue);
												if (f.HasVariable(x))
												{
													f.GetVariable(x).stackAddr = reg;
													//Console.WriteLine("buff1 : " + reg);
													if (cstuff.ContainsKey(reg))
													{
														cstuff[reg] = x;
													}
													else
													{
														cstuff.Add(reg, x);
													}
												}

												//if (f.HasVariable(x))
												//{
												//	f.GetVariable(x).stackAddr = reg;

												//}
												//else
												//{
												//	if (cstuff.ContainsKey(reg))
												//	{
												//		Console.WriteLine("SOULDNT BE DONE! : " + reg);
												//		cstuff[reg] = x;
												//	}
												//	else
												//	{
												//		cstuff.Add(reg, x);
												//	}

												//}
												frame = frames.First();
												int i = (int)ParseToPointer(reg) -1;

												string ToWrite = "";

												if (instr.args[1].ToString().StartsWith("0x"))
												{
													int n = 0;
													if (arg1.First() == 'Q')
													{
														n = QWORD;
													}
													if (arg1.First() == 'D')
													{
														n = DWORD;
													}
													ToWrite = HexToASCII(AutoComplete(value.Substring(2), n));
													//Console.WriteLine("Adresss : " + AutoComplete(value.Substring(2), n));
													//Console.WriteLine("ToWrite : " + ToWrite);
													//Console.WriteLine("ToWrite : " + ASCIIToHex(ToWrite));
												}
												else if(registers.ContainsKey(value))
												{
													//Console.WriteLine("WRITING : "+ registers[value]);
													ToWrite = HexToASCII(registers[value]);
												}
												foreach (char c in ToWrite)
												{
													stack[i] = c;
													i--;
												}
											}
										}
									}
								}
							}
							else // caso em que o primeiro argumento é um registo
							{
								foreach (var r in registers)
								{
									if (arg1.Equals(r.Key)) // achar o primeiro registo
									{
										if (instr.args.Length == 3) // 3 args -> tem comentario do tipo # 601040 <stdin@@GLIBC_2.2.5>
										{
											string[] toks = instr.args[2].ToString().Split(' ');
											int n = 0;
											if(instr.args[1].ToString().First() == 'Q')
											{
												n = QWORD;
											}
											if (instr.args[1].ToString().First() == 'D')
											{
												n = DWORD;
											}
											if (toks.Length == 3)
											{
												instr.args[instr.args.Count() - 1] = toks[1];
												toks[1] = AutoComplete(toks[1], n);
												
												registers[r.Key] = toks[1];
												cstuff.Add(toks[1], toks[2]); 
										
											}
										}
										else if(instr.args.Length == 2) // 2  args 
										{
											string arg = instr.args[1].ToString();

											string[] toks = instr.args[1].ToString().Split(' '); //arg.Split(' ');
											string aux = ParseHex(instr.args[1].ToString(), r.Value.Length);

											if (aux != null) //mov reg1, 0x00...
											{
												registers[r.Key] = aux;
											}
											else if(toks.Length == 3) // mov reg, WORD PTR [addr]
											{
												//string addr = arg1.Substring(4);
												if (toks[2].First() == '[' && toks[2].Last() == ']') 
												{

													string addr = toks[2].Trim('[',']');
													string[] args = addr.Split('-');

													if (args.Length == 2) // 
													{
														int intValue = Convert.ToInt32(args[1].Substring(2), 16);
														if (!string.IsNullOrEmpty(instr.args[1].ToString()))
														{
															string reg = SubReg2(args[0], intValue-1);
															//Console.WriteLine("buff1 addr: " + addr);
															int n = 0;
															if (toks[0].First() == 'Q')
															{
																n = QWORD;
															}
															else if (toks[0].First() == 'D')
															{
																n = DWORD;
															}
															//Console.WriteLine("mov {0}  , {1}", instr.args[0].ToString(), reg);
															//Console.WriteLine("mov {0}  , {1}", instr.args[0].ToString()), instr.args[1].ToString());
															registers[instr.args[0].ToString()] = ASCIIToHex(GetString(reg, n));
															Console.WriteLine("buff1 : {0} , {1} , {2}", addr, args[0], registers[instr.args[0].ToString()]);

															//if (f.HasVariable(addr))
															//{
																
															//	f.GetVariable(addr).stackAddr = registers[instr.args[0].ToString()];
															//	//cstuff.Add(GetString(reg, 8), addr);

															//}

														}
													}


												}
											}
											else // mov reg1, reg2
											{
												foreach (var reg in registers)
												{
													if (arg.Equals(reg.Key))
													{
														registers[r.Key] = registers[reg.Key];
														break;
													}
												}
											}
										}
										break;
									}
								}
							}
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
                            {																			  //Console.WriteLine("buff2 : " + SubReg("rbp", intValue));
								string x = args1.Trim('[', ']');
								if (f.HasVariable(x))
								{

									int intValue = Convert.ToInt32(split[1], 16); //Address starting from RBP to save
									string reg = SubReg2("rbp", intValue);
									f.GetVariable(x).stackAddr = reg;

									if (cstuff.ContainsKey(reg))
									{
										Console.WriteLine("SOULDNT BE DONE! : " + reg);

										cstuff[reg] = x;
									}
									else
									{
										cstuff.Add(reg, x);
									}
									//Frame frame = frames.First();
									registers[registername] = SubReg2("rbp", intValue);
									//registers[registername] = (Convert.ToInt32(registers["rbp"], 16) - intValue).ToString("X8");
								}else
								{
									Console.WriteLine("lea : variable {0} doesnt exist.", x);

								}
							}
                        }
						//if (f.HasVariable(x))
						//{
						//	f.GetVariable(x).stackAddr = reg;

						//}
						//else
						//{
						//	if (cstuff.ContainsKey(reg))
						//	{
						//		Console.WriteLine("SOULDNT BE DONE! : " + reg);
						//		cstuff[reg] = x;
						//	}
						//	else
						//	{
						//		cstuff.Add(reg, x);
						//	}

						//}
						break;
					case "call":
						if (instr.args.Length < 1)
						{
							Console.WriteLine("This call makes no sense");
						}
						Console.WriteLine(instr.args[0]);
						//fgets is dangerous, there's 3 arguments  that are put in registers before calling an fgets
						//the buffer (LEA'd into register)
						//the buff_len (mov'd into register)
						//the stdinstream (This is accessed via mov [rip-"code"])
						string buffstart = registers["rdi"];
						string rip = registers["rip"];
						string rdx = registers["rdx"];
						string input;
						Console.WriteLine("default functions input address: " + rdx);
						switch (instr.args[0].ToString())
						{
							//case for dangerous function calls
							case "<fgets@plt>":
								if(cstuff.ContainsKey(rdx))
								{
									//Console.WriteLine("input: " + cstuff[input]);

									if (cstuff[rdx].Equals("<stdin@@GLIBC_2.2.5>"))
									{
										if (cstuff.ContainsKey(buffstart))
										{
											//Console.WriteLine("cstuff[buffstart] : " + cstuff[buffstart]);
											Variable v = f.GetVariable(cstuff[buffstart]);
											if (v != null)
											{
												int varmaxlen = v.bytes;
												int bufflen = Convert.ToInt32(registers["esi"], 16);
												Console.WriteLine("buflen : {0}", bufflen);
												input = InputString(bufflen);
												frame = frames.First();
												int i;
												int start =(int) ParseToPointer(buffstart)-1;
												Console.WriteLine("user input {0}", input);

												for (i = 0; i < bufflen; i++)
												{
													if (i < varmaxlen || i > frame.end)
													{
														if (stack.ContainsKey(start - i))
														{
															stack[start - i] = input[i];

														}
														else
														{
															Console.WriteLine("SEGFAULT: line {0}, pointer {1}", instr.address, start - i);
														}
													}
													else
													{
														Console.WriteLine("OVERFLOW : fgets CANT RIDE OUTSIDE VARIABLE BAUNDARIES var {0}, pointer {1}", v.name, start - i);
													}
												}

												stack[i < varmaxlen ? start - i : start - varmaxlen] = '#';
											}else
											{
												Console.WriteLine("CANT FIND VARIABLE: var {0}", buffstart);
											}
										}
										else
										{
											Console.WriteLine("CANT FIND VARIABLE: var {0}", buffstart);
										}
									}
								}

								break;
							case "<strcpy@plt>":
								//Console.WriteLine("{0} {1} {2}", instr.pos, instr.args[0], instr.args[1]);
								
								if (cstuff.ContainsKey(buffstart) && cstuff.ContainsKey(rdx))
								{
									//Console.WriteLine("buffstart : " + buffstart);
									//Console.WriteLine("cstuff[buffstart] : " + cstuff[buffstart]);
									//Console.WriteLine("input : " + input);

									Variable v = f.GetVariable(cstuff[buffstart]);
									if (v != null)
									{
										int varmaxlen = v.bytes;
										input = GetString(rdx);
										frame = frames.First();
										Console.WriteLine("frame end : " + frame.end +" , bytes : " + v.bytes);
										Console.WriteLine("strcpy input : " + input + ", address : "+ rdx);
										//int var_size = f.GetVariable().bytes;
										for (int i = 0; i < input.Length; i++)
										{
											if (i < varmaxlen)
											{
												if (stack.ContainsKey(frame.end - i))
												{
													stack[frame.end - i] = input[i];
													//Console.WriteLine("line {0}, pointer {1}", instr.address, frame.end - i);

												}
												else
												{
													Console.WriteLine("SEGFAULT: line {0}, pointer {1}", instr.address, frame.end - i);
												}
											}
											else
											{
												Console.WriteLine("OVERFLOW : strcpy CANT RIDE OUTSIDE VARIABLE BAUNDARIES var {0}, pointer {1}", v.name, frame.end - i);
											}
										}
									}
									else
									{
										Console.WriteLine("CANT FIND VARIABLE: var {0}", buffstart);
									}
								}
								else
								{
									Console.WriteLine("CANT FIND VARIABLE: var {0}", rdx);
								}
								break;
                            case "<gets@plt>":
                                //gets assumes everything in the stack can be overwritten
                                //cstuff has addresses in registers, and the rbp-x as value
                                Console.WriteLine("GETS");
                                Variable vg = f.GetVariable(cstuff[buffstart]);
                                List<string> overflown = new List<string>();
                                string address;
                                overflown.Add(vg.name);
                                // checka o RDI para ter o buffstart
                                if (cstuff.ContainsKey(buffstart))
                                {
                                    int i = 4;
                                    Console.WriteLine("bufferstart - " + buffstart); //address do buffer no registo rdi
                                    Console.WriteLine("cstuff[bufferstart] - " + cstuff[buffstart]); // vai buscar o rbp - x onde começa o buffer
                                    //anda pela stack de 4 em 4 e procura vars declaradas, quando encontra um endereço que não é key de cstuff já ñ há mais vars.
                                    while (i < pointer)
                                    {
                                        address = SubReg("rbp", i);
                                        if (cstuff.ContainsKey(address))
                                        {
                                            Variable var = f.GetVariable(cstuff[address]);
                                            if (var != null)
                                            {
                                                i += 4;
                                                overflown.Add(var.name);
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        //i += 4;
                                    }
                                    if (vg != null)
                                    {
                                        //EVERYTHING HIGHER UP IN THE STACK THAN BUFFSTART ADDRESS CAN BE OVERWRITTEN
                                        //VAROVERFLOW
                                        Console.WriteLine("fname: gets " + " vuln_function: main " + " vulnurability: VAROVERFLOW");
                                        foreach (string s in overflown)
                                        {
                                            Console.WriteLine("overflown_var: " + s);
                                        }
                                        //FINDING THE FIRST ADDRESS TO BE OVERWRITTEN
                                        int vglen = vg.bytes;
                                        string[] split = cstuff[buffstart].Split('-');
                                        string offset = split[1];
                                        int intValue = Convert.ToInt32(offset, 16);
                                        frame = frames.First();
                                        int sc = frame.start + intValue - 1;
                                        for (i = 0; i < vglen; i++)
                                        {
                                            stack[sc] = '$';
                                            Console.WriteLine(i.ToString());
                                            sc -= 1;
                                        }
                                        Console.WriteLine("overflown_address: rbp-0x" + (sc - frame.start + 1).ToString("X8"));

                                    }
                                }
                                break;
                            default:
								//Console.WriteLine("function : {0} {1} {2}", instr.pos, instr.args[0], instr.args[1]);
								name = instr.args[0].ToString().Trim('<', '>');
								if (functions_dict.ContainsKey(name))
								{
									//pointer++;
									stack.Concat(Stack(name));
									frames.Pop();
								}
								break;
						}
						break;

                            
					case "leave":

						break;
					case "ret":

						break;
					default:
						break;
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
