using NLog;
using Stump.Core.Attributes;
using Stump.Core.Threading;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace Stump.Server.BaseServer
{
	public class ConsoleBase
	{
		[Variable(true)]
		public static int AskWaiterInterval = 20;
		private readonly Logger logger = LogManager.GetCurrentClassLogger();
		public static readonly string[] AsciiLogo = new string[]
		{
			"  _____  _______  _     _   __   __   _____   ",
			" (_____)(__ _ __)(_)   (_) (__)_(__) (_____)  ",
			"(_)___     (_)   (_)   (_)(_) (_) (_)(_)__(_) ",
			"  (___)_   (_)   (_)   (_)(_) (_) (_)(_____)  ",
			"  ____(_)  (_)   (_)___(_)(_)     (_)(_)      ",
			" (_____)   (_)    (_____) (_)     (_)(_)      "
		};
		public static readonly ConsoleColor[] LogoColors = new ConsoleColor[]
		{
			ConsoleColor.DarkCyan,
			ConsoleColor.DarkRed,
			ConsoleColor.DarkGray,
			ConsoleColor.DarkGreen,
			ConsoleColor.DarkYellow,
			ConsoleColor.Green,
			ConsoleColor.Red,
			ConsoleColor.White
		};
		protected string Cmd = "";
		protected readonly ConditionWaiter m_conditionWaiter;
		public bool EnteringCommand
		{
			get;
			set;
		}
		public bool AskingSomething
		{
			get;
			set;
		}
		protected ConsoleBase()
		{
			Func<bool> predicate = () => !this.AskingSomething && Console.KeyAvailable;
			this.m_conditionWaiter = new ConditionWaiter(predicate, -1, 20);
		}
		public static void SetTitle(string str)
		{
			Console.Title = str;
		}
		public static void DrawAsciiLogo()
		{
			ConsoleColor foregroundColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleBase.LogoColors.ElementAt(new Random().Next(ConsoleBase.LogoColors.Count<ConsoleColor>()));
			string[] asciiLogo = ConsoleBase.AsciiLogo;
			for (int i = 0; i < asciiLogo.Length; i++)
			{
				string text = asciiLogo[i];
				int totalWidth = (Console.BufferWidth + text.Length) / 2;
				Console.WriteLine(text.PadLeft(totalWidth));
			}
			Console.ForegroundColor = foregroundColor;
		}
		protected virtual void Process()
		{
		}
		public void Start()
		{
			Task.Factory.StartNew(new Action(this.Process));
		}
		public bool AskAndWait(string request, int delay)
		{
			bool result;
			lock (this.m_conditionWaiter)
			{
				try
				{
					this.AskingSomething = true;
					this.logger.Warn(string.Concat(new object[]
					{
						request,
						Environment.NewLine,
						"[CANCEL IN ",
						delay,
						" SECONDS] (y/n)"
					}));
					if (ConditionWaiter.WaitFor(() => !this.EnteringCommand && Console.KeyAvailable, delay * 1000, ConsoleBase.AskWaiterInterval))
					{
						string a = Console.ReadLine().ToLower();
						this.AskingSomething = false;
						result = (a == "y" || a == "yes");
					}
					else
					{
						this.AskingSomething = false;
						result = false;
					}
				}
				finally
				{
					this.AskingSomething = false;
				}
			}
			return result;
		}
	}
}
