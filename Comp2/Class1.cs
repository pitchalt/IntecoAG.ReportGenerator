
using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Text;
using iagcomp2;

namespace IntecoAG.iagcomp2
{
	/// <summary>
    /// структура для XSD схемы
	/// </summary>
	public class XSDChild 
	{
        /// <summary>
        /// Значение
        /// </summary>
		public string value;
        /// <summary>
        /// Тип
        /// </summary>
		public int type;
        /// <summary>
        /// Ссылка на дочерний элемент
        /// </summary>
		public XSDNode child;
        /// <summary>
        /// Ссылка на родителя
        /// </summary>
		public XSDNode parent;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="value">Знаечние</param>
        /// <param name="parent">Родитель</param>
		public XSDChild(string value, XSDNode parent)
		{
			this.value = value;
			type = 0;
			child = new XSDNode(parent);
			//parent =parent;
		}
	};
    /// <summary>
    /// Узел XSD-дерева
    /// </summary>
	public class XSDNode
	{
        /// <summary>
        /// Является ли массивом или содержит только один элемент
        /// </summary>
		public bool isArray = false;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parent">Ссылка на родителя</param>
		public XSDNode(XSDNode parent)
		{
			this.parent = parent;
		}
        /// <summary>
        /// Родитель
        /// </summary>
		public XSDNode parent;
        /// <summary>
        /// Список детей
        /// </summary>
		public ArrayList list = new ArrayList(1);
        /// <summary>
        /// Добавить нового ребенка
        /// </summary>
        /// <param name="value">Знаечние (ребенок)</param>
        /// <returns>Ссылка на ребенка</returns>
		public XSDNode AddChild(string value)
		{
			XSDChild ch = new XSDChild(value, this);
			list.Add(ch);
			return ch.child;
		}
        /// <summary>
        /// Не используется
        /// </summary>
		public void SetComplex()
		{

		}
        /// <summary>
        /// Делает тип Sequence
        /// </summary>
		public void SetSeq()
		{
            if (parent == null)
                return;
			//((Child)list[list.Count-1]).type++;
			//isArray = true;
			((XSDChild)parent.list[parent.list.Count - 1]).type++;
		}
        /// <summary>
        /// Возвращает ссылку на родителя
        /// </summary>
        /// <returns></returns>
		public XSDNode EndChild()
		{
			//Child ch = (Child)list[0];
			return parent;
		}
        /// <summary>
        /// Пробегаем по всем дочерним узлам с заданным именем и считаем кол-во массивов (type > 0)
        /// </summary>
        /// <param name="str">Имя узла</param>
        /// <returns>кол-ов детей</returns>
		public int GetCount(string str)
		{
			for (int i=0;i<list.Count;i++)
			{
				XSDChild ch = (XSDChild)list[i];
				if (ch.value == str)
				{
					//return list.Count;
					//Нашли.. значит пробегаем по всем дочерним и считаем кол0во массивов
					int cc = 0;
					for (int j=0;j<list.Count;j++)
					{
						XSDChild c = (XSDChild)list[j];
						if (c.type > 0)
						{
							cc++;
						}
					}
					return cc;
				}
				else
				{
					int count = ch.child.GetCount(str);
					if (count >0)
					{
						return count;
					}
				}
			}
			return 0;
		}
        /// <summary>
        /// Проверяет, является ли узел с заданым именем простым
        /// </summary>
        /// <param name="name">Имя узла</param>
        /// <returns>true, если простой</returns>
		public bool isSingle(string name)
		{
            for (int i=0;i<list.Count;i++)
			{
				XSDChild ch = (XSDChild)list[i];
				//если мы нашли подходящее по имени
				if (ch.value == name)
				{
                    if (ch.type > 0)
						return false;
					else
						return true;
				}else
				{
					if (ch.child.isSingle(name))
						return true;
				}
			}
			return false;
		}
	};

	/// <summary>
    /// Структура хранения ребенка
	/// </summary>
	public class Child 
	{
		public string value;
		public int type;
		public Node child;
		public Node parent;
		public Child(string value, Node parent)
		{
			this.value = value;
			type = 0;
			child = new Node(parent);
		}
	};
    /// <summary>
    /// Класс-занчение узла
    /// </summary>
	class XmlNodeValue
	{
		public XmlNodeValue(string name, XmlNodeType type, string value)
		{
			this.name = name;
			this.type = type;
			this.value = value;
		}
		public string name;
		public XmlNodeType type;
		public string value;
		public bool isEmpty = false;
	};
	/// <summary>
    /// Элемент содержит код Word-ML создается для кажого тега
	/// </summary>
	class XSD
	{
		XmlTextReader tr;
        /// <summary>
        /// Текущий жлменент XSD-дерева
        /// </summary>
		public XSDNode xsd = new XSDNode(null);
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="XmlFileName">Имя XML-файла</param>
		public XSD(string XmlFileName)
		{
			FileStream file = new FileStream(XmlFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			tr = new XmlTextReader(file);
		}

        /// <summary>
        /// Загружает XSD-схему из файла и сохраняет ее в this.xsd
        /// </summary>
		public void Load()
		{
			XSDNode curent = xsd;
			while (tr.Read())
			{
				if (tr.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (tr.Name.IndexOf("element") > 0 && !tr.IsEmptyElement)
					{
						tr.MoveToFirstAttribute();
						while (true)
						{
							if (tr.Name == "name")
							{
								curent = curent.AddChild(tr.Value);
							}
							if (!tr.MoveToNextAttribute())
								break;
						}
						//
					}
					else if (tr.Name.IndexOf("complextype") > 0 )
					{
						curent.SetComplex();
					}
					else if (tr.Name.IndexOf("sequence") > 0 || tr.Name.IndexOf("choice") > 0)
					{
						curent.SetSeq();
					}
				}
				else if (tr.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					if (tr.Name.IndexOf("element") >0 )
					{
						curent = curent.EndChild();
					}                   
				}
			}
		}

        /// <summary>
        /// Проверяет, простой узел  или массив
        /// </summary>
        /// <param name="name">Имя узла</param>
        /// <returns></returns>
		public bool isSingle(string name)
		{
            return xsd.isSingle(name);
		}

        public string RootNode
        {
            get
            {
                XSDChild xsdChild = this.xsd.list[0] as XSDChild;
                if (xsdChild == null)
                    throw new Exception("Ошибка xsdChild");

                return xsdChild.value;
            }
        }
	};
    /// <summary>
    /// Узел дерева, хранит в себе ссылку на детей, а так же все XML-тэги (поддерево) которые записаны в текущем элменете.
    /// </summary>
	public class Node
	{
		/// <summary>
		/// Ссылка на родиетля
		/// </summary>
		public Node parent;
        /// <summary>
        /// Имя узла
        /// </summary>
		public string Name;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parent">Родитель</param>
		public Node(Node parent)
		{
			this.parent = parent;
		}
		/// <summary>
		/// Список детей
		/// </summary>
		public ArrayList list = new ArrayList(1);
        /// <summary>
        /// Список всех XML-элементов, хранящихся в текущем узле
        /// </summary>
		public ArrayList listXml = new ArrayList();
        /// <summary>
        /// Список все аттрибутов
        /// </summary>
		public ArrayList att = new ArrayList(1);
        /// <summary>
        /// Добавляет нового ребенка
        /// </summary>
        /// <param name="value">Имя ребенка</param>
        /// <returns>Сылка на ребенка</returns>
		public Node AddChild(string value)
		{
			Child ch = new Child(value, this);
			list.Add(ch);
			listXml.Add(new XmlNodeValue(value, XmlNodeType.Element, null));
			return ch.child;
		}
        /// <summary>
        /// Добавляет аттрибут к текущему узлу
        /// </summary>
        /// <param name="name">Имя аттрибута</param>
        /// <param name="value">Значение аттрибута</param>
		public void AddAttribute(string name, string value)
		{
            att.Add(new XmlNodeValue(name, XmlNodeType.Attribute ,value));
		}

		/// <summary>
		/// Добавляет XML-теги (тэги WORD)
		/// </summary>
		/// <param name="nameNode">Имя тэга</param>
		/// <param name="typeNode">Тип тэга</param>
		/// <param name="valueNode">Знаечние</param>
        public void AddXmlNode(string nameNode, XmlNodeType typeNode, string valueNode)
		{
			listXml.Add(new XmlNodeValue(nameNode,typeNode, valueNode));
		}
        /// <summary>
        /// Сылка на родителя
        /// </summary>
        /// <returns></returns>
		public Node GetParent()
		{
			return parent;
		}
	};

	/*public class Child 
	{
		public string value;
		public int type;
		public Node child;
		public Node parent;
		public Child(string value, Node parent)
		{
			this.value = value;
			type = 0;
			child = new Node(parent);
		}
	};

	public class Node
	{
		public bool isArray = false;
		public Node(Node parent)
		{
			this.parent = parent;
		}
		public Node parent;
		public ArrayList list = new ArrayList(1);
		public Node AddChild(string value)
		{
			Child ch = new Child(value, this);
			list.Add(ch);
			return ch.child;
		}
		public void SetComplex()
		{

		}
		public void SetSeq()
		{
			//((Child)list[list.Count-1]).type++;
			//isArray = true;
			((Child)parent.list[parent.list.Count - 1]).type++;
		}
		public Node EndChild()
		{
			//Child ch = (Child)list[0];
			return parent;
		}
		public int GetCount(string str)
		{
			for (int i=0;i<list.Count;i++)
			{
				Child ch = (Child)list[i];
				if (ch.value == str)
				{
					//return list.Count;
					//Нашли.. значит пробегаем по всем дочерним и считаем кол0во массивов
					int cc = 0;
					for (int j=0;j<list.Count;j++)
					{
						Child c = (Child)list[j];
						if (c.type > 0)
						{
							cc++;
						}
					}
					return cc;
				}
				else
				{
					int count = ch.child.GetCount(str);
					if (count >0)
					{
						return count;
					}
				}
			}
			return 0;
		}
	};*/

	//класс чтения из word файла
	/// <summary>
	/// Пишет Word-файл по распарсенному дереву <see cref="Node"/>
	/// </summary>
    class WriteWordXml
	{
        /// <summary>
        /// Параметры
        /// </summary>
		private class Parametrs
		{
            public string parent;
			public ArrayList attr;
			public bool isEmpty = false;
			public string parentname;
			public XmlNodeType parenttype;
		};

        /// <summary>
        /// Верхушка дерева
        /// </summary>
		private Node Head;
        /// <summary>
        /// Котоа записи
        /// </summary>
		private XmlTextWriter tw;
        /// <summary>
        /// Cхема XSD
        /// </summary>
		private XSD xsd;

        /// <summary>
        /// Конструктор, пишутся все необхордиммые заголовки для XSLT-Word-файла
        /// </summary>
        /// <param name="Head">Верхушка дерева</param>
        /// <param name="fileName">имя файла</param>
        /// <param name="XsdFileName">Имя XSD-файла</param>
		public WriteWordXml(Node Head, string fileName, string XsdFileName)
		{
            this.Head = Head;
			TextWriter file = new StreamWriter(fileName);
			tw = new XmlTextWriter(file);
			//tw.Formatting = Formatting.None;
			tw.WriteStartDocument(true);

			tw.WriteStartElement("xsl:stylesheet");
			tw.WriteAttributeString("version","2.0");
			tw.WriteAttributeString("xmlns:xsl","http://www.w3.org/1999/XSL/Transform");
			tw.WriteStartElement("xsl:output");
			tw.WriteAttributeString("method", "xml");
			tw.WriteAttributeString("encoding", "utf-8");
			tw.WriteEndElement();
			tw.WriteStartElement("xsl:template");
					
			tw.WriteAttributeString("match", "/");
			tw.WriteStartElement("xsl:processing-instruction");
			tw.WriteAttributeString("name", "mso-application");
			tw.WriteStartElement("xsl:text");
			tw.WriteString("progid=\"Word.Document\"");
			tw.WriteEndElement();
			tw.WriteEndElement();


			xsd = new XSD(XsdFileName);
			xsd.Load();
		}
        /// <summary>
        /// Записывает готовый документ в Word-XSLT-файл
        /// </summary>
		public void Write()
		{
			Write(Head);


			tw.WriteEndElement();
			tw.WriteEndElement();

			tw.WriteEndDocument();
			tw.Flush();
			tw.Close();
		}
        /// <summary>
        /// Пишет строку в файл: xsl:value-of(
        /// А так же делает необходимое форматирование числа и даты
        /// </summary>
        /// <param name="p"></param>
		private void WriteString(Parametrs p)
		{
            //tw.WriteString(p.parent);
			//tw.WriteWhitespace("");
			tw.WriteStartElement("xsl:value-of");
			bool exit = false;
			for (int i=0;i<p.attr.Count;i++)
			{
				XmlNodeValue val = (XmlNodeValue)p.attr[i];
				if (val.name.IndexOf("formatting_number") != -1 && !exit)
				{
					tw.WriteAttributeString("select", "format-number(" + p.parent + ",'" + val.value + "')");
					exit = true;
				}
				else if (val.name.IndexOf("formatting_date") != -1 && !exit)
				{
					tw.WriteAttributeString("select", "format-date(xs:date(" + p.parent + "),'" + val.value + "')");
					exit = true;
				}
			}
			if (!exit)
			{
				tw.WriteAttributeString("select", p.parent);
			}
			
			
			tw.WriteEndElement();
			//tw.WriteWhitespace("");
		}

        /// <summary>
        /// Нужен для вствки значений пустых тегов, т.е. если в XSD-тэге пусто, то туда необходимо добавить нужные теэги для вывода строки в WORD
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
		private bool WriteXSLT(Parametrs p)
		{
			bool res = false;
			if (p.parentname == "w:p")
			{
				if (p.parenttype == XmlNodeType.EndElement)
					tw.WriteStartElement("w:p");
                tw.WriteStartElement("w:r");
				tw.WriteStartElement("w:t");
                WriteString(p);
				tw.WriteEndElement();
				tw.WriteEndElement();
				if (p.parenttype == XmlNodeType.EndElement)
					tw.WriteEndElement();
				res = true;
			}else if (p.parentname == "w:r")
			{
				if (p.parenttype == XmlNodeType.EndElement)
					tw.WriteStartElement("w:r");
				tw.WriteStartElement("w:t");
				WriteString(p);
				tw.WriteEndElement();
				if (p.parenttype == XmlNodeType.EndElement)
					tw.WriteEndElement();
				res = true;
			}else if(p.parentname == "w:t")
			{
				if (p.parenttype == XmlNodeType.EndElement)
					tw.WriteStartElement("w:t");
				WriteString(p);
				if (p.parenttype == XmlNodeType.EndElement)
					tw.WriteEndElement();
				res = true;
			}
			return res;
		}
        /// <summary>
        /// Смотрит, существует ли одинаковые тэги на -1 элементе
        /// </summary>
        /// <param name="node">Узел дерева</param>
        /// <param name="id">Номер элемента в списке xml-тэгов ворда</param>
        /// <param name="i">Номер элемента в списке xml-тэгов ворда</param>
        /// <returns></returns>
		private bool isExistBefore(Node node, int id, int i)
		{
            if (id == 0 || i == 0)
				return false;
			string name1 = ((Child)node.list[id-1]).value;
			string name2 = ((XmlNodeValue)node.listXml[i-1]).name;
			string val = ((XmlNodeValue)node.listXml[i-1]).value;
			if (val == null && name1 == name2)
				return true;
			else
				return false;
		}
        /// <summary>
        /// Смотрит, существует ли одинаковые тэги на +1 элементе
        /// </summary>
        /// <param name="node">Узел дерева</param>
        /// <param name="id">Номер элемента в списке xml-тэгов ворда</param>
        /// <param name="i">Номер элемента в списке xml-тэгов ворда</param>
        /// <returns></returns>
		private bool isExistNext(Node node, int id, int i)
		{
			if (id+1 >= node.list.Count || i+1 >= node.listXml.Count)
				return false;
			string name1 = ((Child)node.list[id+1]).value;
			string name2 = ((XmlNodeValue)node.listXml[i+1]).name;
			string val = ((XmlNodeValue)node.listXml[i+1]).value;
			if (val == null && name1 == name2)
				return true;
			else
				return false;
		}
		
        /// <summary>
        /// Пишет XSLT-Word-файл
        /// </summary>
        /// <param name="node">Узле дереве</param>
        /// <param name="id">Номер 1</param>
        /// <param name="i">Номер 2</param>
        /// <returns></returns>
		private int WriteXSLT(Node node, int id, int i)
		{
			//Node xx = node.parent;
			//Node xx2 = node.
			string prefix = "xsl:";
			Child ch = (Child) node.list[id];
			int count = 0;
			//Вначале мы должны посмареть на атрибуты
			bool attr = false;
			Child v = null;
			if (node.list.Count >0)
			{
				v = (Child)node.list[id];
				if (v.child.att.Count >0)
					attr = true;
			}
			
			if (attr)
			{
				string str = "";
                for (int ii=0;ii<v.child.att.Count;ii++)
                {
					XmlNodeValue val = (XmlNodeValue) v.child.att[ii];
					//Если это keyed
					if (val.name.IndexOf("key-") == 0)
					{
                        if (str != "")
							str += " and ";
						else
							str+="";
						str += "@" + val.name.Substring(4) + "=\'" + val.value + "\'";
					}
                }
				if (str.Length > 0)
				{
					tw.WriteStartElement(prefix + "for-each");
					tw.WriteAttributeString("select", "./" + ch.value +"[" + str +"]");
					count =1;
				}
			}
			else
			{
				if (!isExistNext(node,id,i) && !isExistBefore(node,id,i))
				{
					//т.е. не существует тега до и после текущего, значит он 1 и значит ставим for-each и закрываем его
					tw.WriteStartElement(prefix + "for-each");
					if (node.parent == null)
						tw.WriteAttributeString("select", "//" + ch.value);
					else
						tw.WriteAttributeString("select", "./" + ch.value);

					count =1;
				}
				else if (!isExistBefore(node,id,i))
				{
					tw.WriteStartElement(prefix + "for-each");
					tw.WriteAttributeString("select", "./*");
					tw.WriteStartElement(prefix + "if");
					tw.WriteAttributeString("test","local-name() = '" + ch.value + "'");
					count = 1;
				}
				else if (!isExistNext(node,id,i))
				{
					//tw.WriteStartElement(prifix + "for-each");
					//tw.WriteAttributeString("select", "./" + ch.value);
					tw.WriteStartElement(prefix + "if");
					tw.WriteAttributeString("test","local-name() = '" + ch.value + "'");
					count = 2;
				}
				else
				{
					tw.WriteStartElement(prefix + "if");
					tw.WriteAttributeString("test","local-name() = '" + ch.value + "'");
					count = 1;
				}			
				//tw.WriteStartElement("ns2:" + ch.value);
			}

			return count;
		}
        /// <summary>
        /// Получат Ф-ия получает для ind-ого элемента.
        /// </summary>
        /// <param name="listXml">Список XML-тэгов</param>
        /// <param name="ind">Номер тэга</param>
        /// <param name="p">Параметр (пишется результат)</param>
		private void GetParent(ArrayList listXml, int ind, Parametrs p)
		{
			//Parametrs p = new Parametrs();
			//Ф-ия получает потомка для i-ого элемента.
			int count = 1;
			int i = ind -1;
			while (i >= 0)
			{
				XmlNodeValue nd = (XmlNodeValue)listXml[i];
				if (nd.type == XmlNodeType.EndElement)
					count++;
                else if (nd.type == XmlNodeType.Element && nd.value != null)
					count--;
				if (count == 0)
				{
					p.parentname = nd.name;
					p.parenttype = nd.type;
					break;
				}
				i--;
			}
		}
        /// <summary>
        /// Пишет текущий узел
        /// </summary>
        /// <param name="node">Узел</param>
        /// <param name="isSingle">Single или Array</param>
        /// <param name="parent">Параметры</param>
		private void Write(Node node, bool isSingle, Parametrs parent)
		{
			int id = 0;
			for (int i=0;i<node.listXml.Count;i++)
			{
				XmlNodeValue val = (XmlNodeValue) node.listXml[i];
				if (val.value == null)
				{
					//тада пишем Child
					int count = 0;
					Child ch = (Child) node.list[id];
					//Здесь должна быть обработка и вставка конструкций XSLT
					if (xsd.isSingle(ch.value) && ch.child.listXml.Count != 0)
					{
						//count = WriteXSLT(node, id, i);
						Parametrs p = new Parametrs();
						p.parent = ch.value;
						p.attr = ch.child.att;
						p.isEmpty = false;
						p.parentname = null;

                        Write(ch.child ,true, p);
					}
					else if (ch.child.listXml.Count == 0 && xsd.isSingle(ch.value))
					{
						//count = WriteXSLT(node, id, i);
						Parametrs p = new Parametrs();
						p.parent = ch.value;
						p.attr = ch.child.att;
						//надо выбрать настоящего предка...
						/*Parametrs p1 =*/ GetParent(node.listXml, i, p);
						/*p.parentname = p1.parentname;
						p.parenttype = p1.parenttype;*/
						//p.parentname = ((XmlNodeValue) node.listXml[i-1]).name;
						//p.parenttype = ((XmlNodeValue) node.listXml[i-1]).type;
						p.isEmpty = true;
						WriteXSLT(p);
					}
					else
					{
						//else - (isSingle=false)
						count = WriteXSLT(node, id, i);
						Write(ch.child, false);
						isSingle = false;
					}

					//Здесь должна быть обработка и вставка конструкций XSLT
					for (int k=0;k<count;k++)
						tw.WriteEndElement();
					id++;
				}
				else
				{
					switch (val.type)
					{
						case XmlNodeType.Element:
						{
							tw.WriteStartElement(val.name);
						}break;

						case XmlNodeType.EndElement:
						{
							
							if (isSingle)
							{
								parent.parentname = val.name;
								bool res = WriteXSLT(parent);
								if (res)
									isSingle = false;
								
							}
                            
							/*if (val.name == "w:t" && isSingle)
							{
								//tw.WriteStartElement("xsl:value-of");
								//tw.WriteAttributeString("select","value");
								//tw.WriteEndElement();
								string value = parent.parent;
								tw.WriteString(value);
								if (parent.attr!= null)
								{
                                    tw.WriteString(parent.attr.Count.ToString());
								}

								isSingle = false;
							}*/
							tw.WriteEndElement();
						}break;

						case XmlNodeType.Attribute:
						{
							tw.WriteAttributeString(val.name,val.value);
						}break;
						case XmlNodeType.Text:
						{
							tw.WriteString(val.value);
						}break;
					}
				}
			}
		}
        /// <summary>
        /// Пишет поддерево <see cref="Write(Node,bool,Parametrs)"/>
        /// </summary>
        /// <param name="node">Поддерево</param>
		private void Write(Node node)
		{
			Write(node, false, null);
		}
        /// <summary>
        /// Пишет поддерево <see cref="Write(Node,bool,Parametrs)"/>
        /// </summary>
        /// <param name="node">Поддерево</param>
        /// <param name="isSingle">Single или Array</param>
		private void Write(Node node, bool isSingle)
		{
			Write(node, isSingle, null);
		}
        /// <summary>
        /// Пишет XSLT-файл
        /// </summary>
		public void WriteWord()
		{
			WriteWord(Head);
			tw.Flush();
			tw.Close();
		}
        /// <summary>
        /// Пишет поддерево
        /// </summary>
        /// <param name="node"></param>
		private void WriteWord(Node node)
		{
			//tw.WriteStartElement(node.)
			for (int i=0; i<node.list.Count; i++)
			{
                Child ch = (Child) node.list[i];
				tw.WriteStartElement(ch.value);
                //Пишем атрибуты
				for (int j=0; j<ch.child.att.Count; j++)
				{
                    XmlNodeValue val = (XmlNodeValue) ch.child.att[j];
					tw.WriteAttributeString(val.name, val.value);
				}
				//Далее пишем для child
                WriteWord(ch.child);
				tw.WriteEndElement();
			}
		}
	};
    /// <summary>
    /// Класс парсит исходный WORD-XML-файл.
    /// Файл может содержать "тэги Word" - обычные теги которые пишутся Word-ом
    /// А так же XSD-тэги, которые вствил пользователь по схеме
    /// </summary>
	class ReadWordXml
	{
		private string URI;
		private string XMLNS = null;
		private string XmlFileName;
		private XmlTextReader tr;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="fileName">Имя входного файла</param>
        /// <param name="URI">URI-схемы (уникальное имя)</param>
        public ReadWordXml(string fileName, string URI)
		{
            this.XmlFileName = fileName;
			//Создаем поток чтения
			FileStream file = new FileStream(this.XmlFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			tr = new XmlTextReader(file);
			if (tr == null)
				throw new FileNotFoundException("Ошибка. Не найден фаил: " + this.XmlFileName);

			this.URI = URI;
		}
        /// <summary>
        /// Дерево
        /// </summary>
		private Node Storage = new Node(null);
        /// <summary>
        /// Текущий узел
        /// </summary>
		private Node CurNode;
		/// <summary>
        /// Функция запускает чтение файла и записывает в this.Storage
		/// </summary>
		public void Read()
		{
			bool cont = false;
			this.CurNode = this.Storage;
            //Читаем word-документ
			//Читаем заголовок и префикс 
			ReadMainTag(CurNode);
			//если XMLNS = null  то ошибка...

			//read 
			while(tr.Read())
			{
				//Если это теги word-a (те которые мы сами расставили)
				if ( tr.Name.Length > XMLNS.Length)
				{

					if (this.XMLNS == tr.Name.Substring(0,XMLNS.Length))
					{
						//Если начало, то добавляем child
						if (tr.NodeType == XmlNodeType.Element)
						{
							CurNode = this.CurNode.AddChild(tr.Name.Substring(XMLNS.Length +1));
							bool isEnd = tr.IsEmptyElement;

							//Далее должны пробежать по атрибутам потомка...
							//ДОДЕЛАТЬ!!!!
							if (tr.AttributeCount > 0)
							{
                            
								tr.MoveToFirstAttribute();
								while (true)
								{
									//пробегаем по всем атрибутам
									CurNode.AddAttribute(tr.Name, tr.Value);
									if (!tr.MoveToNextAttribute())
										break;
								}
							}
							if (isEnd)
							{
								CurNode = CurNode.GetParent();
							}
						}
						//Если конец текущего эл-та, то возвращяем потомка
						else if (tr.NodeType == XmlNodeType.EndElement)
						{
							this.CurNode =  CurNode.GetParent();
						}
					}
					else
					{
						if (tr.Name.IndexOf("schemaLibrary") != -1)
						{
							cont = !cont;
							continue;
						}
                        if (cont)
							continue;
                        //XmlTextReader n = tr;
						bool isEmpty = tr.IsEmptyElement;
						string name = tr.Name;
						XmlNodeType type = tr.NodeType;
						this.CurNode.AddXmlNode(tr.Name, tr.NodeType, tr.Value);

						if (tr.AttributeCount > 0)
						{
							tr.MoveToFirstAttribute();
							while (true)
							{
								//пробегаем по всем атрибутам
								CurNode.listXml.Add(new XmlNodeValue(tr.Name, XmlNodeType.Attribute, tr.Value));
								if (!tr.MoveToNextAttribute())
									break;
							}
						}

						if (isEmpty && type == XmlNodeType.Element)
						{
                            this.CurNode.AddXmlNode(name, XmlNodeType.EndElement,"");
						}
					}

				}
				else
				{
					if (tr.Name.IndexOf("schemaLibrary") != -1)
					{
						cont = !cont;
						continue;
					}
					if (cont)
						continue;
					//XmlTextReader n = tr;
					bool isEmpty = tr.IsEmptyElement;
					string name = tr.Name;
					XmlNodeType type = tr.NodeType;
					this.CurNode.AddXmlNode(tr.Name, tr.NodeType, tr.Value);
					if (tr.AttributeCount > 0)
					{
                            
						tr.MoveToFirstAttribute();
						while (true)
						{
							//пробегаем по всем атрибутам
							//CurNode.AddAttribute(tr.Name, tr.Value);
							CurNode.listXml.Add(new XmlNodeValue(tr.Name, XmlNodeType.Attribute, tr.Value));
							if (!tr.MoveToNextAttribute())
								break;
						}
					}
					if (isEmpty && type == XmlNodeType.Element)
					{
						this.CurNode.AddXmlNode(name, XmlNodeType.EndElement,"");
					}
				}
			}
		}

		/// <summary>
		/// Ссылка на Storge
		/// </summary>
		public Node node
		{
			get
			{
				return this.Storage;
			}
		}
        /// <summary>
        /// Читает имя тэга(точнее его префикс) которые записано в Word перед XSD-тэгами
        /// Например ns0:
        /// </summary>
        /// <param name="node"></param>
		private void ReadMainTag(Node node)
		{
			XmlTextReader tr = this.tr;
			XMLNS = "";
			while(tr.Read())
			{
				if (tr.NodeType == XmlNodeType.Element)
				{
					//tw.WriteStartElement(tr.Name);
					node.listXml.Add(new XmlNodeValue(tr.Name,XmlNodeType.Element, tr.Value));
					if (tr.AttributeCount == 0)
						return;
					if (!tr.MoveToFirstAttribute())
					{
						return;
					}
					do
					{
						//tw.WriteAttributeString(tr.Name, tr.Value);
						node.listXml.Add(new XmlNodeValue(tr.Name,XmlNodeType.Attribute, tr.Value));

						if (tr.Value.ToLower() == this.URI.ToLower())
						{
							int ind = tr.Name.LastIndexOf(":");
							XMLNS = tr.Name.Substring(ind + 1); 
						}

					}while (tr.MoveToNextAttribute());
					break;
				}
			}
			if (this.XMLNS == "")
				throw new Exception("Ошибка, в Word документе не найдено схемы с URI: " + this.URI);
		}
	};
    /// <summary>
    /// Класс-компилятор, содержит ссылки на входной файл, схему, и выходной XSLT-Word-файл и ее URI
    /// </summary>
	class Comp2
	{
        /// <summary>
        /// Входной XML-файл
        /// </summary>
		public string inputXMLFile;
        /// <summary>
        /// Входная XSD-схема
        /// </summary>
		public string inputXSDFile;
        /// <summary>
        /// Выходной XML-файл
        /// 
        /// </summary>
		public string outXMLFile;
        /// <summary>
        /// URI
        /// </summary>
		public string URI;

        public DocumentType DocumentType { get; set; }
	};
	/// <summary>
	/// Класс содержит функцию MAIN
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
            Comp2 c = new Comp2();
            c.DocumentType = DocumentType.Word;
			for (int i=0;i<args.Length;i++)
			{
				if (args[i].ToLower().Trim('/','-') == "?")
				{
					Console.WriteLine("//Правила аргументов командной строки такие....");
					Console.WriteLine("// - WordXML		имя_файла		-	имя входного WORD файла");
                    Console.WriteLine("// - ExcelXML	имя_файла		-	имя входного Excel файла");
					Console.WriteLine("// - readXSD		имя_файла		-	имя входной схемы XSD");
					Console.WriteLine("// - outXSL		имя_файла		-	имя выходного XSLT файла");
					Console.WriteLine("// - uri			имя_URI			-	URI (тип документа)");
					Console.WriteLine("// - ?							-	помощь");
					return -1;
				}
				if (args[i].ToLower().Trim('/','-') == "wordxml" && i+1 < args.Length)
				{
                    c.DocumentType = DocumentType.Word;
					c.inputXMLFile = args[i+1].Trim('\"','\'','`');
					i++;
					continue;
				}
                if (args[i].ToLower().Trim('/', '-') == "excelxml" && i + 1 < args.Length)
                {
                    c.DocumentType= DocumentType.Excel;
                    c.inputXMLFile = args[i + 1].Trim('\"', '\'', '`');
                    i++;
                    continue;
                }
				if (args[i].ToLower().Trim('/','-') == "readxsd" && i+1 < args.Length)
				{
					c.inputXSDFile = args[i+1].Trim('\"','\'','`');
					i++;
					continue;
				}
				if (args[i].ToLower().Trim('/','-') == "outxsl" && i+1 < args.Length)
				{
					c.outXMLFile = args[i+1].Trim('\"','\'','`');
					i++;
					continue;
				}
				if (args[i].ToLower().Trim('/','-') == "uri" && i+1 < args.Length)
				{
					c.URI = args[i+1].Trim('\"','\'','`');
					i++;
					continue;
				}
			}
			/*string input = "apof1w1.xml";
			string output = "out_word.xsl";
			string URI = "APOF1";
			string xsd = "apof1s.xsd";*/

            if (c.DocumentType == DocumentType.Word)
            {

                ReadWordXml read = new ReadWordXml(c.inputXMLFile, c.URI);
                read.Read();
                WriteWordXml write = new WriteWordXml(read.node, c.outXMLFile, c.inputXSDFile);
                write.Write();
                return 0;
            }
            else
            {
                //Excel
                XSD xsd = null;
                try
                {
                    xsd = new XSD(c.inputXSDFile);
                    xsd.Load();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка парсинга xsd: " + ex);
                    return 1;
                }

                ExcelReader reader = new ExcelReader(c.inputXMLFile);

                ExcelDocument st = reader.Process();

                XmlNode node = st.GenerateXSLT(xsd);

                node.OwnerDocument.Save(c.outXMLFile);

                return 0;
            }
		}
	}

    enum DocumentType
    {
        Word,
        Excel
    }
}
