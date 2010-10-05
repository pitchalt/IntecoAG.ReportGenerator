using System;
using System.IO;
using System.Data;
using System.Collections;
using Comp1;


namespace Comp1
{
	/// <summary>
	/// Запись схемы XML description for XSD.
	/// </summary>
	public class XSD
	{
		private SQLStruct sql=null;
		private string DataSetName;
		private string XMLNS = "http://www.w3.org/1999/XSL/Transform";
		private DataSet ds;
		private TextWriter sw = null;
        /// <summary>
        /// Заголовок схемы
        /// </summary>
		private void WriteHeadXSD()
		{
			sw.WriteLine("<?xml version=\"1.0\"?>\n");
			sw.WriteLine("<xs:schema id=\"IntekoDataSet\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:msdata=\"urn:schemas-microsoft-com:xml-msdata\" attributeFormDefault=\"qualified\" elementFormDefault=\"qualified\">");
		}
        /// <summary>
        /// Конец схемы
        /// </summary>
		private void WriteEndXSD()
		{
			sw.WriteLine("</xs:schema>");
			sw.Close();
		}
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="sw">Stream куда будет писаться файл</param>
        /// <param name="sql">SQL-запрос</param>
        /// <param name="DataSetName">Имя схемы</param>
		public XSD(TextWriter sw, SQLStruct sql, string DataSetName)
		{
			this.sw = sw;
			this.DataSetName = DataSetName;
			this.sql = sql;
		}
        /// <summary>
        /// Пишет теги для группировки
        /// </summary>
        /// <param name="sql">SQL-запрос</param>
		public void WriteGroup(SQLStruct sql)
		{
			for (int idGroup =0; idGroup <sql.group.Count; idGroup++)
			{

				//Footer
				if (sql.group[idGroup].CountValueRecFooter > 0)
				{
					sw.WriteLine("<xs:element name=\"" + sql.group[idGroup].UseTag(TypeFH.FOOTER) + "\">\n\t<xs:complexType>\n\t<xs:choice maxOccurs=\"unbounded\">");
					for (int j=0;j< sql.group[idGroup].CountValueRecFooter;j++)
					{
						string node = (string) sql.group[idGroup].ValueRec(j, TypeFH.FOOTER);
                    
						sw.WriteLine("<xs:element type=\"xs:string\" name=\"" + node + "\"/>");
					}
					sw.WriteLine("</xs:choice></xs:complexType></xs:element>");
				}
				if (sql.group[idGroup].CountValueRecHeader > 0)
				{
					sw.WriteLine("<xs:element name=\"" + sql.group[idGroup].UseTag(TypeFH.HEADER) + "\">\n\t<xs:complexType>\n\t<xs:choice maxOccurs=\"unbounded\">");
					for (int j=0;j< sql.group[idGroup].CountValueRecHeader;j++)
					{
						string node = (string) sql.group[idGroup].ValueRec(j, TypeFH.HEADER);
                    
						sw.WriteLine("<xs:element type=\"xs:string\" name=\"" + node + "\"/>");
					}
					sw.WriteLine("</xs:choice></xs:complexType></xs:element>");
				}
			}
            
		}
        /// <summary>
        /// Пишет закрывающиеся теги для группировки
        /// </summary>
        /// <param name="sql">SQL-запрос</param>
		public void WriteGroupEnd(SQLStruct sql)
		{
			for (int idGroup =0; idGroup <sql.group.Count; idGroup++)
			{
				if (sql.group[idGroup].type == TypeGroup.CONTAIN)
				{
					sw.WriteLine("</xs:choice></xs:complexType></xs:element>");
				}
			}
		}
        /// <summary>
        /// Пишет XSD схему. Главная функция, запускает все остальные
        /// </summary>
		public void WriteXSD()
		{
			WriteHeadXSD();

			WriteGroup(sql);
			//Для начала пишем шапку
			sw.WriteLine("<xs:element name=\"" + sql.As[0] + "\">\n\t<xs:complexType>\n\t<xs:choice maxOccurs=\"unbounded\">");
			
			for (int i=0;i< sql.select.Count; i++)
			{
				if (sql.select.isString(i))
				{
					string node = sql.select[i];
					
					switch (sql.select.GetType(i))
					{
						case ElementType.DECIMAL:
							sw.WriteLine("<xs:element name=\"" + node + "\">");
							sw.WriteLine("<xs:complexType><xs:attribute name=\"formatting_number\"/></xs:complexType>");
							break;
						case ElementType.DATE:
							sw.WriteLine("<xs:element name=\"" + node + "\" >");
							sw.WriteLine("<xs:complexType><xs:attribute name=\"formatting_date\"/></xs:complexType>");
							break;
						default:
							sw.WriteLine("<xs:element name=\"" + node + "\" type=\"xs:string\" minOccurs=\"0\">");
							break;
					}
					sw.WriteLine("</xs:element>");
				}
				else
				{
					WriteXSD(sql.select.GetSQL(i));
				}
			}
			sw.WriteLine("\t</xs:choice>\n\t");
			if (sql.select.selectType != SELECTTYPE.ONCE)
			{
				//sw.WriteLine("<xs:attribute name=\"attr\" form=\"unqualified\" type=\"xs:string\" />");
				//Расставляем ключи
				for (int i=0;i<sql.select.KeyCount;i++)
				{
					sw.WriteLine("<xs:attribute name=\"key-" + sql.select.GetKey(i) + "\" form=\"unqualified\" type=\"xs:string\" />");
				}
			}
			sw.WriteLine("</xs:complexType>\n</xs:element>");

			WriteGroupEnd(sql);

			this.WriteEndXSD();
		}
        /// <summary>
        /// Пишет теги TR- 
        /// </summary>
        /// <param name="sql">SQl-запрос</param>
		public void WriteTRBegin(SQLStruct sql)
		{
			if (sql.select.selectType != SELECTTYPE.ONCE && sql.select.KeyCount == 0)
			{
				if (sql.select.UseTag == null)
				{
					//Если не задан тег обрамления, то смотрим на Paint By

					sw.WriteLine("<xs:element name=\"tr-" + sql.As[0] + "\" minOccurs=\"0\" maxOccurs=\"unbounded\"><xs:complexType>");
				}
				else
					sw.WriteLine("<xs:element name=\"" + sql.select.UseTag + "\" minOccurs=\"0\" maxOccurs=\"unbounded\"><xs:complexType>");
				//Далее мы должны добавить аттрибуты
				//sw.WriteLine("<xs:attribute name=\"XXX_formatting_number\"/>");
				sw.WriteLine("<xs:sequence>");
			}
		}
        /// <summary>
        /// Пишет закрывающиеся тэги для TR-
        /// 
        /// </summary>
        /// <param name="sql">SQL-запрос</param>
		public void WriteTREnd(SQLStruct sql)
		{
			if (sql.select.selectType != SELECTTYPE.ONCE && sql.select.KeyCount == 0)
			{
				sw.WriteLine("</xs:sequence>");
				//
				sw.WriteLine("</xs:complexType>");

				sw.WriteLine("</xs:element>");
			}
		}
        /// <summary>
        /// Пишет все тэги из SELECT
        /// </summary>
        /// <param name="sql">SQL-запрос</param>
		public void WriteSelect(SQLStruct sql)
		{
			for (int i=0;i< sql.select.Count; i++)
			{
				if (sql.select.isString(i))
				{
					string node = sql.select[i];
					//sw.WriteLine("<xs:element name=\"" + node + "\" type=\"xs:string\" minOccurs=\"0\" />");
					switch (sql.select.GetType(i))
					{
						case ElementType.DECIMAL:
							sw.WriteLine("<xs:element name=\"" + node + "\" >");
							sw.WriteLine("<xs:complexType><xs:attribute name=\"formatting_number\"/></xs:complexType>");
							break;
						case ElementType.DATE:
							sw.WriteLine("<xs:element name=\"" + node + "\" >");
							sw.WriteLine("<xs:complexType><xs:attribute name=\"formatting_date\"/></xs:complexType>");
							break;
						default:
							sw.WriteLine("<xs:element name=\"" + node + "\" type=\"xs:string\" minOccurs=\"0\">");
							break;
					}
					sw.WriteLine("</xs:element>");
				}
				else
				{
					//outxd.WriteLine("<xs:element name=\"" + node + "\" minOccurs=\"0\" maxOccurs=\"unbounded\"><xs:complexType>\n\t<xs:sequence>");
					WriteXSD(sql.select.GetSQL(i));
				}
			}
		}
        /// <summary>
        /// Пишет дочерние SQL-запросы
        /// </summary>
        /// <param name="sql">Схема SQL</param>
		public void WriteXSD(SQLStruct sql)
		{
			//Для начала пишем шапку
			sw.WriteLine("<xs:element name=\"" + sql.As[0] + "\">\n\t<xs:complexType>\n\t<xs:choice maxOccurs=\"unbounded\">");
			
			if (sql.select.paint.Count == 0)
			{
				WriteGroup(sql);
				WriteTRBegin(sql);
				WriteSelect(sql);
				WriteTREnd(sql);
				WriteGroupEnd(sql);
			}else
			{
				//Пишем все теги из Paint
				for (int i=0;i<sql.select.paint.Count+1;  i++)
				{
					string node = "";
					if (i != sql.select.paint.Count)
						node =  sql.select.paint[i];
					else
						node = sql.select.paint.Other;
                    
					//WriteTRBegin(sql);
					sw.WriteLine("<xs:element name=\"" + node + "\" minOccurs=\"0\" maxOccurs=\"unbounded\">");
					sw.WriteLine("<xs:complexType><xs:sequence>");

					WriteSelect(sql);

					sw.WriteLine("</xs:sequence> </xs:complexType> </xs:element>");
					//WriteTREnd(sql);
				}

			}

			sw.WriteLine("\t</xs:choice>\n\t");
			if (sql.select.selectType != SELECTTYPE.ONCE)
			{
				//sw.WriteLine("<xs:attribute name=\"attr\" form=\"unqualified\" type=\"xs:string\" />");
				//Расставляем ключи
				for (int i=0;i<sql.select.KeyCount;i++)
				{
					sw.WriteLine("<xs:attribute name=\"key-" + sql.select.GetKey(i) + "\" form=\"unqualified\" type=\"xs:string\" />");
				}
			}
			
            
			sw.WriteLine("</xs:complexType>\n</xs:element>");
		}


	}
}
