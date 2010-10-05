using System;

namespace Comp1
{
    /// <summary>
    /// Базовый класс для WHERE
    /// </summary>
	abstract class BaseWhere : Base
	{
		public BaseWhere(Link l):
			base(l){}

		/*protected string TableName1 = null;
		protected string TableName2 = null;
		protected string ColumnName1 = null;
		protected string ColumnName2 = null;
		protected TypeOper oper = TypeOper.NULL;
		protected ColumnType columnType1 = ColumnType.NAME;
		protected ColumnType columnType2 = ColumnType.NAME;*/
		public abstract override bool NextState(TheWord word);

	};
	/// <summary>
	/// Where состояние автомата
	/// </summary>
	class Where : BaseWhere
	{
		public Where(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//int y;
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM && word.text.IndexOf("'") != -1)
			{
				//Значит это значение, а не имя колонки
				this.mainClass.CurentExpr.Type1 = ColumnType.VALUE;
				this.mainClass.CurentExpr.ColName1 = word.text.Trim('\'','\"');
			}
			else if (word.type == TypeWord.TERM)
			{
				this.link = this.mainClass.where_table1;
				this.mainClass.CurentExpr.ColName1 = word.text;
			}
			else
				throw new NoOneClass("Состояние: Where, слово = " + word.text);
			return true;
		}
	}

	/// <summary>
	/// Where_Table1 состояние автомата
	/// </summary>
	class Where_Table1: BaseWhere
	{
		public Where_Table1(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.NonTERM && (word.text.ToLower()== "=" || word.text.ToLower()== "<" || word.text.ToLower()== ">" || word.text.ToLower()== "!=" || word.text.ToLower()== "<=" || word.text.ToLower()== ">" ))
			{
				this.link = this.mainClass.where_comp;
				switch (word.text)
				{
					case "=":
						this.mainClass.CurentExpr.oper = TypeOper.RAVNO;
						break;
					case "!=":
						this.mainClass.CurentExpr.oper = TypeOper.NERAVNO;
						break;
					case ">":
						this.mainClass.CurentExpr.oper = TypeOper.BOLSHE;
						break;
					case "<":
						this.mainClass.CurentExpr.oper = TypeOper.MENSHE;
						break;
					case "<=":
						this.mainClass.CurentExpr.oper = TypeOper.BOLSHERAVNO;
						break;
					case ">=":
						this.mainClass.CurentExpr.oper = TypeOper.MENSHERAVNO;
						break;
					default:
						break;
				}
			}
			else if (word.type == TypeWord.NonTERM && word.text.ToLower()== ".")
				this.link = this.mainClass.where_point1;
			else
				throw new NoOneClass("Состояние: Where_Table1, слово = " + word.text);
			return true;
		}
	};
	/// <summary>
	/// Where_Table2 состояние автомата
	/// </summary>
	class Where_Table2: BaseWhere
	{
		public Where_Table2(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM && word.text.ToLower() == "as" && mainClass.WhereReturn == null)
			{
				this.mainClass.data.where.Add(mainClass.CurentExpr);
				this.link = this.mainClass.asClass;
			}
			else if (word.text.ToLower() == "group")
				this.link = this.mainClass.group;
			else if (word.type == TypeWord.TERM && (word.text.ToLower() == "or" || word.text.ToLower() == "and"))
			{
				//Expr ex = new Expr(this.TableName1, this.ColumnName1, this.TableName2, this.ColumnName2);
				if (word.text.ToLower() == "or")
					this.mainClass.data.where.AddOr(mainClass.CurentExpr);
				else if (word.text.ToLower() == "and")
					this.mainClass.data.where.AddAnd(mainClass.CurentExpr);

				//Создаем новые экземпляры
				this.mainClass.CurentExpr = new Expr();
				this.mainClass.CurentOper = new Oper();
				this.link = this.mainClass.whereClass;
			}
			else if (word.type == TypeWord.NonTERM && word.text.ToLower()== ".")
				this.link = this.mainClass.where_point2;
			else if (this.mainClass.WhereReturn != null)
			{
				this.mainClass.data.where.Add(mainClass.CurentExpr);
				this.mainClass.AddWordBegin(word.text, word.type);
				this.link = this.mainClass.WhereReturn;
				this.mainClass.WhereReturn = null;
			}
			else
				throw new NoOneClass("Состояние: Where_Table2, слово = " + word.text);
			return true;
		}
	};
	/// <summary>
	/// Where_Column1 состояние автомата
	/// </summary>
	class Where_Column1: BaseWhere
	{
		public Where_Column1(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);

			if (word.type == TypeWord.NonTERM && (word.text.ToLower()== "=" || word.text.ToLower()== "<" || word.text.ToLower()== ">" || word.text.ToLower()== "!=" || word.text.ToLower()== "<=" || word.text.ToLower()== ">" ))
			{
				this.link = this.mainClass.where_comp;
				switch (word.text)
				{
					case "=":
						this.mainClass.CurentExpr.oper = TypeOper.RAVNO;
						break;
					case "!=":
						this.mainClass.CurentExpr.oper = TypeOper.NERAVNO;
						break;
					case ">":
						this.mainClass.CurentExpr.oper = TypeOper.BOLSHE;
						break;
					case "<":
						this.mainClass.CurentExpr.oper = TypeOper.MENSHE;
						break;
					case "<=":
						this.mainClass.CurentExpr.oper = TypeOper.BOLSHERAVNO;
						break;
					case ">=":
						this.mainClass.CurentExpr.oper = TypeOper.MENSHERAVNO;
						break;
					default:
						break;
				}
			}
			else
				throw new NoOneClass("Состояние: Where_Column1, слово = " + word.text);
			return true;
		}
	};
	/// <summary>
	/// Where_Column2 состояние автомата
	/// </summary>
	class Where_Column2: BaseWhere
	{
		public Where_Column2(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);

			if (word.type == TypeWord.TERM && word.text.ToLower() == "as" && mainClass.WhereReturn == null)
			{
				this.link = this.mainClass.asClass;
				this.mainClass.data.where.Add(mainClass.CurentExpr);
			}
			else if (word.text.ToLower() == "group")
				this.link = this.mainClass.group;
			else if (word.type == TypeWord.TERM && (word.text.ToLower() == "or" || word.text.ToLower() == "and"))
			{
				//Expr ex = new Expr(this.TableName1, this.ColumnName1, this.TableName2, this.ColumnName2);
				if (word.text.ToLower() == "or")
					this.mainClass.data.where.AddOr(mainClass.CurentExpr);
				else if (word.text.ToLower() == "and")
					this.mainClass.data.where.AddAnd(mainClass.CurentExpr);

				//Создаем новые экземпляры
				this.mainClass.CurentExpr = new Expr();
				this.mainClass.CurentOper = new Oper();
				this.link = this.mainClass.whereClass;
			}
			else if (this.mainClass.WhereReturn != null)
			{
				this.mainClass.data.where.Add(mainClass.CurentExpr);
				this.mainClass.AddWordBegin(word.text, word.type);
				this.link = this.mainClass.WhereReturn;
				this.mainClass.WhereReturn = null;
			}
			else
				throw new NoOneClass("Состояние: Where_Column2, слово = " + word.text);
			return true;
		}
	};
	/// <summary>
	/// Where_Point1 состояние автомата
	/// </summary>
	class Where_Point1: BaseWhere
	{
		public Where_Point1(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);

			if (word.type == TypeWord.TERM)
			{
				this.mainClass.CurentExpr.TblName1 = mainClass.CurentExpr.ColName1;
				mainClass.CurentExpr.ColName1 = word.text;
				this.link = this.mainClass.where_column1;
			}
			else
				throw new NoOneClass("Состояние: Where_Point1, слово = " + word.text);
			return true;
		}
	};
	/// <summary>
	/// Where_Point2 состояние автомата
	/// </summary>
	class Where_Point2: BaseWhere
	{
		public Where_Point2(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);

			if (word.type == TypeWord.TERM)
			{
				this.mainClass.CurentExpr.TblName2 = mainClass.CurentExpr.ColName2;
				mainClass.CurentExpr.ColName2 = word.text;
				this.link = this.mainClass.where_column2;
			}
			else
				throw new NoOneClass("Состояние: Where_Point2, слово = " + word.text);
			return true;
		}
	};
	/// <summary>
	/// Where_Comp состояние автомата
	/// </summary>
	class Where_Comp: BaseWhere
	{
		public Where_Comp(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);

			if (word.type == TypeWord.TERM && word.text.IndexOf("'") == -1)
			{
				this.mainClass.CurentExpr.ColName2 = word.text;
				this.link = this.mainClass.where_table2;
			}
			else if (word.type == TypeWord.TERM && word.text.IndexOf("'") != -1)
			{
				//Значит это значение, а не имя колонки
				//this.columnType2 = ColumnType.VALUE;
				//this.ColumnName2 = word.text.Trim('\'','\"');
				mainClass.CurentExpr.Type2 = ColumnType.VALUE;
				mainClass.CurentExpr.ColName2 = word.text.Trim('\'','\"');

				this.link = this.mainClass.where_table2;
			}
			else
				throw new NoOneClass("Состояние: Where_Comp, слово = " + word.text);
			return true;
		}
	};
}
