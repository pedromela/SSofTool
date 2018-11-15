using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSofTool
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if (args.Count() == 0)
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new Form1());
			}
			else if(args.Count() == 1)
			{
				//Application.EnableVisualStyles();
				//Application.SetCompatibleTextRenderingDefault(false);
				Form1 form = new Form1(args[0]);
			}
		}
	}
}
