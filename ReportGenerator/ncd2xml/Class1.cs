using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Data;


namespace IntecoAG.ncd2xml
{
	/// <summary>
	/// Класс разбирающий NCF, NCD файлы, проверяет их и создает XML, XSD-файлы с данными
	/// </summary>
	class Class1
	{
		//private string XMLNS = "http://www.w3.org/1999/XSL/Transform";
		private int type = 0;
		private string XMLNS = "";
        /// <summary>
        /// Информация о колонки
        /// </summary>
		private struct ColumnInfo
		{
			public string	name;
			public int		type;
			public float	len;
			public ArrayList Data;

			public ColumnInfo(string name, int type, float len)
			{
				this.name = name;
				this.type = type;
				this.len = len;
				this.Data = new ArrayList();
			}
		}; 
		private ArrayList TableList = new ArrayList();
		private string xmlns = "SimpleSample";
		//private string XMLOutFile = ""
        
        /// <summary>
        /// Stream выходного текстового файла XML
        /// </summary>
		public TextWriter XMLOutFile = null;
        /// <summary>
        /// Схема БД
        /// </summary>
		public DataSet DataSetSchema = null ;
        /// <summary>
        /// имя файла в котором содержится входная схема проверки БД
        /// </summary>
		public string inXSDFileName = "";
        /// <summary>
        /// Выходная XML-схема - XSD
        /// </summary>
		public string OutXsd = "sampledoc.xsd";
		
        /// <summary>
        /// Имя выходного XML-файла
        /// </summary>
		public string FileName
		{
			get
			{
				if (XMLOutFile == null)
					return "";
				else
					return "file";
			}
			set
			{
                XMLOutFile = new StreamWriter(value);
			}

		}
        /// <summary>
        /// Структура хранящая в себе информацию о таблице (имя таблицы, имена колонок и XMLNS)
        /// </summary>
		public struct TableInfo 
		{
            /// <summary>
            /// Имя таблицы
            /// </summary>
			public string name;
            /// <summary>
            /// Список колонок
            /// </summary>
			public ArrayList ColumnList;
            /// <summary>
            /// XMLNS
            /// </summary>
			public string xmlns;
            /// <summary>
            /// Конструктор по умолчанию
            /// </summary>
            /// <param name="name">Имя таблицы</param>
            /// <param name="xmlns">XMLNS</param>
			public TableInfo(string name, string xmlns)
			{
				this.name = name;
				this.ColumnList = new ArrayList();
				this.xmlns = xmlns;
			}
		};


		
	    /// <summary>
        /// /Сравнивает схему полученную из NCF и XSD схему, записанную в DataDet
	    /// </summary>
	    /// <returns></returns>
		public int Compare()
		{
			foreach (TableInfo table in TableList)
			{
				int ind = this.DataSetSchema.Tables.IndexOf(table.name);
				foreach (ColumnInfo info in table.ColumnList)
				{
                    switch(info.type)
					{
						case 1: //A
							if (DataSetSchema.Tables[ind].Columns[info.name].DataType.Equals(typeof(String)))
								break;
							else
								return 1;
						case 2: case 3: //N,P
							if (DataSetSchema.Tables[ind].Columns[info.name].DataType.Equals(typeof(Decimal)))
								break;
							else
								return 1;
						case 4: //D
							if (DataSetSchema.Tables[ind].Columns[info.name].DataType.Equals(typeof(DateTime)))
								break;
							else
								return 1;
						case 5: //L
							if (DataSetSchema.Tables[ind].Columns[info.name].DataType.Equals(typeof(Boolean)))
								break;
							else
								return 1;
					}
				}
			}

            return 0;

		}
        /// <summary>
        /// Переводит число из float в int по нужному типу
        /// если на входе A15 - длина будет 15
        /// если на входе P2.5 - длина будет 2+5+1 = 8
        /// </summary>
        /// <param name="len"></param>
        /// <param name="type"></param>
        /// <returns></returns>
		private int Len(float len, int type)
		{
			string str = len.ToString();
			switch (type)
			{
				case 1:	//A
					return int.Parse(str);
				case 2: case 3: //P, N
				{
					//3,2 = 3 + 2 + 1(точка) + 1(знак) = 7
					int ind = str.IndexOf(",");
					if (ind != -1)
					{
						string first = str.Substring(0,ind);
						string second = str.Substring(ind+1,str.Length - ind-1);
						return int.Parse(first) + int.Parse(second) + 2;
					}else
						return int.Parse(str) + 1;
					
				}
				case 4:	//D
					return 8;
				case 5: //L
					return 1;
			}
			return 0;

			/*int ind = str.IndexOf(",");
			if (ind == -1)
			{
                //Значит в строке нет запятой, а занчит там записано целое число
				return int.Parse(str);
			}else{
                //значение не целое, а значит
				//3,2 = 3 + 2 + 1(точка) + 1(знак) = 7
				string first = str.Substring(0,ind);
				string second = str.Substring(ind+1,str.Length - ind-1);
				return int.Parse(first) + int.Parse(second) + 1;
            }*/
		}
        /// <summary>
        /// Пишет XSD-схему одной таблицы
        /// </summary>
        /// <param name="table">Таблица</param>
        /// <param name="tw">Путь до потока-писателя</param>
		private void WriteXSD(TableInfo table, XmlTextWriter tw)
		{
			tw.WriteStartElement("xs:element");
			tw.WriteAttributeString("name",table.name.ToLower());
			tw.WriteStartElement("xs:complexType");
			tw.WriteStartElement("xs:sequence");
			//int CountData =0;
			/*if (table.ColumnList.Count>0)
			{
				ColumnInfo info = (ColumnInfo) table.ColumnList[0];
				CountData = info.Data.Count;
			}*/
			//for (int i=0; i< CountData; i++)
			{
				for (int j=0; j<table.ColumnList.Count; j++)
				{
					ColumnInfo info = (ColumnInfo) table.ColumnList[j];
					tw.WriteStartElement("xs:element");
					tw.WriteAttributeString("name",info.name);
					switch (info.type)
					{
						case 1: //A
							tw.WriteAttributeString("type","xs:string");
							break;
						case 2: case 3://N , P
                            tw.WriteAttributeString("type","xs:decimal");
							break;
						case 4: //D
							tw.WriteAttributeString("type","xs:date");
							break;
						case 5: //L
							tw.WriteAttributeString("type","xs:boolean");
							break;
						default:
							tw.WriteAttributeString("type","xs:string");
							break;
					}
					tw.WriteAttributeString("minOccurs","0");
					tw.WriteEndElement();
				}

			}		tw.WriteEndElement();
			tw.WriteEndElement();
			tw.WriteEndElement();
		}
		/// <summary>
		/// Пишет всю XSD схему
		/// </summary>
		private void WriteXSD()
		{
			//StreamWriter tw = new StreamWriter(FileName + ".xsd");
			XmlTextWriter xtw = new XmlTextWriter(OutXsd,null);
			//xtw.Formatting = Formatting.Indented;
			xtw.WriteStartDocument(true);
			xtw.WriteStartElement("xs:schema");
			xtw.WriteAttributeString("id","Document");
			xtw.WriteAttributeString("xmlns:xs" ,"http://www.w3.org/2001/XMLSchema");
			xtw.WriteAttributeString("xmlns:msdata" ,"urn:schemas-microsoft-com:xml-msdata");
			xtw.WriteStartElement("xs:element");
			xtw.WriteAttributeString("name","document");
			xtw.WriteAttributeString("msdata:isDataSet","true");
			xtw.WriteAttributeString("msdata:Locale","ru-RU");
			xtw.WriteStartElement("xs:complexType");
			xtw.WriteStartElement("xs:choice");
			xtw.WriteAttributeString("maxOccurs","unbounded");
						


			//			tw.WriteLine("<xs:element name=\"document\" msdata:IsDataSet=\"true\" msdata:Locale=\"ru-RU\">");
			
			foreach (TableInfo table in TableList)
			{
				WriteXSD(table, xtw);
			}
			xtw.WriteEndElement();
			xtw.WriteEndElement();
			xtw.WriteEndElement();
			xtw.WriteEndElement();
			xtw.WriteEndDocument();
			xtw.Flush();
			xtw.Close();
		}
        /// <summary>
        /// Пишет XML: создает Stream, пишет заголовочные теги и вызывает WriteXML для каждой таблицы
        /// </summary>
        /// <returns>0 -успешно, 1 - ошибка</returns>
		private int WriteXML()
		{
			if (XMLOutFile == null)
				return 1;
			XmlTextWriter tw = new XmlTextWriter(XMLOutFile);
			//tw.Formatting = Formatting.Indented;
			tw.WriteStartDocument();
			tw.WriteStartElement("document");
			//tw.WriteAttributeString("xmlns:xsl",this.XMLNS);
			foreach (TableInfo table in TableList)
			{
				WriteXML(table, tw);
			}
			tw.WriteEndElement();
			tw.WriteEndDocument();
			tw.Flush();
			tw.Close();
			return 0;
		}
        /// <summary>
        /// Удаляет нули в конце чисел, т.е. если есть число 2,500000 = 2,5
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
		private string DeleteZero(string val)
		{
			if (val.Length == 0)
				return val;

			//return val;
			string res = "";
			int i = 0;
			int j = 0;
			while (i<val.Length)
			{
                if (val[i] == '0')
					j++;
				else
					break;
				i++;
			}
			res = val.Substring(j);
			if (res.Length> 0)
			{
				if (res[0] == '.' || res[0] == ',')
				{
					res = "0" + res;
				}
			}
			return res;
		}
        /// <summary>
        /// Пишет XML одной таблицы
        /// </summary>
        /// <param name="table">Таблица</param>
        /// <param name="tw">Поток для записи</param>
		private void WriteXML(TableInfo table,XmlTextWriter tw)
		{
			int ind = 0;
			if (type == 1)
				tw.WriteStartElement(table.name.ToLower());
			//tw.WriteAttributeString("xmlns","SimpleSample");
			int CountData =0;
			if (table.ColumnList.Count>0)
			{
				ColumnInfo info = (ColumnInfo) table.ColumnList[0];
				CountData = info.Data.Count;
			}
			for (int i=0; i< CountData; i++)
			{
				if ( type == 1)
				{
					if (CountData > ind)
						tw.WriteStartElement("tr-" + table.name.ToLower());
				}
				else
				{
					tw.WriteStartElement(table.name.ToLower());
				}
				for (int j=0; j<table.ColumnList.Count; j++)
				{
					ColumnInfo info = (ColumnInfo) table.ColumnList[j];
					//if (info.type == 0)
					//continue;
					tw.WriteStartElement(info.name.ToLower());
					//
					//tw.WriteAttributeString("type",info.type.ToString().ToLower());
					//tw.WriteStartAttribute("type",info.type.ToString());
					//tw.WriteEndAttribute();
					//tw.WriteAttributeString("length",info.len.ToString().ToLower().Replace(",","."));
					if (info.type == 4)//data
					{
						string input = ((string)info.Data[i]);
						string date1;
						if (input.Length == 8)
							date1 = input.Substring(0,4) + "-" + input.Substring(4,2) + "-" + input.Substring(6);
						else
							date1 = input;
						tw.WriteString(date1.Trim());
					}
					else
					{
						if (info.Data.Count == 0)
							tw.WriteString("");
						else
						{
							string val = ((string)info.Data[i]).Trim();
							val = DeleteZero(val);
							tw.WriteString(val);
						}
					}

					tw.WriteEndElement();
					//
				}
				if (type == 1)
				{
					if (CountData > ind)
						tw.WriteEndElement();
				}
				else
					tw.WriteEndElement();
			}
			if (type == 1)
				tw.WriteEndElement();
		}
        /// <summary>
        /// Обрезает необходимые символы
        /// </summary>
        /// <param name="txt">строка к которой применятеся обрезка</param>
        /// <returns>обрезанная строка</returns>
		public string Trim(string txt)
		{
			string str = txt.Replace("<","&lt;");
			str = str.Replace(">", "&gt;");

			char [] ch = {'\0','\r','\n'};
			str = str.Trim(ch);

			return str;

		}
        /// <summary>
        /// Читает из файла строку (до \r \n) длиной не больше чем len
        /// </summary>
        /// <param name="read">Поток для чтения</param>
        /// <param name="len">Максимальная длина</param>
        /// <returns></returns>
		private string Read(StreamReader read, int len)
		{
			string str;
			char [] ch = new char[len];
			int count = 0;
            for (int i=0;i<len;i++)
            {
				int res = read.Read(ch,i,1);
				if (res == 0)
					break;
				//если новая строка... то мы заканчиваем ввод
				if (ch[i] == '\r' || ch[i] == '\n')
					break;
				count++;
            }
			str = new String(ch);
			if (count < len)
			{
				str = str.Trim();
				str = str.Trim('\0');
				if (str.Length  == 0)
				{
					str = null;
					count = 0;
				}
			}
			
			return str;
		}
        /// <summary>
        /// Разбирает  NCD-файла
        /// </summary>
        /// <param name="FileName">Имя входного файла</param>
        /// <param name="table">Таблица, в которую будет писаться информация</param>
		public void ReadNCD(string FileName, TableInfo table)
		{
			//FileStream f = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			StreamReader read = new StreamReader(FileName,System.Text.Encoding.Default);
			int position = 0;
			int numberColumn = 0;
            while (true)
            {
				//object obj = null;
				ColumnInfo info =(ColumnInfo) table.ColumnList[numberColumn];
				int len = Len(info.len, info.type);
				char[] ch = new char[len];
				//Получаем колонку по нужным номером
				//Читаем нужное кол-во символов
				//int yy=0;

				string str = Read(read, len);
				if (str == null && numberColumn < table.ColumnList.Count && numberColumn!=0)
				{
					numberColumn++;
					if (numberColumn >= table.ColumnList.Count)
						numberColumn = 0;
					read.ReadLine();
					continue;
				}

				if (str == null)
				{
					break;
				}
                //int yy = read.Read(ch,position,len);
				/*if (yy != len)
				{
					if (yy < len && yy!=0)
					{
						str = new string(ch);
					}
					else					
						break;
				}else
					str = new string(ch);*/

				////Console.WriteLine(str);
				///
				/*if (numberColumn >= table.ColumnList.Count)
				{
					//Читаем символы до конца строки
					read.ReadLine();
					//continue;
					numberColumn = 0;
				}*/
                info.Data.Add(str);
				table.ColumnList[numberColumn] = (object)info;
				//position+=len;
				numberColumn++;
				if (numberColumn >= table.ColumnList.Count)
				{
					//Читаем символы до конца строки
					//if (!exit)
					//{
					read.ReadLine();
					//}
					numberColumn = 0;
				}
            }
			read.Close();
		}
        /// <summary>
        /// Читает и разбирает NCF-файл
        /// </summary>
        /// <param name="FileName">Имя NCF-фАЙЛА</param>
        /// <param name="table">Таблица, в которую будут записываться распарсенные данные</param>
		public void ReadNCF(string FileName, TableInfo table)
		{
			FileStream f = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None);
			StreamReader read = new StreamReader(f);
			string NextLine;
			//table.name = FileName;
			//table.ColumnList = new ArrayList();
			//Console.WriteLine("FileName = " + FileName);
			while((NextLine = read.ReadLine()) != null)
			{
				//Разбираем строку

				//Шаблон для Format:A10,N3.2, итд
				string pattern = @"\s*Format\s*:\s*(((A[0-9]+)|(N[0-9.]+)|(P[0-9.]+)|(R[0-9]+)|([D]?)|([L]?)),)+";

				//Шаблон для Names :
				//					val1
				//					val2
				string patternNames = @"\s*Names\s*:[\s\n]*";

				string patternFormat2 = @"\s*Format\s*:\s*([A-Za-z0-9.]+)*";

				Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
				Match m = r.Match(NextLine);

				Regex rName = new Regex(patternNames, RegexOptions.IgnoreCase);
				Match mName = rName.Match(NextLine);

				Regex rF2 = new Regex(patternFormat2, RegexOptions.IgnoreCase);
				Match mF2 = rF2.Match(NextLine);


				int Arr = 1;
					

				if (m.Success)
				{
					//Нашли Format:
					Group g = m.Groups[1];
					////Console.WriteLine("Group  = \'"+g + "\'");
					CaptureCollection cc = g.Captures;
					for (int j = 0; j < cc.Count; j++) 
					{
						Capture c = cc[j];
						//Дальше мы сморим на 1-ю букву и пределеяем тип, остот переводим в число: (int/float)
						//При этом надо удалить последний символ - запятую.
						if (c.Value[0] == 'A')
						{
							string str = c.Value.Substring(1,c.Value.Length-2);
							int len = int.Parse(str);
							for (int k=0;k<Arr;k++)
							{
                                table.ColumnList.Add(new ColumnInfo(c.Value.Substring(0,c.Value.Length-1),1,len));
								//Console.WriteLine("A = " + str);
							}
							Arr = 1;
							//table.ColumnList.Add(new ColumnInfo(c.Value.Substring(0,c.Value.Length-1),1,len));
						}else if (c.Value[0] == 'N')
						{
							string str = c.Value.Substring(1,c.Value.Length-2);
							str = str.Replace(".",",");
							float len = float.Parse(str);
							for (int k=0;k<Arr;k++)
							{
								table.ColumnList.Add(new ColumnInfo(c.Value.Substring(0,c.Value.Length-1),2,len));
								//Console.WriteLine("N = " + str);
							}
							Arr = 1;
						}else if (c.Value[0] == 'P')
						{
							string str = c.Value.Substring(1,c.Value.Length-2);
							str = str.Replace(".",",");
							float len = float.Parse(str);
							for (int k=0;k<Arr;k++)
							{
								table.ColumnList.Add(new ColumnInfo(c.Value.Substring(0,c.Value.Length-1),3,len));
								//Console.WriteLine("P = " + str);
							}
							Arr = 1;
						}else if (c.Value[0] == 'D')
						{
							for (int k=0;k<Arr;k++)
							{
								table.ColumnList.Add(new ColumnInfo("D",4,8));
								//Console.WriteLine("D");
							}
							Arr = 1;
						}else if (c.Value[0] == 'L')
						{
							for (int k=0;k<Arr;k++)
							{
								table.ColumnList.Add(new ColumnInfo("L",4,1));
								//Console.WriteLine("L");
							}
							Arr = 1;
						}else if (c.Value[0] == 'R')
						{
							string str = c.Value.Substring(1,c.Value.Length-2);
							int len = int.Parse(str);
							Arr = len;
							//Console.WriteLine("R");
						}
					}
					//m = m.NextMatch();
				}else if(mName.Success)
				{
					//Если нашли Names:
                    //То мы до конца файла читаем имена переменных, расположенные на каждой строке
					int i=0;
					while((NextLine = read.ReadLine()) != null)
					{
						if (i >= table.ColumnList.Count) 
                            return;
						ColumnInfo info = (ColumnInfo)table.ColumnList[i];
                        info.name = NextLine.Trim().Replace("-","_").Replace(".","-");

						table.ColumnList[i] = (object)info;
						i++;
					}
				}else if (mF2.Success)
				{
					Group g = mF2.Groups[1];
					////Console.WriteLine("Group  = \'"+g + "\'");
					CaptureCollection cc = g.Captures;
					int yy = cc.Count;
					Capture c = cc[0];
					string str = c.Value;
					//Работаем с str -  в ней содержится вся строка...
					int i=0;
					while (i<str.Length)
					{
						if (str[i]=='A')
						{
							//Вырезаем следующие 2 символа...
							string sub = str.Substring(i+1,2);
							i+=3;
							//Преобразуем в число шеcтнадцатиричное.
							int len = int.Parse(sub,System.Globalization.NumberStyles.HexNumber);
							//for (int j=0;j<Arr;j++)
								
							//for (int j=0;j<Arr;j++)
							//if (Arr > 1)
								for (int j=0;j<Arr;j++)
							//		table.ColumnList.Add(new ColumnInfo("S" + sub,0,len));
							//else
								table.ColumnList.Add(new ColumnInfo("A" + sub,1,len));
							//Console.Write("A = " + len.ToString());
							//Console.WriteLine();
							Arr = 1;
						}
						else if (str[i] == 'N' )
						{
							//То что до точки
							string sub = str.Substring(i+1,2);
							//то что после точки
							string sub1 = str.Substring(i+4,1);
							int tseloe = int.Parse(sub, System.Globalization.NumberStyles.HexNumber);
							int drob =  int.Parse(sub1, System.Globalization.NumberStyles.HexNumber);
							float len = float.Parse(tseloe.ToString() + "," +  drob.ToString());
							//if (Arr > 1)
								for (int j=0;j<Arr;j++)
							//		table.ColumnList.Add(new ColumnInfo("S" + sub,0,len));
							//else
								table.ColumnList.Add(new ColumnInfo("N" + sub + "." + sub1,2,len));
							i+=5;
							for (int j=0;j<Arr;j++)
								//Console.Write("N = " + sub +"."+sub1);
							Arr = 1;
							//Console.WriteLine();
						}
						else if (str[i] == 'P')
						{
							//То что до точки
							string sub = str.Substring(i+1,2);
							//то что после точки
							string sub1 = str.Substring(i+4,1);
							int tseloe = int.Parse(sub, System.Globalization.NumberStyles.HexNumber);
							int drob =  int.Parse(sub1, System.Globalization.NumberStyles.HexNumber);
							float len = float.Parse(tseloe.ToString() + "," +  drob.ToString());
							//if (Arr > 1)
								for (int j=0;j<Arr;j++)
							//		table.ColumnList.Add(new ColumnInfo("S" + sub,0,len));
							//else
								table.ColumnList.Add(new ColumnInfo("P" + sub + "," + sub1,3,len));
							i+=5;
							//for (int j=0;j<Arr;j++)
								//Console.Write("P = " + sub +"."+sub1);
							//Console.WriteLine();
							Arr = 1;
						}
						else if (str[i] == 'D')
						{
							int len = 8;
							//if (Arr > 1)
								for (int j=0;j<Arr;j++)
							//	table.ColumnList.Add(new ColumnInfo("S8",0,len));
							//else
								table.ColumnList.Add(new ColumnInfo("D",4,len));
							i+=1;
							//for (int j=0;j<Arr;j++)
								//Console.Write("D");
							//Console.WriteLine();
							Arr = 1;
						}else if (str[i] == 'L')
						{
                            int len = 1;
							//if (Arr > 1)
								for (int j=0;j<Arr;j++)
							//		table.ColumnList.Add(new ColumnInfo("S1",0,len));
							//else
								table.ColumnList.Add(new ColumnInfo("L",5,len));
                            //for (int j=0;j<Arr;j++)
								//Console.Write("L");
							i+=1;
							//Console.WriteLine();
							Arr = 1;
						}
						else if (str[i]=='R')
						{
							//Вырезаем следующие 2 символа...
							string sub = str.Substring(i+1,2);
							i+=3;
							//Преобразуем в число шеcтнадцатиричное.
							int len = int.Parse(sub,System.Globalization.NumberStyles.HexNumber);
							//table.ColumnList.Add(new ColumnInfo("A" + sub,1,len));
							//Console.Write("R");
							Arr = len;
						}
						else
						{
							//Console.WriteLine("Error in ='" + str[i] + "'");
							i++;
						}
						//i++;
					}

					//int a=0;


				}
			}
			read.Close();

			/*for (int i=0;i<table.ColumnList.Count;i++)
			{
				ColumnInfo info = (ColumnInfo)table.ColumnList[i];
				//Console.Write(info.name + " " + info.type + " " + info.len);
				//Console.WriteLine();
			}*/
		}
        /// <summary>
        /// Добавляет файл к списку всех файлов для разбора
        /// </summary>
        /// <param name="file">Файл для разбора</param>
		public void AddFile(FileInfo file)
		{
			TableInfo table = new TableInfo(file.Name.Substring(0, file.Name.Length-4), xmlns);
			//Read NCF
			ReadNCF(file.FullName, table);

			//Read NCD
			try
			{
				ReadNCD(file.FullName.Substring(0, file.FullName.Length-4) + ".ncd", table);
			}
			catch 
			{
				Console.Error.Write("Error. Can't read \"ncd\" file");
			}
					
			TableList.Add(table);
		}
		
		/// <summary>
		/// The main entry point for the application.
		/// Главная функция
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{

			//Правила аргументов командной строки такие....
			// - outfile	имя_файла		-	имя выходного файла
			// - readXSD	имя_файла		-	имя входной схемы XSD
			// - WriteXSD	имя_файла		-	если надо создать XSD схему
			// - Dir		имя-файла		-	Имя папки из которой берем Natural-вские файлы
			// - ?							-	помощь
			//
			// TODO: Add code to start application here
			//
			Class1 c = new Class1();

			string Dir = ".\\";

			for (int i=0;i<args.Length;i++)
			{
				if (args[i].ToLower().Trim('/','-') == "?")
				{
					Console.WriteLine("//Правила аргументов командной строки такие....");
					Console.WriteLine("// - outfile	имя_файла		-	имя выходного файла");
					Console.WriteLine("// - readXSD	имя_файла		-	имя входной схемы XSD");
					Console.WriteLine("// - WriteXSD	имя_файла	-	если надо создать XSD схему");
					Console.WriteLine("// - Dir		имя-файла		-	Имя папки из которой берем Natural-вские файлы");
					Console.WriteLine("// - ?						-	помощь");
					return -1;
				}
				if (args[i].ToLower().Trim('/','-') == "outfile" && i+1 < args.Length)
				{
                    c.FileName = args[i+1];
					i++;
					continue;
				}
				if (args[i].ToLower().Trim('/','-') == "readxsd" && i+1 < args.Length)
				{
					c.inXSDFileName = args[i+1];
					i++;
					continue;
				}
				if (args[i].ToLower().Trim('/','-') == "writexsd" && i+1 < args.Length)
				{
					c.OutXsd = args[i+1];
					i++;
					continue;
				}
				if (args[i].ToLower().Trim('/','-','\"','\'') == "dir" && i+1 < args.Length)
				{
					string str = args[i+1].Trim('\"','\'');
					if (str.Length > 0)
					{
						char ch = str[str.Length-1];
						if (str[1] == ':')
						{
							Dir = str;
						}
						else
						{
							if( ch == '\\')
							{
								Dir = ".\\" + str;
							}
							else
								Dir = ".\\" + str + "\\";
						}
					}
					i++;
					continue;
				}
			} 
			//c.FileName = "sampledoc.xml";
			//c.OutXsd = "sampledoc.xsd";

			string tr = Path.GetDirectoryName(Dir);
			DirectoryInfo dir = new DirectoryInfo(tr);

			FileInfo[] files;

			//если файл с XSD схемой не задан, то
			if (c.inXSDFileName == "")
			{
				//плучаем все файлы из текущей папки
				files = dir.GetFiles();
			}else
			{
				FileInfo[] fs = dir.GetFiles();
                //Иначе берем списов из базы данных
				c.DataSetSchema = new DataSet();
				c.DataSetSchema.ReadXmlSchema(c.inXSDFileName);
                files = new FileInfo[c.DataSetSchema.Tables.Count];
				//Пробегаем все таблицы в БД

				for (int i=0; i<c.DataSetSchema.Tables.Count; i++)
				{
                    files[i] = new FileInfo(c.DataSetSchema.Tables[i].TableName + ".ncf");
				}
			}

			foreach (FileInfo file in files)
			{
				if (file.Extension.ToLower() == ".ncf")
				{
					c.AddFile(file);
				}
			}
			
			//Далее если указана XSD-схема, то сравниваем DataSet из XSD-схемы с тем что мы прочитали из NCF-файла
			if (c.inXSDFileName != "")
			{
				if (c.Compare() != 0)
				{
					////Console.WriteLine("Схемы не совпадают");
					Console.Error.Write("Ошибка, схемы не совпадают");
					Console.ReadLine();
					return 1; //Ошибка. Схемы не совпадают
				}
			}

			if (c.OutXsd !="")
				c.WriteXSD();

			if (c.FileName == "")
                c.XMLOutFile = Console.Out;
				
			c.WriteXML();

			//Console.ReadLine();
			
			return 0;			
		}
	}







}
