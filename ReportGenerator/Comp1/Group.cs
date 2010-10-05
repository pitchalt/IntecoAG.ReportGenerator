using System;

namespace Comp1
{
	/// <summary>
	/// Summary description for Group.
	/// </summary>
	class GroupMain : Base
	{
		public GroupMain(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.text.ToLower() == "by" && word.type == TypeWord.TERM)
			{				
				this.link = this.mainClass.group_by;
				mainClass.data.group.AddGroup();
				this.mainClass.isGroup = true;
			}else if(word.text.ToLower() == "all" && word.type == TypeWord.TERM)
			{
                this.link = this.mainClass.group_by_all;
				mainClass.data.group.AddGroup();
				this.mainClass.isGroup = true;
				mainClass.data.group.once.isAll = true;
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	/// <summary>
	/// Summary description for Group.
	/// </summary>
	class Group_By : Base
	{
		public Group_By(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.text.ToLower() == "all" && word.type == TypeWord.TERM)
			{
				this.link = this.mainClass.group_by_all;
				//mainClass.data.group.once.group.Add(word.text);
				mainClass.data.group.once.isAll =true;
			}
			else if (word.type == TypeWord.TERM)
			{
				this.link = this.mainClass.group_by_word;
				mainClass.data.group.once.group.Add(word.text);
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_By_All : Base
	{
		public Group_By_All(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			mainClass.data.group.once.isAll = true;
			if (word.text.ToLower() == "contain" && word.type == TypeWord.TERM)
			{				
				this.link = this.mainClass.group_contain;
			}
			else if (word.text.ToLower() == "flat" && word.type == TypeWord.TERM)
			{
				this.link = this.mainClass.group_flat;
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_By_Word : Base
	{
		public Group_By_Word(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.text.ToLower() == "," && word.type == TypeWord.NonTERM)
			{				
				this.link = this.mainClass.group_by;
			}
			else if (word.text.ToLower() == "contain" && word.type == TypeWord.TERM)
			{				
				this.link = this.mainClass.group_contain;
			}
			else if (word.text.ToLower() == "flat" && word.type == TypeWord.TERM)
			{
				this.link = this.mainClass.group_flat;
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Contain : Base
	{
		public Group_Contain(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{				
				this.link = this.mainClass.group_contain_value_rec;
				mainClass.data.group.once.AddContain(word.text);
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Contain_Value_Column : Base
	{
		public Group_Contain_Value_Column(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{				
				this.link = this.mainClass.group_contain_value_rec;
				//mainClass.data.group.once.AddContain(word.text);
				this.mainClass.data.group.once.AddColumnName(word.text);
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	//group_contain_value_column
	class Group_Contain_Value_Rec : Base
	{
		public Group_Contain_Value_Rec(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.text == ".")
			{
				//Если через точку, значит предыдущее было имя таблицы, а сейчас имя колонки
				this.link = mainClass.group_contain_value_column;
			}else
			if (word.type == TypeWord.TERM && word.text.ToLower() == "use")
			{				
				this.link = this.mainClass.group_contain_use;
			}
			else if (word.type == TypeWord.NonTERM && word.text.ToLower() == ",")
			{
				this.link = this.mainClass.group_contain;
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Contain_Use : Base
	{
		public Group_Contain_Use(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{
				this.link = this.mainClass.group_contain_use_tag;
				mainClass.data.group.once.UseTagH = word.text;
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Contain_Use_Tag : Base
	{
		public Group_Contain_Use_Tag(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM && word.text.ToLower() == "as")
			{
				this.link = this.mainClass.asClass;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "order")
			{
				this.link = this.mainClass.orderClass;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "group")
			{
                this.link = this.mainClass.group;
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Flat : Base
	{
		public Group_Flat(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM && word.text.ToLower() == "header")
			{
				this.link = this.mainClass.group_flat_header;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "footer")
			{
				this.link = this.mainClass.group_flat_footer;
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Flat_Header : Base
	{
		public Group_Flat_Header(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{
				this.link = this.mainClass.group_flat_header_value_rec;
				//this.link = this.mainClass.group_flat_header_func;
				mainClass.data.group.once.AddHeader(word.text);
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Flat_Header_Func : Base
	{
		public Group_Flat_Header_Func(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{
				//this.link = this.mainClass.group_flat_header_value_rec;
				this.link = this.mainClass.group_flat_header_func_end;
				mainClass.data.group.once.AddFunc(word.text, TypeFH.HEADER);
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Flat_Header_Func_End : Base
	{
		public Group_Flat_Header_Func_End(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.text == ")")
			{
				//this.link = this.mainClass.group_flat_header_value_rec;
				this.link = this.mainClass.group_flat_header_func_next;
				//mainClass.data.group.once.AddFunc(word.text);
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Flat_Header_Func_Next : Base
	{
		public Group_Flat_Header_Func_Next(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM && word.text.ToLower() == "use")
			{
				this.link = this.mainClass.group_flat_header_use;
			}
			else if (word.type == TypeWord.NonTERM && word.text.ToLower() == ",")
			{
				this.link = this.mainClass.group_flat_header;
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Flat_Header_Value_Column : Base
	{
		public Group_Flat_Header_Value_Column(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{				
				this.link = this.mainClass.group_flat_header_value_rec;
				this.mainClass.data.group.once.AddColumnName(word.text);
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Flat_Header_Value_Rec : Base
	{
		public Group_Flat_Header_Value_Rec(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.text == ".")
			{
				//Если через точку, значит предыдущее было имя таблицы, а сейчас имя колонки
				this.link = mainClass.group_flat_header_value_column;
			}else if (word.text.ToLower() == "(")
			{
                this.link = this.mainClass.group_flat_header_func;
			}else if (word.type == TypeWord.TERM && word.text.ToLower() == "use")
			{
				this.link = this.mainClass.group_flat_header_use;
			}
			else if (word.type == TypeWord.NonTERM && word.text.ToLower() == ",")
			{
				this.link = this.mainClass.group_flat_header;
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Flat_Header_Use : Base
	{
		public Group_Flat_Header_Use(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{
				this.link = this.mainClass.group_flat_header_use_tag;
				mainClass.data.group.once.UseTagH = word.text;
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Flat_Header_Use_Tag : Base
	{
		public Group_Flat_Header_Use_Tag(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM && word.text.ToLower() == "footer")
			{
				this.link = this.mainClass.group_flat_footer;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "order")
			{
				this.link = this.mainClass.orderClass;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "as")
			{
				this.link = this.mainClass.asClass;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "group")
			{
				this.link = this.mainClass.group;
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Flat_Footer : Base
	{
		public Group_Flat_Footer(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{
				this.link = this.mainClass.group_flat_footer_value_rec;
				mainClass.data.group.once.AddFooter(word.text);
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Flat_Footer_Func : Base
	{
		public Group_Flat_Footer_Func(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{
				//this.link = this.mainClass.group_flat_header_value_rec;
				this.link = this.mainClass.group_flat_footer_func_end;
				mainClass.data.group.once.AddFunc(word.text, TypeFH.FOOTER);
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Flat_Footer_Func_End : Base
	{
		public Group_Flat_Footer_Func_End(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.text == ")")
			{
				//this.link = this.mainClass.group_flat_header_value_rec;
				this.link = this.mainClass.group_flat_footer_func_next;
				//mainClass.data.group.once.AddFunc(word.text);
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Flat_Footer_Func_Next : Base
	{
		public Group_Flat_Footer_Func_Next(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM && word.text.ToLower() == "use")
			{
				this.link = this.mainClass.group_flat_footer_use;
			}
			else if (word.type == TypeWord.NonTERM && word.text.ToLower() == ",")
			{
				this.link = this.mainClass.group_flat_footer;
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Flat_Footer_Value_Column : Base
	{
		public Group_Flat_Footer_Value_Column(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{				
				this.link = this.mainClass.group_flat_footer_value_rec;
				this.mainClass.data.group.once.AddColumnName(word.text);
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Flat_Footer_Value_Rec : Base
	{
		public Group_Flat_Footer_Value_Rec(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.text == ".")
			{
				this.link = this.mainClass.group_flat_footer_value_column;
			}else
			if (word.text.ToLower() == "(")
			{
				this.link = this.mainClass.group_flat_footer_func;
			}else if (word.type == TypeWord.TERM && word.text.ToLower() == "use")
			{
				this.link = this.mainClass.group_flat_footer_use;
			}
			else if (word.type == TypeWord.NonTERM && word.text.ToLower() == ",")
			{
				this.link = this.mainClass.group_flat_footer;
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Flat_Footer_Use : Base
	{
		public Group_Flat_Footer_Use(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM)
			{
				this.link = this.mainClass.group_flat_footer_use_tag;
				mainClass.data.group.once.UseTagF = word.text;
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
	class Group_Flat_Footer_Use_Tag : Base
	{
		public Group_Flat_Footer_Use_Tag(Link l) :
			base (l) {}

		public override bool NextState(TheWord word)
		{
			//Console.WriteLine(this.ToString() + ": " + word.text);
			if (word.type == TypeWord.TERM && word.text.ToLower() == "order")
			{
				this.link = this.mainClass.orderClass;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "as")
			{
				this.link = this.mainClass.asClass;
			}
			else if (word.type == TypeWord.TERM && word.text.ToLower() == "group")
			{
				this.link = this.mainClass.group;
			}
			else
				throw new NoOneClass("Состояние: Group, слово = " + word.text);
			return true;
		}
	};
}
