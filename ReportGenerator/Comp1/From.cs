using System;

namespace Comp1
{
	/// <summary>
	/// From состояние автомата
	/// </summary>
	class From : Base
	{
		public From(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//int u=0;
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (mainClass.data.from.Count > 1)
				mainClass.data.select.SetTable(null);
			else
                mainClass.data.select.SetTable(word.text);

			if (word.type == TypeWord.TERM)
			{
				this.link = this.mainClass.from_1;
				this.mainClass.data.from.Add(word.text);
			}
			else
				throw new NoOneClass("Состояние: From, слово = " + word.text);
			return true;
		}
	};
	class From_1 :Base
	{
		public From_1(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (mainClass.data.from.Count > 1)
                mainClass.data.select.SetTable(null);

			if (word.text.ToLower() == "as")
			{
				this.link = this.mainClass.asClass;
			}
			else if (word.text.ToLower() == "group")
				this.link = this.mainClass.group;
			else if (word.text.ToLower() == "where")
				this.link = this.mainClass.whereClass;
			else if (word.type == TypeWord.NonTERM && word.text.ToLower() == ",")
				this.link = this.mainClass.fromClass;
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "join")
			{
				this.link = this.mainClass.from_join;
			}
			else if (this.mainClass.FromReturn != null)
			{
				this.mainClass.AddWordBegin(word.text, word.type);
				this.link = this.mainClass.FromReturn;
				this.mainClass.FromReturn = null;
			}
			else //Если задан Алиас:
				if (mainClass.data.from.Count > 0)
				{
					string TName = mainClass.data.from[mainClass.data.from.Count-1];
					this.mainClass.data.alias.AddTable(TName, word.text);
					this.mainClass.data.from.ReNameTable(word.text);
					this.link = this.mainClass.from_1;
				}
			else
				throw new Exception("Ошибка!" + this.ToString() + " " + word.text);
			return true;
		}
	};
	class From_Table_1 :Base
	{
		public From_Table_1(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.text.ToLower() == "join")
			{
				this.link = this.mainClass.from_join;
			}
			else
				throw new NoOneClass("Состояние: From_1, слово = " + word.text);
			return true;

		}
	}

	class From_Join :Base
	{
		public From_Join(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			mainClass.iCountJoin++;
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{
				this.link = this.mainClass.from_table_2;
				this.mainClass.data.from.Add(word.text);
			}
			else
				throw new NoOneClass("Состояние: From_1, слово = " + word.text);

			return true;
		}
	}

	class From_Table_2 :Base
	{
		public From_Table_2(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM && word.text.ToLower() == "on")
			{
				//this.link = this.mainClass.from_where;
				this.link = this.mainClass.whereClass;
				this.mainClass.WhereReturn = mainClass.from_where;
				mainClass.iCountJoin--;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "join")
			{
				this.link = this.mainClass.from_join;
			}
			else if (mainClass.data.from.Count > 0)
			{
				string TName = mainClass.data.from[mainClass.data.from.Count-1];
				this.mainClass.data.alias.AddTable(TName, word.text);
				this.mainClass.data.from.ReNameTable(word.text);
				//this.link = this.mainClass.from_1;
				this.link = this.mainClass.from_table_2;
			}
			else
				throw new NoOneClass("Состояние: From-Table_2, слово = " + word.text);
			
			return true;
		}
	}

	class From_Where :Base
	{
		public From_Where(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			mainClass.data.from.AddWhere(mainClass.data.where);
			mainClass.CurentExpr = new Expr();
			mainClass.CurentOper = new Oper();
			mainClass.data.where = new WhereClass(mainClass.data);

			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM && word.text.ToLower() == "on")
			{
				//this.link = this.mainClass.from_where;
				this.link = this.mainClass.whereClass;
				this.mainClass.WhereReturn = mainClass.from_where;
				mainClass.iCountJoin--;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "join")
			{
				this.link = this.mainClass.from_join;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "as")
			{
				this.link = this.mainClass.asClass;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "where")
			{
				this.link = this.mainClass.whereClass;
			}
			else if (word.text.ToLower() == "group")
				this.link = this.mainClass.group;
			else
				throw new NoOneClass("Состояние: From-Table_2, слово = " + word.text);
			
			return true;
		}
	}
	
}
