using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Xml;
using System.Data;

namespace Comp1
{
	public enum ClassType {OPER, EXPR};
	public enum Operations {Ravno, Bolshe, NeRavno};
	public enum Uslovie {AND, OR};
	public enum ColumnType {NAME, VALUE};
    /// <summary>
    /// Значение колонки
    /// </summary>
	public struct ColumnValue 
	{
		public string Value;
        public ColumnType type;
		public ColumnValue (string val, ColumnType t)
		{
			Value = val;
			type = t;
		}
	}; 
	/// <summary>
	/// Класс хранит в себе схему SQL-запроса, то есть при парсинге файла, происходит запись его содержимого
	/// </summary>
    public class SqlStruct
	{
        /// <summary>
        /// SELECT - класс
        /// </summary>
		public ArrayList	Select;
        /// <summary>
        /// From - класс
        /// </summary>
		public ArrayList	From;
        /// <summary>
        /// AS - класс
        /// </summary>
		public string		As;
        /// <summary>
        /// WHERE - класс
        /// </summary>
		public WhereClass	Where;
        /// <summary>
        /// не используется
        /// </summary>
		public int			Col;
        /// <summary>
        /// не используется
        /// </summary>
		public int			min;
        /// <summary>
        /// не используется
        /// </summary>
		public int			max;
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
		public SqlStruct()
		{
			Select = new ArrayList();
			From = new ArrayList();
			As = null;
			Col = -1;
			min = -1;
			max = -1;
		}

	};

    /// <summary>
    /// Не используется. старая версия, тестовая
    /// </summary>
	public class SqlFile
	{
		public int type = 0;
		
		private void Print(ArrayList list)
		{
			foreach (string str in list)
			{
                Console.Write(str + ",");
			}
			//Console.WriteLine();
		}
		public void Print()
		{
			for (int i=0;i<ArraySQL.Count;i++)
			{
				Console.Write("Select = '");
				Print(((SqlStruct)ArraySQL[i]).Select);
				Console.WriteLine("'");
				Console.Write("From = '");
				Print(((SqlStruct)ArraySQL[i]).From);
				Console.WriteLine("'");
                Console.Write("As = '");
				Console.WriteLine(((SqlStruct)ArraySQL[i]).As);
				Console.WriteLine("'");
				Console.Write("Column = '");
				Console.WriteLine(((SqlStruct)ArraySQL[i]).Col.ToString());
				Console.WriteLine("'");


			}
		}

		public ArrayList ArraySQL = new ArrayList();

		
		private TextReader sql;
		private ArrayList words = new ArrayList();

		public SqlFile(TextReader sql)
		{
            this.sql = sql;
		}

		public bool ifExistInSQL(string node)
		{
			for (int i=0;i<ArraySQL.Count;i++)
			{
				if (((SqlStruct)ArraySQL[i]).As == node)
				{
					return true;
				}
			}
			return false;
		}

		private string Trim(string txt)
		{
			char[] arr = {',',' ','\t','\n','\r','.'};
			return txt.Trim(arr);
		}

		private string GetWord()
		{
			if (words.Count == 0)
			{
				//Читаем из файла строчку, разбиваем на слова и добавляем в этот массив
				string str;
				while (true)
				{
					str = sql.ReadLine();
					if (str == null)
						return null;
					if (str != "")
						break;
				}
                string pattern = @"[\s]*(([#a-z_\-0-9а-я=<>\']+)[,.\s\t\n\r]*)+";
				Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
				Match m = r.Match(str.ToLower());
				if (!m.Success)
					return null;

				Group g = m.Groups[1];
				CaptureCollection cc = g.Captures;

				for (int j = 0; j < cc.Count; j++) 
				{
					Capture c = cc[j];
					string txt = Trim(c.ToString().ToLower());
                    //Console.WriteLine("'" + txt + "'");
					this.words.Add(txt);
				}
			}
			string outtxt = (string) words[0];
			return outtxt;
		}

		public string CreateSQL(int state)
		{
			//Читаем по слову
			
			//type = 1 - Вложенный
			//type = 2 - последовательный
			SqlStruct SQLst = new SqlStruct();
			string word;
			while ((word = GetWord()) != null){;};

			return "";
		}
		
		//По умолчанию state = 0;

		/*
		  public string CreateSQL(int state)
		{
			//Читаем по слову
			
			//type = 1 - Вложенный
			//type = 2 - последовательный
			SqlStruct SQLst = new SqlStruct();
			string word;
			while ((word = GetWord()) != null)
			{
				
				if (word == "select")
				{
					//если мы тока начинаем SQL
					if (state == 0)
					{
						state = 1;
						words.RemoveAt(0);
						continue;
					}else if (state == 2 || state == 1 )
					{
						if (type == 2)
							return null;
						type = 1;
						words.RemoveAt(0);
						//у нас вложенный запрос
						//Получам его структуру.
						string As;
						if ((As = CreateSQL(1)) == null)
							return null;
                         
						//Добавляем данный запрос в список
						//ArraySQL.Add(st);
						//Добавляем в текущий список "Select" параметр из st "AS".
						SQLst.Select.Add(As);
						continue;
					}else if (state == 6 || state == 8)
					{
						if (type == 1)
							return null;
						type = 2;
						//Мы word добавляем обратно вначало и возвращем сформированный запрос...
						ArraySQL.Add(SQLst);
						//words.RemoveAt(0);
						return SQLst.As;
					}

				}else if (state == 2 && word == "from")
				{
					state = 3;
					words.RemoveAt(0);
					continue;
				}else if (state == 4 && word == "as")
				{
					state = 5;
					words.RemoveAt(0);
					continue;
				}else if (state == 6 && word == "column")
				{
					state = 7;
					words.RemoveAt(0);
					continue;
				}
					//Должно быть в самом конце...
				else if (state == 6 || state == 8)
				{
					//если вложенный (т.е. мы хотим вставить в AS или Column еще по 1 слову, т.к. 1 уже было)
					ArraySQL.Add(SQLst);
					return SQLst.As;
				}
				words.RemoveAt(0);
				
				//Теперь будем формировать структуру.
				if (state == 1 || state == 2)
				{
					//select
					SQLst.Select.Add(word);
					state = 2;
				}else if (state == 3 || state == 4)
				{
					//from
					SQLst.From.Add(word);
					state = 4;
				}else if (state == 5 || state == 6)
				{
					//as
					SQLst.As = word;
					state = 6;
				}else if (state == 7 || state == 8)
				{
					try
					{
						SQLst.Col = int.Parse(word);
					}
					catch (System.Exception e)
					{
						return null;
					}
					state = 8;
				}
			}
			if (state == 6 || state == 8)
			{
				ArraySQL.Add(SQLst);
				return SQLst.As;
			}else
				return null;
		}

		*/
		/*public string CreateSQL(int state)
		{
			//Читаем по слову
			
			//type = 1 - Вложенный
			//type = 2 - последовательный
			SqlStruct SQLst = new SqlStruct();
			string word;
			while ((word = GetWord()) != null)
			{
				
				if (word == "select")
				{
					//если мы тока начинаем SQL
					if (state == 0)
					{
						state = 1;
						words.RemoveAt(0);
						continue;
					}
					else if (state == 2 || state == 1 )
					{
						if (type == 2)
							return null;
						type = 1;
						words.RemoveAt(0);
						//у нас вложенный запрос
						//Получам его структуру.
						string As;
						if ((As = CreateSQL(1)) == null)
							return null;
                         
						//Добавляем данный запрос в список
						//ArraySQL.Add(st);
						//Добавляем в текущий список "Select" параметр из st "AS".
						SQLst.Select.Add(As);
						continue;
					}
					else if (state == 8 || state == 10)
					{
						if (type == 1)
							return null;
						type = 2;
						//Мы word добавляем обратно вначало и возвращем сформированный запрос...
						ArraySQL.Add(SQLst);
						//words.RemoveAt(0);
						return SQLst.As;
					}

				}
				else if (state == 2 && word == "from")
				{
					state = 3;
					words.RemoveAt(0);
					continue;
				}else if (state == 4 && word == "where")
				{
					state = 5;
					words.RemoveAt(0);
					continue;
				}
				else if ( (state == 4 || state == 6) && word == "as")
				{
					state = 7;
					words.RemoveAt(0);
					continue;
				}
				else if (state == 8 && word == "column")
				{
					state = 9;
					words.RemoveAt(0);
					continue;
				}
					//Должно быть в самом конце...
				else if (state == 8 || state == 10)
				{
					//если вложенный (т.е. мы хотим вставить в AS или Column еще по 1 слову, т.к. 1 уже было)
					ArraySQL.Add(SQLst);
					return SQLst.As;
				}
				words.RemoveAt(0);
				
				//Теперь будем формировать структуру.
				if (state == 1 || state == 2)
				{
					//select
					SQLst.Select.Add(word);
					state = 2;
				}
				else if (state == 3 || state == 4)
				{
					//from
					SQLst.From.Add(word);
					state = 4;
				}
				else if (state == 5 || state == 6)
				{
					string str_xxx = word;

					state = 6;
				}
				else if (state == 7 || state == 8)
				{
					//as
					SQLst.As = word;
					state = 8;
				}
				else if (state == 9 || state == 10)
				{
					try
					{
						SQLst.Col = int.Parse(word);
					}
					catch (System.Exception e)
					{
						return null;
					}
					state = 10;
				}
			}
			if (state == 8 || state == 10)
			{
				ArraySQL.Add(SQLst);
				return SQLst.As;
			}
			else
				return null;
		}*/

		public SqlStruct GetSQL()
		{
			if (this.ArraySQL.Count > 0)
			{
				SqlStruct st =(SqlStruct) ArraySQL[0];
				ArraySQL.RemoveAt(0);
				return st;
			}else
				return null;
		}

		public bool Validate(string inxd)
		{
            DataSet ds = new DataSet();
			if (inxd !=null)
			{
				//inxd.Close();
				ds.ReadXmlSchema(inxd);
			}
			for (int i=0;i<this.ArraySQL.Count;i++)
			{
				SqlStruct st = (SqlStruct)ArraySQL[i];
				DataTable dt = ds.Tables[(string)st.From[0]];
				if (dt == null)
					return false;
                foreach (string str in st.Select)
				{
					if (!this.ifExistInSQL(str))
						if (dt.Columns[str] == null)
							return false;
				}
			}
			return true;
		}
	};
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	
	class Sql
	{
		public string inXmlFile;
		public string outXmlFile;

		private SqlFile f;
		private string inputXMLFile= "";
		private int globalid = 0;
		
		public TextWriter sw = null;
		public TextWriter outxd = null;
		public string inxd = null;
		public TextReader sql = null;

		private string XMLNS = "http://www.w3.org/1999/XSL/Transform";
		private string NameSpace = "SimpleSample";

		public string OutXsd
		{
			set
			{
				outxd = new StreamWriter(value);
			}
		}

		public string SQLFile
		{
            set
			{
                sql = new StreamReader(value);
			}
		}
		public string inXSDFileName
		{
			set
			{
				inxd =  value;
			}
		}
		private string xsltOutput = "";
		public string Output
		{
			set
			{
                sw = new StreamWriter(value);
				xsltOutput = value;
				//sw = Console.Out;
				//xd = new StreamWriter("sxema.xsd");
			}
			get
			{
				return xsltOutput;
			}
		}
		private string MainTag = "doc";
		
		private ArrayList SQLLines;
		private ArrayList SQLLinesXSD;
		
		private string Trim(string txt)
		{
			char[] arr = {',',' ','\t','\n','\r'};
			return txt.Trim(arr);
		}
		private ArrayList GetArray(string str)
		{
			string pattern = @"[\s]*(([#a-z_0-9а-я]+)[,\s]*)+";
            
			Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
			Match m = r.Match(str);

			ArrayList list = new ArrayList();

			if (m.Success)
			{
				Group g = m.Groups[1];
				CaptureCollection cc = g.Captures;
				for (int j = 0; j < cc.Count; j++) 
				{
					Capture c = cc[j];
					string txt = c.ToString();
					//Trim(txt);
					list.Add (Trim(txt));
					//Console.WriteLine(c.ToString());

				}
			}
			return list;
		}

		public void Parse(SqlStruct str, int ind)
		{
			//если первый SQL то мы вставляем заголовочные теги
			if (ind == 0)
			{
				sw.WriteLine("<xsl:template match=\"" + this.MainTag + "\"><" + this.MainTag + " xmlns:xsl=\"" + XMLNS + "\"><xsl:apply-templates select=\"" + str.From[0] + "\"/></" + this.MainTag +"></xsl:template>");
			}

			//Если первый SQL запрос, то делаем template match
			//иначе template name
			
			if (ind == 0)
				sw.WriteLine("<xsl:template match=\"" + str.From[0] + "\">");
			else
				sw.WriteLine("<xsl:template name=\"" + str.From[0] + "\">");
			
			//Получаем имя дочернего элемента (имя тега, которое разделяет на строки, ex. "tr")
			//string tr_ = GetChildElement((string)listFROM[0]);

			string tr_ = "tr-" + str.From[0];
			//Записываем цикл выборки
			sw.WriteLine("<xsl:for-each select=\"//" + this.MainTag + "//" + str.From[0] + "\">");
			
			//Пишем разделительный (на строки) тэг
			if (ind != 0)
				sw.WriteLine("<" + tr_ + ">");
			//Вставляем тока те элементы которые указаны в select-e
			foreach (string node in str.Select)
			{
				//При этом мы должны "пробежать все SQL запросы и посмареть что сюда вставить..."
				if (f.ifExistInSQL(node) == false)
					sw.WriteLine("<" + node + "><xsl:value-of select=\"" + node + "\"/></" + node + ">");
				else
				{
					string tagName = GetTableName(node);
					sw.WriteLine("<" + node + "><xsl:call-template name=\"" + tagName + "\"/></" + node + ">");
					globalid++;
				}

                
			}
			//закрываем элементы
			if (ind != 0)
				sw.WriteLine("</" + tr_ +">");
			//конец цикла
			sw.WriteLine("</xsl:for-each>");
			//конец шаблона
			sw.WriteLine("</xsl:template>	");
			
		}

		public void Parse(string str, int ind)
		{
			
			str = str.ToLower();
			
			int iSELECT = str.IndexOf("select ");
			int iFROM = str.IndexOf(" from ");
			int iAS = str.IndexOf(" as ");

			if (iSELECT <0 || iFROM < 0 || iAS < 0)
			{
				return;
			}


			//Выделаем подстоки из строки
			string strSELECT = str.Substring(iSELECT + 7, iFROM - 6 - iSELECT);
			string strFROM   = str.Substring(iFROM + 5, iAS - 4 - iFROM);
			string strAS	 = str.Substring(iAS + 3);

			//Разбиваем на массив (ArrayList)
			ArrayList listSELECT = GetArray(strSELECT);
			ArrayList listFROM = GetArray(strFROM);
			ArrayList listAS = GetArray(strAS);

			//если первый SQL то мы вставляем заголовочные теги
			if (ind == 0)
			{
				sw.WriteLine("<xsl:template match=\"" + this.MainTag + "\"><doc xmlns:xsl=\"" + XMLNS + "\"><xsl:apply-templates select=\"" + listFROM[0] + "\"/></doc></xsl:template>");
			}

			//Если первый SQL запрос, то делаем template match
			//иначе template name
			
			if (ind == 0)
				sw.WriteLine("<xsl:template match=\"" + listFROM[0] + "\">");
			else
				sw.WriteLine("<xsl:template name=\"" + listFROM[0] + "\">");
			
			//Получаем имя дочернего элемента (имя тега, которое разделяет на строки, ex. "tr")
			//string tr_ = GetChildElement((string)listFROM[0]);
			string tr_ = "tr-" + listFROM[0];
			//Записываем цикл выборки
			sw.WriteLine("<xsl:for-each select=\"//" + this.MainTag + "//" + listFROM[0] + "\">");
			
			//Пишем разделительный (на строки) тэг
			if (ind != 0)
				sw.WriteLine("<" + tr_ + ">");
			
			//Вставляем тока те элементы которые указаны в select-e
			foreach (string node in listSELECT)
			{
				//При этом мы должны "пробежать все SQL запросы и посмареть что сюда вставить..."
				if (f.ifExistInSQL(node) == false)
					sw.WriteLine("<" + node + "><xsl:value-of select=\"" + node + "\"/></" + node + ">");
				else
				{
					string tagName = GetTableName(node);
					sw.WriteLine("<" + node + "><xsl:call-template name=\"" + tagName + "\"/></" + node + ">");
					globalid++;
				}

                
			}
			//закрываем элементы
			if (ind != 0)
				sw.WriteLine("</" + tr_ +">");
			//конец цикла
			sw.WriteLine("</xsl:for-each>");
			//конец шаблона
			sw.WriteLine("</xsl:template>	");
			
		}

		private string GetTableName(string node)
		{
			for (int i=0;i<f.ArraySQL.Count;i++)
			{
				if (((SqlStruct)f.ArraySQL[i]).As == node)
				{
					return (string) ((SqlStruct)f.ArraySQL[i]).From[0];
				}
			}
			return null;
		}
		/*private string GetChildElement(string ElName)
		{
			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(this.inputXMLFile);
				XmlNodeList list = doc.GetElementsByTagName(ElName);
				foreach (XmlNode node in list)
				{
					return node.FirstChild.Name;
				}
			}
			catch (System.Exception e)
			{
                Console.WriteLine("Ошиька в XML файле");
			}
			return "";
		}
*/
		public void WriteHead()
		{
            sw.WriteLine("<xsl:transform xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">");
			outxd.WriteLine("<?xml version=\"1.0\"?>\n");
			//xd.WriteLine("<xs:schema id=\"IntekoDataSet\" targetNamespace=\""+ this.NameSpace +"\" xmlns:mstns=\"SimpleSample\" xmlns=\""+this.XMLNS+"\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:msdata=\"urn:schemas-microsoft-com:xml-msdata\" attributeFormDefault=\"qualified\" elementFormDefault=\"qualified\">");
			outxd.WriteLine("<xs:schema id=\"IntekoDataSet\" xmlns:mstns=\"SimpleSample\" xmlns=\""+this.XMLNS+"\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:msdata=\"urn:schemas-microsoft-com:xml-msdata\" attributeFormDefault=\"qualified\" elementFormDefault=\"qualified\">");
		}

		public void WriteEnd()
		{
			sw.WriteLine("</xsl:transform>");
			sw.Close();

			outxd.WriteLine("</xs:schema>");
			outxd.Close();
		}

		public int Start()
		{
			//Ошибка
			if (sw == null || inxd == null || outxd == null || sql == null)
				return 1;
			
			WriteHead();

			//Находим "главный" тэг.
			SetMainXMLTag();
			f = new SqlFile(sql);
			//Парсим SQL
			while(f.CreateSQL(0) != null);

			//Проверяем то что напарсили по XSD схеме
			if (!f.Validate(inxd))
				return 2;

			if (f.type == 1)
				f.ArraySQL.Reverse();
			//f.ArraySQL.Reverse();
			SqlStruct st;
			for (int i=0;i<f.ArraySQL.Count;i++)
			{
				st = (SqlStruct) f.ArraySQL[i];
				Parse(st,i);
			}
			ParseforXSD(0,this.MainTag);

			//SetArraySQL();

			//для 1-ого Sql делаем парсинг
			
			/*while (this.SQLLines.Count > 0)
			{
				string str = (string) SQLLines[0];
				SQLLines.Remove(str);
				Console.WriteLine("Парсинг: '" + str + "'");
				this.Parse(str,i);
				i++;
			}*/

			
			//this.SQLLines = this.SQLLinesXSD;
			
			WriteEnd();
			return 0;
		}

		public int StartOld()
		{
			//Ошибка
			if (sw == null || inxd == null || outxd == null || sql == null)
				return 1;
			
			WriteHead();

			//Находим "главный" тэг.
			SetMainXMLTag();
			SetArraySQL();

			//для 1-ого Sql делаем парсинг
			int i=0;
			while (this.SQLLines.Count > 0)
			{
				string str = (string) SQLLines[0];
				SQLLines.Remove(str);
				Console.WriteLine("Парсинг: '" + str + "'");
				this.Parse(str,i);
				i++;
			}
			this.SQLLines = this.SQLLinesXSD;
			ParseforXSD(0);
			
			WriteEnd();
			return 0;
		}

		private string GetSQL()
		{
			string str = "";
			int iSELECT = str.IndexOf("select ");
			int iFROM = str.IndexOf(" from ");
			int iAS = str.IndexOf(" as ");
			while (this.SQLLines.Count > 0)
			{
				string tmp = (string) this.SQLLines[0];
				str += tmp;
				this.SQLLines.Remove(tmp);
				iSELECT = str.IndexOf("select ");
				iFROM = str.IndexOf(" from ");
				iAS = str.IndexOf(" as ");
			
                if (iSELECT >= 0 && iFROM >0 && iAS > 0)
                {
					return str;
                }
			}
			iSELECT = str.IndexOf("select ");
			iFROM = str.IndexOf(" from ");
			iAS = str.IndexOf(" as ");
			if (iSELECT == -1)
			{
				Console.WriteLine("Ошибка в SQL. Ожидается тэг \"SELECT\"");
			}
			if (iFROM == -1)
			{
				Console.WriteLine("Ошибка в SQL. Ожидается тэг \"FROM\"");
			}
			if (iAS == -1)
			{
				Console.WriteLine("Ошибка в SQL. Ожидается тэг \"AS\"");
			}
			return "";
			/**/
		}


		private void ParseforXSD(int ind,string tagName)
		{
			for (int i=0;i<f.ArraySQL.Count;i++)
			{
				SqlStruct st = (SqlStruct)f.ArraySQL[i];
				if (st.As == tagName)
				{
					ParseforXSD(st,ind);
				}
			}
		}
		private void ParseforXSD(SqlStruct st, int ind)
		{
			//вставляем заголовочные теги <document>
			if (ind == 0)
				outxd.WriteLine("<xs:element name=\"" + MainTag + "\"><xs:complexType><xs:choice maxOccurs=\"unbounded\">");
			
			string tr_ = "tr-" + (string)(st.From[0]);

			if (ind != 0)
				outxd.WriteLine("<xs:element name=\"" + tr_ + "\" minOccurs=\"0\" maxOccurs=\"unbounded\"><xs:complexType><xs:sequence>");
			
			//Вставляем тока те элементы которые указаны в select-e
			foreach (string node in st.Select)
			{
				//При этом мы должны "пробежать все SQL запросы и посмареть что сюда вставить..."
				if (f.ifExistInSQL(node) == false)
					outxd.WriteLine("<xs:element name=\"" + node + "\" type=\"xs:string\" minOccurs=\"0\" />");
				else
				{
					string tagName = GetTableName(node);
					//xd.WriteLine("\t\t<" + node + "><xsl:call-template name=\"" + tagName + "\"/></" + node + ">");
					outxd.WriteLine("<xs:element name=\"" + node + "\" minOccurs=\"0\" maxOccurs=\"unbounded\"><xs:complexType>\n\t<xs:sequence>");
					this.ParseforXSD(ind+1, node);

					outxd.WriteLine("\n\t</xs:sequence></xs:complexType></xs:element>");
				}

			}
			if (ind != 0)
				outxd.WriteLine("</xs:sequence></xs:complexType></xs:element>");
			if (ind ==0)
			{
				outxd.WriteLine("\t</xs:choice>\n\t</xs:complexType>\n</xs:element>");
			}
		}

		private void ParseforXSD(int ind)
		{
			if (SQLLinesXSD.Count <= 0)
				return;

			string str = (string) SQLLines[0];
			this.SQLLines.Remove(str);
			str = str.ToLower();
			//string pattern = @"(([a-z_0-9]+[\s\,]+))+";
			int iSELECT = str.IndexOf("select ");
			int iFROM = str.IndexOf(" from ");
			int iAS = str.IndexOf(" as ");


			//Выделаем подстоки из строки
			string strSELECT = str.Substring(iSELECT + 7, iFROM - 6 - iSELECT);
			string strFROM   = str.Substring(iFROM + 5, iAS - 4 - iFROM);
			string strAS	 = str.Substring(iAS + 3);

			//Разбиваем на массив (ArrayList)
			ArrayList listSELECT = GetArray(strSELECT);
			ArrayList listFROM = GetArray(strFROM);
			ArrayList listAS = GetArray(strAS);

			//вставляем заголовочные теги <document>
			if (ind == 0)
				outxd.WriteLine("<xs:element name=\"" + MainTag + "\">\n\t<xs:complexType>\n\t<xs:choice maxOccurs=\"unbounded\">");
			//else
				//xd.WriteLine("<xs:element name=\"" + listAS[0] + "\">\n\t<xs:complexType>\n\t<xs:sequence>");
			//sw.WriteLine("<xsl:template match=\"" + this.MainTag + "\"><doc><xsl:apply-templates select=\"" + listFROM[0] + "\"/></doc></xsl:template>");
			
									
			//Получаем имя дочернего элемента (имя тега, которое разделяет на строки, ex. "tr")
			//string tr_ = GetChildElement((string)listFROM[0]);
			string tr_ = "tr-" + listFROM[0];

			if (ind != 0)
				outxd.WriteLine("<xs:element name=\"" + tr_ + "\" minOccurs=\"0\" maxOccurs=\"unbounded\"><xs:complexType><xs:sequence>");
			
			//Вставляем тока те элементы которые указаны в select-e
			foreach (string node in listSELECT)
			{
				//При этом мы должны "пробежать все SQL запросы и посмареть что сюда вставить..."
				if (f.ifExistInSQL(node) == false)
					//xd.WriteLine("\t\t<" + node + "><xsl:value-of select=\"" + node + "\"/></" + node + ">");
					outxd.WriteLine("<xs:element name=\"" + node + "\" type=\"xs:string\" minOccurs=\"0\" />");
				else
				{
					string tagName = GetTableName(node);
					//xd.WriteLine("\t\t<" + node + "><xsl:call-template name=\"" + tagName + "\"/></" + node + ">");
					outxd.WriteLine("<xs:element name=\"" + node + "\" minOccurs=\"0\" maxOccurs=\"unbounded\"><xs:complexType>\n\t<xs:sequence>");
                    this.ParseforXSD(ind+1);

					outxd.WriteLine("\n\t</xs:sequence></xs:complexType></xs:element>");
				}

			}
			if (ind != 0)
				outxd.WriteLine("</xs:sequence></xs:complexType></xs:element>");
			if (ind ==0)
			{
				outxd.WriteLine("\t</xs:choice>\n\t</xs:complexType>\n</xs:element>");
			}
		}

		//Ф-ия преобразует входной фаил sql.txt в  массив SQL
		private int SetArraySQLOld()
		{
			if (sql == null)
			{
				return 1;
			}
			this.SQLLines = new ArrayList();
			this.SQLLinesXSD = new ArrayList();
			string line;
			string str = "";
			while ((line = this.sql.ReadLine())!=null)
			{
				str += line.ToLower();
				
				int iSELECT = str.IndexOf("select ");
				int iFROM = str.IndexOf(" from ");
				int iAS = str.IndexOf(" as ");
			
				if (iSELECT >= 0 && iFROM >0 && iAS > 0)
				{
					this.SQLLines.Add(str.ToLower());
					this.SQLLinesXSD.Add(str.ToLower());
					str = "";
				}
			}
			sql.Close();
			return 0;
		}

		private int SetArraySQL()
		{
			if (sql == null)
			{
				return 1;
			}
			//this.SQLLines = new ArrayList();
			//this.SQLLinesXSD = new ArrayList();
			//string line;
			//string str = "";

			SqlFile sq = new SqlFile(sql);
			//this.SQLLines = sq.GetArray();
			//this.SQLLines = sq.GetArray();

			sql.Close();
			return 0;
		}

		private int SetMainXMLTag()
		{
			/*FileStream fxml = new FileStream(inputXmlFile, FileMode.Open);
            XmlTextReader read = new XmlTextReader(fxml);
			while (read.Read())
			{
				if (read.NodeType == XmlNodeType.Element)
				{
                    this.MainTag =  read.Name;
					fxml.Close();
					break;
				}
			}
			fxml.Close();*/
			if (this.inxd == null)
				return 1;
			DataSet ds = new DataSet();
			ds.ReadXmlSchema(inxd);
			this.MainTag = ds.DataSetName;
			return 0;
		}

		public void Execute(string XMLFile, string XSLFile, string outXMLFile)
		{
			XPathDocument doc = new XPathDocument(XMLFile);
			XslTransform tr = new XslTransform();
			tr.Load(XSLFile);
			
			XPathNavigator nav = ((IXPathNavigable)doc).CreateNavigator();

			FileStream fs = new FileStream(outXMLFile, FileMode.Create);
			tr.Transform(nav,null,fs);
			fs.Close();


			//Create XSD
            /*DataSet ds = new DataSet("DataSet");
			ds.ReadXml(outXMLFile);
			ds.WriteXmlSchema("schema.xsd");*/
		}


		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
			Auto auto = new Auto();
			

			//return -1;
			//Sql c = new Sql();

			//Правила аргументов командной строки такие....
			// - outfile	имя_файла		-	имя выходного файла
			// - readXSD	имя_файла		-	имя входной схемы XSD
			// - WriteXSD	имя_файла		-	если надо создать XSD схему
			// - readSQL	имя_файла		-	фаил с SQL
			// - ?							-	помощь
			//
			// TODO: Add code to start application here
			//
			string URI = null;
			string URIText = null;
			string inXSD = null;
			string inSQL = null;
			string inXML = null;
			string outXSL = null;
			string outXSD = null;
			string outXML = null;
			string Manifest = null;

			for (int i=0;i<args.Length;i++)
			{
				if (args[i].ToLower().Trim('/','-') == "?")
				{
					Console.WriteLine("Правила аргументов командной строки такие....");
					Console.WriteLine(" - outfile		имя_файла	-	имя выходного XSL файла");
					Console.WriteLine(" - readXSD		имя_файла	-	имя входной схемы XSD");
					Console.WriteLine(" - WriteXSD	имя_файла		-	если надо создать XSD схему");
					Console.WriteLine(" - readSQL		имя_файла	-	фаил с SQL");
					Console.WriteLine(" - readXML		имя_файла	-	входной XML фаил данных из Natural-e");
					Console.WriteLine(" - writeXML	имя_файла		-	XML фаил данных для Word-a");
					Console.WriteLine(" - URI			имя_схемы	-	URI");
					Console.WriteLine(" - URIText		Текст		-	Текст который будет отображаться в Word-e");
					Console.WriteLine(" - Manifest	имя_файла		-	Имя манифеста");
					Console.WriteLine(" - ?							-	помощь");
					return -1;
				}
				if (args[i].ToLower().Trim('/','-') == "outfile" && i+1 < args.Length)
				{
					outXSL = args[i+1].Trim('\"','\'','`');
					i++;
					continue;
				}
				if (args[i].ToLower().Trim('/','-') == "readxsd" && i+1 < args.Length)
				{
					inXSD = args[i+1].Trim('\"','\'','`');
					i++;
					continue;
				}
				if (args[i].ToLower().Trim('/','-') == "uri" && i+1 < args.Length)
				{
					URI = args[i+1].Trim('\"','\'','`');
					i++;
					continue;
				}
				if (args[i].ToLower().Trim('/','-') == "uritext" && i+1 < args.Length)
				{
					URIText = args[i+1].Trim('\"','\'','`');
					i++;
					continue;
				}
				if (args[i].ToLower().Trim('/','-') == "manifest" && i+1 < args.Length)
				{
					Manifest = args[i+1].Trim('\"','\'','`');
					i++;
					continue;
				}
				if (args[i].ToLower().Trim('/','-') == "writexsd" && i+1 < args.Length)
				{
					outXSD = args[i+1].Trim('\"','\'','`');
					i++;
					continue;
				}
				if (args[i].ToLower().Trim('/','-') == "readsql" && i+1 < args.Length)
				{
					inSQL = args[i+1].Trim('\"','\'','`');
					i++;
					continue;
				}
				if (args[i].ToLower().Trim('/','-') == "readxml" && i+1 < args.Length)
				{
					inXML = args[i+1].Trim('\"','\'','`');
					i++;
					continue;
				}
				if (args[i].ToLower().Trim('/','-') == "writexml" && i+1 < args.Length)
				{
					outXML = args[i+1].Trim('\"','\'','`');
					i++;
					continue;
				}
			} 
			//вводим данные по умолчанию
            //c.Output = "transform.xsl";
			//c.inXSDFileName = "sampledoc.xsd";
			//c.OutXsd = "schema.xsd";
			//c.SQLFile = "sql.txt";
			if (!auto.Create(inXSD, inSQL, outXSL, outXSD, URI, URIText, Manifest))
			{
                Console.WriteLine("Ошибка. не задан 1 из параметров.");
				return 1;
			}

			try
			{
				auto.Run();
			}
			catch (System.Exception e)
			{
				Console.WriteLine("Ошибка при компиляции :" + e.Message);
				Console.ReadLine();
				return 1;
			}
			try
			{
				//c.Execute(c.inXmlFile, c.Output, c.outXmlFile);
				if (inXML != null && outXML != null)
				{
					auto.Execute(inXML, outXML);
				}
			}
			catch (System.Exception e)
			{
				Console.WriteLine("Ошибка при выполнении :" + e.Message);
				Console.ReadLine();
				return 1;
			}
			//
			return 0;
		}
	}
}
