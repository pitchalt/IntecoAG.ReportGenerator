using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using IntecoAG.iagcomp2;

namespace iagcomp2
{
    class XPath
    {
        public string Value { get; private set; }

        private XPath() { }
        public XPath(string xpath)
        {
            Value = xpath;
        }

        public static XPath Combine(XPath xPath1, XPath supXPath)
        {
            XPath xpath = new XPath(xPath1.Value);
            xpath.Value += supXPath.Value;
            return xpath;
        }

        public static XPath Intersect(XPath xPath1, XPath xPath2)
        {
            string result = xPath1.Value.Replace(xPath2.Value, "");
            XPath xpath = new XPath(result);
            return xpath;
        }
    }

    class Range
    {
        public string Value { get; private set; }

        public string WS { get; private set; }
        
        public int Row1 { get; private set; }
        
        public int Column1 { get; private set; }

        public int Row2 { get; private set; }
        
        public int Column2 { get; private set; }

        public int RowsCount  { get { return Row2 - Row1 + 1; } }

        public int ColumnsCount { get { return Column2 - Column1 + 1; } }

        private Range() { }

        public Range(string range)
        {
            Value = range;

            bool multiRange = false;

            int spliter = Value.IndexOf(":");

            if (spliter > 0)
                multiRange = true;

            
            int row1 = 0, column1 = 0;
            string ws1 = string.Empty;
            Parse(range.Substring(0, spliter > 0 ? spliter : range.Length) , out row1, out column1, out ws1);

            WS = ws1;
            Row1 = row1;
            Column1 = column1;

            if (multiRange)
            {
                int row2, column2;
                string ws2 = string.Empty;
                Parse(range.Substring(spliter + 1), out row2, out column2, out ws2);

                Row2 = row2;
                Column2 = column2;
            }
            else
            {
                Row2 = Row1;
                Column2 = Column1;
            }
        }

        private void Parse(string range, out int row, out int column, out string ws)
        {
            /*парсим первый range*/
            string workSpace = string.Empty;
            string rowStr = string.Empty;
            string columnStr = string.Empty;
            if (range.IndexOf('!') > 0)
                workSpace = range.Substring(0, range.IndexOf('!'));

            ws = workSpace;

            int indexOfR = range.IndexOf('R');
            int indexOfC = range.IndexOf('C');
            if (indexOfR >= 0 && indexOfC >= 0)
            {
                rowStr = range.Substring(indexOfR + 1, indexOfC - indexOfR - 1);
                columnStr = range.Substring(indexOfC + 1);
            }

            rowStr = rowStr.Trim(new char[] {'[', ']'});
            columnStr = columnStr.Trim(new char[] { '[', ']' });

            if (!string.IsNullOrEmpty(rowStr))
            {
                if (!int.TryParse(rowStr, out row))
                    throw new Exception("Invalid parse: " + rowStr);
            }
            else
                row = 0;


            if (!string.IsNullOrEmpty(columnStr))
            {
                if (!int.TryParse(columnStr, out column))
                    throw new Exception("Invalid parse: " + columnStr);
            }else
                column = 0;

        }

        /// <summary>
        /// Объединяет ренжи
        /// </summary>
        /// <param name="range1"></param>
        /// <param name="range2"></param>
        /// <returns></returns>
        public static Range Combine(Range range1, Range range2)
        {
            Range range = new Range();

            range.Value = string.Format("{0}+{1}", range1, range2);
            range.WS = string.IsNullOrEmpty(range1.WS) ? range2.WS : range1.WS;
            range.Row1 = range1.Row1 + range2.Row1;
            range.Column1 = range1.Column1 + range2.Column1;

            range.Row2 = range1.Row1 + range2.Row2;
            range.Column2 = range1.Column1 + range2.Column2;

            return range;
        }
    }

    class Global
    {
        public const string SS = "urn:schemas-microsoft-com:office:spreadsheet";
        
        public const string S2 = "http://schemas.microsoft.com/office/excel/2003/xml";

        public const string XSL = "http://www.w3.org/1999/XSL/Transform";

        public const string Default = "urn:schemas-microsoft-com:office:spreadsheet";

        public const string XsltVersion = "1";
    }

    class ExcelReader
    {  
        XmlDocument _document;
        public ExcelReader(string file)
        {
            _document = new XmlDocument();
            if (!File.Exists(file))
                throw new Exception("File doesn't exist: " + file);

            _document.Load(file);
        }

        public ExcelDocument Process()
        {
            return new ExcelDocument(_document);
        }
    }

    abstract class AbstaractElement
    {
        public XmlNode Node { get; set; }

        protected XmlDocument Document
        {
            get
            {

                return Node.OwnerDocument ?? Node as XmlDocument;
            }
        }

        protected AbstaractElement(XmlNode node)
        {
            Node = node;
        }


    }

    class Table : AbstaractElement
    {
        private Dictionary<int, Row> _rows = new Dictionary<int, Row>();

        public Table(XmlNode node) : base(node)
        {
            if (node.Name != "Table")
                throw new Exception("Invalid type: " + node.Name);

            XmlAttribute expandedRowCount = node.Attributes["ExpandedRowCount", Global.SS];
            expandedRowCount.Value = "100" + expandedRowCount.Value;

            int index = 0;
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "Row")
                {
                    Row row = new Row(child, index, this);
                    _rows.Add(row.Index, row);

                    index = row.Index;
                }
            }
        }

        public Cell this[int indexOfR, int indexOfC]
        {
            get
            {
                if (!_rows.ContainsKey(indexOfR))
                    return null;
                return _rows[indexOfR][indexOfC];
            }
        }

        public void AddCount(string xpath)
        {
            
        }
    }

    class WorkSheet : AbstaractElement
    {
        private Table _table;

        public WorkSheet(XmlNode item) : base(item)
        {
            //Заполяем Имя листа
            XmlAttribute wsName = item.Attributes["Name", Global.SS];
            if (wsName == null)
                return;
            Name = wsName.Value;

            //Далее загружаем строки

            XmlNode table = item["Table"];
            if (table == null)
                _table = null;
            else
                _table = new Table(table);            
        }
        
        public string Name { get; private set; }

        public Cell this[int indexOfR, int indexOfC]
        {
            get
            {
                if (_table == null)
                    return null;
                return _table[indexOfR, indexOfC];
            }
        }
    }

    class ExcelDocument : AbstaractElement
    {
        public WorkBook WoorkBook { get; protected set; }

        public ExcelDocument(XmlDocument node) 
            : base(node)
        {
            XmlNode workBook = node["Workbook"];
            if (workBook == null)
                throw new Exception("Missing element Workbook");

            WoorkBook = new WorkBook(workBook);
        }

        public XmlNode GenerateXSLT(XSD xsd)
        {
            XmlNode stylesheet = Document.CreateElement("xsl:stylesheet", Global.XSL);
            {
                XmlAttribute version = Document.CreateAttribute("version");
                version.InnerText = Global.XsltVersion;
                stylesheet.Attributes.Append(version);
            }

            Document.ReplaceChild(stylesheet, WoorkBook.Node);

            {
                XmlNode output = Document.CreateElement("xsl:output", Global.XSL);

                XmlAttribute method = Document.CreateAttribute("method");
                method.InnerText = "xml";
                output.Attributes.Append(method);

                XmlAttribute encoding = Document.CreateAttribute("encoding");
                encoding.InnerText = "utf-8";
                output.Attributes.Append(encoding);

                stylesheet.AppendChild(output);
            }

            XmlNode template = Document.CreateElement("xsl:template", Global.XSL);
            {
                XmlAttribute match = Document.CreateAttribute("match");
                match.InnerText = "/";
                template.Attributes.Append(match);
            }

            XmlNode forEach = Document.CreateElement("xsl:for-each", Global.XSL);
            {
                XmlAttribute select = Document.CreateAttribute("select");
                select.Value = "/" + xsd.RootNode;

                forEach.Attributes.Append(select);
            }

            stylesheet.AppendChild(template);

            template.AppendChild(forEach);
            forEach.AppendChild(WoorkBook.Node);

            XmlProcessingInstruction instruction = null;

            foreach (XmlNode item in Node.ChildNodes)
            {
                if (item is XmlProcessingInstruction)
                    instruction = item as XmlProcessingInstruction;
            }
            if (instruction != null)
            {
                XmlNode xslProcessingInstruction = Document.CreateElement("xsl:processing-instruction", Global.XSL);

                {
                    XmlNode xslProcessingInstructionText = Document.CreateElement("xsl:text", Global.XSL);
                    xslProcessingInstructionText.InnerText = instruction.Value;
                    xslProcessingInstruction.AppendChild(xslProcessingInstructionText);
                }

                XmlAttribute xslProcessingInstructionName = Document.CreateAttribute("name");
                xslProcessingInstructionName.Value = instruction.Name;

                xslProcessingInstruction.Attributes.Append(xslProcessingInstructionName);

                template.InsertBefore(xslProcessingInstruction, template.LastChild);
                Node.RemoveChild(instruction);
            }

            WoorkBook.GenerateXSLT();

            return stylesheet;
        }
    }

    class WorkBook : AbstaractElement
    {
        /// <summary>
        /// Список листов
        /// </summary>
         Dictionary<string, WorkSheet> _list = new Dictionary<string, WorkSheet>();

        /// <summary>
        /// Преобразование xsd -> xls
        /// </summary>
        private MapInfo _map;

        public WorkBook(XmlNode node)
            : base(node)
        {
            foreach (XmlNode item in node.ChildNodes)
            {
                if (item.LocalName == "Worksheet")
                {
                    WorkSheet ws = new WorkSheet(item);
                    _list.Add(ws.Name, ws);
                }
                else if (item.LocalName == "MapInfo" && item.NamespaceURI == Global.S2)
                {
                    _map = new MapInfo(item);
                }
            }
        }

        public Cell this[Range range]
        {
            get
            {
                WorkSheet sheet = default(WorkSheet);

                if (!string.IsNullOrEmpty(range.WS) && _list.ContainsKey(range.WS))
                    sheet = _list[range.WS];
                else
                {
                    var en = _list.Values.GetEnumerator();
                    if (en.MoveNext())
                        sheet = en.Current;
                }

                if (sheet == null)
                    throw new Exception("Не существует листа: " + range.WS);

                return sheet[range.Row1, range.Column1];
            }


        }

        public void GenerateXSLT()
        {
            //Пробегам по всем параметрам
            foreach (Entry entry in _map.Map)
            {
                Range range = entry.Range; //Получаем путь в таблице

                Cell cellRow = this[range];

                Row row = cellRow.Row;

                row.SetXPathForEach(entry.XPath, _map.SelectionNamespaces);

                foreach (Field field in entry.Fields)
                {
                    XPath xpath = entry.XPath;
                    //Плолучаем путь до ячейки
                    Cell cell = field.Range == null ?
                        this[range] : this[Range.Combine(range, field.Range)];

                    if (cell == null)
                        throw new Exception("Не удалось получить ячейку: " + range);

                    row = cell.Row;

                    cell.SetXPath(field.XPath, _map.SelectionNamespaces);
                }
            }

            //Преобразуем все агрегаты



            //Удалем карту из конечного документа
            _map.Node.ParentNode.RemoveChild(_map.Node);
        }
    }

    class Row :AbstaractElement
    {
        public int Index { get; private set; }

        public Dictionary<int, Cell> _cells = new Dictionary<int, Cell>();

        public Table Table { get; private set; }

        public Row(XmlNode node, int lastIndex, Table table) : base(node)
        {
            Table = table;
            XmlAttribute attr = node.Attributes["Index", Global.SS];
            if (attr != null)
                Index = int.Parse(attr.Value);
            else
                Index = lastIndex + 1;

            int index = 0;
            foreach (XmlNode item in node.ChildNodes)
            {
                if (item.Name == "Cell")
                {
                    Cell cell = new Cell(item, index, this);
                    index = cell.Index;
                    _cells.Add(cell.Index, cell);
                }
            }
        }

        public Cell this[int column]
        {
            get
            {
                if (!_cells.ContainsKey(column))
                    throw new Exception("Invaliud column: " + column);
                return _cells[column];
            }
        }

        public void SetXPathForEach(XPath xPath, string nm)
        {
            string xpath = xPath.Value.Replace(nm + ":", "");
            XmlNode forEach = Document.CreateElement("xsl:for-each", Global.XSL);
            {
                XmlAttribute select = Document.CreateAttribute("select");
                select.Value = xpath;

                forEach.Attributes.Append(select);
            }

            Node.ParentNode.ReplaceChild(forEach, Node);
            forEach.AppendChild(Node);
        }
    }

    class Cell : AbstaractElement
    {
        public int Index { get; private set; }

        public Row Row { get; private set; }

        public Cell(XmlNode node, int lastIndex, Row row) : base(node)
        {
            Row = row;
            XmlAttribute attr = node.Attributes["Index", Global.SS];
            if (attr != null)
                Index = int.Parse(attr.Value);
            else
                Index = lastIndex + 1;
        }

        protected XmlNode InnerChild
        {
            get
            {
                XmlNode node = Node;
                while ( node.FirstChild != null 
                        && node.FirstChild.NodeType == XmlNodeType.Element 
                        && node.FirstChild.Name != "NamedCell")

                    node = node.FirstChild;
                return node;
            }
        }

        /// <summary>
        /// Добавлет к текущему узлу xslt:xpath
        /// </summary>
        /// <param name="xpath"></param>
        public void SetXPath(XPath xPath, string selectionNamespaces)
        {
            string xpath;
            if (xPath == null)
                xpath = ".";
            else
                xpath = xPath.Value.Replace(selectionNamespaces + ":", "");
            //<xsl:value-of select="xpath" />
            XmlNode node = Document.CreateElement("xsl:value-of", Global.XSL);

            XmlAttribute attribute = Document.CreateAttribute("select");
            attribute.Value = xpath;

            node.Attributes.Append(attribute);

            XmlNode innerChild = InnerChild;

            if (innerChild.Name != "Data")
            {
                XmlNode data = Document.CreateElement("Data", Global.Default);

                innerChild.AppendChild(data);

                data.AppendChild(node);

                XmlAttribute dataAttrib = Document.CreateAttribute("Type", Global.SS);
                dataAttrib.Value = "String";

                data.Attributes.Append(dataAttrib);

                
            }
            else
                InnerChild.AppendChild(node);
        }

    }

    #region Mapper
    class MapInfo : AbstaractElement
    {
        public string Name { get; set; }

        public Map Map { get; private set; }

        public string SelectionNamespaces { get; private set; }

        public MapInfo(XmlNode node)
            : base(node)
        {
            Name = node.LocalName;

            XmlAttribute selectionNamespace = Node.Attributes["SelectionNamespaces", Global.S2];
            string nm = selectionNamespace.InnerText;

            int index = nm.IndexOf(':');
            if (index > 0)
            {
                int end = nm.IndexOf('=', index);
                if (end > 0)
                    SelectionNamespaces = nm.Substring(index + 1, end - index -1);
            }

            XmlNode child = Node["Map", Global.S2];

            if (child == null)
                Map = null;
            else
                Map = new Map(child);
        }
    }

    class Map : AbstaractElement , IEnumerable<Entry>
    {
        List<Entry> _list = new List<Entry>();
        public Map(XmlNode node)
            : base(node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.LocalName == "Entry" && child.NamespaceURI == Global.S2)
                {
                    Entry entry = new Entry(child);
                    _list.Add(entry);
                }
            }
        }

        #region Члены IEnumerable<Entry>

        public IEnumerator<Entry> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region Члены IEnumerable

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion
    }

    class Entry : AbstaractElement
    {
        public Range Range { get; private set; }

        public string HeaderRange { get; private set; }

        public XPath XPath { get; private set; }

        public Field[] Fields { get; private set; }

        public Entry(XmlNode node)
            : base(node)
        {
            XmlNode xmlRange = node["Range", Global.S2];
            if (xmlRange == null)
                throw new Exception("Missing range");

            Range = new Range(xmlRange.InnerText);

            XmlNode xmlHeaderRange = node["HeaderRange", Global.S2];

            if (xmlHeaderRange == null)
            {
                //Warning no one HeaderRange 
            }
            if (xmlHeaderRange != null)
                HeaderRange = xmlHeaderRange.InnerText;

            XmlNode xmlXPath = node["XPath", Global.S2];

            if (xmlXPath == null)
                throw new Exception("Missing XPath");

            XPath = new XPath(xmlXPath.InnerText);

            List<Field> fields = new List<Field>();
            foreach (XmlNode xmlField in node.ChildNodes)
            {
                if (xmlField.LocalName == "Field" && xmlField.NamespaceURI == Global.S2)
                {
                    if (xmlField == null)
                        throw new Exception("Missing Field");

                    fields.Add(new Field(xmlField));
                }
            }
            Fields = fields.ToArray();
        }
    }

    class Field : AbstaractElement
    {
        public Range Range { get; private set; }

        public XPath XPath { get; private set; }

        public string XSDType { get; private set; }

        public string Cell { get; private set; }

        public string Aggregate { get; private set; }

        public string Id { get; private set; }

        public Field(XmlNode node)
            : base(node)
        {
            XmlNode xmlId = node.Attributes["ID", Global.S2];
            if (xmlId == null)
            {
                //Warning
            }else
                Id = xmlId.InnerText;

            XmlNode xmlRange = node["Range", Global.S2];
            if (xmlRange == null)
            {
                //warning
                Range = null;
            }else
                Range = new Range(xmlRange.InnerText);

            XmlNode xmlPath = node["XPath", Global.S2];
            if (xmlPath == null)
            {
                //Warning
                XPath = null;
            }
            else 
                XPath = new XPath(xmlPath.InnerText);

            XmlNode xmlXSDType = node["XSDType", Global.S2];
            if (xmlXSDType != null)
                XSDType = xmlXSDType.InnerText;

            XmlNode xmlCell = node["Cell", Global.S2];
            if (xmlCell != null)
                Cell = xmlCell.InnerText;

            XmlNode xmlAggregate = node["Aggregate", Global.S2];
            if (xmlAggregate != null)
                Aggregate = xmlAggregate.InnerText;
        }
    }
    #endregion

    /*class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            string XsdFileName = "c:\\tmp\\proposal.xsd";

            var xsd = new XSD(XsdFileName);
            xsd.Load();

            ExcelReader reader = new ExcelReader("c:\\Test1.xml");

            ExcelDocument st = reader.Process();

            XmlNode node = st.GenerateXSLT(xsd);

            node.OwnerDocument.Save("c:\\tmp\\res.xslt");

            return 0;
        }
    }*/

}
