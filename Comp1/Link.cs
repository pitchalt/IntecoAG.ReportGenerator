using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;

namespace Comp1
{
	/// <summary>
	/// ������� �����, �������� ������ �� ��� ��������� ��������,� ��� �� ������ ������� SQL ���������
	/// ���������� ��������� ��������:
    /// �������� ������� ���������� � ���� ���������� ������ Link, � ������� �������� ������ �� ��� ��������� ��������� ��������.
    /// ������ ��������� ������������ ����� �����, ����������� �� ������������ <see cref="Base"/>. 
    /// � �������� ������ �������� ������ �� ������� ����� ���������. 
    /// ��� ������ ��������� ���� ���������� ������� NextWord, ������� ���������� ������� ����� �� SQL-�����, ��� ����� ���������� "�������� ���������",� ����������� �� ���������� �������� NextState, ������� � ��������� ��������� �����-���������
	/// </summary>
	class Link
	{
		public int			iCountJoin = 0;
		public SQLStruct	data;// = new SQLStruct();
		public int			idata = 0;
		public bool isGroup = false;

		public Base SelectReturn;
		public Base FromReturn;
		public Base WhereReturn;
		public Base AsReturn;

		private ArrayList words = new ArrayList();
		private TextReader sql;
        /// <summary>
        /// ������� �����
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
		private string Trim(string txt)
		{
			char[] arr = {' ','\t','\n','\r'};
			return txt.Trim(arr);
		}
        /// <summary>
        /// ���������� ��������� ����� �� �������� SQL-�����
        /// </summary>
        /// <returns>��������� �����</returns>
		public TheWord NextWord()
		{
			if (words.Count == 0)
			{
				//������ �� ����� �������, ��������� �� ����� � ��������� � ���� ������
				string str;
				while (true)
				{
					str = sql.ReadLine();
					if (str == null)
						return null;
					if (str != "")
						break;
				}
				//string pattern = @"[\s]*([A-Z�-�#*a-z_\-0-9�-�\']+)+([\(\),.\s\t\n\r \0=<>]+)*";
				string pattern = @"([#*a-z_\-0-9�-�\']+|[\(\)]+){1}([,.\s\t\n\r \0=<>]+)*";
				MatchCollection Matches = Regex.Matches(str, pattern, RegexOptions.IgnoreCase);

				foreach (Match m1 in Matches)
				{
					////Console.WriteLine(m1.Index);
					this.words.Add(new TheWord(Trim(m1.Groups[1].ToString()), TypeWord.TERM));
					if (Trim(m1.Groups[2].ToString()) != "")
					{
						if (Trim(m1.Groups[3].ToString()) != "" && Trim(m1.Groups[3].ToString()) != Trim(m1.Groups[2].ToString()))
						{
							this.words.Add(new TheWord(Trim(m1.Groups[3].ToString()), TypeWord.NonTERM));

						}
						this.words.Add(new TheWord(Trim(m1.Groups[2].ToString()), TypeWord.NonTERM));
					}
					
					////Console.Write(m1.Groups[1].Captures[0].ToString());
					////Console.WriteLine(" : ");
					////Console.Write(m1.Groups[1].Captures[1].ToString());

					////Console.Write(":  ");
					////Console.WriteLine(m1.Groups[2].ToString());
				}


				//Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
				/*Match m = r.Match(str.ToLower());
				Group g = m.Groups[1];
				CaptureCollection cc = g.Captures;

				for (int j = 0; j < cc.Count; j++) 
				{
					Capture c = cc[j];
					string txt = Trim(c.ToString().ToLower());
					////Console.WriteLine("'" + txt + "'");
					this.words.Add(txt);
				}*/
			}
			TheWord outtxt = (TheWord) this.words[0];
			words.Remove(outtxt);
			return outtxt;
		}

		public Base CurState;
		public Select selectClass;
		public Select_Stmt select_stmt;
		public Select_Stmt_0 select_stmt_0;
		public Select_Stmt_1 select_stmt_1;
		public Select_Stmt_2 select_stmt_2;
		public Select_Point  select_point;

		public Select_All	select_all;
		public Select_All_1	select_all_1;
		public Select_All_Word select_all_word;
		public Select_Once select_once;
		public Select_All_Use	select_all_use;
		public Select_Stmt_As select_stmt_as;

		public Select_Key_1 select_key_1;
		public Select_Key_2 select_key_2;
		public Select_Key_Value select_key_value;
        public Select_Key_Point select_key_point;
		public Select_Key_Value_2 select_key_value_2;

        public Select_Paint_1 select_paint_1;
		public Select_Paint_2 select_paint_2;
		public Select_Paint_Col select_paint_col;
		public Select_Paint_For select_paint_for;
		public Select_Paint_Value select_paint_value;
		public Select_Paint_Use select_paint_use;
		public Select_Paint_Col_Name select_paint_col_name;
		public Select_Paint_Col_Point select_paint_col_point;
		public Select_Paint_Col_Word select_paint_col_word;
		public Select_Paint_Other select_paint_other;
		public Select_Paint_Other_Use select_paint_other_use;
		public Select_Paint_Other_Tag select_paint_other_tag;



		public From fromClass;
		public From_1 from_1;
		public From_Table_1 from_table_1;
		public From_Table_2 from_table_2;
		public From_Join from_join;
		public From_Where from_where;

		public Where whereClass;
		public Where_Column1 where_column1;
		public Where_Column2 where_column2;
		public Where_Table1 where_table1;
		public Where_Table2 where_table2;
		public Where_Point1 where_point1;
		public Where_Point2 where_point2;
		public Where_Comp where_comp;

		public GroupMain group;
		public Group_By group_by;
		public Group_By_All group_by_all;
		public Group_By_Word group_by_word;
		public Group_Contain group_contain;
		public Group_Contain_Value_Rec group_contain_value_rec;
		public Group_Contain_Use group_contain_use;
		public Group_Contain_Use_Tag group_contain_use_tag;
		public Group_Flat group_flat;
		//public Group_Contain_Flat
		public Group_Flat_Header group_flat_header;
		public Group_Flat_Header_Value_Rec group_flat_header_value_rec;
		public Group_Flat_Header_Use group_flat_header_use;
		public Group_Flat_Header_Use_Tag group_flat_header_use_tag;
		public Group_Flat_Header_Func group_flat_header_func;
		public Group_Flat_Header_Func_End group_flat_header_func_end;
		public Group_Flat_Header_Func_Next group_flat_header_func_next;

		public Group_Flat_Footer group_flat_footer;
		public Group_Flat_Footer_Value_Rec group_flat_footer_value_rec;
		public Group_Flat_Footer_Use group_flat_footer_use;
		public Group_Flat_Footer_Use_Tag group_flat_footer_use_tag;

		public Group_Flat_Footer_Func group_flat_footer_func;
		public Group_Flat_Footer_Func_End group_flat_footer_func_end;
		public Group_Flat_Footer_Func_Next group_flat_footer_func_next;

		public Group_Flat_Footer_Value_Column group_flat_footer_value_column;
		public Group_Flat_Header_Value_Column group_flat_header_value_column;
		public Group_Contain_Value_Column group_contain_value_column;

		public Base orderClass = null;


		public As asClass;
		public Expr CurentExpr;
		public Oper CurentOper;
		
        /// <summary>
        /// SQL-����
        /// </summary>
		public TextReader Tr
		{
			get
			{
				return sql;
			}
			set
			{
				sql = value;
			}
		}
        /// <summary>
        /// ������ ����
        /// </summary>
		public ArrayList ArrayWords
		{
			get
			{
				return this.words;
			}
		}
        /// <summary>
        /// ������� ��� ���������
        /// </summary>
		private void CreateClass()
		{
			//������� ������
			selectClass = new Select(this);
			select_stmt = new Select_Stmt(this);
			select_stmt_1 = new Select_Stmt_1(this);
			select_stmt_2 = new Select_Stmt_2(this);
			select_point = new Select_Point(this);

			select_stmt_0 = new Select_Stmt_0(this);
			select_all = new Select_All(this);
			select_all_use = new Select_All_Use(this);
			select_stmt_as = new Select_Stmt_As(this);

			select_all_1 = new Select_All_1(this);
			select_all_word = new Select_All_Word(this);
			select_once = new Select_Once(this);

			select_key_1 = new Select_Key_1(this);
			select_key_2 = new Select_Key_2(this);
			select_key_value = new Select_Key_Value(this);
			select_key_point = new Select_Key_Point(this);
			select_key_value_2 = new Select_Key_Value_2(this);

			select_paint_1 = new Select_Paint_1(this);
			select_paint_2 = new Select_Paint_2(this);
			select_paint_col = new Select_Paint_Col(this);
			select_paint_for = new Select_Paint_For(this);
			select_paint_value = new Select_Paint_Value(this);
			select_paint_use = new Select_Paint_Use(this);
			select_paint_col_name = new Select_Paint_Col_Name(this);
			select_paint_col_point = new Select_Paint_Col_Point(this);
			select_paint_col_word = new Select_Paint_Col_Word(this);
			select_paint_other = new Select_Paint_Other(this);
			select_paint_other_use = new Select_Paint_Other_Use(this);
			select_paint_other_tag = new Select_Paint_Other_Tag(this);


			fromClass = new From(this);
			from_1 = new From_1(this);
			from_table_1 =  new From_Table_1(this);
			from_table_2 =  new From_Table_2(this);
			from_join = new From_Join(this);
			from_where = new From_Where(this);
			//from_stmt = new From_Stmt(this);

			whereClass = new Where(this);
			//where_stmt = new Where_Stmt(this);
			where_column1 = new Where_Column1(this);
			where_column2 = new Where_Column2(this);
			where_table1 = new Where_Table1(this);
			where_table2 = new Where_Table2(this);
			where_point1 = new Where_Point1(this);
			where_point2 = new Where_Point2(this);
			where_comp = new Where_Comp(this);

			//group

			this.group = new GroupMain(this);
			group_by = new Group_By(this);
			group_by_all = new Group_By_All(this);
			group_by_word = new Group_By_Word(this);
			group_contain = new Group_Contain(this);
			group_contain_value_rec = new Group_Contain_Value_Rec(this);
			group_contain_use = new Group_Contain_Use(this);
			group_contain_use_tag = new Group_Contain_Use_Tag(this);
			group_flat = new Group_Flat(this);
			group_flat_header = new Group_Flat_Header(this);
			group_flat_header_value_rec = new Group_Flat_Header_Value_Rec(this);
			group_flat_header_use = new Group_Flat_Header_Use(this);
			group_flat_header_use_tag = new Group_Flat_Header_Use_Tag(this);

			group_flat_header_func = new Group_Flat_Header_Func(this);
			group_flat_header_func_end = new Group_Flat_Header_Func_End(this);
			group_flat_header_func_next = new Group_Flat_Header_Func_Next(this);

			group_flat_footer = new Group_Flat_Footer(this);
			group_flat_footer_value_rec = new Group_Flat_Footer_Value_Rec(this);
			group_flat_footer_use = new Group_Flat_Footer_Use(this);
			group_flat_footer_use_tag = new Group_Flat_Footer_Use_Tag(this);

			group_flat_footer_func = new Group_Flat_Footer_Func(this);
			group_flat_footer_func_end = new Group_Flat_Footer_Func_End(this);
			group_flat_footer_func_next = new Group_Flat_Footer_Func_Next(this);

			group_flat_footer_value_column = new Group_Flat_Footer_Value_Column(this);
			group_flat_header_value_column = new Group_Flat_Header_Value_Column(this);
			group_contain_value_column = new Group_Contain_Value_Column(this);

			asClass = new As(this);
			//as_stmt = new As_Stmt(this);

			CurentExpr = new Expr();
			CurentOper = new Oper();
		}
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="tr">������� ����</param>
		public Link(TextReader tr)
		{
			CreateClass();
			this.sql = tr;
			//������������� ��������� ���������.
			CurState = selectClass;
		}
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="tr">������� ����</param>
        /// <param name="words">������ ����</param>
		public Link(TextReader tr, ArrayList words)
		{
			CreateClass();
			this.sql = tr;
			//������������� ��������� ���������.
			CurState = selectClass;
			this.words = words;						
		}
        /// <summary>
        /// �������� �����, ����������� �� ���������� �������� �������
        /// </summary>
        /// <returns>���������� ������� �����</returns>
		public bool Run()
		{
			TheWord word;
			bool Ok = true;

            ArrayList ErrorList = new ArrayList();
			while ((word = this.NextWord()) != null && Ok)
			{
				ErrorList.Add(word.text);
				try 
				{	
					Ok = this.CurState.NextState(word);
					if (Ok == false)
						break;
				}
				catch (System.Exception e)
				{
					string str_error = "";
                    for (int i=0;i<ErrorList.Count;i++)
						str_error += ErrorList[i] +" ";
					string val = "������ ��� ����������: " + e.Message + "\n " +str_error;
					throw new Exception(val);
				}
			}
			if (word!= null)
				return true;
			else
				return false;

		}
        /// <summary>
        /// ������������ � ��������� ���������
        /// </summary>
		public void ResetState()
		{
			CurState = selectClass;            
		}
        /// <summary>
        /// ��������� ����� � ������ ������
        /// </summary>
        /// <param name="word">�����</param>
        /// <param name="type">��� �����</param>
		public void AddWordBegin(string word, TypeWord type)
		{
			this.words.Insert(0, new TheWord(word, type));
		}
        /// <summary>
        /// ��������� �����
        /// </summary>
        /// <param name="l">������ ���� ��� ����������</param>
		public void AddWords(ArrayList l)
		{
			foreach (string obj in l)
			{
				this.words.Add(obj);
			}
		}
	};
}
