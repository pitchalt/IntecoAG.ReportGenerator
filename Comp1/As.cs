using System;

namespace Comp1
{
	/// <summary>
	/// As состояние автомата
	/// </summary>
	class As : Base
	{
		public As(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//////Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{
				this.link = null;
				this.mainClass.data.As.Add(word.text);
			}				
			else if (this.mainClass.AsReturn !=null)
			{
				this.mainClass.AddWordBegin(word.text, word.type);
				this.link = this.mainClass.AsReturn;
				this.mainClass.AsReturn = null;
			}else
				throw new NoOneClass("Состояние: As, слово = " + word.text);
			return false;
		}
	};
}
