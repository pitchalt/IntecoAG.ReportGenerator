using System;
using System.Collections;
using System.Data;

namespace Comp1
{
    /// <summary>
    /// Имя колонки
    /// </summary>
	public class ColumnName
	{
		public string name;
		public string table;
        /// <summary>
        /// Коснтруктор
        /// </summary>
        /// <param name="name">Имя колонки</param>
        /// <param name="table">Имя таблицы</param>
		public ColumnName(string name, string table)
		{
			this.name = name;
			this.table = table;
		}
	};
    /// <summary>
    /// Тип группировки
    /// </summary>
	public enum TypeGroup {FOOTER, HEADER, CONTAIN, HEADERFOOTER};
    /// <summary>
    /// Тип тэга, используется ля обозначение имени тега footer
    /// </summary>
	public enum TypeFH {FOOTER, HEADER, CONTAIN};
    /// <summary>
    /// Тип выборки
    /// </summary>
	public enum SELECTTYPE {ALL, ONCE};
    /// <summary>
    /// То что являетя результатом запроса
    /// </summary>
	public enum ReturnSQLType
    {
        /// <summary>
        /// Таблица
        /// </summary>
        TABLE, 
        /// <summary>
        /// Одна запись
        /// </summary>
        ONCE, 
        /// <summary>
        /// Список
        /// </summary>
        LIST
    }

	/// <summary>
	/// Класс переименования колонок и таблиц
	/// </summary>
	public class Alias
	{
        /// <summary>
        /// Класс  хранения 2-х имен таблицы
        /// </summary>
		public class TableStore
		{
            /// <summary>
            /// Основоное имя
            /// </summary>
			public string name;
            /// <summary>
            /// Новое имя
            /// </summary>
			public string rename;
            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="name">Основное имя</param>
            /// <param name="rename">Новое имя</param>
			public TableStore(string name, string rename)
			{
				this.name = name;
				this.rename = rename;
			}
		};
        /// <summary>
        /// Класс хранение 2-х имен колонки
        /// </summary>
		public class ColumnStore
		{
            /// <summary>
            /// Основное имя
            /// </summary>
			public ColumnName name;
            /// <summary>
            /// Новое имя
            /// </summary>
			public string rename;
            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="name">Основное имя</param>
            /// <param name="rename">Новое имя</param>
			public ColumnStore(ColumnName name, string rename)
			{
				this.name = name;
				this.rename = rename;
			}
            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="name">Основное имя</param>
            /// <param name="rename">Новое имя</param>
            public ColumnStore(string name, string rename)
			{
				this.name = new ColumnName(name, null);
				this.rename = rename;
			}
		};
       /// <summary>
       /// Список таблиц
       /// </summary>
		public ArrayList tables = new ArrayList();
        /// <summary>
        /// Список колонок
        /// </summary>
		public ArrayList columns = new ArrayList();
		public Alias() { }
        /// <summary>
        /// Добавлет новое переименование к таблице
        /// </summary>
        /// <param name="name">Старое имя</param>
        /// <param name="rename">Новое имя</param>
		public void AddTable(string name, string rename)
		{
			tables.Add(new TableStore(name, rename));
		}
        /// <summary>
        /// Добавлет новое переименование к колонки
        /// </summary>
        /// <param name="name">Старое имя</param>
        /// <param name="rename">Новое имя</param>
		public void AddColumn(string name, string rename)
		{
            columns.Add(new ColumnStore(name,rename));
		}
        /// <summary>
        /// Добавлет новое переименование к колонки
        /// </summary>
        /// <param name="name">Старое имя</param>
        /// <param name="rename">Новое имя</param>
		public void AddColumn(ColumnName name, string rename)
		{
			columns.Add(new ColumnStore(name,rename));
		}
        /// <summary>
        /// Получает имя колонки
        /// </summary>
        /// <param name="name">Имя колонки</param>
        /// <returns>Переименнованное имя</returns>
        public ColumnName GetColumnAlias(string name)
		{
            for (int i=0;i<columns.Count;i++)
            {
				ColumnStore col = (ColumnStore)columns[i];
				if (col.rename == name)
				{
					return col.name;
				}
            }
			return null;
		}
        /// <summary>
        /// Получает еще одно имя таблицы
        /// </summary>
        /// <param name="table">Имя таблицы</param>
        /// <returns>Alias</returns>
		public string GetTableAlias(string table)
		{
			for (int i=0;i<tables.Count;i++)
			{
				TableStore st = (TableStore)tables[i];
				if (st.rename == table)
					return st.name;
			}
			return null;
		}
	};
    /// <summary>
    /// Класс "раскраски" Select
    /// </summary>
	public class Paint
	{
		/// <summary>
		/// Список полей раскраски
		/// </summary>
		private ArrayList list = new ArrayList();
		public string Other;
		public ColumnName By;
		public void Validate(DataSet ds, string defaultTable)
		{
			//return;
			//Провереям By.. Он должен быть в датасете
			if (By.table == null || By.table == "") 
                By.table = defaultTable;
            DataTable t = ds.Tables[By.table];
			if (t == null)
				throw new NotExistException("Не найдено таблицы в DataSet-e: " + By.table);
			//Проверем колонку...
			if (By.name == null || By.name == "")
				throw new NotExistException("PAINT By: Не заданы имя колонки");
			if (t.Columns[By.name] == null)
				throw new NotExistException("Не найдено колонки в DataSet-e: " + By.table + "." + By.name);
			//Проверяем Other...
			if (Other == null || Other == "")
				throw new NotExistException("Не задан тэг OTHER! ");
		}
        private class Value
		{
			public string val;
			public string tag;
			public Value(string val, string tag)
			{
                this.val = val;
				this.tag = tag;
			}
		};
		public void AddVal(string val)
		{
			list.Add(new Value(val,""));
		}
		public void AddTag(string tag)
		{
			((Value)list[list.Count-1]).tag = tag;
            //this.Other = select.
		}
		public void Add(string val, string tag)
		{
			list.Add(new Value(val,tag));
		}
		public string Tag(int i)
		{
			return ((Value) (list[i])).tag;
		}
		public string Val(int i)
		{
			return ((Value) (list[i])).val;
		}
		public string this[int i]
		{
			get
			{
				return ((Value) (list[i])).tag;
			}
		}
		public int Count
		{
			get
			{
				return list.Count;
			}
		}
	};
	/// <summary>
	/// Класс который хранит Select из SQL запроса
	/// </summary>
	public class SelectClass 
	{
		private class SelectCompareClass : IComparer  
		{

			// Calls CaseInsensitiveComparer.Compare with the parameters reversed.
			int IComparer.Compare( Object x, Object y )  
			{
				Data d_x = (Data)x;
				Data d_y = (Data)y;
				if (d_x.typeobj == typeObj.SQL && d_y.typeobj == typeObj.STRING)
					return 1;
				else if (d_x.typeobj == typeObj.STRING && d_y.typeobj == typeObj.SQL)
					return -1;
				else
					return 0;

				//return( (new CaseInsensitiveComparer()).Compare( y, x ) );
			}

		}


        /// <summary>
        /// Стортировка
        /// </summary>
        public void Sort()
		{
			SelectCompareClass c = new SelectCompareClass();
			l.Sort(c);
		}
        /// <summary>
        /// Базовый класс (родитель)
        /// </summary>
		public SQLStruct baseSql;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="baseSql">Базовый класс</param>
		public SelectClass(SQLStruct baseSql)
		{
			this.baseSql = baseSql;
		}

		private string defaultTable;
		private enum typeObj {STRING, SQL};
		private class Data
		{
			//Флаг, о том используется ли эта колонка в дочерних селектах, если да, то по нему создатся переменная
			public bool isUseInChild = false;

			public object select;
			public string tableName;
			public typeObj typeobj;
			public ElementType	type; 
			public Data(string name, ElementType type1)
			{
				select = (object) name;
				typeobj = typeObj.STRING;
				this.type = type1;
			}
			public Data(string tableName, string name, ElementType type1)
			{
				select = (object) name;
				typeobj = typeObj.STRING;
				this.type = type1;
				this.tableName = tableName;
			}

			public Data(string name)
			{
				select = (object) name;
				typeobj = typeObj.STRING;
				this.type = ElementType.STRING;
			}

			public Data(string tableName, string name)
			{
				select = (object) name;
				typeobj = typeObj.STRING;
				this.type = ElementType.STRING;
				this.tableName = tableName;
			}

			public Data(SQLStruct name)
			{
				select = (object) name;
				typeobj = typeObj.SQL;
				type = ElementType.STRING;
			}
		};
		private ArrayList l = new ArrayList();
		private ArrayList key = new ArrayList();
        /// <summary>
        /// Тип выборки ALL, ONCE
        /// </summary>
		public SELECTTYPE selectType = SELECTTYPE.ALL;
        /// <summary>
        /// Тег AS
        /// </summary>
		public string UseTag = null;
        /// <summary>
        /// Класс раскраски
        /// </summary>
		public Paint paint = new Paint();
        /// <summary>
        /// Проверят, является ли ключем
        /// </summary>
        /// <param name="node">Имя ключа</param>
        /// <returns>Совпадает или нет</returns>
		public bool isKey(string node)
		{
            for (int i=0;i<key.Count;i++)
            {
				string k = (string)((Data)key[i]).select;
				if (node == k)
				{
					return true;
				}
				//l.Add(new (name.ToLower(),ElementType.STRING));
            }
			return false;
		}
		/// <summary>
		/// Добавлет AS-тэг
		/// </summary>
		/// <param name="str">Имя нового as-тэга</param>
		public void AddAs(string str)
		{
			if (l.Count ==0)
				throw new Exception("Нет ниодной колонки для переимнования");
			Data d = (Data)l[l.Count - 1];
			if (d.typeobj == typeObj.SQL)
				throw new Exception("Нильзя переимновать SQL запрос");

			string name =(string) d.select;
			string table =(string) d.tableName;
            this.baseSql.alias.AddColumn(new ColumnName(name,table), str);
			d.select = str;
		}
        /// <summary>
        /// Добавлет ключ
        /// </summary>
        /// <param name="column">Имя колонки</param>
		public void AddKey(string column)
		{
			key.Add(new Data(column, ElementType.STRING));
		}
        /// <summary>
        /// Добавлет ключ
        /// </summary>
        /// <param name="table">Имя таблицы</param>
        /// <param name="column">Имя колонки</param>
		public void AddKey(string table, string column)
		{
            key.Add(new Data(table, column, ElementType.STRING));
		}
        /// <summary>
        /// Устанавливает ключ таблицы
        /// </summary>
        /// <param name="table">Имя таблицы</param>
		public void SetKeyTable(string table)
		{
			Data d = (Data)(key[key.Count-1]);
			//d.select = 
			d.tableName = (string) d.select;
			d.select = table;
		}

        /// <summary>
        /// ПОлучает ключ
        /// </summary>
        /// <param name="i">Номер ключа</param>
        /// <returns>Имя</returns>
		public string GetKey(int i)
		{
			if (i >= key.Count)
				throw new IndexOutOfRangeException("Не существует элемента под номером: " + i.ToString());
            return (string) ((Data)key[i]).select;
		}
        /// <summary>
        /// Получает ключ таблицу
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
		public string GetKeyTable(int i)
		{
			if (i >= key.Count)
				throw new IndexOutOfRangeException("Не существует элемента под номером: " + i.ToString());
			if ((string)((Data)(key[i])).tableName != null && (string)((Data)(key[i])).tableName != "")
			{
				return (string)((Data)(key[i])).tableName;
			}
			else
				return defaultTable;

		}
        /// <summary>
        /// кол-во ключей
        /// </summary>
		public int KeyCount
		{
			get
			{
				return key.Count;
			}
		}
        /// <summary>
        /// Добавляет колонку
        /// </summary>
        /// <param name="name">Имя колонки</param>
		public void Add(string name)
		{
			if(!isExist(name.ToLower()))
				l.Add(new Data(name.ToLower(),ElementType.STRING));
		}
        /// <summary>
        /// Добавлет колонку
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="name">Имя колонки</param>
		public void Add(string tableName, string name)
		{
			if(!isExist(name.ToLower()))
				l.Add(new Data(tableName.ToLower(), name.ToLower(),ElementType.STRING));
		}
        /// <summary>
        /// Добавлет колонку
        /// </summary>
        /// <param name="name">Имя колонки</param>
        /// <param name="type">Тип колонки</param>
		public void Add(string name, ElementType type)
		{
			if(!isExist(name.ToLower()))
				l.Add(new Data(name.ToLower(),type));
		}
		
        /// <summary>
        /// Добавляе колонку
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="name">Имя колонки</param>
        /// <param name="type">тип колонки</param>
		public void Add(string tableName, string name, ElementType type)
		{
			if(!isExist(name.ToLower()))
				l.Add(new Data(tableName.ToLower(), name.ToLower(), type));
		}
        /// <summary>
        /// Добавлет в качестве колонки  SQL-выражение
        /// </summary>
        /// <param name="name"> SQL-выражение</param>
		public void Add(SQLStruct name)
		{
			l.Add(new Data(name));
		}
        /// <summary>
        /// Устанавливает иям таблицы  по умолчанию
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
		public void SetTable(string tableName)
		{
			this.defaultTable = tableName;
		}
        /// <summary>
        /// Получает имя таблицы у i-ой колонки, если имя не установлено, то ставится имя таблицы по умолчанию
        /// </summary>
        /// <param name="i">Номер колонки</param>
        /// <returns></returns>
		public string Table(int i)
		{
			if (i >= l.Count)
				throw new IndexOutOfRangeException("Не существует элемента под номером: " + i.ToString());
			if ((string)((Data)(l[i])).tableName != null && (string)((Data)(l[i])).tableName != "")
			{
				return (string)((Data)(l[i])).tableName;
			}else
				return defaultTable;
		}
        /// <summary>
        /// Устанавливает в качестве выборки SQL-запрос по номеру колонки
        /// </summary>
        /// <param name="st">SQL-запрос</param>
        /// <param name="i">Номер колонки</param>
		public void Set(SQLStruct st, int i)
		{
			if (i >= l.Count)
				throw new IndexOutOfRangeException("Не существует элемента под номером: " + i.ToString());
			l[i] = new Data(st);
		}
        /// <summary>
        /// Устанавливает имя колонки и имя таблицы у i-ой колонки
        /// </summary>
        /// <param name="table">Имя таблицы</param>
        /// <param name="column">Имя колонки</param>
        /// <param name="i">Номер колонки в select</param>
		public void Set(string table, string column, int i)
		{
			if (i >= l.Count)
				throw new IndexOutOfRangeException("Не существует элемента под номером: " + i.ToString());
			l[i] = new Data(table, column);
		}
        /// <summary>
        /// Уставливает что колонка испозьется у детей
        /// </summary>
        /// <param name="i">Номер колонки</param>
		public void SetUsedInChild(int i)
		{
			Data d = ((Data)l[i]);
			d.isUseInChild = true;
			
		}
        /// <summary>
        /// Получает, используется ли колонка у детей
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
		public bool IsUsedInChild(int i)
		{
			Data d = ((Data)l[i]);
			return d.isUseInChild;
		}

        /// <summary>
        /// Устанавливает тип колонки
        /// </summary>
        /// <param name="type">тип колонки</param>
        /// <param name="i">Номер колонки</param>
		public void SetType(ElementType type, int i)
		{
			if (i >= l.Count)
				throw new IndexOutOfRangeException("Не существует элемента под номером: " + i.ToString());
			Data d = ((Data)l[i]);
			d.type = type;
		}
        /// <summary>
        /// Получает тип колонки
        /// </summary>
        /// <param name="i">Номер колонки</param>
        /// <returns>Тип колонки</returns>
		public ElementType GetType(int i)
		{
			if (i >= l.Count)
				throw new IndexOutOfRangeException("Не существует элемента под номером: " + i.ToString());
			return ((Data)l[i]).type;
		}
        /// <summary>
        /// Кол-во колонок
        /// </summary>
		public int Count
		{
			get
			{
				return l.Count;
			}
		}
        /// <summary>
        /// Имя i-ой колонки
        /// </summary>
        /// <param name="i">Номер колонки</param>
        /// <returns>Имя</returns>
		public string this[int i]
		{
			get
			{
				if (i >= l.Count)
					throw new IndexOutOfRangeException("Не существует элемента под номером: " + i.ToString());
				if (((Data)(l[i])).typeobj != typeObj.STRING)
				{
					return null;
				}
				return (string)((Data)(l[i])).select;
			}
			set
			{
				if (i >= l.Count)
					throw new IndexOutOfRangeException("Не существует элемента под номером: " + i.ToString());
				l[i] = new Data((string)value);
			}
		}
        /// <summary>
        /// Проверят, i-ая колонка является строкой или SQL-запросом
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
		public bool isString(int i)
		{
			if (i >= l.Count)
				throw new IndexOutOfRangeException("Не существует элемента под номером: " + i.ToString());
			
			if (((Data)(l[i])).typeobj == typeObj.STRING)
				return true;
			else
				return false;

		}
        /// <summary>
        /// Получает SQL_запрос из i-ой колонки
        /// </summary>
        /// <param name="i">Номер колонки</param>
        /// <returns></returns>
		public SQLStruct GetSQL(int i)
		{
			if (i >= l.Count)
				throw new IndexOutOfRangeException("Не существует элемента под номером: " + i.ToString());

			if (((Data)(l[i])).typeobj == typeObj.SQL)
			{
				return (SQLStruct) ((Data) l[i]).select;
			}
			return null;

		}
        /// <summary>
        /// Смотрит, существет ли колонка с заданным именем
        /// </summary>
        /// <param name="node">Имя колонки</param>
        /// <returns></returns>
		public bool isExist(string node)
		{
			for (int i=0; i<l.Count; i++)
				if (isString(i))
                    if (this[i] == node)
						return true;
			return false;
		}

        /// <summary>
        /// Проверяет правильность составленого SELECT по схеме исходной БД
        /// </summary>
        /// <param name="ds">Исходный DataSet</param>
        /// <param name="curTable">Имя текущей таблицы</param>
		public void Validate(DataSet ds, string curTable)
		{
			DataTable table = ds.Tables[curTable];
			if (table == null)
				throw new NotExistException("Не найдено таблицы в DataSet-e: " + curTable);
			for (int i=0; i<l.Count; i++)
			{
				if (isString(i))
				{
					if (this[i] == "*")
					{
						l.RemoveAt(i);
						for (int j=0; j<table.Columns.Count;j++)
						{
							//l.Add( new Data());
							this.Add(curTable, table.Columns[j].ColumnName.ToString());
						}
						continue;
					}
					
					if (table.Columns[this[i]] == null)
						throw new NotExistException("Не найдено колонки в DataSet-e: "+ curTable+"."+ this[i]);
					//Мы добавлем типы данных в Select
					if (table.Columns[this[i]].DataType == typeof(String))
						((Data)l[i]).type = ElementType.STRING;
					else if (table.Columns[this[i]].DataType == typeof(Decimal))
						((Data)l[i]).type = ElementType.DECIMAL;
					else if (table.Columns[this[i]].DataType == typeof(DateTime))
						((Data)l[i]).type = ElementType.DATE;
					//
					//}
				}
				else
				{
					GetSQL(i).Validate(ds);
				}
			}
		}
        /// <summary>
        /// Проверяет правильность составленого SELECT по схеме исходной БД
        /// </summary>
        /// <param name="ds">Схема исходной БД</param>
        /// <param name="from">Оператор FROM</param>
		public void Validate(DataSet ds, FromClass from)
		{
			//если таблица одна, то мы ее ставим по умолчанию
			if (from.Count == 1)
			{
				this.defaultTable = from[0];
			}
			if (paint.Count >0 )
			{
				this.paint.Validate(ds, this.defaultTable);
			}
			
			for (int i=0; i<l.Count; i++)
			{
				if (isString(i))
				{
					string curTable = this.Table(i);
					DataTable table = ds.Tables[curTable];
					if (table == null)
					{
						string al = this.baseSql.alias.GetTableAlias(curTable);
						if (al == null)
							throw new NotExistException("Не найдено таблицы в DataSet-e: " + curTable);
						if (ds.Tables[al]!=null)
							table = ds.Tables[al];
						else
							throw new NotExistException("Не найдено таблицы в DataSet-e: " + curTable);
					}

					if (this[i] == "*")
					{
						l.RemoveAt(i);
						i=-1;
						for (int j=0; j<table.Columns.Count;j++)
						{
							//l.Add( new Data(curTable, table.Columns[j].ColumnName.ToString()));
							//if (!isExist(table.Columns[j].ColumnName.ToString()))
							//{
								this.Add(curTable, table.Columns[j].ColumnName.ToString());
							//}
							
						}
						continue;
					}
                    
					
					string ColName = this[i];
					if (table.Columns[this[i]] == null)
					{
						//проверяем наличие колонки в Алиасе
						if (this.baseSql.alias.GetColumnAlias(this[i]) == null)
							throw new NotExistException("Не найдено колонки в DataSet-e: "+ curTable+"."+ this[i]);
						else
							ColName = this.baseSql.alias.GetColumnAlias(this[i]).name;
					}
					//Мы добавлем типы данных в Select
					if (table.Columns[ColName].DataType == typeof(String))
						((Data)l[i]).type = ElementType.STRING;
					else if (table.Columns[ColName].DataType == typeof(Decimal))
						((Data)l[i]).type = ElementType.DECIMAL;
					else if (table.Columns[ColName].DataType == typeof(DateTime))
						((Data)l[i]).type = ElementType.DATE;
					//
					//}
				}
				else
				{
					GetSQL(i).Validate(ds);
				}
			}
		}
        /// <summary>
        /// Перенумеровывает SQL-запросы
        /// </summary>
        /// <param name="idSQL"></param>
        /// <returns></returns>
		public int ReCount(int idSQL)
		{
			int id = idSQL;
			for (int i=0; i<l.Count; i++)
			{
				if (!isString(i))
				{
					id = GetSQL(i).ReCount(id);
				}
			}
			return id;
		}
        /// <summary>
        /// Упрощает схему SQL-запрсоа. Проверяет колонок и таблиц на существование в DataSet-е
        /// </summary>
        /// <param name="ds">Схема датасета</param>
        /// <param name="from">From-класс</param>
		public void Simplify(DataSet ds, FromClass from)
		{
			for (int i=0; i<l.Count; i++)
			{
				if (isString(i))
				{
					string curTable = this.Table(i);
					DataTable table = ds.Tables[curTable];
					if (table == null)
					{
						//Должны посмареть Алиас
						string al = this.baseSql.alias.GetTableAlias(curTable);
						if (al == null)
							throw new NotExistException("Не найдено таблицы в DataSet-e: " + curTable);
						if (ds.Tables[al]!=null)
							table = ds.Tables[al];
						else
							throw new NotExistException("Не найдено таблицы в DataSet-e: " + curTable);
					}

					if (this[i] == "*")
					{
						l.RemoveAt(i);
						i--;
						for (int j=0; j<table.Columns.Count;j++)
							this.Add(curTable, table.Columns[j].ColumnName.ToString());
						continue;
					}
				}
				else
				{
					GetSQL(i).Siplify(ds);
				}
			}
		}
	};
	/// <summary>
	/// Класс который хранит FROM из SQL запроса
	/// </summary>
	public class FromClass
	{
        /// <summary>
        /// Базовый класс
        /// </summary>
		public SQLStruct baseSql;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="baseSql">Схема sql-запроса в котором находится текущий from</param>
		public FromClass(SQLStruct baseSql)
		{
			this.baseSql = baseSql;
		}
	
		private ArrayList l = new ArrayList();
		private ArrayList where = new ArrayList();
        /// <summary>
        /// Добавляет таблицу
        /// </summary>
        /// <param name="name"></param>
		public void Add(string name)
		{
			l.Add(name.ToLower());
		}
        /// <summary>
        /// Добавляем where к JOIN
        /// </summary>
        /// <param name="where">Класс-ограничение</param>
		public void AddWhere(WhereClass where)
		{
			//this.where.Insert(0, where);
			this.where.Add(where);
		}
        /// <summary>
        /// Получает Where в i-ом декартовом произведении
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
		public WhereClass GetWhere(int i)
		{
			if (i >= where.Count)
				throw new IndexOutOfRangeException("Не существует элемента под номером: " + i.ToString());
            return (WhereClass) where[i];
		}
        /// <summary>
        /// получает i-. таблицу
        /// </summary>
        /// <param name="i">Номер таблицы</param>
        /// <returns></returns>
		public string this[int i]
		{
			get
			{
				if (i >= l.Count)
					throw new IndexOutOfRangeException("Не существует элемента под номером: " + i.ToString());
				return (string)l[i];
			}
		}
        /// <summary>
        /// Колв-во таблиц
        /// </summary>
		public int Count
		{
			get
			{
				return l.Count;
			}
		}
        /// <summary>
        /// Проверяет таблицы по датасету
        /// </summary>
        /// <param name="ds">Датасет</param>
		public void Validate(DataSet ds)
		{
			for (int i=0;i<l.Count;i++)
			{
				if (ds.Tables[this[i]] == null)
				{
					//Сморим в Alias-ах
					if (this.baseSql.alias.GetTableAlias(this[i])== null)
						throw new NotExistException("Не найдено таблицы в DataSet-e: " + this[i]);
				}
			}
		}
        /// <summary>
        /// Пересчитывает SQL запросы
        /// </summary>
        /// <param name="idSQL"></param>
        /// <returns></returns>
		public int ReCount(int idSQL)
		{
			return idSQL;
		}
        /// <summary>
        /// Переименовывает таблицу
        /// </summary>
        /// <param name="name">Новое имя таблицы</param>
		public void ReNameTable(string name)
		{
			//переимновываем последнюю таблицу.
			if (l.Count == 0)
				throw new Exception("Ошибка, нет таблиц для переименования (Alias).");
			l[l.Count-1] = name;
		}
	};
    /// <summary>
    /// Абстрактный класс
    /// </summary>
	public abstract class BaseClass
	{
		public abstract ClassType type();
	};
	/// <summary>
	/// Класс может хранить в себе условие AND, OR
	/// </summary>
	public class Oper : BaseClass
	{
		private Uslovie op;
        /// <summary>
        /// Получает тип
        /// </summary>
        /// <returns></returns>
		public override ClassType type()
		{
			return ClassType.OPER;
		}
        /// <summary>
        /// Получает тип операции
        /// </summary>
		public Uslovie oper
		{
			get
			{
				return op;
			}
			set
			{
				op = value;
			}
			
		}
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="oper"></param>
		public Oper(Uslovie oper)
		{
			this.op = oper;
		}
		public Oper(){}
        /// <summary>
        /// К стрингу
        /// </summary>
        /// <returns></returns>
		public override string ToString()
		{
			if (op == Uslovie.AND)
			{
				return " and ";
			}
			else if (op == Uslovie.OR)
			{
				return " or ";
			}

			return op.ToString();
		}

	};
	/// <summary>
	/// Класс, описывающий выражение в Where, например, table1.col1 = table2.col2;
	/// </summary>
	public class Expr : BaseClass
	{
		private string ColumnName1;
		private string ColumnName2;
		private string TableName1;
		private string TableName2;

		private TypeOper op;

		private ColumnType type1;
		private ColumnType type2;

		public override ClassType type()
		{
			return ClassType.EXPR;
		}
		public Expr() {}
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="table1">таблица 1</param>
        /// <param name="column1">колонка 1</param>
        /// <param name="table2">Таблица 2</param>
        /// <param name="column2">колонка 2</param>
		public Expr(string table1, string column1, string table2, string column2)
		{
			this.ColumnName1 = column1;
			this.TableName1 = table1;
			this.ColumnName2 = column2;
			this.TableName2 = table2;
		}
		
        /// <summary>
        /// Тип операции
        /// </summary>
		public TypeOper oper
		{
			set
			{
				this.op = value;
			}
			get{ return op;}
		}
        /// <summary>
        /// Тип колонки 1
        /// </summary>
		public ColumnType Type1
		{
			set
			{
				type1 = value;
			}
			get
			{
				return type1;
			}
		}
        /// <summary>
        /// Тип колонки 2
        /// </summary>
		public ColumnType Type2
		{
			set
			{
				type2 = value;
			}
			get
			{
				return type2;
			}
		}
        /// <summary>
        /// Имя колонки 1
        /// </summary>
		public string ColName1
		{
			set
			{
				this.ColumnName1 = value;
			}
			get{return ColumnName1;}
		}
        /// <summary>
        /// Имя колонки 2
        /// </summary>
		public string ColName2
		{
			set
			{
				this.ColumnName2 = value;
			}
			get{return ColumnName2;}
		}
        /// <summary>
        /// Имя таблицы 1
        /// </summary>
		public string TblName1
		{
			set
			{
				this.TableName1 = value;
			}
			get
			{
				return TableName1;
			}
		}
        /// <summary>
        /// Имя таблицы 2
        /// </summary>
		public string TblName2
		{
			set
			{
				this.TableName2 = value;
			}
			get
			{
				return TableName2;
			}
		}

        /// <summary>
        /// К стрингу
        /// </summary>
        /// <param name="curenttable">Имя текущей таблицы</param>
        /// <param name="sql">Родительский запрос</param>
        /// <returns></returns>
		public  string ToString(string curenttable, SQLStruct sql)
		{
			//Aias
			string AliasTable1 = TableName1;
			/*if (sql.alias.GetTableAlias(TableName1)!=null)
				AliasTable1 = sql.alias.GetTableAlias(TableName1);*/

			string AliasTable2 = TableName2;
			/*if (sql.alias.GetTableAlias(TableName2)!=null)
				AliasTable2 = sql.alias.GetTableAlias(TableName2);*/
			
			string str = "";
			if (AliasTable1 != null )
			{
				if (curenttable != AliasTable1)
					str += "$var_"+ AliasTable1 + "/";
				
			}
			if (ColumnName1!= null)
				if (type1 == ColumnType.NAME)
					str += ColumnName1;
				else
					str += "'" + ColumnName1 + "'";

			if (op == TypeOper.RAVNO)
			{
				str+="=";
			}
			//str += " " + this.op + " ";

			if (AliasTable2 != null )
			{
				if (curenttable != AliasTable2)
					str += "$var_"+ AliasTable2 + "/";
				
			}
			if (ColumnName2!= null)
				if (type2 == ColumnType.NAME)
					str += ColumnName2;
				else
					str += "'" + ColumnName2 + "'";
			
			return str;
		}

	
        /// <summary>
        /// Проверяет по дата сету на существование колонок и таблицы
        /// </summary>
        /// <param name="ds">Датасет</param>
        /// <param name="curTable">Имя текущей таблицы</param>
		public void Validate(DataSet ds, string curTable)
		{
			if (type1 == ColumnType.NAME)
			{
			
				if (TableName1 != null)
				{
					DataTable table = ds.Tables[TableName1];
					if (table == null)
						throw new NotExistException("Не найдено таблицы в DataSet-e: " + TableName1);
					if (ColumnName1!= null)
					{
						if (table.Columns[ColumnName1] == null)
							throw new NotExistException("Не найдено колонки в DataSet-e: " + TableName1+ "." + ColumnName1);
					}
				}
				else
				{
					DataTable table =  ds.Tables[curTable];
					if (table == null)
						throw new NotExistException("Не найдено таблицы в DataSet-e: " + TableName1);
					if (ColumnName1!= null)
					{
						if (table.Columns[ColumnName1] == null)
							throw new NotExistException("Не найдено колонки в DataSet-e: " + TableName1+ "."+ ColumnName1);
					}                
				}
			}

			if (type2 == ColumnType.NAME)
			{

			
				if (TableName2 != null)
				{
					DataTable table = ds.Tables[TableName2];
					if (table == null)
						throw new NotExistException("Не найдено таблицы в DataSet-e: " + TableName2);
					if (ColumnName2!= null)
					{
						if (table.Columns[ColumnName2] == null)
							throw new NotExistException("Не найдено колонки в DataSet-e: " + TableName2+ "." + ColumnName2);
					}
				}
				else
				{
					DataTable table = ds.Tables[curTable];
					if (table == null)
						throw new NotExistException("Не найдено таблицы в DataSet-e: " + TableName2);
					if (ColumnName2!= null)
					{
						if (table.Columns[ColumnName2] == null)
							throw new NotExistException("Не найдено колонки в DataSet-e: " + TableName2+"." + ColumnName2);
					}
				}

				if (TableName1 != null)
				{
					if (ds.Tables[TableName1] == null)
						throw new NotExistException("Не найдено таблицы в DataSet-e: " + TableName1);
				}
			}
		}
	};
	/// <summary>
	/// Класс который хранит Where из SQL запроса
	/// </summary>
	public class WhereClass
	{
        /// <summary>
        /// Родительский SQL
        /// </summary>
		public SQLStruct baseSql;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="baseSql">Родительский SQL</param>
		public WhereClass(SQLStruct baseSql)
		{
			this.baseSql = baseSql;
		}

		private ArrayList list = new ArrayList();
        /// <summary>
        /// Добавляет ограничение
        /// </summary>
        /// <param name="ex">Ограничение</param>
		public void Add(Expr ex)
		{
			list.Add(ex);
		}
        /// <summary>
        /// Добавлет ограничение и условие соединения ограничений
        /// </summary>
        /// <param name="ex">Ограничение</param>
        /// <param name="op">AND/OR</param>
		public void Add(Expr ex, Oper op)
		{
			list.Add(ex);
			list.Add(op);			
		}
        /// <summary>
        /// Добавляет чере OR ограничение
        /// </summary>
        /// <param name="ex">Ограничение</param>
		public void AddOr(Expr ex)
		{
			list.Add(ex);
			list.Add(new Oper(Uslovie.OR));
			
		}
        /// <summary>
        /// Добавляет чере AND ограничение
        /// </summary>
        /// <param name="ex">Ограничение</param>
		public void AddAnd(Expr ex)
		{
			list.Add(ex);
			list.Add(new Oper(Uslovie.AND));
			
		}
        /// <summary>
        /// Не используется
        /// </summary>
        /// <returns>Всегда true</returns>
		public bool isOnlyIf()
		{
			return true;

			/*bool Ok = true;
			for (int i=0;i<list.Count;i++)
			{
				if (this[i].type() == ClassType.EXPR)
				{
					Expr ex = ((Expr)this[i]);
					if ((ex.Type1 == ColumnType.NAME && ex.Type2 == ColumnType.NAME))
					{
						Ok = false;
					}
				}
			}
			return Ok;*/
		}
        /// <summary>
        /// Ограничение по номеру
        /// </summary>
        /// <param name="i">Номер ограничения</param>
        /// <returns></returns>

		public BaseClass this [int i]
		{
			get
			{
				if (i >= list.Count )
					throw new IndexOutOfRangeException("Не существует элемент с номером: " + i.ToString());
				return (BaseClass)this.list[i];
			}
		}

        /// <summary>
        /// Кол-во ограничений
        /// </summary>
		public int Count
		{
			get
			{
				return list.Count;
			}
		}
        /// <summary>
        /// К стрингу
        /// </summary>
        /// <param name="CurTable">Имя текущей таблицы</param>
        /// <returns></returns>
		public string ToString(string CurTable)
		{
			string str="";
			for (int i=0;i<list.Count;i++)
			{
				if (this[i].type() == ClassType.EXPR)
				{
					Expr ex = ((Expr)this[i]);
					str += ex.ToString(CurTable, this.baseSql);
				}
				else if (this[i].type() == ClassType.OPER)
				{
					Oper o = (Oper)this[i];
					str += o.ToString();
				}

			}
			return str;
		}
        /// <summary>
        /// Проверяет проавильность по dataSet-у
        /// </summary>
        /// <param name="ds">dataSet</param>
        /// <param name="curTable">имя текущей таблицы</param>
		public void Validate(DataSet ds, string curTable)
		{
			for (int i=0;i<list.Count;i++)
			{
				if (this[i].type() == ClassType.EXPR)
				{
					((Expr)this[i]).Validate(ds, curTable);
					
				}                
			}
		}
        /// <summary>
        /// Не используется. возвращает idSql
        /// </summary>
        /// <param name="idSQL"></param>
        /// <returns></returns>
		public int ReCount(int idSQL)
		{
			return idSQL;
		}

	};
    /// <summary>
    /// Класс, описывающий одну группировку
    /// </summary>
	public class GroupClassOnce
	{
        /// <summary>
        /// Базовый класс группировки
        /// </summary>
		public GroupClass baseGroup;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="baseGroup">Базовый класс группировки</param>
		public GroupClassOnce(GroupClass baseGroup)
		{
            this.baseGroup = baseGroup;
		}

		private class ValueGroup
		{
			public int idGroup = -1;
			//Имя селекта + имя колонки
			public string FullName;
			private string func;
			public string Func
			{
                get{ return func;}
				set
				{
                    func = colName;
					colName = value;
				}
			}
			private string colName;
			private string tblName;
			public string ColumnName
			{
				get{ return colName; }
				set{
                    colName = value;
				}
			}
			public string TableName
			{
				get{ return tblName; }
				set
				{
					tblName = value;
				}
			}
			public ValueGroup(string ColName)
			{
                colName = ColName;
			}
		};
		/// <summary>
		/// Массив группировок
		/// </summary>
		public ArrayList group = new ArrayList();
        /// <summary>
        /// Флаг ALL
        /// </summary>
		public bool isAll = false;
		private ArrayList arrayH = new ArrayList(1);
		private ArrayList arrayF = new ArrayList(1);
        /// <summary>
        /// тип группировки
        /// </summary>
		public TypeGroup type = TypeGroup.CONTAIN;
        /// <summary>
        /// Тэг для Footer
        /// </summary>
		public string UseTagF;
        /// <summary>
        /// Тэг для HEADER
        /// </summary>
		public string UseTagH;

        /// <summary>
        /// Получает имя тэга по типу (Header, footer)
        /// </summary>
        /// <param name="t">Тип получаемого тэаг</param>
        /// <returns></returns>
		public string UseTag(TypeFH t)
		{
			if (t == TypeFH.FOOTER)
				return UseTagF;
			else 
				//Header и для Contain
				return UseTagH;
		}

        /// <summary>
        /// Кол-во колонок в Header
        /// </summary>
		public int CountValueRecHeader
		{
			get
			{
				return arrayH.Count;
			}
		}
        /// <summary>
        /// Кол-во колонок в Footer
        /// </summary>
		public int CountValueRecFooter
		{
			get
			{
				return arrayF.Count;
			}
		}
        /// <summary>
        /// Получает функцию по номеру колонки и типу группировки
        /// </summary>
        /// <param name="i">Номер группировки</param>
        /// <param name="t">Тип группировки</param>
        /// <returns>Имя функции</returns>
		public string Func(int i, TypeFH t)
		{
			ValueGroup g;
			if (t == TypeFH.HEADER)
				g = (ValueGroup) arrayH[i];
			else
				g = (ValueGroup) arrayF[i];

			return g.Func;
		}
        /// <summary>
        /// Получает номер из <see cref="ValueGroup"/>
        /// </summary>
        /// <param name="i">Номер группировки</param>
        /// <param name="t">Тип группировки</param>
        /// <returns>Номер</returns>
		public int IdGroup(int i, TypeFH t)
		{
			ValueGroup g;
			if (t == TypeFH.HEADER)
				g = (ValueGroup) arrayH[i];
			else
				g = (ValueGroup) arrayF[i];

			return g.idGroup;
		}
        /// <summary>
        /// Получает имя колонки по номеру колонки и типу группировки
        /// </summary>
        /// <param name="i">Номер колонки</param>
        /// <param name="t">Тип круппировки</param>
        /// <returns>Имя колонки</returns>
		public string ValueRec(int i, TypeFH t)
		{
			ValueGroup g;
			if (t == TypeFH.HEADER)
				g = (ValueGroup) arrayH[i];
			else
				g = (ValueGroup) arrayF[i];

			return g.ColumnName;
		}
        /// <summary>
        /// Получает полное имя с таблице <see cref="ValueGroup"/>
        /// </summary>
        /// <param name="i">Имя группировки</param>
        /// <param name="t">Тип группировки</param>
        /// <returns></returns>
		public string FullName(int i, TypeFH t)
		{
			//ValueGroup g = (ValueGroup) array[i];
			ValueGroup g;
			if (t == TypeFH.HEADER)
				g = (ValueGroup) arrayH[i];
			else
				g = (ValueGroup) arrayF[i];

			return g.FullName;
		}
        /// <summary>
        /// Добавлет колонку в Contain
        /// </summary>
        /// <param name="value">Имя колонки</param>
		public void AddContain(string value)
		{
            type = TypeGroup.CONTAIN;
			if (arrayH == null)
			{
				arrayH = new ArrayList();
			}
            arrayH.Add(new ValueGroup(value));
		}
        /// <summary>
        /// Добавляет имя колонки
        /// </summary>
        /// <param name="value">Имя колонки</param>
		public void AddColumnName(string value)
		{
			if (type == TypeGroup.HEADER || type == TypeGroup.CONTAIN)
			{
				ValueGroup g = (ValueGroup)arrayH[arrayH.Count-1];
				g.ColumnName = value;
				g.TableName = g.ColumnName;
			}else if (type == TypeGroup.FOOTER || type == TypeGroup.HEADERFOOTER)
			{
				ValueGroup g = (ValueGroup)this.arrayF[arrayF.Count-1];
				g.ColumnName = value;
				g.TableName = g.ColumnName;
			}
		}
        /// <summary>
        /// Добавляет колонку в Header
        /// </summary>
        /// <param name="value">Имя колонки</param>
		public void AddHeader(string value)
		{
            type = TypeGroup.HEADER;
			if (arrayH == null)
			{
				arrayH = new ArrayList();
			}
			arrayH.Add(new ValueGroup(value));
		}
        /// <summary>
        /// Добавляет агрегат
        /// </summary>
        /// <param name="value">Имя функции</param>
        /// <param name="t">Тип группировки</param>
		public void AddFunc(string value, TypeFH t)
		{
			ValueGroup g;
			if (t == TypeFH.HEADER)
				g = (ValueGroup) arrayH[arrayH.Count - 1];
			else
				g = (ValueGroup) arrayF[arrayF.Count - 1];
            //ValueGroup g = (ValueGroup) array[array.Count-1];
            g.Func = value;
		}
        /// <summary>
        /// Добавляет колонку в Footer
        /// </summary>
        /// <param name="value">имя колонки</param>
		public void AddFooter(string value)
		{
			
			if (type == TypeGroup.HEADER)
			{
				type = TypeGroup.HEADERFOOTER;
			}else if (type != TypeGroup.HEADERFOOTER)
				type = TypeGroup.FOOTER;


			if (arrayF == null)
			{
				arrayF = new ArrayList();
			}
			arrayF.Add(new ValueGroup(value));
		}
        /// <summary>
        /// Получает группировку по номеру
        /// </summary>
        /// <param name="i">Номер группировки</param>
        /// <returns></returns>
		public string this[int i]
		{
			get
			{
				return (string) group[i];
			}
		}
        /// <summary>
        /// Кол-во группировок
        /// </summary>
		public int Count
		{
			get{return group.Count;}
		}
        /// <summary>
        /// Всегда true
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
		public bool Validate(DataSet ds)
		{
			return true;
		}
        /// <summary>
        /// всегда isSQL
        /// </summary>
        /// <param name="idSQL"></param>
        /// <returns></returns>
		public int ReCount(int idSQL)
		{
			return idSQL;
		}
        /// <summary>
        /// полное имя
        /// </summary>
        /// <param name="st">Схема SQL</param>
        /// <param name="ColumnName">Имя колонки</param>
        /// <returns>Полное имя</returns>
		public string GetFullName2(SQLStruct st, string ColumnName)
		{
			for (int i=0;i<st.select.Count;i++)
			{
				if (st.select.isString(i))
				{
					//Если мы нашли совпадение, то...
					if (st.select[i] == ColumnName)
					{
						st.select.SetUsedInChild(i);
						return st.GetSQLUniqueName() + "_select_" + ColumnName;
					}
				}
			}
			//После чего должны обратиться к предку.
			if (st.parentSQL != null)
			{
				return GetFullName(st.parentSQL, ColumnName);
			}
			return null;
		}
        /// <summary>
        /// полное имя
        /// </summary>
        /// <param name="st">Схема SQL</param>
        /// <param name="ColumnName">Имя колонки</param>
        /// <returns>Полное имя</returns>
        public string GetFullName(SQLStruct st, string ColumnName)
		{
            for (int i=0;i<st.select.Count;i++)
            {
				if (st.select.isString(i))
				{
					//Если мы нашли совпадение, то...
					if (st.select[i] == ColumnName)
					{
						st.select.SetUsedInChild(i);
                        return st.GetSQLUniqueName() + "_select_" + ColumnName;
					}
				}
            }
			//После чего должны обратиться к предку.
			if (st.parentSQL != null)
			{
				return GetFullName(st.parentSQL, ColumnName);
			}
            return null;
		}
        /// <summary>
        /// Упрощает группировку и проверят существование колонок в SELECT и пронумеровывает группировки
        /// </summary>
        /// <param name="select">Массив колонок</param>
        /// <param name="array">Массив ValueGroup</param>
        /// <returns></returns>
		public bool Siplify(SelectClass select, ArrayList array)
		{
			for (int i=0; i<array.Count; i++)
			{
				ValueGroup g = (ValueGroup) array[i];
				//g.ColumnName = 
				//Смотрим откуда эта колонка.
				bool Ok = false;
				for (int j=0;j<select.Count;j++)
				{
					if (select.isString(j))
					{
						//проверям имя на совпадение
						if (select[j] == g.ColumnName)
						{
							Ok = true;
							//Далее проверяем есть ли в текущем group
							bool ok2= false;
							for (int k=0;k<this.Count;k++)
							{
								if (g.ColumnName == this[k])
									ok2 = true;
							}
							if (!ok2 && (g.Func == null || g.Func == ""))
							{
								//Значит в текущем гроупе этого не найдено,надо искать в предыдущих.
								int idGroup = baseGroup.GetIdGroup(g.ColumnName);
								if (idGroup == -1)
								{
									throw new Exception("Ошибка, в синтаксисе, не найдено колонки:" + g.ColumnName);
								}
								g.idGroup = idGroup;
							}
						}
					}
				}
				if (!Ok)
				{
					//Значит не совпало, т.е. это имя не из этого Select
					//Проверяем из какого select-а она
					//Пробегаем все родительские :)
					string fullname = GetFullName(this.baseGroup.baseSql.parentSQL, g.ColumnName);
					if (fullname == null)
					{
						throw new NotExistException("GroupClassOne, GetFullName: Ошибка в синтаксисе - не найдено колонки '" + g.ColumnName+ "'");
					}
					//g.ColumnName = fullname;
					g.FullName = fullname;
				}
				
			}
			return true;
		}

        /// <summary>
        /// упрощает по select
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        public bool Siplify(SelectClass select)
		{
			//нам надо расставаить ссылки на то если у теги в group не из текущего SQL
			Siplify(select, this.arrayH);
			Siplify(select, this.arrayF);

            

			//Заменяем олл в Group By
            /*if (isAll)
            {
				group.RemoveRange(0,group.Count);
                for (int i=0;i<select.Count;i++)
                {
					if (select.isString(i))
					{
						group.Add(select[i]);
					}
                }
            }*/
			//Заменяем олл в Contain/Header/Footer
			return true;
		}
	};

    /// <summary>
    /// Класс группировки
    /// </summary>
	public class GroupClass
	{
        /// <summary>
        /// Получает номер группировки по имени
        /// </summary>
        /// <param name="columnName">Имя колонки</param>
        /// <returns>Номер группировки</returns>
		public int GetIdGroup(string columnName)
		{
			//Пробегаем все гроупы
            for (int i=0;i<this.Count;i++)
            {
                //пробегаем все колонки в текущем гроупе
				for (int j=0;j<this[i].Count;j++)
				{
					if (this[i].group[j].ToString() == columnName)
					{
						return i;
					}
				}
            }
			return -1;
		}
		public SQLStruct baseSql;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="baseSql">Базовый SQL-класс</param>
		public GroupClass(SQLStruct baseSql)
		{
			this.baseSql = baseSql;
			once = new GroupClassOnce(this);
		}
		private ArrayList grouplist = new ArrayList();
        /// <summary>
        /// Текущая группировка
        /// </summary>
		public GroupClassOnce once;
        /// <summary>
        /// Добавляет новую группировку
        /// </summary>
		public void AddGroup()
		{
			once = new GroupClassOnce(this);
            grouplist.Add(once);
		}
        /// <summary>
        /// Кол-во группировок
        /// </summary>
		public int Count
		{
			get
			{
				return grouplist.Count;
			}
		}
        /// <summary>
        /// Получение группировки по номеру
        /// </summary>
        /// <param name="i">Номер группировки</param>
        /// <returns></returns>
		public GroupClassOnce this [int i]
		{
			get
			{
				return (GroupClassOnce)grouplist[i];
			}
		}
        /// <summary>
        /// Проверяет группировку
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <returns>Успешность проверки</returns>
		public bool Validate(DataSet ds)
		{
			foreach (GroupClassOnce group in grouplist)
			{
				group.Validate(ds);
			}
			return true;
		}
        /// <summary>
        /// Пересчитывает SQL
        /// </summary>
        /// <param name="idSQL"></param>
        /// <returns></returns>
		public int ReCount(int idSQL)
		{
			int id = idSQL;
			for (int i=0;i< grouplist.Count;i++)
			{
				GroupClassOnce tmp = (GroupClassOnce)grouplist[i];
				id = tmp.ReCount(id);
			}
            return id;
		}
        /// <summary>
        /// упрощает группировки
        /// </summary>
        /// <param name="select">SELECt-класс</param>
        /// <returns></returns>
		public bool Simplify(SelectClass select)
		{
			//Проверяем наличие всех таблиц...
            foreach (GroupClassOnce group in grouplist)
            {
				//GroupClassOnce.Validate(ds, select);
				group.Siplify(this.baseSql.select);
            }
			return true;
		}
	};

    /// <summary>
    /// AS- класс
    /// </summary>
	public class AsClass
	{
		public SQLStruct baseSql;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="baseSql">Базовый SQL</param>
		public AsClass(SQLStruct baseSql)
		{
			this.baseSql = baseSql;
		}

		private ArrayList l = new ArrayList();
        /// <summary>
        /// Добавляет к AS
        /// </summary>
        /// <param name="name">Имя нового AS</param>
		public void Add(string name)
		{
			l.Add(name);
		}
        /// <summary>
        /// Получение AS по номеру
        /// </summary>
        /// <param name="i">Номер</param>
        /// <returns></returns>
		public string this[int i]
		{
			get
			{
				if (i >= l.Count)
					throw new IndexOutOfRangeException("Не существует элемента под номером: " + i.ToString());
				return (string)l[i];
			}
		}        
	};
	/// <summary>
	/// Класс, хранящий в себе распарсенный SQL запрос
	/// </summary>
	public class SQLStruct
	{
		public Alias alias = new Alias();
		//Возвращает имя TR 
		public string GetTr(int idgroup)
		{
            if (idgroup == -1)
            {
				if (this.select.selectType == SELECTTYPE.ALL)
				{
					if (this.select.UseTag == null)
					{
                        return "tr-" + this.As[0];
					}else
						return this.select.UseTag;
				}else if (this.select.selectType == SELECTTYPE.ONCE)
				{
                    return null;
				}else if (this.As[0] != null)
				{
					return "tr-" + this.As[0];
				}
            }else if (idgroup >= 0)
			{
                if (this.group.Count < idgroup)
					throw new Exception("SQLStruct:GetTr. Номер группы больше чем кол-во групп");
				//Получаем гроуп по имени.
                GroupClassOnce gr = group[idgroup];
                if (gr.type == TypeGroup.CONTAIN)
                {
                    //string val = GetTr(-1);
					return gr.UseTag(TypeFH.CONTAIN);
                }else if (gr.type == TypeGroup.FOOTER || gr.type == TypeGroup.HEADER)
				{
                    return null;
				}
			}
			return null;
		}

		//Уникальный номер SQL
		public int idSQL = 0;
        /// <summary>
        /// Ссылка на первый SQL
        /// </summary>
		public SQLStruct baseSQL = null;
        /// <summary>
        /// Ссылка на родителя
        /// </summary>
		public SQLStruct parentSQL = null;

        /// <summary>
        /// Перенумеровывает SQL
        /// </summary>
        /// <param name="idSQL">Начальный номер</param>
        /// <returns>Последний номер + 1</returns>
		public int ReCount(int idSQL)
		{
			//Присваиваем номер для текущего SQL
			this.idSQL = idSQL;
			int id = idSQL + 1;

			//по очереди меняем номера вложенных SQL
			id = select.ReCount(id);
			id = this.from.ReCount(id);
			id = this.group.ReCount(id);
			return id;
		}
        /// <summary>
        /// Конструктор
        /// </summary>
		public SQLStruct()
		{
			select = new SelectClass(this);
			from = new FromClass(this);
			where = new WhereClass(this);
			As = new AsClass(this);
			group = new GroupClass(this);
		}
        /// <summary>
        /// Получает уникальное имя SQL: var_sql_ID
        /// </summary>
        /// <returns></returns>
		public string GetSQLUniqueName()
		{
			return "var_sql_" + idSQL.ToString();
		}
        /// <summary>
        /// Ссылка на SELECT-класс
        /// </summary>
		public SelectClass select;
        /// <summary>
        /// Ссылка на FROM-класс
        /// </summary>
		public FromClass from;
        /// <summary>
        /// Ссылка на WHERE-класс
        /// </summary>
		public WhereClass where;
        /// <summary>
        /// Ссылка на AS-класс
        /// </summary>
		public AsClass As;
        /// <summary>
        /// Сслыка на GROUP-класс
        /// </summary>
		public GroupClass group;
		private DataSet dataSet;
        /// <summary>
        /// Дебаговская функция
        /// Выводит SQL-запрос
        /// </summary>
		public void Print()
		{
			Print(0);
		}
        /// <summary>
        /// Дебаговская функция
        /// Выводит SQL-запрос
        /// </summary>
        /// <param name="ind">Кол-во пробелов перед запросом (отсутуп)</param>
		public void Print(int ind)
		{
			Console.WriteLine();
			for (int i=0;i<ind*5;i++)
				Console.Write(" ");
			//As s;
			Console.Write("SELECT ");
			for (int i=0;i< this.select.Count;i++)
			{
				if (select.isString(i))
				{
					Console.Write("{0}, ", select[i]);
				}
				else
				{
					select.GetSQL(i).Print(ind+1);
					//Print(ind+1);
				}
			}
			Console.WriteLine();
			for (int i=0;i<ind*5;i++)
				Console.Write(" ");
			Console.Write("FROM ");
			for (int i=0;i<from.Count;i++)
			{
				Console.Write("{0}, ", from[i]);
			}
			Console.WriteLine();
			for (int i=0;i<ind*5;i++)
				Console.Write(" ");
			if (this.where.Count > 0)
			{
				Console.Write("WHERE ");
				for (int i=0;i<where.Count;i++)
				{
					Console.Write("{0}, ", where[i]);
				}
			}
			Console.WriteLine();
			for (int i=0;i<ind*5;i++)
				Console.Write(" ");
			Console.Write("AS ");
			Console.Write("{0}, ", this.As[0]);
			Console.WriteLine();

		}
        /// <summary>
        /// Упрощает SQL-запрос
        /// </summary>
        /// <param name="ds"></param>
		public void Siplify(DataSet ds)
		{
			dataSet = ds;
			select.Simplify(ds, from);
			select.Sort();
			group.Simplify(select);
		}
        /// <summary>
        /// Проверяет SQL-запрос по Dataset-у
        /// </summary>
        /// <param name="ds">Схема БД</param>
		public void Validate(DataSet ds)
		{
			dataSet = ds;
			//Проверяем Select
			select.Validate(ds, from);
			from.Validate(ds);
			where.Validate(ds, from[0]);
			//group.Validate(ds, select);
		}
		/// <summary>
		/// Возвращает типа SQL запроса
		/// </summary>
		/// <returns>Возвращает типа SQL запроса</returns>
		public ReturnSQLType Return()
		{
			if (select.selectType == SELECTTYPE.ONCE)
			{
				return ReturnSQLType.ONCE;
			}
			else if (select.selectType == SELECTTYPE.ALL && select.UseTag != "" && select.UseTag != null)
			{
                return ReturnSQLType.LIST;
			}
			else if (select.KeyCount > 0)
			{
                return ReturnSQLType.ONCE;
			}
			else
				return ReturnSQLType.TABLE;
		}
        /// <summary>
        /// Empty
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
		public string GetTableName(string columnName)
		{
			//Смотрим текущую таблицу...
			/*for (int i=0;i<dataSet.Tables.Count;i++)
			{
				if (dataSet.Tables[i].)
				{
				}
			}*/
			return "";
			
		}
		//public 
	};
}
