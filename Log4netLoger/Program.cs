using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;
using Log4netLoger;

namespace Log4netLoger
{
	class Program
	{
		static void Main(string[] args)
		{
			var loger = LogManager.GetLogger("LOG");

			log4net.Config.XmlConfigurator.Configure(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));

			var random = new Random();
			int i = 1;

			while (true)
			{
				try
				{
					throw new Exception(DateTime.Now.ToString());
				}
				catch (Exception ex)
				{
					Console.Out.WriteLine(i.ToString());
					loger.Info("Program.Main" + " " + i.ToString() + " " + ex.Message);
					i++;
				}

				Thread.Sleep(random.Next(1000 * 10));
			}

		}
	}
}
