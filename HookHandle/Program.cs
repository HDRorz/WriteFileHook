using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using WriteFileHooker;

namespace HookHandle
{
	class Program
	{
		static void Main(string[] args)
		{

			var t = new Thread(Hook);

			t.Start();

			while(true)
			{
				//Console.Out.WriteLine("");
			}
			

			Console.In.Read();
		}



		static void Hook()
		{
			try
			{
				var hooker = new WriteFileHooker.WriteFileHooker("Log4netLoger.exe");

				while (true)
				{
					Thread.CurrentThread.GetApartmentState();
				}
			}
			catch (Exception ex)
			{
				Console.Out.WriteLine(ex.ToString());
			}
		}
	}
}
