using System;
using System.Collections;
using System.Data;

namespace Comp1
{
    /// <summary>
    /// ��� �������
    /// </summary>
	public class ColumnName
	{
		public string name;
		public string table;
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="name">��� �������</param>
        /// <param name="table">��� �������</param>
		public ColumnName(string name, string table)
		{
			this.name = name;
			this.table = table;
		}
	};
    /// <summary>
    /// ��� �����������
    /// </summary>
	public enum TypeGroup {FOOTER, HEADER, CONTAIN, HEADERFOOTER};
    /// <summary>
    /// ��� ����, ������������ �� ����������� ����� ���� footer
    /// </summary>
	public enum TypeFH {FOOTER, HEADER, CONTAIN};
    /// <summary>
    /// ��� �������
    /// </summary>
	public enum SELECTTYPE {ALL, ONCE};
    /// <summary>
    /// �� ��� ������� ����������� �������
    /// </summary>
	public enum ReturnSQLType
    {
        /// <summary>
        /// �������
        /// </summary>
        TABLE, 
        /// <summary>
        /// ���� ������
        /// </summary>
        ONCE, 
        /// <summary>
        /// ������
        /// </summary>
        LIST
    }

	/// <summary>
	/// ����� �������������� ������� � ������
	/// </summary>
	public class Alias
	{
        /// <summary>
        /// �����  �������� 2-� ���� �������
        /// </summary>
		public class TableStore
		{
            /// <summary>
            /// ��������� ���
            /// </summary>
			public string name;
            /// <summary>
            /// ����� ���
            /// </summary>
			public string rename;
            /// <summary>
            /// �����������
            /// </summary>
            /// <param name="name">�������� ���</param>
            /// <param name="rename">����� ���</param>
			public TableStore(string name, string rename)
			{
				this.name = name;
				this.rename = rename;
			}
		};
        /// <summary>
        /// ����� �������� 2-� ���� �������
        /// </summary>
		public class ColumnStore
		{
            /// <summary>
            /// �������� ���
            /// </summary>
			public ColumnName name;
            /// <summary>
            /// ����� ���
            /// </summary>
			public string rename;
            /// <summary>
            /// �����������
            /// </summary>
            /// <param name="name">�������� ���</param>
            /// <param name="rename">����� ���</param>
			public ColumnStore(ColumnName name, string rename)
			{
				this.name = name;
				this.rename = rename;
			}
            /// <summary>
            /// �����������
            /// </summary>
            /// <param name="name">�������� ���</param>
            /// <param name="rename">����� ���</param>
            public ColumnStore(string name, string rename)
			{
				this.name = new ColumnName(name, null);
				this.rename = rename;
			}
		};
       /// <summary>
       /// ������ ������
       /// </summary>
		public ArrayList tables = new ArrayList();
        /// <summary>
        /// ������ �������
        /// </summary>
		public ArrayList columns = new ArrayList();
		public Alias() { }
        /// <summary>
        /// �������� ����� �������������� � �������
        /// </summary>
        /// <param name="name">������ ���</param>
        /// <param name="rename">����� ���</param>
		public void AddTable(string name, string rename)
		{
			tables.Add(new TableStore(name, rename));
		}
        /// <summary>
        /// �������� ����� �������������� � �������
        /// </summary>
        /// <param name="name">������ ���</param>
        /// <param name="rename">����� ���</param>
		public void AddColumn(string name, string rename)
		{
            columns.Add(new ColumnStore(name,rename));
		}
        /// <summary>
        /// �������� ����� �������������� � �������
        /// </summary>
        /// <param name="name">������ ���</param>
        /// <param name="rename">����� ���</param>
		public void AddColumn(ColumnName name, string rename)
		{
			columns.Add(new ColumnStore(name,rename));
		}
        /// <summary>
        /// �������� ��� �������
        /// </summary>
        /// <param name="name">��� �������</param>
        /// <returns>���������������� ���</returns>
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
        /// �������� ��� ���� ��� �������
        /// </summary>
        /// <param name="table">��� �������</param>
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
    /// ����� "���������" Select
    /// </summary>
	public class Paint
	{
		/// <summary>
		/// ������ ����� ���������
		/// </summary>
		private ArrayList list = new ArrayList();
		public string Other;
		public ColumnName By;
		public void Validate(DataSet ds, string defaultTable)
		{
			//return;
			//��������� By.. �� ������ ���� � ��������
			if (By.table == null || By.table == "") 
                By.table = defaultTable;
            DataTable t = ds.Tables[By.table];
			if (t == null)
				throw new NotExistException("�� ������� ������� � DataSet-e: " + By.table);
			//�������� �������...
			if (By.name == null || By.name == "")
				throw new NotExistException("PAINT By: �� ������ ��� �������");
			if (t.Columns[By.name] == null)
				throw new NotExistException("�� ������� ������� � DataSet-e: " + By.table + "." + By.name);
			//��������� Other...
			if (Other == null || Other == "")
				throw new NotExistException("�� ����� ��� OTHER! ");
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
	/// ����� ������� ������ Select �� SQL �������
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
        /// �����������
        /// </summary>
        public void Sort()
		{
			SelectCompareClass c = new SelectCompareClass();
			l.Sort(c);
		}
        /// <summary>
        /// ������� ����� (��������)
        /// </summary>
		public SQLStruct baseSql;
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="baseSql">������� �����</param>
		public SelectClass(SQLStruct baseSql)
		{
			this.baseSql = baseSql;
		}

		private string defaultTable;
		private enum typeObj {STRING, SQL};
		private class Data
		{
			//����, � ��� ������������ �� ��� ������� � �������� ��������, ���� ��, �� �� ���� �������� ����������
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
        /// ��� ������� ALL, ONCE
        /// </summary>
		public SELECTTYPE selectType = SELECTTYPE.ALL;
        /// <summary>
        /// ��� AS
        /// </summary>
		public string UseTag = null;
        /// <summary>
        /// ����� ���������
        /// </summary>
		public Paint paint = new Paint();
        /// <summary>
        /// ��������, �������� �� ������
        /// </summary>
        /// <param name="node">��� �����</param>
        /// <returns>��������� ��� ���</returns>
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
		/// �������� AS-���
		/// </summary>
		/// <param name="str">��� ������ as-����</param>
		public void AddAs(string str)
		{
			if (l.Count ==0)
				throw new Exception("��� ������� ������� ��� �������������");
			Data d = (Data)l[l.Count - 1];
			if (d.typeobj == typeObj.SQL)
				throw new Exception("������ ������������ SQL ������");

			string name =(string) d.select;
			string table =(string) d.tableName;
            this.baseSql.alias.AddColumn(new ColumnName(name,table), str);
			d.select = str;
		}
        /// <summary>
        /// �������� ����
        /// </summary>
        /// <param name="column">��� �������</param>
		public void AddKey(string column)
		{
			key.Add(new Data(column, ElementType.STRING));
		}
        /// <summary>
        /// �������� ����
        /// </summary>
        /// <param name="table">��� �������</param>
        /// <param name="column">��� �������</param>
		public void AddKey(string table, string column)
		{
            key.Add(new Data(table, column, ElementType.STRING));
		}
        /// <summary>
        /// ������������� ���� �������
        /// </summary>
        /// <param name="table">��� �������</param>
		public void SetKeyTable(string table)
		{
			Data d = (Data)(key[key.Count-1]);
			//d.select = 
			d.tableName = (string) d.select;
			d.select = table;
		}

        /// <summary>
        /// �������� ����
        /// </summary>
        /// <param name="i">����� �����</param>
        /// <returns>���</returns>
		public string GetKey(int i)
		{
			if (i >= key.Count)
				throw new IndexOutOfRangeException("�� ���������� �������� ��� �������: " + i.ToString());
            return (string) ((Data)key[i]).select;
		}
        /// <summary>
        /// �������� ���� �������
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
		public string GetKeyTable(int i)
		{
			if (i >= key.Count)
				throw new IndexOutOfRangeException("�� ���������� �������� ��� �������: " + i.ToString());
			if ((string)((Data)(key[i])).tableName != null && (string)((Data)(key[i])).tableName != "")
			{
				return (string)((Data)(key[i])).tableName;
			}
			else
				return defaultTable;

		}
        /// <summary>
        /// ���-�� ������
        /// </summary>
		public int KeyCount
		{
			get
			{
				return key.Count;
			}
		}
        /// <summary>
        /// ��������� �������
        /// </summary>
        /// <param name="name">��� �������</param>
		public void Add(string name)
		{
			if(!isExist(name.ToLower()))
				l.Add(new Data(name.ToLower(),ElementType.STRING));
		}
        /// <summary>
        /// �������� �������
        /// </summary>
        /// <param name="tableName">��� �������</param>
        /// <param name="name">��� �������</param>
		public void Add(string tableName, string name)
		{
			if(!isExist(name.ToLower()))
				l.Add(new Data(tableName.ToLower(), name.ToLower(),ElementType.STRING));
		}
        /// <summary>
        /// �������� �������
        /// </summary>
        /// <param name="name">��� �������</param>
        /// <param name="type">��� �������</param>
		public void Add(string name, ElementType type)
		{
			if(!isExist(name.ToLower()))
				l.Add(new Data(name.ToLower(),type));
		}
		
        /// <summary>
        /// �������� �������
        /// </summary>
        /// <param name="tableName">��� �������</param>
        /// <param name="name">��� �������</param>
        /// <param name="type">��� �������</param>
		public void Add(string tableName, string name, ElementType type)
		{
			if(!isExist(name.ToLower()))
				l.Add(new Data(tableName.ToLower(), name.ToLower(), type));
		}
        /// <summary>
        /// �������� � �������� �������  SQL-���������
        /// </summary>
        /// <param name="name"> SQL-���������</param>
		public void Add(SQLStruct name)
		{
			l.Add(new Data(name));
		}
        /// <summary>
        /// ������������� ��� �������  �� ���������
        /// </summary>
        /// <param name="tableName">��� �������</param>
		public void SetTable(string tableName)
		{
			this.defaultTable = tableName;
		}
        /// <summary>
        /// �������� ��� ������� � i-�� �������, ���� ��� �� �����������, �� �������� ��� ������� �� ���������
        /// </summary>
        /// <param name="i">����� �������</param>
        /// <returns></returns>
		public string Table(int i)
		{
			if (i >= l.Count)
				throw new IndexOutOfRangeException("�� ���������� �������� ��� �������: " + i.ToString());
			if ((string)((Data)(l[i])).tableName != null && (string)((Data)(l[i])).tableName != "")
			{
				return (string)((Data)(l[i])).tableName;
			}else
				return defaultTable;
		}
        /// <summary>
        /// ������������� � �������� ������� SQL-������ �� ������ �������
        /// </summary>
        /// <param name="st">SQL-������</param>
        /// <param name="i">����� �������</param>
		public void Set(SQLStruct st, int i)
		{
			if (i >= l.Count)
				throw new IndexOutOfRangeException("�� ���������� �������� ��� �������: " + i.ToString());
			l[i] = new Data(st);
		}
        /// <summary>
        /// ������������� ��� ������� � ��� ������� � i-�� �������
        /// </summary>
        /// <param name="table">��� �������</param>
        /// <param name="column">��� �������</param>
        /// <param name="i">����� ������� � select</param>
		public void Set(string table, string column, int i)
		{
			if (i >= l.Count)
				throw new IndexOutOfRangeException("�� ���������� �������� ��� �������: " + i.ToString());
			l[i] = new Data(table, column);
		}
        /// <summary>
        /// ����������� ��� ������� ���������� � �����
        /// </summary>
        /// <param name="i">����� �������</param>
		public void SetUsedInChild(int i)
		{
			Data d = ((Data)l[i]);
			d.isUseInChild = true;
			
		}
        /// <summary>
        /// ��������, ������������ �� ������� � �����
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
		public bool IsUsedInChild(int i)
		{
			Data d = ((Data)l[i]);
			return d.isUseInChild;
		}

        /// <summary>
        /// ������������� ��� �������
        /// </summary>
        /// <param name="type">��� �������</param>
        /// <param name="i">����� �������</param>
		public void SetType(ElementType type, int i)
		{
			if (i >= l.Count)
				throw new IndexOutOfRangeException("�� ���������� �������� ��� �������: " + i.ToString());
			Data d = ((Data)l[i]);
			d.type = type;
		}
        /// <summary>
        /// �������� ��� �������
        /// </summary>
        /// <param name="i">����� �������</param>
        /// <returns>��� �������</returns>
		public ElementType GetType(int i)
		{
			if (i >= l.Count)
				throw new IndexOutOfRangeException("�� ���������� �������� ��� �������: " + i.ToString());
			return ((Data)l[i]).type;
		}
        /// <summary>
        /// ���-�� �������
        /// </summary>
		public int Count
		{
			get
			{
				return l.Count;
			}
		}
        /// <summary>
        /// ��� i-�� �������
        /// </summary>
        /// <param name="i">����� �������</param>
        /// <returns>���</returns>
		public string this[int i]
		{
			get
			{
				if (i >= l.Count)
					throw new IndexOutOfRangeException("�� ���������� �������� ��� �������: " + i.ToString());
				if (((Data)(l[i])).typeobj != typeObj.STRING)
				{
					return null;
				}
				return (string)((Data)(l[i])).select;
			}
			set
			{
				if (i >= l.Count)
					throw new IndexOutOfRangeException("�� ���������� �������� ��� �������: " + i.ToString());
				l[i] = new Data((string)value);
			}
		}
        /// <summary>
        /// ��������, i-�� ������� �������� ������� ��� SQL-��������
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
		public bool isString(int i)
		{
			if (i >= l.Count)
				throw new IndexOutOfRangeException("�� ���������� �������� ��� �������: " + i.ToString());
			
			if (((Data)(l[i])).typeobj == typeObj.STRING)
				return true;
			else
				return false;

		}
        /// <summary>
        /// �������� SQL_������ �� i-�� �������
        /// </summary>
        /// <param name="i">����� �������</param>
        /// <returns></returns>
		public SQLStruct GetSQL(int i)
		{
			if (i >= l.Count)
				throw new IndexOutOfRangeException("�� ���������� �������� ��� �������: " + i.ToString());

			if (((Data)(l[i])).typeobj == typeObj.SQL)
			{
				return (SQLStruct) ((Data) l[i]).select;
			}
			return null;

		}
        /// <summary>
        /// �������, ��������� �� ������� � �������� ������
        /// </summary>
        /// <param name="node">��� �������</param>
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
        /// ��������� ������������ ������������ SELECT �� ����� �������� ��
        /// </summary>
        /// <param name="ds">�������� DataSet</param>
        /// <param name="curTable">��� ������� �������</param>
		public void Validate(DataSet ds, string curTable)
		{
			DataTable table = ds.Tables[curTable];
			if (table == null)
				throw new NotExistException("�� ������� ������� � DataSet-e: " + curTable);
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
						throw new NotExistException("�� ������� ������� � DataSet-e: "+ curTable+"."+ this[i]);
					//�� �������� ���� ������ � Select
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
        /// ��������� ������������ ������������ SELECT �� ����� �������� ��
        /// </summary>
        /// <param name="ds">����� �������� ��</param>
        /// <param name="from">�������� FROM</param>
		public void Validate(DataSet ds, FromClass from)
		{
			//���� ������� ����, �� �� �� ������ �� ���������
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
							throw new NotExistException("�� ������� ������� � DataSet-e: " + curTable);
						if (ds.Tables[al]!=null)
							table = ds.Tables[al];
						else
							throw new NotExistException("�� ������� ������� � DataSet-e: " + curTable);
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
						//��������� ������� ������� � ������
						if (this.baseSql.alias.GetColumnAlias(this[i]) == null)
							throw new NotExistException("�� ������� ������� � DataSet-e: "+ curTable+"."+ this[i]);
						else
							ColName = this.baseSql.alias.GetColumnAlias(this[i]).name;
					}
					//�� �������� ���� ������ � Select
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
        /// ���������������� SQL-�������
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
        /// �������� ����� SQL-�������. ��������� ������� � ������ �� ������������� � DataSet-�
        /// </summary>
        /// <param name="ds">����� ��������</param>
        /// <param name="from">From-�����</param>
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
						//������ ��������� �����
						string al = this.baseSql.alias.GetTableAlias(curTable);
						if (al == null)
							throw new NotExistException("�� ������� ������� � DataSet-e: " + curTable);
						if (ds.Tables[al]!=null)
							table = ds.Tables[al];
						else
							throw new NotExistException("�� ������� ������� � DataSet-e: " + curTable);
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
	/// ����� ������� ������ FROM �� SQL �������
	/// </summary>
	public class FromClass
	{
        /// <summary>
        /// ������� �����
        /// </summary>
		public SQLStruct baseSql;
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="baseSql">����� sql-������� � ������� ��������� ������� from</param>
		public FromClass(SQLStruct baseSql)
		{
			this.baseSql = baseSql;
		}
	
		private ArrayList l = new ArrayList();
		private ArrayList where = new ArrayList();
        /// <summary>
        /// ��������� �������
        /// </summary>
        /// <param name="name"></param>
		public void Add(string name)
		{
			l.Add(name.ToLower());
		}
        /// <summary>
        /// ��������� where � JOIN
        /// </summary>
        /// <param name="where">�����-�����������</param>
		public void AddWhere(WhereClass where)
		{
			//this.where.Insert(0, where);
			this.where.Add(where);
		}
        /// <summary>
        /// �������� Where � i-�� ���������� ������������
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
		public WhereClass GetWhere(int i)
		{
			if (i >= where.Count)
				throw new IndexOutOfRangeException("�� ���������� �������� ��� �������: " + i.ToString());
            return (WhereClass) where[i];
		}
        /// <summary>
        /// �������� i-. �������
        /// </summary>
        /// <param name="i">����� �������</param>
        /// <returns></returns>
		public string this[int i]
		{
			get
			{
				if (i >= l.Count)
					throw new IndexOutOfRangeException("�� ���������� �������� ��� �������: " + i.ToString());
				return (string)l[i];
			}
		}
        /// <summary>
        /// ����-�� ������
        /// </summary>
		public int Count
		{
			get
			{
				return l.Count;
			}
		}
        /// <summary>
        /// ��������� ������� �� ��������
        /// </summary>
        /// <param name="ds">�������</param>
		public void Validate(DataSet ds)
		{
			for (int i=0;i<l.Count;i++)
			{
				if (ds.Tables[this[i]] == null)
				{
					//������ � Alias-��
					if (this.baseSql.alias.GetTableAlias(this[i])== null)
						throw new NotExistException("�� ������� ������� � DataSet-e: " + this[i]);
				}
			}
		}
        /// <summary>
        /// ������������� SQL �������
        /// </summary>
        /// <param name="idSQL"></param>
        /// <returns></returns>
		public int ReCount(int idSQL)
		{
			return idSQL;
		}
        /// <summary>
        /// ��������������� �������
        /// </summary>
        /// <param name="name">����� ��� �������</param>
		public void ReNameTable(string name)
		{
			//�������������� ��������� �������.
			if (l.Count == 0)
				throw new Exception("������, ��� ������ ��� �������������� (Alias).");
			l[l.Count-1] = name;
		}
	};
    /// <summary>
    /// ����������� �����
    /// </summary>
	public abstract class BaseClass
	{
		public abstract ClassType type();
	};
	/// <summary>
	/// ����� ����� ������� � ���� ������� AND, OR
	/// </summary>
	public class Oper : BaseClass
	{
		private Uslovie op;
        /// <summary>
        /// �������� ���
        /// </summary>
        /// <returns></returns>
		public override ClassType type()
		{
			return ClassType.OPER;
		}
        /// <summary>
        /// �������� ��� ��������
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
        /// �����������
        /// </summary>
        /// <param name="oper"></param>
		public Oper(Uslovie oper)
		{
			this.op = oper;
		}
		public Oper(){}
        /// <summary>
        /// � �������
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
	/// �����, ����������� ��������� � Where, ��������, table1.col1 = table2.col2;
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
        /// �����������
        /// </summary>
        /// <param name="table1">������� 1</param>
        /// <param name="column1">������� 1</param>
        /// <param name="table2">������� 2</param>
        /// <param name="column2">������� 2</param>
		public Expr(string table1, string column1, string table2, string column2)
		{
			this.ColumnName1 = column1;
			this.TableName1 = table1;
			this.ColumnName2 = column2;
			this.TableName2 = table2;
		}
		
        /// <summary>
        /// ��� ��������
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
        /// ��� ������� 1
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
        /// ��� ������� 2
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
        /// ��� ������� 1
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
        /// ��� ������� 2
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
        /// ��� ������� 1
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
        /// ��� ������� 2
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
        /// � �������
        /// </summary>
        /// <param name="curenttable">��� ������� �������</param>
        /// <param name="sql">������������ ������</param>
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
        /// ��������� �� ���� ���� �� ������������� ������� � �������
        /// </summary>
        /// <param name="ds">�������</param>
        /// <param name="curTable">��� ������� �������</param>
		public void Validate(DataSet ds, string curTable)
		{
			if (type1 == ColumnType.NAME)
			{
			
				if (TableName1 != null)
				{
					DataTable table = ds.Tables[TableName1];
					if (table == null)
						throw new NotExistException("�� ������� ������� � DataSet-e: " + TableName1);
					if (ColumnName1!= null)
					{
						if (table.Columns[ColumnName1] == null)
							throw new NotExistException("�� ������� ������� � DataSet-e: " + TableName1+ "." + ColumnName1);
					}
				}
				else
				{
					DataTable table =  ds.Tables[curTable];
					if (table == null)
						throw new NotExistException("�� ������� ������� � DataSet-e: " + TableName1);
					if (ColumnName1!= null)
					{
						if (table.Columns[ColumnName1] == null)
							throw new NotExistException("�� ������� ������� � DataSet-e: " + TableName1+ "."+ ColumnName1);
					}                
				}
			}

			if (type2 == ColumnType.NAME)
			{

			
				if (TableName2 != null)
				{
					DataTable table = ds.Tables[TableName2];
					if (table == null)
						throw new NotExistException("�� ������� ������� � DataSet-e: " + TableName2);
					if (ColumnName2!= null)
					{
						if (table.Columns[ColumnName2] == null)
							throw new NotExistException("�� ������� ������� � DataSet-e: " + TableName2+ "." + ColumnName2);
					}
				}
				else
				{
					DataTable table = ds.Tables[curTable];
					if (table == null)
						throw new NotExistException("�� ������� ������� � DataSet-e: " + TableName2);
					if (ColumnName2!= null)
					{
						if (table.Columns[ColumnName2] == null)
							throw new NotExistException("�� ������� ������� � DataSet-e: " + TableName2+"." + ColumnName2);
					}
				}

				if (TableName1 != null)
				{
					if (ds.Tables[TableName1] == null)
						throw new NotExistException("�� ������� ������� � DataSet-e: " + TableName1);
				}
			}
		}
	};
	/// <summary>
	/// ����� ������� ������ Where �� SQL �������
	/// </summary>
	public class WhereClass
	{
        /// <summary>
        /// ������������ SQL
        /// </summary>
		public SQLStruct baseSql;
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="baseSql">������������ SQL</param>
		public WhereClass(SQLStruct baseSql)
		{
			this.baseSql = baseSql;
		}

		private ArrayList list = new ArrayList();
        /// <summary>
        /// ��������� �����������
        /// </summary>
        /// <param name="ex">�����������</param>
		public void Add(Expr ex)
		{
			list.Add(ex);
		}
        /// <summary>
        /// �������� ����������� � ������� ���������� �����������
        /// </summary>
        /// <param name="ex">�����������</param>
        /// <param name="op">AND/OR</param>
		public void Add(Expr ex, Oper op)
		{
			list.Add(ex);
			list.Add(op);			
		}
        /// <summary>
        /// ��������� ���� OR �����������
        /// </summary>
        /// <param name="ex">�����������</param>
		public void AddOr(Expr ex)
		{
			list.Add(ex);
			list.Add(new Oper(Uslovie.OR));
			
		}
        /// <summary>
        /// ��������� ���� AND �����������
        /// </summary>
        /// <param name="ex">�����������</param>
		public void AddAnd(Expr ex)
		{
			list.Add(ex);
			list.Add(new Oper(Uslovie.AND));
			
		}
        /// <summary>
        /// �� ������������
        /// </summary>
        /// <returns>������ true</returns>
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
        /// ����������� �� ������
        /// </summary>
        /// <param name="i">����� �����������</param>
        /// <returns></returns>

		public BaseClass this [int i]
		{
			get
			{
				if (i >= list.Count )
					throw new IndexOutOfRangeException("�� ���������� ������� � �������: " + i.ToString());
				return (BaseClass)this.list[i];
			}
		}

        /// <summary>
        /// ���-�� �����������
        /// </summary>
		public int Count
		{
			get
			{
				return list.Count;
			}
		}
        /// <summary>
        /// � �������
        /// </summary>
        /// <param name="CurTable">��� ������� �������</param>
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
        /// ��������� ������������� �� dataSet-�
        /// </summary>
        /// <param name="ds">dataSet</param>
        /// <param name="curTable">��� ������� �������</param>
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
        /// �� ������������. ���������� idSql
        /// </summary>
        /// <param name="idSQL"></param>
        /// <returns></returns>
		public int ReCount(int idSQL)
		{
			return idSQL;
		}

	};
    /// <summary>
    /// �����, ����������� ���� �����������
    /// </summary>
	public class GroupClassOnce
	{
        /// <summary>
        /// ������� ����� �����������
        /// </summary>
		public GroupClass baseGroup;

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="baseGroup">������� ����� �����������</param>
		public GroupClassOnce(GroupClass baseGroup)
		{
            this.baseGroup = baseGroup;
		}

		private class ValueGroup
		{
			public int idGroup = -1;
			//��� ������� + ��� �������
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
		/// ������ �����������
		/// </summary>
		public ArrayList group = new ArrayList();
        /// <summary>
        /// ���� ALL
        /// </summary>
		public bool isAll = false;
		private ArrayList arrayH = new ArrayList(1);
		private ArrayList arrayF = new ArrayList(1);
        /// <summary>
        /// ��� �����������
        /// </summary>
		public TypeGroup type = TypeGroup.CONTAIN;
        /// <summary>
        /// ��� ��� Footer
        /// </summary>
		public string UseTagF;
        /// <summary>
        /// ��� ��� HEADER
        /// </summary>
		public string UseTagH;

        /// <summary>
        /// �������� ��� ���� �� ���� (Header, footer)
        /// </summary>
        /// <param name="t">��� ����������� ����</param>
        /// <returns></returns>
		public string UseTag(TypeFH t)
		{
			if (t == TypeFH.FOOTER)
				return UseTagF;
			else 
				//Header � ��� Contain
				return UseTagH;
		}

        /// <summary>
        /// ���-�� ������� � Header
        /// </summary>
		public int CountValueRecHeader
		{
			get
			{
				return arrayH.Count;
			}
		}
        /// <summary>
        /// ���-�� ������� � Footer
        /// </summary>
		public int CountValueRecFooter
		{
			get
			{
				return arrayF.Count;
			}
		}
        /// <summary>
        /// �������� ������� �� ������ ������� � ���� �����������
        /// </summary>
        /// <param name="i">����� �����������</param>
        /// <param name="t">��� �����������</param>
        /// <returns>��� �������</returns>
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
        /// �������� ����� �� <see cref="ValueGroup"/>
        /// </summary>
        /// <param name="i">����� �����������</param>
        /// <param name="t">��� �����������</param>
        /// <returns>�����</returns>
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
        /// �������� ��� ������� �� ������ ������� � ���� �����������
        /// </summary>
        /// <param name="i">����� �������</param>
        /// <param name="t">��� �����������</param>
        /// <returns>��� �������</returns>
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
        /// �������� ������ ��� � ������� <see cref="ValueGroup"/>
        /// </summary>
        /// <param name="i">��� �����������</param>
        /// <param name="t">��� �����������</param>
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
        /// �������� ������� � Contain
        /// </summary>
        /// <param name="value">��� �������</param>
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
        /// ��������� ��� �������
        /// </summary>
        /// <param name="value">��� �������</param>
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
        /// ��������� ������� � Header
        /// </summary>
        /// <param name="value">��� �������</param>
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
        /// ��������� �������
        /// </summary>
        /// <param name="value">��� �������</param>
        /// <param name="t">��� �����������</param>
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
        /// ��������� ������� � Footer
        /// </summary>
        /// <param name="value">��� �������</param>
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
        /// �������� ����������� �� ������
        /// </summary>
        /// <param name="i">����� �����������</param>
        /// <returns></returns>
		public string this[int i]
		{
			get
			{
				return (string) group[i];
			}
		}
        /// <summary>
        /// ���-�� �����������
        /// </summary>
		public int Count
		{
			get{return group.Count;}
		}
        /// <summary>
        /// ������ true
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
		public bool Validate(DataSet ds)
		{
			return true;
		}
        /// <summary>
        /// ������ isSQL
        /// </summary>
        /// <param name="idSQL"></param>
        /// <returns></returns>
		public int ReCount(int idSQL)
		{
			return idSQL;
		}
        /// <summary>
        /// ������ ���
        /// </summary>
        /// <param name="st">����� SQL</param>
        /// <param name="ColumnName">��� �������</param>
        /// <returns>������ ���</returns>
		public string GetFullName2(SQLStruct st, string ColumnName)
		{
			for (int i=0;i<st.select.Count;i++)
			{
				if (st.select.isString(i))
				{
					//���� �� ����� ����������, ��...
					if (st.select[i] == ColumnName)
					{
						st.select.SetUsedInChild(i);
						return st.GetSQLUniqueName() + "_select_" + ColumnName;
					}
				}
			}
			//����� ���� ������ ���������� � ������.
			if (st.parentSQL != null)
			{
				return GetFullName(st.parentSQL, ColumnName);
			}
			return null;
		}
        /// <summary>
        /// ������ ���
        /// </summary>
        /// <param name="st">����� SQL</param>
        /// <param name="ColumnName">��� �������</param>
        /// <returns>������ ���</returns>
        public string GetFullName(SQLStruct st, string ColumnName)
		{
            for (int i=0;i<st.select.Count;i++)
            {
				if (st.select.isString(i))
				{
					//���� �� ����� ����������, ��...
					if (st.select[i] == ColumnName)
					{
						st.select.SetUsedInChild(i);
                        return st.GetSQLUniqueName() + "_select_" + ColumnName;
					}
				}
            }
			//����� ���� ������ ���������� � ������.
			if (st.parentSQL != null)
			{
				return GetFullName(st.parentSQL, ColumnName);
			}
            return null;
		}
        /// <summary>
        /// �������� ����������� � �������� ������������� ������� � SELECT � ��������������� �����������
        /// </summary>
        /// <param name="select">������ �������</param>
        /// <param name="array">������ ValueGroup</param>
        /// <returns></returns>
		public bool Siplify(SelectClass select, ArrayList array)
		{
			for (int i=0; i<array.Count; i++)
			{
				ValueGroup g = (ValueGroup) array[i];
				//g.ColumnName = 
				//������� ������ ��� �������.
				bool Ok = false;
				for (int j=0;j<select.Count;j++)
				{
					if (select.isString(j))
					{
						//�������� ��� �� ����������
						if (select[j] == g.ColumnName)
						{
							Ok = true;
							//����� ��������� ���� �� � ������� group
							bool ok2= false;
							for (int k=0;k<this.Count;k++)
							{
								if (g.ColumnName == this[k])
									ok2 = true;
							}
							if (!ok2 && (g.Func == null || g.Func == ""))
							{
								//������ � ������� ������ ����� �� �������,���� ������ � ����������.
								int idGroup = baseGroup.GetIdGroup(g.ColumnName);
								if (idGroup == -1)
								{
									throw new Exception("������, � ����������, �� ������� �������:" + g.ColumnName);
								}
								g.idGroup = idGroup;
							}
						}
					}
				}
				if (!Ok)
				{
					//������ �� �������, �.�. ��� ��� �� �� ����� Select
					//��������� �� ������ select-� ���
					//��������� ��� ������������ :)
					string fullname = GetFullName(this.baseGroup.baseSql.parentSQL, g.ColumnName);
					if (fullname == null)
					{
						throw new NotExistException("GroupClassOne, GetFullName: ������ � ���������� - �� ������� ������� '" + g.ColumnName+ "'");
					}
					//g.ColumnName = fullname;
					g.FullName = fullname;
				}
				
			}
			return true;
		}

        /// <summary>
        /// �������� �� select
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        public bool Siplify(SelectClass select)
		{
			//��� ���� ����������� ������ �� �� ���� � ���� � group �� �� �������� SQL
			Siplify(select, this.arrayH);
			Siplify(select, this.arrayF);

            

			//�������� ��� � Group By
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
			//�������� ��� � Contain/Header/Footer
			return true;
		}
	};

    /// <summary>
    /// ����� �����������
    /// </summary>
	public class GroupClass
	{
        /// <summary>
        /// �������� ����� ����������� �� �����
        /// </summary>
        /// <param name="columnName">��� �������</param>
        /// <returns>����� �����������</returns>
		public int GetIdGroup(string columnName)
		{
			//��������� ��� ������
            for (int i=0;i<this.Count;i++)
            {
                //��������� ��� ������� � ������� ������
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
        /// �����������
        /// </summary>
        /// <param name="baseSql">������� SQL-�����</param>
		public GroupClass(SQLStruct baseSql)
		{
			this.baseSql = baseSql;
			once = new GroupClassOnce(this);
		}
		private ArrayList grouplist = new ArrayList();
        /// <summary>
        /// ������� �����������
        /// </summary>
		public GroupClassOnce once;
        /// <summary>
        /// ��������� ����� �����������
        /// </summary>
		public void AddGroup()
		{
			once = new GroupClassOnce(this);
            grouplist.Add(once);
		}
        /// <summary>
        /// ���-�� �����������
        /// </summary>
		public int Count
		{
			get
			{
				return grouplist.Count;
			}
		}
        /// <summary>
        /// ��������� ����������� �� ������
        /// </summary>
        /// <param name="i">����� �����������</param>
        /// <returns></returns>
		public GroupClassOnce this [int i]
		{
			get
			{
				return (GroupClassOnce)grouplist[i];
			}
		}
        /// <summary>
        /// ��������� �����������
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <returns>���������� ��������</returns>
		public bool Validate(DataSet ds)
		{
			foreach (GroupClassOnce group in grouplist)
			{
				group.Validate(ds);
			}
			return true;
		}
        /// <summary>
        /// ������������� SQL
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
        /// �������� �����������
        /// </summary>
        /// <param name="select">SELECt-�����</param>
        /// <returns></returns>
		public bool Simplify(SelectClass select)
		{
			//��������� ������� ���� ������...
            foreach (GroupClassOnce group in grouplist)
            {
				//GroupClassOnce.Validate(ds, select);
				group.Siplify(this.baseSql.select);
            }
			return true;
		}
	};

    /// <summary>
    /// AS- �����
    /// </summary>
	public class AsClass
	{
		public SQLStruct baseSql;
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="baseSql">������� SQL</param>
		public AsClass(SQLStruct baseSql)
		{
			this.baseSql = baseSql;
		}

		private ArrayList l = new ArrayList();
        /// <summary>
        /// ��������� � AS
        /// </summary>
        /// <param name="name">��� ������ AS</param>
		public void Add(string name)
		{
			l.Add(name);
		}
        /// <summary>
        /// ��������� AS �� ������
        /// </summary>
        /// <param name="i">�����</param>
        /// <returns></returns>
		public string this[int i]
		{
			get
			{
				if (i >= l.Count)
					throw new IndexOutOfRangeException("�� ���������� �������� ��� �������: " + i.ToString());
				return (string)l[i];
			}
		}        
	};
	/// <summary>
	/// �����, �������� � ���� ������������ SQL ������
	/// </summary>
	public class SQLStruct
	{
		public Alias alias = new Alias();
		//���������� ��� TR 
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
					throw new Exception("SQLStruct:GetTr. ����� ������ ������ ��� ���-�� �����");
				//�������� ����� �� �����.
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

		//���������� ����� SQL
		public int idSQL = 0;
        /// <summary>
        /// ������ �� ������ SQL
        /// </summary>
		public SQLStruct baseSQL = null;
        /// <summary>
        /// ������ �� ��������
        /// </summary>
		public SQLStruct parentSQL = null;

        /// <summary>
        /// ���������������� SQL
        /// </summary>
        /// <param name="idSQL">��������� �����</param>
        /// <returns>��������� ����� + 1</returns>
		public int ReCount(int idSQL)
		{
			//����������� ����� ��� �������� SQL
			this.idSQL = idSQL;
			int id = idSQL + 1;

			//�� ������� ������ ������ ��������� SQL
			id = select.ReCount(id);
			id = this.from.ReCount(id);
			id = this.group.ReCount(id);
			return id;
		}
        /// <summary>
        /// �����������
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
        /// �������� ���������� ��� SQL: var_sql_ID
        /// </summary>
        /// <returns></returns>
		public string GetSQLUniqueName()
		{
			return "var_sql_" + idSQL.ToString();
		}
        /// <summary>
        /// ������ �� SELECT-�����
        /// </summary>
		public SelectClass select;
        /// <summary>
        /// ������ �� FROM-�����
        /// </summary>
		public FromClass from;
        /// <summary>
        /// ������ �� WHERE-�����
        /// </summary>
		public WhereClass where;
        /// <summary>
        /// ������ �� AS-�����
        /// </summary>
		public AsClass As;
        /// <summary>
        /// ������ �� GROUP-�����
        /// </summary>
		public GroupClass group;
		private DataSet dataSet;
        /// <summary>
        /// ����������� �������
        /// ������� SQL-������
        /// </summary>
		public void Print()
		{
			Print(0);
		}
        /// <summary>
        /// ����������� �������
        /// ������� SQL-������
        /// </summary>
        /// <param name="ind">���-�� �������� ����� �������� (�������)</param>
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
        /// �������� SQL-������
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
        /// ��������� SQL-������ �� Dataset-�
        /// </summary>
        /// <param name="ds">����� ��</param>
		public void Validate(DataSet ds)
		{
			dataSet = ds;
			//��������� Select
			select.Validate(ds, from);
			from.Validate(ds);
			where.Validate(ds, from[0]);
			//group.Validate(ds, select);
		}
		/// <summary>
		/// ���������� ���� SQL �������
		/// </summary>
		/// <returns>���������� ���� SQL �������</returns>
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
			//������� ������� �������...
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
