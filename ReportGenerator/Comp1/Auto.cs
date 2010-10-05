using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
//using Saxon.Api;
using System.CodeDom.Compiler;
//using Saxon.dotnet;

namespace Comp1
{
	
	//const string SELECT = "";


	/// <summary>
	/// ��� ���� ���� � SQL �������: ������������ � �������������� ������
	/// </summary>
	public enum TypeWord {TERM, NonTERM};
	/// <summary>
	/// �������� ���������
	/// </summary>
	public enum TypeOper {BOLSHE, MENSHE, RAVNO, NERAVNO, MENSHERAVNO, BOLSHERAVNO, NULL};
	public enum ElementType {STRING, DATE, INT, DECIMAL};
	/// <summary>
	/// Exception, ������� ��������� ��� ��������� � XSD ������
	/// </summary>
	class NotExistException : ApplicationException
	{
		public NotExistException(string str) : 
			base("������ � ����������: " + str)
		{}

		public NotExistException(string str, Exception innerException) : 
			base("������ � ����������: " + str, innerException)
		{}
	};

	/// <summary>
	/// ����� ������, ����� �� ������� ����������� ��������� 
	/// </summary>
	class NoOneClass : ApplicationException
	{
        public NoOneClass(string str) : 
			base("�� ������� �������� ���������� ���������.\n " + str)
		{}

		public NoOneClass(string str, Exception innerException) : 
			base("�� ������� �������� ���������� ���������. \n " + str, innerException)
		{}

	};
    /// <summary>
    /// ������ � ���� ����� ��� �������� �����
    /// </summary>
	public class TheWord
	{
		public string text;
		
		public TypeWord type;
		public TheWord(string str, TypeWord type )
		{
			this.type = type;
			this.text = str;
		}
	};
	/// <summary>
	/// ������� ����� ��������� ��������
	/// </summary>
	abstract class Base
	{
		protected Link mainClass = null;

		public Base link
		{
			get { return mainClass.CurState;}
			set { mainClass.CurState = value;}
		}
		public Base(Link l)
		{
			mainClass = l;
		}
		public abstract bool NextState(TheWord word);
		//public static Base mainClass = null;
	};

    /// <summary>
    /// ����� ��������. ���������� ��� �������� �������. ��������� ��� ����������� ��������: ������� ��������, ������ ����, ������� XSD, XSLT
    /// </summary>
	public class Auto
	{
		private string inXSD;
		private string inSQL;
		private string outXSL;
		private string outXSD;
		private string URI;
		private string URIText;
		private string Manifest;
		private Link l;
        /// <summary>
        /// ������� ��� ����������� ��� ������
        /// </summary>
        /// <param name="inputXSD">����� ������</param>
        /// <param name="inputSQL">SQL-����</param>
        /// <param name="outputXSL">���� �� ��������� XSL-file</param>
        /// <param name="outputXSD">���� �� ��������� XSD-file</param>
        /// <param name="URI">URI - ����� APOF1</param>
        /// <param name="URIText">�������� ��� WORD</param>
        /// <param name="Manifest">���� �� ���������</param>
        /// <returns></returns>
		public bool Create(string inputXSD, string inputSQL, string outputXSL, string outputXSD, string URI, string URIText, string Manifest)
		{
			this.URIText = URIText;
			this.URI = URI;
            this.inSQL = inputSQL;
            this.inXSD = inputXSD;
			this.outXSD = outputXSD;
			this.outXSL = outputXSL;
			this.Manifest = Manifest;
			if (inSQL == null || inXSD == null || outXSD == null || outXSL == null)
			{
				return false;
			}

			//������� �������
			TextReader tr = new StreamReader(inSQL, System.Text.Encoding.Default);
			l = new Link(tr);
			return true;
		}
        /// <summary>
        /// ����� ��������
        /// </summary>
		public void WriteManifest()
		{
			int Ver = 1;
			int SubVer = 1;
			//������� ������ Manifest ���� � ���������� ��� ������.

			try
			{
				if (Manifest == null)
				{
					Manifest = URI + "M.xml";
				}
				TextReader read = new StreamReader(Manifest);
				XmlTextReader tr = new XmlTextReader(read);
				
				bool ok = false;
				string version = "";
				while (tr.Read())
				{
					string str = tr.Name;
					if (ok && tr.NodeType == XmlNodeType.Text)
					{
                        version = tr.Value.Trim();
						if (version.IndexOf(".") != -1 && version.Length > 1)
						{
							int id = version.IndexOf(".");
							Ver = int.Parse(version.Substring(0,id));
							SubVer = int.Parse(version.Substring(id+1));
						}
						tr.Close();
						break;
					}
                    if (str.ToLower().IndexOf("version") != -1 && tr.NodeType == XmlNodeType.Element)
                    {
						ok = true;
                    }
				}
				//��������� ����...
			}
			catch (FileNotFoundException e)
			{
				Ver = 1;
				SubVer = 1;                
			}
			if (URI == null || URI == "")
                throw new ArgumentNullException("�������� URI �� �����.");

			if (URIText == null || URIText == "")
			{
                URIText = "IntecoAG " + URI.ToUpper();
			}
			
			TextWriter manifest = new StreamWriter(URI + "M.xml");
			manifest.WriteLine("<SD:manifest xmlns:SD=\"http://schemas.microsoft.com/office/xmlexpansionpacks/2003\">");
			manifest.WriteLine("<SD:version>"+ Ver.ToString() + "." + (SubVer+1).ToString() +"</SD:version>");
			manifest.WriteLine("<SD:updateFrequency>20160</SD:updateFrequency>");
			manifest.WriteLine("<SD:uri>" + URI.ToUpper() + "</SD:uri>");
			manifest.WriteLine("<SD:solution>");
			manifest.WriteLine("<SD:solutionID>schema</SD:solutionID>");
			manifest.WriteLine("<SD:type>schema</SD:type>");
			manifest.WriteLine("<SD:alias lcid=\"*\">" + URIText + "</SD:alias>");
			manifest.WriteLine("<SD:file>");
			manifest.WriteLine("<SD:type>schema</SD:type>");
			manifest.WriteLine("<SD:version>1.0</SD:version>");
			manifest.WriteLine("<SD:filePath>" + URI + "S.xsd</SD:filePath>");
			manifest.WriteLine("</SD:file>");
			manifest.WriteLine("</SD:solution>");
			manifest.WriteLine("</SD:manifest>");
			manifest.Close();
		}
        /// <summary>
        /// ��������� ������� �� ����������
        /// </summary>
		public void Run()
		{
			//������� Manifest
			WriteManifest();

			//� ���������� � list ����� ���� ������ SQL ��������
			ArrayList list = new ArrayList();
			while (l.Run())
			{
				l.ResetState();
				list.Add(l.data);
			}
			SQLStruct st = Simplify(list);

			//Console.WriteLine("SQL: ");
			//st.Print();

			XMLclass x = new XMLclass(st);
			x.LoadXSD(this.inXSD);
			x.Simplify();
			x.Validate();

			x.ReCount();
			//string DataSetName = x.GetDataSetName();

			//����� XSL


			TextWriter tw = new StreamWriter(this.outXSL);
			//x.WriteXSL(tw);
			XSL xsl = new XSL(tw, st, x.GetDataSetName(), l.isGroup);
			xsl.WriteXSL();

			//����� XSD
			TextWriter tw_xsd = new StreamWriter(this.outXSD);
			/*x.WriteXSD(tw_xsd);*/
			XSD xsd = new XSD(tw_xsd,st,x.GetDataSetName());
			xsd.WriteXSD();

			//return st;
		}
        /// <summary>
        /// �������� XSLT-��������������
        /// </summary>
        /// <param name="inputXML"></param>
        /// <param name="outXML"></param>
		public void Execute(string inputXML, string outXML)
		{
			/*XPathDocument doc = new XPathDocument(inputXML);
			XslTransform tr1 = new XslTransform();
			tr1.Load(this.outXSL);
			
			XPathNavigator nav = ((IXPathNavigable)doc).CreateNavigator();

			FileStream fs = new FileStream(outXML, FileMode.Create);
			tr1.Transform(nav,null,fs);
			fs.Close();*/
			/*Processor processor = new Processor();

			
			//outXSL = ".\\" + outXSL;
			//outXML = ".\\" + outXML;

			string CurDir = Directory.GetCurrentDirectory();

			//Console.WriteLine("XSL = " + Path.Combine(CurDir,outXSL));
			//Console.WriteLine("XML = " + Path.Combine(CurDir,outXML));
			//Console.WriteLine("XML = " + Path.Combine(outXML);

			//FileStream f = new FileStream(inputXML,FileMode.Open);
			// Load the source document
			XdmNode input = processor.NewDocumentBuilder().Build(new Uri(Path.Combine(CurDir,inputXML)));

			
			//DirectoryName()

			
			//outXSL = "c:\\r26\\templates\\m\\os\\mosf1c.xsl";
			//outXML = "c:\\r26\\templates\\m\\os\\mosf1d.xml";
			// Create a transformer for the stylesheet.
			FileStream f1 = new FileStream(outXSL,FileMode.Open);
			XsltTransformer transformer = processor.NewXsltCompiler().Compile(new Uri(Path.Combine(CurDir,outXSL))).Load();

			// Set the root node of the source document to be the initial context node
			transformer.InitialContextNode = input;

			// Create a serializer
			Serializer serializer = new Serializer();
			serializer.SetOutputStream(new FileStream(outXML, FileMode.Create, FileAccess.Write));

			// Transform the source XML to System.out.
			transformer.Run(serializer);


            
			/*System.CodeDom.Compiler.Executor exe;
			TempFileCollection file = new TempFileCollection();
			System.CodeDom.Compiler.Executor.ExecWait("Transform.exe " + inputXML + " " + outXSL + " > " + outXML,file);

            
			return;*/

			/*Processor processor = new Processor();

			
			//outXSL = ".\\" + outXSL;
			//outXML = ".\\" + outXML;

			string CurDir = Directory.GetCurrentDirectory();

			//Console.WriteLine("XSL = " + Path.Combine(CurDir,outXSL));
			//Console.WriteLine("XML = " + Path.Combine(CurDir,outXML));
			//Console.WriteLine("XML = " + Path.Combine(outXML);

			//FileStream f = new FileStream(inputXML,FileMode.Open);
			// Load the source document
			XdmNode input = processor.NewDocumentBuilder().Build(new Uri(Path.Combine(CurDir,inputXML)));

			
			//DirectoryName()

			
			//outXSL = "c:\\r26\\templates\\m\\os\\mosf1c.xsl";
			//outXML = "c:\\r26\\templates\\m\\os\\mosf1d.xml";
			// Create a transformer for the stylesheet.
			FileStream f1 = new FileStream(outXSL,FileMode.Open);
			XsltTransformer transformer = processor.NewXsltCompiler().Compile(new Uri(Path.Combine(CurDir,outXSL))).Load();

			// Set the root node of the source document to be the initial context node
			transformer.InitialContextNode = input;

			// Create a serializer
			Serializer serializer = new Serializer();
			serializer.SetOutputStream(new FileStream(outXML, FileMode.Create, FileAccess.Write));

			// Transform the source XML to System.out.
			transformer.Run(serializer);*/


		}
        /// <summary>
        /// ������� ������ �� SQL ��������, ���� ��� �������� � ���� ������
        /// </summary>
        /// <param name="st">�������� SQL</param>
        /// <param name="list">������ SQLStruct</param>
        /// <param name="baseSQL">������� SQL</param>
		private void MakeTreeFromList(SQLStruct st, ArrayList list, SQLStruct baseSQL)
		{
			for (int j=0; j<st.select.Count; j++)
			{
				for (int i=1; i<list.Count; i++)
				{
					SQLStruct node = (SQLStruct) list[i];

					if (st.select[j] == node.As[0])
					{
						node.parentSQL = st;
						node.baseSQL = st;
						st.select.Set(node, j);
						MakeTreeFromList(node, list, baseSQL);
						break;
					}
				}
			}
		}
        /// <summary>
        /// �������� ���������, �������� ������� ������ �� ������
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
		private SQLStruct Simplify(ArrayList list)
		{
			//������ �� ����������������� ������, ���������
            if (list.Count == 1)
				return (SQLStruct) list[0];

			SQLStruct st = (SQLStruct) list[0];

			//�������� ������� SQL
			st.baseSQL = null;
			st.parentSQL = null;

			MakeTreeFromList(st, list, st);

            return st;
		}
	}
	
	/// <summary>
	/// �� ������������ ����� ����������� ���������� SQLStruct � �����
	/// </summary>
	class XMLclass
	{
		private SQLStruct sql;
		private string XMLNS = "http://www.w3.org/1999/XSL/Transform";
		//private string MainTag;
		private DataSet ds;

		public void ReCount()
		{
			sql.ReCount(0);
		}
		public string GetDataSetName()
		{
			return ds.DataSetName;
		}
		public XMLclass(SQLStruct sql)
		{
            this.sql = sql;
		}
		public void LoadXSD(string xsd)
		{
			ds = new DataSet();
			if (xsd !=null)
			{
				ds.ReadXmlSchema(xsd);
			}
		}
		public void Simplify()
		{
			sql.Siplify(ds);
		}
		public bool Validate()
		{
			sql.Validate(ds);
            return true;
		}

		/*private void WriteHeadXSL(TextWriter sw)
		{
			sw.WriteLine("<xsl:transform xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">");
			//outxd.WriteLine("<?xml version=\"1.0\"?>\n");
			//xd.WriteLine("<xs:schema id=\"IntekoDataSet\" targetNamespace=\""+ this.NameSpace +"\" xmlns:mstns=\"SimpleSample\" xmlns=\""+this.XMLNS+"\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:msdata=\"urn:schemas-microsoft-com:xml-msdata\" attributeFormDefault=\"qualified\" elementFormDefault=\"qualified\">");
			//outxd.WriteLine("<xs:schema id=\"IntekoDataSet\" xmlns:mstns=\"SimpleSample\" xmlns=\""+this.XMLNS+"\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:msdata=\"urn:schemas-microsoft-com:xml-msdata\" attributeFormDefault=\"qualified\" elementFormDefault=\"qualified\">");
		}

		private void WriteEndXSL(TextWriter sw)
		{
			sw.WriteLine("</xsl:transform>");
			sw.Close();
		}*/

		private void WriteHeadXSD(TextWriter sw)
		{
			//sw.WriteLine("<xsl:transform xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">");
			sw.WriteLine("<?xml version=\"1.0\"?>\n");
			sw.WriteLine("<xs:schema id=\"IntekoDataSet\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:msdata=\"urn:schemas-microsoft-com:xml-msdata\" attributeFormDefault=\"qualified\" elementFormDefault=\"qualified\">");
			//outxd.WriteLine("<xs:schema id=\"IntekoDataSet\" xmlns:mstns=\"SimpleSample\" xmlns=\""+this.XMLNS+"\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:msdata=\"urn:schemas-microsoft-com:xml-msdata\" attributeFormDefault=\"qualified\" elementFormDefault=\"qualified\">");
		}

		private void WriteEndXSD(TextWriter sw)
		{
			//sw.WriteLine("</xsl:transform>");
			sw.WriteLine("</xs:schema>");
			sw.Close();
		}

		public void WriteXSD(TextWriter tw)
		{
			WriteHeadXSD(tw);
			//��� ������ ����� �����
			tw.WriteLine("<xs:element name=\"" + sql.As[0] + "\">\n\t<xs:complexType>\n\t<xs:choice maxOccurs=\"unbounded\">");
			
			for (int i=0;i< sql.select.Count; i++)
			{
				if (sql.select.isString(i))
				{
					string node = sql.select[i];
					
					switch (sql.select.GetType(i))
					{
						case ElementType.DECIMAL:
							tw.WriteLine("<xs:element name=\"" + node + "\">");
							tw.WriteLine("<xs:complexType><xs:attribute name=\"formatting_number\"/></xs:complexType>");
							break;
						case ElementType.DATE:
							tw.WriteLine("<xs:element name=\"" + node + "\" >");
							tw.WriteLine("<xs:complexType><xs:attribute name=\"formatting_date\"/></xs:complexType>");
							break;
						default:
							tw.WriteLine("<xs:element name=\"" + node + "\" type=\"xs:string\" minOccurs=\"0\">");
							break;
					}
					tw.WriteLine("</xs:element>");
				}
				else
				{
					WriteXSD(tw,sql.select.GetSQL(i));
				}
			}
			tw.WriteLine("\t</xs:choice>\n\t</xs:complexType>\n</xs:element>");
			this.WriteEndXSD(tw);
		}

		public void WriteXSD(TextWriter tw, SQLStruct sql)
		{
			//WriteHeadXSD(tw);
			//��� ������ ����� �����
			tw.WriteLine("<xs:element name=\"" + sql.As[0] + "\">\n\t<xs:complexType>\n\t<xs:choice maxOccurs=\"unbounded\">");
			//tw.WriteLine("<" + sql.from[0] + ">");
			tw.WriteLine("<xs:element name=\"tr-" + sql.from[0] + "\" minOccurs=\"0\" maxOccurs=\"unbounded\"><xs:complexType><xs:sequence>");
			
			for (int i=0;i< sql.select.Count; i++)
			{
				if (sql.select.isString(i))
				{
					string node = sql.select[i];
					//tw.WriteLine("<xs:element name=\"" + node + "\" type=\"xs:string\" minOccurs=\"0\" />");
					switch (sql.select.GetType(i))
					{
						case ElementType.DECIMAL:
							tw.WriteLine("<xs:element name=\"" + node + "\" >");
							tw.WriteLine("<xs:complexType><xs:attribute name=\"formatting_number\"/></xs:complexType>");
							break;
						case ElementType.DATE:
							tw.WriteLine("<xs:element name=\"" + node + "\" >");
							tw.WriteLine("<xs:complexType><xs:attribute name=\"formatting_date\"/></xs:complexType>");
							break;
						default:
							tw.WriteLine("<xs:element name=\"" + node + "\" type=\"xs:string\" minOccurs=\"0\">");
							break;
					}
					tw.WriteLine("</xs:element>");
				}
				else
				{
					//outxd.WriteLine("<xs:element name=\"" + node + "\" minOccurs=\"0\" maxOccurs=\"unbounded\"><xs:complexType>\n\t<xs:sequence>");
					WriteXSD(tw,sql.select.GetSQL(i));
				}
			}
            tw.WriteLine("</xs:sequence></xs:complexType></xs:element>");
			tw.WriteLine("\t</xs:choice>\n\t</xs:complexType>\n</xs:element>");
			//this.WriteEndXSD(tw);
		}

		//public void WriteXSL
		public void WriteXSL(TextWriter tw)
		{
			//XSL x = new XSL(tw, sql, ds.DataSetName);
			//x.WriteXSL();
			/*WriteHeadXSL(tw);
            //��� ������ ����� �����
			tw.WriteLine("<xsl:template match=\"" + ds.DataSetName + "\"><" + sql.As[0] + " xmlns:xsl=\"" + XMLNS + "\">");
			
			tw.WriteLine("\t\t<xsl:for-each select=\"//" + ds.DataSetName + "//" + sql.from[0] + "\">");
            for (int i=0;i< sql.select.Count; i++)
            {
				if (sql.select.isString(i))
				{
					string node = sql.select[i];
					tw.WriteLine("\t\t<" + node + "><xsl:value-of select=\"" + node + "\"/></" + node + ">");
				}
				else
				{
					WriteXSL(tw,sql.select.GetSQL(i));
				}
            }
			tw.WriteLine("\t\t</xsl:for-each>");
			tw.WriteLine("</" + sql.As[0] +"></xsl:template>");
			this.WriteEndXSL(tw);*/
		}

		private void WriteXSL(TextWriter tw, SQLStruct sql)
		{
			tw.WriteLine("<" + sql.As[0] + ">");
			
			tw.WriteLine("\t\t<xsl:for-each select=\"//" + ds.DataSetName + "//" + sql.from[0] + "\">");
			tw.WriteLine("<tr-" + sql.from[0] + ">");
			for (int i=0;i< sql.select.Count; i++)
			{
				if (sql.select.isString(i))
				{
					string node = sql.select[i];
					tw.WriteLine("\t\t<" + node + "><xsl:value-of select=\"" + node + "\"/></" + node + ">");
				}
				else
				{
					
					WriteXSL(tw,sql.select.GetSQL(i));
				}

			}
			tw.WriteLine("</tr-" + sql.from[0] + ">");
			tw.WriteLine("\t\t</xsl:for-each>");
			tw.WriteLine("</" + sql.As[0] +">");
		}
	};
}
