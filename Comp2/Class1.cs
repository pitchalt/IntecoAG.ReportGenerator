
using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Text;
using iagcomp2;

namespace IntecoAG.iagcomp2
{
	/// <summary>
    /// ��������� ��� XSD �����
	/// </summary>
	public class XSDChild 
	{
        /// <summary>
        /// ��������
        /// </summary>
		public string value;
        /// <summary>
        /// ���
        /// </summary>
		public int type;
        /// <summary>
        /// ������ �� �������� �������
        /// </summary>
		public XSDNode child;
        /// <summary>
        /// ������ �� ��������
        /// </summary>
		public XSDNode parent;
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="value">��������</param>
        /// <param name="parent">��������</param>
		public XSDChild(string value, XSDNode parent)
		{
			this.value = value;
			type = 0;
			child = new XSDNode(parent);
			//parent =parent;
		}
	};
    /// <summary>
    /// ���� XSD-������
    /// </summary>
	public class XSDNode
	{
        /// <summary>
        /// �������� �� �������� ��� �������� ������ ���� �������
        /// </summary>
		public bool isArray = false;
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="parent">������ �� ��������</param>
		public XSDNode(XSDNode parent)
		{
			this.parent = parent;
		}
        /// <summary>
        /// ��������
        /// </summary>
		public XSDNode parent;
        /// <summary>
        /// ������ �����
        /// </summary>
		public ArrayList list = new ArrayList(1);
        /// <summary>
        /// �������� ������ �������
        /// </summary>
        /// <param name="value">�������� (�������)</param>
        /// <returns>������ �� �������</returns>
		public XSDNode AddChild(string value)
		{
			XSDChild ch = new XSDChild(value, this);
			list.Add(ch);
			return ch.child;
		}
        /// <summary>
        /// �� ������������
        /// </summary>
		public void SetComplex()
		{

		}
        /// <summary>
        /// ������ ��� Sequence
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
        /// ���������� ������ �� ��������
        /// </summary>
        /// <returns></returns>
		public XSDNode EndChild()
		{
			//Child ch = (Child)list[0];
			return parent;
		}
        /// <summary>
        /// ��������� �� ���� �������� ����� � �������� ������ � ������� ���-�� �������� (type > 0)
        /// </summary>
        /// <param name="str">��� ����</param>
        /// <returns>���-�� �����</returns>
		public int GetCount(string str)
		{
			for (int i=0;i<list.Count;i++)
			{
				XSDChild ch = (XSDChild)list[i];
				if (ch.value == str)
				{
					//return list.Count;
					//�����.. ������ ��������� �� ���� �������� � ������� ���0�� ��������
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
        /// ���������, �������� �� ���� � ������� ������ �������
        /// </summary>
        /// <param name="name">��� ����</param>
        /// <returns>true, ���� �������</returns>
		public bool isSingle(string name)
		{
            for (int i=0;i<list.Count;i++)
			{
				XSDChild ch = (XSDChild)list[i];
				//���� �� ����� ���������� �� �����
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
    /// ��������� �������� �������
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
    /// �����-�������� ����
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
    /// ������� �������� ��� Word-ML ��������� ��� ������ ����
	/// </summary>
	class XSD
	{
		XmlTextReader tr;
        /// <summary>
        /// ������� �������� XSD-������
        /// </summary>
		public XSDNode xsd = new XSDNode(null);
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="XmlFileName">��� XML-�����</param>
		public XSD(string XmlFileName)
		{
			FileStream file = new FileStream(XmlFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			tr = new XmlTextReader(file);
		}

        /// <summary>
        /// ��������� XSD-����� �� ����� � ��������� �� � this.xsd
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
        /// ���������, ������� ����  ��� ������
        /// </summary>
        /// <param name="name">��� ����</param>
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
                    throw new Exception("������ xsdChild");

                return xsdChild.value;
            }
        }
	};
    /// <summary>
    /// ���� ������, ������ � ���� ������ �� �����, � ��� �� ��� XML-���� (���������) ������� �������� � ������� ��������.
    /// </summary>
	public class Node
	{
		/// <summary>
		/// ������ �� ��������
		/// </summary>
		public Node parent;
        /// <summary>
        /// ��� ����
        /// </summary>
		public string Name;
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="parent">��������</param>
		public Node(Node parent)
		{
			this.parent = parent;
		}
		/// <summary>
		/// ������ �����
		/// </summary>
		public ArrayList list = new ArrayList(1);
        /// <summary>
        /// ������ ���� XML-���������, ���������� � ������� ����
        /// </summary>
		public ArrayList listXml = new ArrayList();
        /// <summary>
        /// ������ ��� ����������
        /// </summary>
		public ArrayList att = new ArrayList(1);
        /// <summary>
        /// ��������� ������ �������
        /// </summary>
        /// <param name="value">��� �������</param>
        /// <returns>����� �� �������</returns>
		public Node AddChild(string value)
		{
			Child ch = new Child(value, this);
			list.Add(ch);
			listXml.Add(new XmlNodeValue(value, XmlNodeType.Element, null));
			return ch.child;
		}
        /// <summary>
        /// ��������� �������� � �������� ����
        /// </summary>
        /// <param name="name">��� ���������</param>
        /// <param name="value">�������� ���������</param>
		public void AddAttribute(string name, string value)
		{
            att.Add(new XmlNodeValue(name, XmlNodeType.Attribute ,value));
		}

		/// <summary>
		/// ��������� XML-���� (���� WORD)
		/// </summary>
		/// <param name="nameNode">��� ����</param>
		/// <param name="typeNode">��� ����</param>
		/// <param name="valueNode">��������</param>
        public void AddXmlNode(string nameNode, XmlNodeType typeNode, string valueNode)
		{
			listXml.Add(new XmlNodeValue(nameNode,typeNode, valueNode));
		}
        /// <summary>
        /// ����� �� ��������
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
					//�����.. ������ ��������� �� ���� �������� � ������� ���0�� ��������
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

	//����� ������ �� word �����
	/// <summary>
	/// ����� Word-���� �� ������������� ������ <see cref="Node"/>
	/// </summary>
    class WriteWordXml
	{
        /// <summary>
        /// ���������
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
        /// �������� ������
        /// </summary>
		private Node Head;
        /// <summary>
        /// ����� ������
        /// </summary>
		private XmlTextWriter tw;
        /// <summary>
        /// C���� XSD
        /// </summary>
		private XSD xsd;

        /// <summary>
        /// �����������, ������� ��� ������������� ��������� ��� XSLT-Word-�����
        /// </summary>
        /// <param name="Head">�������� ������</param>
        /// <param name="fileName">��� �����</param>
        /// <param name="XsdFileName">��� XSD-�����</param>
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
        /// ���������� ������� �������� � Word-XSLT-����
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
        /// ����� ������ � ����: xsl:value-of(
        /// � ��� �� ������ ����������� �������������� ����� � ����
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
        /// ����� ��� ������ �������� ������ �����, �.�. ���� � XSD-���� �����, �� ���� ���������� �������� ������ ����� ��� ������ ������ � WORD
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
        /// �������, ���������� �� ���������� ���� �� -1 ��������
        /// </summary>
        /// <param name="node">���� ������</param>
        /// <param name="id">����� �������� � ������ xml-����� �����</param>
        /// <param name="i">����� �������� � ������ xml-����� �����</param>
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
        /// �������, ���������� �� ���������� ���� �� +1 ��������
        /// </summary>
        /// <param name="node">���� ������</param>
        /// <param name="id">����� �������� � ������ xml-����� �����</param>
        /// <param name="i">����� �������� � ������ xml-����� �����</param>
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
        /// ����� XSLT-Word-����
        /// </summary>
        /// <param name="node">���� ������</param>
        /// <param name="id">����� 1</param>
        /// <param name="i">����� 2</param>
        /// <returns></returns>
		private int WriteXSLT(Node node, int id, int i)
		{
			//Node xx = node.parent;
			//Node xx2 = node.
			string prefix = "xsl:";
			Child ch = (Child) node.list[id];
			int count = 0;
			//������� �� ������ ��������� �� ��������
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
					//���� ��� keyed
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
					//�.�. �� ���������� ���� �� � ����� ��������, ������ �� 1 � ������ ������ for-each � ��������� ���
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
        /// ������� �-�� �������� ��� ind-��� ��������.
        /// </summary>
        /// <param name="listXml">������ XML-�����</param>
        /// <param name="ind">����� ����</param>
        /// <param name="p">�������� (������� ���������)</param>
		private void GetParent(ArrayList listXml, int ind, Parametrs p)
		{
			//Parametrs p = new Parametrs();
			//�-�� �������� ������� ��� i-��� ��������.
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
        /// ����� ������� ����
        /// </summary>
        /// <param name="node">����</param>
        /// <param name="isSingle">Single ��� Array</param>
        /// <param name="parent">���������</param>
		private void Write(Node node, bool isSingle, Parametrs parent)
		{
			int id = 0;
			for (int i=0;i<node.listXml.Count;i++)
			{
				XmlNodeValue val = (XmlNodeValue) node.listXml[i];
				if (val.value == null)
				{
					//���� ����� Child
					int count = 0;
					Child ch = (Child) node.list[id];
					//����� ������ ���� ��������� � ������� ����������� XSLT
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
						//���� ������� ���������� ������...
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

					//����� ������ ���� ��������� � ������� ����������� XSLT
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
        /// ����� ��������� <see cref="Write(Node,bool,Parametrs)"/>
        /// </summary>
        /// <param name="node">���������</param>
		private void Write(Node node)
		{
			Write(node, false, null);
		}
        /// <summary>
        /// ����� ��������� <see cref="Write(Node,bool,Parametrs)"/>
        /// </summary>
        /// <param name="node">���������</param>
        /// <param name="isSingle">Single ��� Array</param>
		private void Write(Node node, bool isSingle)
		{
			Write(node, isSingle, null);
		}
        /// <summary>
        /// ����� XSLT-����
        /// </summary>
		public void WriteWord()
		{
			WriteWord(Head);
			tw.Flush();
			tw.Close();
		}
        /// <summary>
        /// ����� ���������
        /// </summary>
        /// <param name="node"></param>
		private void WriteWord(Node node)
		{
			//tw.WriteStartElement(node.)
			for (int i=0; i<node.list.Count; i++)
			{
                Child ch = (Child) node.list[i];
				tw.WriteStartElement(ch.value);
                //����� ��������
				for (int j=0; j<ch.child.att.Count; j++)
				{
                    XmlNodeValue val = (XmlNodeValue) ch.child.att[j];
					tw.WriteAttributeString(val.name, val.value);
				}
				//����� ����� ��� child
                WriteWord(ch.child);
				tw.WriteEndElement();
			}
		}
	};
    /// <summary>
    /// ����� ������ �������� WORD-XML-����.
    /// ���� ����� ��������� "���� Word" - ������� ���� ������� ������� Word-��
    /// � ��� �� XSD-����, ������� ������ ������������ �� �����
    /// </summary>
	class ReadWordXml
	{
		private string URI;
		private string XMLNS = null;
		private string XmlFileName;
		private XmlTextReader tr;
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="fileName">��� �������� �����</param>
        /// <param name="URI">URI-����� (���������� ���)</param>
        public ReadWordXml(string fileName, string URI)
		{
            this.XmlFileName = fileName;
			//������� ����� ������
			FileStream file = new FileStream(this.XmlFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			tr = new XmlTextReader(file);
			if (tr == null)
				throw new FileNotFoundException("������. �� ������ ����: " + this.XmlFileName);

			this.URI = URI;
		}
        /// <summary>
        /// ������
        /// </summary>
		private Node Storage = new Node(null);
        /// <summary>
        /// ������� ����
        /// </summary>
		private Node CurNode;
		/// <summary>
        /// ������� ��������� ������ ����� � ���������� � this.Storage
		/// </summary>
		public void Read()
		{
			bool cont = false;
			this.CurNode = this.Storage;
            //������ word-��������
			//������ ��������� � ������� 
			ReadMainTag(CurNode);
			//���� XMLNS = null  �� ������...

			//read 
			while(tr.Read())
			{
				//���� ��� ���� word-a (�� ������� �� ���� ����������)
				if ( tr.Name.Length > XMLNS.Length)
				{

					if (this.XMLNS == tr.Name.Substring(0,XMLNS.Length))
					{
						//���� ������, �� ��������� child
						if (tr.NodeType == XmlNodeType.Element)
						{
							CurNode = this.CurNode.AddChild(tr.Name.Substring(XMLNS.Length +1));
							bool isEnd = tr.IsEmptyElement;

							//����� ������ ��������� �� ��������� �������...
							//��������!!!!
							if (tr.AttributeCount > 0)
							{
                            
								tr.MoveToFirstAttribute();
								while (true)
								{
									//��������� �� ���� ���������
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
						//���� ����� �������� ��-��, �� ���������� �������
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
								//��������� �� ���� ���������
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
							//��������� �� ���� ���������
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
		/// ������ �� Storge
		/// </summary>
		public Node node
		{
			get
			{
				return this.Storage;
			}
		}
        /// <summary>
        /// ������ ��� ����(������ ��� �������) ������� �������� � Word ����� XSD-������
        /// �������� ns0:
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
				throw new Exception("������, � Word ��������� �� ������� ����� � URI: " + this.URI);
		}
	};
    /// <summary>
    /// �����-����������, �������� ������ �� ������� ����, �����, � �������� XSLT-Word-���� � �� URI
    /// </summary>
	class Comp2
	{
        /// <summary>
        /// ������� XML-����
        /// </summary>
		public string inputXMLFile;
        /// <summary>
        /// ������� XSD-�����
        /// </summary>
		public string inputXSDFile;
        /// <summary>
        /// �������� XML-����
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
	/// ����� �������� ������� MAIN
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
					Console.WriteLine("//������� ���������� ��������� ������ �����....");
					Console.WriteLine("// - WordXML		���_�����		-	��� �������� WORD �����");
                    Console.WriteLine("// - ExcelXML	���_�����		-	��� �������� Excel �����");
					Console.WriteLine("// - readXSD		���_�����		-	��� ������� ����� XSD");
					Console.WriteLine("// - outXSL		���_�����		-	��� ��������� XSLT �����");
					Console.WriteLine("// - uri			���_URI			-	URI (��� ���������)");
					Console.WriteLine("// - ?							-	������");
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
                    Console.WriteLine("������ �������� xsd: " + ex);
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
