using System;
using System.Collections;

namespace Comp1
{
	/// <summary>
	/// Select состояние автомата
	/// </summary>
	class Select : Base
	{
		public Select(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.text.ToLower() == "select" && word.type == TypeWord.TERM)
			{				
				this.link = this.mainClass.select_stmt;
				this.mainClass.data = new SQLStruct();
				//;
				//this.mainClass.idata++;
			}
			else
				throw new NoOneClass("Select, слово: " + word.text);
			return true;
		}
	};
	class Select_All_1 : Base
	{
		public Select_All_1(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.text.ToLower() == "use" && word.type == TypeWord.TERM)
			{				
				this.link = this.mainClass.select_all_use;
				mainClass.data.select.selectType = SELECTTYPE.ALL;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "from")
			{
				this.link = this.mainClass.fromClass;
			}
			else
				throw new NoOneClass("Select, слово: " + word.text);
			return true;
		}
	};
	class Select_Stmt : Base
	{
		public Select_Stmt(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			if (word.text.ToLower() == "select")
			{
				//Тогда вложенный запрос.
				Link l = new Link(this.mainClass.Tr, this.mainClass.ArrayWords);
				l.AddWordBegin("select", TypeWord.TERM);
				l.Run();

				SQLStruct st = (SQLStruct) this.mainClass.data;
				if (l.data != null)
					st.select.Add(l.data);
				else
					throw new NoOneClass("Ошибка во вложенном запросе.");
				this.link = this.mainClass.select_stmt_1;
			}/*else if (word.text.ToLower() == "once")
			{
				mainClass.data.select.selectType = SELECTTYPE.ONCE;
                this.link = this.mainClass.select_stmt_0;
			}else if (word.text.ToLower() == "all")
			{
				mainClass.data.select.selectType = SELECTTYPE.ALL;
                this.link = this.mainClass.select_all;
			}*/else if (word.text.ToLower() == "use")
			{
                this.link = this.mainClass.select_all_use;                
			}
			/*else if (word.type == TypeWord.TERM && word.text.ToLower() == "*")
			{
				SQLStruct st = (SQLStruct) this.mainClass.data;
				st.select.Add(word.text);
				this.link = this.mainClass.select_stmt_1;
			}*/
			else if (word.type == TypeWord.TERM)
			{
				SQLStruct st = (SQLStruct) this.mainClass.data;
				st.select.Add(word.text);
				this.link = this.mainClass.select_stmt_1;
			}
			else
				throw new NoOneClass("Состояние: Select_stmt, слово = " + word.text);
			return true;
		}
	};
	class Select_Stmt_0 : Base
	{
		public Select_Stmt_0(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//int u=0;
			////Console.WriteLine(this.ToString() + ": " + word.text);

			if (word.text.ToLower() == "select")
			{
				//Тогда вложенный запрос.
				Link l = new Link(this.mainClass.Tr, this.mainClass.ArrayWords);
				l.AddWordBegin("select", TypeWord.TERM);
				l.Run();

				SQLStruct st = (SQLStruct) this.mainClass.data;
				if (l.data != null)
					st.select.Add(l.data);
				else
					throw new NoOneClass("Ошибка во вложенном запросе.");
				this.link = this.mainClass.select_stmt_1;
			}
			/*else if (word.type == TypeWord.TERM && word.text.ToLower() == "*")
			{
				SQLStruct st = (SQLStruct) this.mainClass.data;
				st.select.Add(word.text);
				this.link = this.mainClass.select_stmt_1;
			}*/
			else if (word.type == TypeWord.TERM)
			{
				SQLStruct st = (SQLStruct) this.mainClass.data;
				st.select.Add(word.text);
				this.link = this.mainClass.select_stmt_1;
			}
			else
				throw new NoOneClass("Состояние: Select_stmt, слово = " + word.text);
			return true;
		}
	};
	class Select_Stmt_1 :Base
	{
		public Select_Stmt_1(Link l) :
			base (l) {}
		public override bool NextState(TheWord word)
		{
			//int u=0;
			//Console.WriteLine(this.ToString() + ": " + word.text);

			if (word.text.ToLower()== "," && word.type == TypeWord.NonTERM)
			{
				this.link = this.mainClass.select_stmt_0;
			}else if (word.text.ToLower()== "." && word.type == TypeWord.NonTERM)
			{
				this.link = mainClass.select_point;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "as")
			{
				this.link = this.mainClass.select_stmt_as;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "keyed")
			{
                this.link = this.mainClass.select_key_1;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "paint")
			{
				this.link = this.mainClass.select_paint_1;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "from")
			{
				this.link = this.mainClass.fromClass;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "once")
			{
				this.link = this.mainClass.select_once;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "all")
			{
				this.link = this.mainClass.select_all_1;
			}
			else if (this.mainClass.SelectReturn != null)
			{
				//Добавляем обратно слово(вначало)
				this.mainClass.AddWordBegin(word.text, word.type);
				this.link = this.mainClass.SelectReturn;
				this.mainClass.SelectReturn = null;
			}else
				throw new NoOneClass("Ошибка перед словом : " + word.text);
			return true;
		}
        
	}

	class Select_Point : Base
	{
		public Select_Point(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			if (word.type == TypeWord.TERM)
			{
				SQLStruct st = (SQLStruct) this.mainClass.data;
				string str = st.select[st.select.Count-1];
				st.select.Set(str, word.text, st.select.Count-1);

				this.link = this.mainClass.select_stmt_1;
			}
			else
				throw new NoOneClass("Ошибка перед словом : " + word.text);
			return true;
		}
	};
	class Select_Stmt_2 :Base
	{
		public Select_Stmt_2(Link l) :
			base (l) {}
		public override bool NextState(TheWord word)
		{
			//int u=0;
			//Console.WriteLine(this.ToString() + ": " + word.text);

			if (word.text.ToLower()== "," && word.type == TypeWord.NonTERM)
			{
				this.link = this.mainClass.select_stmt_0;
			}			
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "as")
			{
				this.link = this.mainClass.select_stmt_as;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "keyed")
			{
				this.link = this.mainClass.select_key_1;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "paint")
			{
				this.link = this.mainClass.select_paint_1;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "from")
			{
				this.link = this.mainClass.fromClass;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "once")
			{
				this.link = this.mainClass.select_once;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "all")
			{
				this.link = this.mainClass.select_all_1;
			}
			else if (this.mainClass.SelectReturn != null)
			{
				//Добавляем обратно слово(вначало)
				this.mainClass.AddWordBegin(word.text, word.type);
				this.link = this.mainClass.SelectReturn;
				this.mainClass.SelectReturn = null;
			}
			else
				throw new NoOneClass("Ошибка перед словом : " + word.text);
			return true;
		}
        
	}

	class Select_All : Base
	{
		public Select_All(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.text.ToLower() == "use" && word.type == TypeWord.TERM)
			{				
				this.link = this.mainClass.select_all_use;
				//this.mainClass.data = new SQLStruct();
				//;
				//this.mainClass.idata++;
			}else if (word.type == TypeWord.TERM)
			{
				SQLStruct st = (SQLStruct) this.mainClass.data;
				st.select.Add(word.text);
				this.link = this.mainClass.select_stmt_1;
			}
			else
				throw new NoOneClass(this.ToString() + ", слово: " + word.text);
			return true;
		}
	};
	class Select_All_Use : Base
	{
		public Select_All_Use(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			if (word.type == TypeWord.TERM)
			{
				mainClass.data.select.UseTag = word.text;
				this.link = this.mainClass.select_all_word;
			}
			else
				throw new NoOneClass(this.ToString() + ", слово: " + word.text);
			return true;
		}
	};
	class Select_All_Word : Base
	{
		public Select_All_Word(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			if (word.type == TypeWord.TERM && word.text.ToLower() == "from")
			{
				//mainClass.data.select.UseTag = word.text;
				this.link = this.mainClass.fromClass;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "once")
			{
				this.link = this.mainClass.select_once;
			}else if (word.type == TypeWord.TERM && word.text.ToLower() == "all")
			{
				this.link = this.mainClass.select_all_1;
			}
			else
				throw new NoOneClass(this.ToString() + ", слово: " + word.text);
			return true;
		}
	};
	class Select_Once : Base
	{
		public Select_Once(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			mainClass.data.select.selectType = SELECTTYPE.ONCE;
			if (word.type == TypeWord.TERM && word.text.ToLower() == "from")
			{
				this.link = this.mainClass.fromClass;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "once")
			{
				this.link = this.mainClass.select_once;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "all")
			{
				this.link = this.mainClass.select_all_1;
			}
			else
				throw new NoOneClass(this.ToString() + ", слово: " + word.text);
			return true;
		}
	};
	class Select_Stmt_As : Base
	{
		public Select_Stmt_As(Link l) :
		base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{				
				this.link = this.mainClass.select_stmt_1;
				this.mainClass.data.select.AddAs(word.text);
			}
			else
				throw new NoOneClass(this.ToString() + ", слово: " + word.text);
			return true;
		}
	};
	class Select_Key_1 : Base
	{
		public Select_Key_1(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM && word.text.ToLower()=="by")
			{				
				this.link = this.mainClass.select_key_2;
			}
			else
				throw new NoOneClass(this.ToString() + ", слово: " + word.text);
			return true;
		}
	};
	class Select_Key_2 : Base
	{
		public Select_Key_2(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{
				this.mainClass.data.select.AddKey(word.text);
				this.link = this.mainClass.select_key_value;
			}
			else
				throw new NoOneClass(this.ToString() + ", слово: " + word.text);
			return true;
		}
	};
	class Select_Key_Value : Base
	{
		public Select_Key_Value(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.NonTERM && word.text.ToLower()== ",")
			{				
				this.link = this.mainClass.select_key_2;
			}
			else if (word.type == TypeWord.NonTERM && word.text.ToLower()== ".")
			{
                this.link = this.mainClass.select_key_point;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "from")
			{				
				this.link = this.mainClass.fromClass;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower()== "once")
			{
				this.link = this.mainClass.select_once;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "all")
			{
				this.link = this.mainClass.select_all_1;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "paint")
			{
				this.link = this.mainClass.select_paint_1;                
			}
			else
				throw new NoOneClass(this.ToString() + ", слово: " + word.text);
			return true;
		}
	};
	class Select_Key_Point : Base
	{
		public Select_Key_Point(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{
				this.mainClass.data.select.SetKeyTable(word.text);
				this.link = this.mainClass.select_key_value_2;
			}
			else
				throw new NoOneClass(this.ToString() + ", слово: " + word.text);
			return true;
		}
	};
	class Select_Key_Value_2 : Base
	{
		public Select_Key_Value_2(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.NonTERM && word.text.ToLower()== ",")
			{				
				this.link = this.mainClass.select_key_2;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower()== "from")
			{				
				this.link = this.mainClass.fromClass;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower()== "once")
			{
				this.link = this.mainClass.select_once;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "all")
			{
				this.link = this.mainClass.select_all_1;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "paint")
			{
				this.link = this.mainClass.select_paint_1;                
			}
			else
				throw new NoOneClass(this.ToString() + ", слово: " + word.text);
			return true;
		}
	};
	class Select_Paint_1 : Base
	{
		public Select_Paint_1(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.text.ToLower() == "by" && word.type == TypeWord.TERM)
			{				
				this.link = this.mainClass.select_paint_2;
			}
			else
				throw new NoOneClass("Состояние: Select, слово = " + word.text);
			return true;
		}
	};
	class Select_Paint_2 : Base
	{
		public Select_Paint_2(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{
				//Добавляем Paint
				mainClass.data.select.paint.By.name = word.text;
				this.link = this.mainClass.select_paint_col;
			}
			else
				throw new NoOneClass("Состояние: Select, слово = " + word.text);
			return true;
		}
	};
	class Select_Paint_Col : Base
	{
		public Select_Paint_Col(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.text.ToLower() == "for" && word.type == TypeWord.TERM)
			{				
				this.link = this.mainClass.select_paint_for;
			}
			else if (word.text.ToLower()== "." && word.type == TypeWord.NonTERM)
			{
                this.link = this.mainClass.select_paint_col_point;
			}
			else
				throw new NoOneClass("Состояние: Select, слово = " + word.text);
			return true;
		}
	};
	class Select_Paint_Col_Point : Base
	{
		public Select_Paint_Col_Point(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{				
				this.link = this.mainClass.select_paint_col_word;
                this.mainClass.data.select.paint.By.table = this.mainClass.data.select.paint.By.name;
				this.mainClass.data.select.paint.By.name = word.text;
			}
			else
				throw new NoOneClass("Состояние: Select, слово = " + word.text);
			return true;
		}
	};
	class Select_Paint_Col_Word : Base
	{
		public Select_Paint_Col_Word(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.text.ToLower() == "for" && word.type == TypeWord.TERM)
			{				
				this.link = this.mainClass.select_paint_for;
			}
			else
				throw new NoOneClass("Состояние: Select, слово = " + word.text);
			return true;
		}
	};
	class Select_Paint_For : Base
	{
		public Select_Paint_For(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{	
				//Добавляем значение для раскрасски
				mainClass.data.select.paint.AddVal(word.text.Trim('\'','\"','`'));
				this.link = this.mainClass.select_paint_value;
			}
			else
				throw new NoOneClass("Состояние: Select, слово = " + word.text);
			return true;
		}
	};
	class Select_Paint_Value : Base
	{
		public Select_Paint_Value(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.text.ToLower() == "use" && word.type == TypeWord.TERM)
			{				
				this.link = this.mainClass.select_paint_use;
			}
			else
				throw new NoOneClass("Состояние: Select, слово = " + word.text);
			return true;
		}
	};
	class Select_Paint_Use : Base
	{
		public Select_Paint_Use(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{	
				//Добавляем имя тега для раскрасски
				mainClass.data.select.paint.AddTag(word.text);
				//mainClass.data.select.paint.Other = word.text;
				this.link = this.mainClass.select_paint_col_name;
			}
			else
				throw new NoOneClass("Состояние: Select, слово = " + word.text);
			return true;
		}
	};
	class Select_Paint_Col_Name : Base
	{
		public Select_Paint_Col_Name(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.text.ToLower() == "," && word.type == TypeWord.NonTERM)
			{				
				this.link = this.mainClass.select_paint_col;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "other")
			{
                this.link = this.mainClass.select_paint_other;                
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "from")
			{				
				this.link = this.mainClass.fromClass;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "once")
			{
				this.link = this.mainClass.select_once;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "all")
			{
				this.link = this.mainClass.select_all_1;
			}
			else
				throw new NoOneClass("Состояние: Select, слово = " + word.text);
			return true;
		}
	};
	class Select_Paint_Other : Base
	{
		public Select_Paint_Other(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM && word.text.ToLower() == "use")
			{
				this.link = this.mainClass.select_paint_other_use;
			}
			else
				throw new NoOneClass("Состояние: Select, слово = " + word.text);
			return true;
		}
	};
	class Select_Paint_Other_Use : Base
	{
		public Select_Paint_Other_Use(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{
				mainClass.data.select.paint.Other = word.text;
				this.link = this.mainClass.select_paint_other_tag;
			}
			else
				throw new NoOneClass("Состояние: Select, слово = " + word.text);
			return true;
		}
	};
	class Select_Paint_Other_Tag : Base
	{
		public Select_Paint_Other_Tag(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM && word.text.ToLower() == "from")
			{
				this.link = this.mainClass.fromClass;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "once")
			{
				this.link = this.mainClass.select_once;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "all")
			{
				this.link = this.mainClass.select_all_1;
			}
			else
				throw new NoOneClass("Состояние: Select, слово = " + word.text);
			return true;
		}
	};
}

