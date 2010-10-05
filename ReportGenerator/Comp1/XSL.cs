using System;
using System.IO;

namespace Comp1
{
	/// <summary>
	/// �����, ����������� ������ � XSL. �� ����� �������� SQLStruct � �� ��� ����� ����������� XSLT-������
	/// </summary>
	class XSL
	{
		private TextWriter tw;
		private SQLStruct sql;
		private string DataSetName ;
		private bool isGroup;
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="tetxw">����� � ������ ����� �������� ����</param>
        /// <param name="st">��������� �������� ������������ SQL</param>
        /// <param name="DsName">��� ����� ��</param>
        /// <param name="isGroup">����, ���� true, ������ � ������������</param>
		public XSL(TextWriter tetxw, SQLStruct st, string DsName, bool isGroup)
		{
			tw = tetxw;
			sql = st;
			DataSetName = DsName;
			this.isGroup = isGroup;
		}

        /// <summary>
        /// ����� ����������� ���� ��� ���� ������ �� From
        /// ����������� ��� ���������� ������������ ���� ������, �.�. ��������� ��������� ���� � ����� for-each
        /// ��� ������ �������� ����� <see cref="WriteBeginWhere"/> ��� ��������� ����������� �����������
        /// ��� �� ����� ����� ��������� ���������� � ������: "var_TableName" � ������ � ���������� ������� �� ������ �������
        /// ��� ������ ������� ����� ��������� ����������� �� ���-�� ����������� ����� (ONCE ��� ALL)
        /// </summary>
        /// <param name="sql">����� SQL �������</param>
        /// <param name="DataSetName">��� ����-����</param>
		public void WriteFrom(SQLStruct sql, string DataSetName)
		{
			//��������� ��� JOIN
			for (int i=0;i<sql.from.Count;i++)
			{
				string fromName = sql.from[i];
				if (sql.alias.GetTableAlias(fromName)!=null)
					fromName = sql.alias.GetTableAlias(fromName);
				if (i == 0)
				{
					tw.WriteLine("<xsl:for-each select=\"//" + DataSetName + "//" + fromName + "\">");
					if (sql.from.Count == 1 && sql.select.selectType == SELECTTYPE.ONCE)
					{
						tw.WriteLine("<xsl:if test=\"position() &lt; 2\">");
					}
				}
				else if (i == sql.from.Count-1 && sql.select.selectType == SELECTTYPE.ONCE)
				{
					//���� ��������� �������
					if (i == sql.from.Count-1 && sql.where.Count > 0)
					{
						//��� ���� �������.. ���� ���������� 2 where...
						WriteBeginWhere(sql.where, sql.from.GetWhere(i-1), sql.from[i], DataSetName, true);
					}
					else
					{
						WriteBeginWhere(sql.from.GetWhere(i-1),sql.from[i],DataSetName, true);
						//tw.WriteLine("\t\t<xsl:for-each select=\"//" + DataSetName + "//" + sql.from[i] + "\">");
					}                    
				}
				else
				{
					if (i == sql.from.Count-1 && sql.where.Count > 0)
					{
						//��� ���� �������.. ���� ���������� 2 where...
						WriteBeginWhere(sql.where, sql.from.GetWhere(i-1),sql.from[i], DataSetName, false);
					}
					else
					{
						WriteBeginWhere(sql.from.GetWhere(i-1),sql.from[i],DataSetName, false);
						//tw.WriteLine("\t\t<xsl:for-each select=\"//" + DataSetName + "//" + sql.from[i] + "\">");
					}
				}
				string TableName = "";
				//if (sql.alias.GetTableAlias(sql.from[i]) == null)
					TableName = sql.from[i];
				//else
				//	TableName = sql.alias.GetTableAlias(sql.from[i]);
				tw.WriteLine("<xsl:variable name=\"var_" + TableName + "\" select=\".\"/>");
			}
		}
        /// <summary>
        /// ����� ������������� ����� ��� From
        ///�.�. ���������� ��� /for-each � /if
        /// </summary>
        /// <param name="sql">����� �������</param>
		public void WriteFromEnd(SQLStruct sql)
		{
			//��������� ��� JOIN
			for (int i=0;i<sql.from.Count;i++)
			{
				if (i == 0 && sql.select.selectType == SELECTTYPE.ONCE)
					tw.WriteLine("</xsl:if>");
				tw.WriteLine("</xsl:for-each>");
			}
		}
        /// <summary>
        /// ��������� ���������� ��� �����������: xsl:variable name="group_var_0"
        /// </summary>
        /// <param name="sql"></param>
		public void GroupBy(SQLStruct sql)
		{	
			if (sql.group.Count <= 0)
				return;
			tw.WriteLine("<xsl:variable name=\"group_var_0\">");
		}
		//new!!! ��������!!!!!!!!!!!!
        /// <summary>
        /// ������ ������, �� ������������
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="prev"></param>
		public void GroupByEnd1(SQLStruct sql, int prev)
		{
			/*if (sql.group.Count == 1)
			{
				GroupByEnd1(sql,prev);
				return;
			}*/


			//���� ���� group-a, �� �������
			//int id = 0;
			if (sql.group.Count <= 0)
				return;
			//��������� ������
			tw.WriteLine("</xsl:variable>");

			//��������� �� ���� group � ������ � �������� ������ group-�� ��� �� ALL
			

			int count = -1;
			for (int idGroup =0; idGroup <sql.group.Count; idGroup++)
			{
				if (!sql.group[idGroup].isAll)
					count++;
			}

			string groupall = "groupall";
			//
			tw.WriteLine("<xsl:variable name=\"" + groupall+ "\">");
			GroupNotAll(sql, prev, count);
			//����� �� ��������� ������
			GroupData(null, false);
			//��������� ��� ������
			GroupNotAllEnd(sql, prev, count);
			tw.WriteLine("</xsl:variable>");

			if (count == -1)
				groupall = "group_var_0";
			
			//���������� ���� ���� ALL
			GroupAll(sql, groupall, count);
			//��������� ��� ������
			GroupData(groupall, true);
			GroupAllEnd(sql, groupall);
		}

        /// <summary>
        /// ����������� ����������� Group ALL
        /// ����������� ����� ����������� ���� ������� ����������� (FLAT, CONTAIN...) � ������ �����. ������� ��� ���
        /// </summary>
        /// <remarks>�������� ��� ��������� ��������������� Group ALL</remarks>
        /// <param name="sql">����� Sql-�������</param>
        /// <param name="groupall">��� ���������� �� ������� ����� ������� ������ ��� �����������</param>
		public void GroupAllEnd(SQLStruct sql, string groupall)
		{
			string VarGroupName = groupall;
			int idAll = -1;
			string TrName = "";
			for (int i=0;i<sql.group.Count;i++)
			{
				TrName = sql.GetTr(i-1) + "/" +TrName;
				if (sql.group[i].isAll)
					idAll = i;
			}
			if (idAll == -1)
				return;

			if (sql.group[idAll].type == TypeGroup.CONTAIN)
				GroupContain2End(sql.group[idAll]);
			else if (sql.group[idAll].type == TypeGroup.FOOTER || sql.group[idAll].type == TypeGroup.HEADERFOOTER)
				GroupFooter2(sql.group[idAll], sql.group[idAll].isAll, groupall, TrName);
		}
        /// <summary>
        /// ����������� ����������� Group ALL
        /// ����������� ����� ����������� ���� ������� ����������� (FLAT, CONTAIN...) � ������ �����. ������� ��� ���
        /// </summary>
        /// <remarks>�������� ��� ��������� �������������� Group ALL (�� ����������). �� ��� ��������� �����, ��� �� �����. � �������� ��������� <see cref="GroupAllEnd"/> �� ����������� ���� ���, ���������� <see cref="GroupContain2"/> ������ <see cref="GroupContain2End"/>  </remarks>
        /// <param name="sql">����� Sql-�������</param>
        /// <param name="groupall">��� ���������� �� ������� ����� ������� ������ ��� �����������</param>
        /// <param name="Count">�� �����</param>
		public void GroupAll(SQLStruct sql, string groupall, int Count)
		{

			int idAll = -1;
			string TrName = "";
			for (int i=0;i<sql.group.Count;i++)
			{
				//TrName = sql.GetTr(i-1) + "/" +TrName;
				if (sql.GetTr(i-1) != null)
				{
					TrName = sql.GetTr(i-1) + "/" +TrName;
				}
				if (sql.group[i].isAll)
                    idAll = i;
			}
			if (idAll == -1)
				return;

			string VarGroupName = groupall;
			
			if (sql.group[idAll].type == TypeGroup.CONTAIN)
				GroupContain2(sql.group[idAll], true, groupall, TrName);
			else if (sql.group[idAll].type == TypeGroup.HEADER || sql.group[idAll].type == TypeGroup.HEADERFOOTER)
				GroupHeader2(sql.group[idAll], true, groupall, TrName);
		}
        /// <summary>
        /// ���������� ��� ����������� ���� ����������� ����� Group ALL
        /// ��� ���� ����������� ������� � �����, ������� � MAX (���������� ������ ����� ������� ��� ��������� GROUP ALL)
        /// � ��������� ����������� ��� ����������� �������� �������, � ��� ������ ���������� ����������� �������, � ����� ��������� ������������� for-each-group
        ///
        /// </summary>
        /// <param name="sql">����� SQL</param>
        /// <param name="prev">�� ������������</param>
        /// <param name="max">��������� �������</param>
		public void GroupNotAllEnd(SQLStruct sql, int prev, int max)
		{
			
			for (int i=max; i>=0; i--)
			{
				string TrName = "";
				for (int j=0;j<=i;j++)
				{
					if (sql.GetTr(i-1) != null)
					{
						TrName = sql.GetTr(j-1) + "/" +TrName;
					}
					//TrName = sql.GetTr(j-1) + "/" +TrName;
				}
				
				string VarGroupName = "group_" + i.ToString();
				if (sql.group[i].type == TypeGroup.CONTAIN)
					GroupContain2End(sql.group[i]);
                else if (sql.group[i].type == TypeGroup.FOOTER || sql.group[i].type == TypeGroup.HEADERFOOTER)
					GroupFooter2(sql.group[i], sql.group[i].isAll, VarGroupName, TrName);

				for (int j=0;j<sql.group[i].Count;j++)
					tw.WriteLine("</xsl:for-each-group>");
			}
		}
        /// <summary>
        /// ����������� ������( ���� ����� ALL). ��� ������ ����������� �������� ������� ��������� ��� 
        /// xsl:for-each-group � � ��� ������� 2 ����������: 
        /// group_key_id - �������� ����� �� �������� ���� �����������
        /// group_value_id - �������� �������� ���� (���������)
        /// ������ group ��������� ������ �� group_var_id (���� ���� �������� ������ �� From)
        /// � ����������� ����� �������� ������ �� current-group()
        /// 
        /// 
        /// </summary>
        /// <remarks>for-each-group  �������� ��� ������ ���������� ������������� � �������</remarks>
        /// <param name="sql">SQL - ������</param>
        /// <param name="prev"></param>
        /// <param name="max">����������� ���-�� Group-�� (��� - ALL)</param>
		public void GroupNotAll(SQLStruct sql, int prev, int max)
		{
			string TrName = "";
			int id= 0;
			for (int i=0;i<=max;i++)
			{
                TrName =  sql.GetTr(i-1);
				if (TrName!=null)
					TrName = "/" + TrName;
				
                				
				string VarGroupName = "group_" + i.ToString();
				string from = "$group_var_" + id.ToString()+ TrName;
				if (i!=0)
					from = "current-group()" + TrName;

				//����� � ����� �� ���������� �� ������ ������� ��� �������� group


				for (int j=0;j<sql.group[i].Count;j++)
				{
					if (j==0)
						tw.WriteLine("<xsl:for-each-group select=\"" + from + "\" group-by=\"" + sql.group[i].group[j] + "\">");
					else
						tw.WriteLine("<xsl:for-each-group select=\"current-group()\" group-by=\"" + sql.group[i].group[j] + "\">");
					

					tw.WriteLine("<xsl:variable name=\"" + VarGroupName+"_key_" + sql.group[i].group[j] + "\" select=\"current-grouping-key()\"/>");
					tw.WriteLine("<xsl:variable name=\"" + VarGroupName+"_value_" + sql.group[i].group[j] + "\" select=\"current-group()\"/>");
				}
				if (sql.group[i].type == TypeGroup.CONTAIN)
					GroupContain2(sql.group[i], sql.group[i].isAll, VarGroupName, TrName);
                else if (sql.group[i].type == TypeGroup.HEADER || sql.group[i].type == TypeGroup.HEADERFOOTER)
					GroupHeader2(sql.group[i], sql.group[i].isAll, VarGroupName, TrName);
			}
		}
        /// <summary>
        /// ��������� CONTAIN
        /// </summary>
        /// <param name="sql">����� �������</param>
        /// <param name="isAll">���� ALL</param>
        /// <param name="VarGroupName">��� ���������� (group_) �� ������� ������������ ������</param>
        /// <param name="TrName">��� ���� xsl:value-of select="$" + VarGroupName + "/" + TrName </param>
		public void GroupContain2(GroupClassOnce sql, bool isAll, string VarGroupName, string TrName)
		{
			tw.WriteLine("<" + sql.UseTag(TypeFH.CONTAIN) + ">");
			//����� ��� ��������.
			GroupValueRec2(sql, isAll, VarGroupName, TrName, TypeFH.CONTAIN);
		}
        /// <summary>
        /// ����� ������������� ��� Contain
        /// </summary>
        /// <param name="sql">SQL -�����</param>
		public void GroupContain2End(GroupClassOnce sql)
		{
            tw.WriteLine("</" + sql.UseTag(TypeFH.CONTAIN) + ">");
		}
        /// <summary>
        /// ����� HEADER
        /// </summary>
        /// <param name="sql">SQL -�����</param>
        /// <param name="isAll">���� ALL</param>
        /// <param name="VarGroupName">��� ���������� (group_) �� ������� ������������ ������</param>
        /// <param name="TrName">��� ���� xsl:value-of select="$" + VarGroupName + "/" + TrName </param>
		public void GroupHeader2(GroupClassOnce sql, bool isAll, string VarGroupName, string TrName)
		{
			tw.WriteLine("<" + sql.UseTag(TypeFH.HEADER) + ">");
			//����� ��� ��������.
			GroupValueRec2(sql,isAll, VarGroupName, TrName, TypeFH.HEADER);
			
			tw.WriteLine("</" + sql.UseTag(TypeFH.HEADER) + ">");
		}
        /// <summary>
        /// ����� FOOTER
        /// </summary>
        /// <param name="sql">SQL -�����</param>
        /// <param name="isAll">���� ALL</param>
        /// <param name="VarGroupName">��� ���������� (group_) �� ������� ������������ ������</param>
        /// <param name="TrName">��� ���� xsl:value-of select="$" + VarGroupName + "/" + TrName </param>
        public void GroupFooter2(GroupClassOnce sql, bool isAll, string VarGroupName, string TrName)
		{
			tw.WriteLine("<" + sql.UseTag(TypeFH.FOOTER) + ">");

            GroupValueRec2(sql,isAll, VarGroupName, TrName, TypeFH.FOOTER);

			tw.WriteLine("</" + sql.UseTag(TypeFH.FOOTER) + ">");
		}
        /// <summary>
        /// ����� ������ (������) � Group 
        /// ���� ��� ���� ������� ���������� �������, �� ��� ���� �������
        /// </summary>
        /// <param name="sql">SQL -����� 1 �����������</param>
        /// <param name="all">���� ALL</param>
        /// <param name="VarGroupName">��� ���������� (group_) �� ������� ������������ ������</param>
        /// <param name="TrName">��� ���� xsl:value-of select="$" + VarGroupName + "/" + TrName </param>
        /// <param name="t">��� �����������</param>
		public void GroupValueRec2(GroupClassOnce sql, bool all, string VarGroupName, string TrName, TypeFH t)
		{
						
			//����� ��� ��������...
			int CountValueRec = 0;
			if (t == TypeFH.FOOTER)
				CountValueRec = sql.CountValueRecFooter;
			else
				CountValueRec = sql.CountValueRecHeader;

			for (int j=0;j< CountValueRec;j++)
			{
				string defTemplate = "#.##";
				//string node = (string) sql.group[idGroup].array[j];
				string node = sql.ValueRec(j, t);
				string node_value = node;
				string func = sql.Func(j, t);
				string fullName = sql.FullName(j, t);
				if ((fullName == null || fullName == "") && !all)
				{
					//��������� ��������� �� � ������� ������ ��� �������
					int idGroup = sql.IdGroup(j,t);
					if (idGroup!=-1)
					{
                        fullName = "group_" + idGroup.ToString() + "_key_" + node;
					}
				}
				
				if (all)
					if (fullName!= null && fullName != "")
					{
						if (func != null && func != "")
							tw.WriteLine("<" + node + "><xsl:value-of select=\"format-number(" + func + "($"+ fullName + "),'" + defTemplate + "')\"/></" + node + ">");
						else
							tw.WriteLine("<" + node + "><xsl:value-of select=\"$" + fullName + "\"/></" + node + ">");
					}
					else
					{
						if (func != null && func != "")
							tw.WriteLine("<" + node + "><xsl:value-of select=\"format-number(" + func + "($"+ VarGroupName + "/" + TrName + "" + node_value + "),'" + defTemplate + "')\"/></" + node + ">");
						else
							tw.WriteLine("<" + node + "><xsl:value-of select=\"$" + VarGroupName + "/" + TrName + "" + node_value + "\"/></" + node + ">");
						//int uu=0;
					}
				else
				{
					if (fullName!= null && fullName != "")
					{
						if (func!=null && func != "")
                            if (func.ToLower() !="val")
							    tw.WriteLine("<" + node + "><xsl:value-of select=\"format-number(" + func + "($" + fullName + "),'" + defTemplate + "')\"/></" + node + ">");
                            else
                                tw.WriteLine("<" + node + "><xsl:value-of select=\"$" + fullName + "\"/></" + node + ">");
						else
							tw.WriteLine("<" + node + "><xsl:value-of select=\"$" +  fullName + "\"/></" + node + ">");
					}
					else
					{
						if (func!=null && func != "")
                            if (func.ToLower() != "val")
							    tw.WriteLine("<" + node + "><xsl:value-of select=\"format-number(" + func + "(current-group()/" + node_value + "),'" + defTemplate + "')\"/></" + node + ">");
                            else
                                tw.WriteLine("<" + node + "><xsl:value-of select=\"current-group()/" + node_value + "\"/></" + node + ">");
						else
							tw.WriteLine("<" + node + "><xsl:value-of select=\"$" + VarGroupName + "_key_" + node + "\"/></" + node + ">");
					}
				}
			}
		}

        /// <summary>
        /// ����� ����������� ���� �����������
        /// � ��� �� ����������� ���� ������� ������ �� �����������
        /// </summary>
        /// <param name="sql">����� SQL</param>
        /// <param name="prev"></param>
		public void GroupByEnd2(SQLStruct sql, int prev)
		{
			//���� ���� group-a, �� �������
			//int id = 0;
			if (sql.group.Count <= 0)
				return;
			//��������� ������
			tw.WriteLine("</xsl:variable>");


			//�������� ��� ���� TR (���� ��� ����)
			//string TrName = sql.

			string TrName = "";
			for (int idGroup =0; idGroup < sql.group.Count; idGroup++)
			{
				//�� idGroup �������� 
				TrName = sql.GetTr(idGroup-1) + "/" +TrName;
				GroupOne(sql.group[idGroup], sql.idSQL, idGroup, prev, TrName);
			}

			tw.WriteLine("<xsl:copy-of select=\"$group_"+ sql.idSQL + "_"+ (sql.group.Count-1).ToString()  + "\"/>");
		}
        /// <summary>
        /// ����� ������ (������) � Group 
        /// ���� ��� ���� ������� ���������� �������, �� ��� ���� �������
        /// </summary>
        /// <param name="sql">SQL -����� 1 �����������</param>
        /// <param name="all">���� ALL</param>
        /// <param name="from">�������� ������ ����� ������� ������</param>
        /// <param name="VarGroupName">��� ���������� (group_) �� ������� ������������ ������</param>
        /// <param name="TrName">��� ���� xsl:value-of select="$" + VarGroupName + "/" + TrName </param>
        /// <param name="t">��� �����������</param>
		public void GroupValueRec(GroupClassOnce sql, bool all, string from, string VarGroupName, string TrName, TypeFH t)
		{
			int CountValueRec = 0;
			if (t == TypeFH.FOOTER)
				CountValueRec = sql.CountValueRecFooter;
			else
				CountValueRec = sql.CountValueRecHeader;
						
			//����� ��� ��������...
			for (int j=0;j< CountValueRec;j++)
			{
				string defTemplate = "#.##";
				//string node = (string) sql.group[idGroup].array[j];
				string node = sql.ValueRec(j,t);
				string node_value = node;
				string func = sql.Func(j,t);
				string fullName = sql.FullName(j,t);

				/*if (sql.FullName!= null && sql.FullName != "")
					node_value = fullName;*/
				
				//tw.WriteLine("<" + node + "><xsl:value-of select=\"" + func + "(current()/" + node + ")\"/></" + node + ">");
				if (all)
					if (fullName!= null && fullName != "")
					{
						if (func != null && func != "")
							tw.WriteLine("<" + node + "><xsl:value-of select=\"format-number(" + func + "($"+ fullName + "),'" + defTemplate + "')\"/></" + node + ">");
						else
							tw.WriteLine("<" + node + "><xsl:value-of select=\"$" + fullName + "\"/></" + node + ">");
					}
					else
					{
						if (func != null && func != "")
							tw.WriteLine("<" + node + "><xsl:value-of select=\"format-number(" + func + "("+ from + "/" + TrName + "" + node_value + "),'" + defTemplate + "')\"/></" + node + ">");
						else
							tw.WriteLine("<" + node + "><xsl:value-of select=\"" + from + "/" + TrName + "" + node_value + "\"/></" + node + ">");
					}
				else
				{
					if (fullName!= null && fullName != "")
					{
						if (func!=null && func != "")
							tw.WriteLine("<" + node + "><xsl:value-of select=\"format-number(" + func + "($" + fullName + "),'" + defTemplate + "')\"/></" + node + ">");
						else
							tw.WriteLine("<" + node + "><xsl:value-of select=\"$" +  fullName + "\"/></" + node + ">");
					}
					else
					{
						if (func!=null && func != "")
							tw.WriteLine("<" + node + "><xsl:value-of select=\"format-number(" + func + "(current-group()/" + node_value + "),'" + defTemplate + "')\"/></" + node + ">");
						else
							tw.WriteLine("<" + node + "><xsl:value-of select=\"$" + VarGroupName + "_key_" + node + "\"/></" + node + ">");
					}
				}
			}
		}
        /// <summary>
        /// ����� Conatin
        /// </summary>
        /// <param name="sql">SQL -����� 1 �����������</param>
        /// <param name="all">���� ALL</param>
        /// <param name="from">������ ����� ������� ������</param>
        /// <param name="VarGroupName">��� ���������� (group_) �� ������� ������������ ������</param>
        /// <param name="TrName">��� ���� xsl:value-of select="$" + VarGroupName + "/" + TrName </param>
        public void GroupContain(GroupClassOnce sql, bool all, string from, string VarGroupName, string TrName)
		{
			tw.WriteLine("<" + sql.UseTag(TypeFH.CONTAIN) + ">");
			//����� ��� ��������.
			GroupValueRec(sql,all,from, VarGroupName, TrName, TypeFH.CONTAIN);
		}
        /// <summary>
        /// ����� ������������� ���
        /// </summary>
        /// <param name="sql"></param>
		public void GroupContainEnd(GroupClassOnce sql)
		{
			tw.WriteLine("</" + sql.UseTag(TypeFH.CONTAIN) + ">");
		}
        /// <summary>
        /// ����� FOOTER. ������ �������� GroupFooter <see cref="GroupFooter"/>
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="all"></param>
        /// <param name="from"></param>
        /// <param name="VarGroupName"></param>
        /// <param name="TrName"></param>
		public void GroupFooter(GroupClassOnce sql, bool all, string from, string VarGroupName, string TrName)
		{
			GroupHeader(sql, all, from, VarGroupName, TrName);
		}

        /// <summary>
        /// ����� HEADER
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="all">���� ALL</param>
        /// <param name="from">������ ����� ������� ������</param>
        /// <param name="VarGroupName">��� ���������� �����������</param>
        /// <param name="TrName">��� ����</param>
		public void GroupHeader(GroupClassOnce sql, bool all, string from, string VarGroupName, string TrName)
		{
			tw.WriteLine("<" + sql.UseTag(TypeFH.HEADER) + ">");
			//����� ��� ��������.
			GroupValueRec(sql,all,from, VarGroupName, TrName, TypeFH.HEADER);
			
			tw.WriteLine("</" + sql.UseTag(TypeFH.HEADER) + ">");

		}
        /// <summary>
        /// ���������� ������ �� ������� �����������
        /// </summary>
		public void GroupDataNotAll()
		{
			tw.WriteLine("<xsl:copy-of select=\"current-group()\"/>");
		}
        /// <summary>
        /// ���������� ������
        /// </summary>
        /// <param name="from">�������� ������</param>
        /// <param name="all">���� ALL</param>
		public void GroupData(string from, bool all)
		{
			if (all)
			{
				//tw.WriteLine("<xsl:for-each select=\"" + from + "/*\" >");
				//tw.WriteLine("<xsl:copy-of select=\"current()\"/>");
				tw.WriteLine("<xsl:copy-of select=\"$" + from + "\"/>");
				//tw.WriteLine("</xsl:for-each>");
			}
			else
			{
				tw.WriteLine("<xsl:copy-of select=\"current-group()\"/>");

			}
		}
        /// <summary>
        /// ����� ����������� ��� ����� �����������
        /// ��� ���� ������ ����������� ���������� � ���������� group_id
        /// </summary>
        /// <param name="sql">SQl - �����</param>
        /// <param name="idSQL">����� SQL-�������</param>
        /// <param name="idGroup">����� �����������</param>
        /// <param name="prevCountGroup">����� ���������� ����������� (�� ���� ����� ������� ������)</param>
        /// <param name="TrName">���� �� ������</param>
		public void GroupOne(GroupClassOnce sql, int idSQL, int idGroup, int prevCountGroup, string TrName)
		{
			int id = 0;
			bool old = true;
			string from = "$group_var_" + id.ToString();
			if (idGroup != 0)
			{
				from = "$group_" + idSQL.ToString() + "_" + ((int)(idGroup-1)).ToString();
				old = false;
			}
			else if (idSQL != 0 && prevCountGroup !=0)
			{
				from = "$group_" + (idSQL-1).ToString() + "_" + prevCountGroup.ToString();
				old = false;
			}

			string VarGroupName = "group_"+ idSQL.ToString() + "_" + idGroup.ToString();

			//if
			tw.WriteLine("<xsl:variable name=\"" + VarGroupName + "\">");

			//����� �� ������ �������� ���� ����������� ���� ����
			if (!sql.isAll)
			{
				string from_val = from;
				if (old)
				{
					string tmp_TrName = sql.baseGroup.baseSql.GetTr(-1);
					from += "/" + tmp_TrName;
				}

				for (int i=0;i<sql.Count;i++)
				{
					if (i == 0)
						tw.WriteLine("<xsl:for-each-group select=\""+from+"\" group-by=\"" + sql[i] + "\">");
					else
						tw.WriteLine("<xsl:for-each-group select=\"current-group()\" group-by=\"" + sql[i] + "\">");

					tw.WriteLine("<xsl:variable name=\"" + VarGroupName+"_key_" + sql[i] + "\" select=\"current-grouping-key()\"/>");
				}
			}

			if (sql.type == TypeGroup.CONTAIN)
				GroupContain(sql, sql.isAll, from, VarGroupName, TrName);
			else if (sql.type == TypeGroup.HEADER)
				GroupHeader(sql, sql.isAll, from, VarGroupName, TrName);

			GroupData(from,sql.isAll);
				
			/*tw.WriteLine("<xsl:for-each select=\"" + from + "" + id.ToString() + "/*\" >");
			tw.WriteLine("<xsl:copy-of select=\"current()\"/>");
			tw.WriteLine("</xsl:for-each");*/

			if (sql.type == TypeGroup.CONTAIN)
				GroupContainEnd(sql);
			else if (sql.type == TypeGroup.FOOTER)
				GroupFooter(sql, sql.isAll, from, VarGroupName, TrName);

			if (!sql.isAll)
			{
				for (int i=0;i<sql.Count;i++)
				{
					tw.WriteLine("</xsl:for-each-group>");
				}
			}

				
			//tw.WriteLine("");
			tw.WriteLine("</xsl:variable>");
		}
        /// <summary>
        /// ����� ������������� ���. �� ������������
        /// </summary>
        /// <param name="sql"></param>
		public void GroupByEnd(SQLStruct sql)
		{
			TypeFH t = TypeFH.HEADER;
			int id = 0;
			if (sql.group.Count <= 0)
				return;

			tw.WriteLine("</xsl:variable>");
			//��������� ������ � variable.

			//����� �� ��� ��� ������ ������������� :)

			bool All = false;

			for (int idGroup =0; idGroup <sql.group.Count; idGroup++)
			{
				if (sql.group[idGroup].isAll)
				{
					All = true;
					//tw.WriteLine("<xsl:for-each select=\"$group_var_" + id.ToString() + "/*\">");
					if (sql.group[idGroup].type == TypeGroup.CONTAIN)
					{
						tw.WriteLine("<" + sql.group[idGroup].UseTag(TypeFH.CONTAIN) + ">");
						//����� ��� ��������.
						for (int j=0;j< sql.group[idGroup].CountValueRecFooter;j++)
						{
							string node = sql.group[idGroup].ValueRec(j,t);
							string func = sql.group[idGroup].Func(j,t);
							tw.WriteLine("<" + node + "><xsl:value-of select=\"" + func + "(current()/" + node + ")\"/></" + node + ">");
						}
					}
					else if (sql.group[idGroup].type == TypeGroup.HEADER)
					{
						tw.WriteLine("<" + sql.group[idGroup].UseTag(TypeFH.HEADER) + ">");
						//����� ��� ��������.
						for (int j=0;j< sql.group[idGroup].CountValueRecFooter;j++)
						{
							//string node = (string) sql.group[idGroup].array[j];
							string node = sql.group[idGroup].ValueRec(j,t);
							string func = sql.group[idGroup].Func(j,t);

							if (idGroup==0)
								tw.WriteLine("<" + node + "><xsl:value-of select=\""+func+"($group_var_" + id.ToString() +"/*/" + node + ")\"/></" + node + ">");
							else
								tw.WriteLine("<" + node + "><xsl:value-of select=\""+func+"($current-group()/" + node + ")\"/></" + node + ">");

							//							tw.WriteLine("<xsl:value-of select=\"sum(current-group()/" + (string) sql.group[idGroup].array[j] + ")\"/>");
						}
						tw.WriteLine("</" + sql.group[idGroup].UseTag(TypeFH.HEADER) + ">");
					}
					if (idGroup==0)
						tw.WriteLine("<xsl:for-each select=\"$group_var_" + id.ToString() + "/*\" >");
					else
						tw.WriteLine("<xsl:for-each select=\"current-group()\" >");
				}
				else if (idGroup == 0 && sql.group[idGroup].Count > 0)
				{
					All = false;
					for (int i=0;i< sql.group[idGroup].Count; i++)
					{
						if (idGroup==0)
							tw.WriteLine("<xsl:for-each-group select=\"$group_var_" + id.ToString() + "/*\" group-by=\"" + sql.group[idGroup][i]+ "\">");
						else
							tw.WriteLine("<xsl:for-each-group select=\"current-group()\" group-by=\"" + sql.group[idGroup][i] + "\">");
					}
					if (sql.group[idGroup].CountValueRecHeader !=0 && sql.group[idGroup].type == TypeGroup.CONTAIN)
					{
						tw.WriteLine("<" + sql.group[idGroup].UseTag(TypeFH.CONTAIN) + ">");
						//����� ��� ��������.
						for (int j=0;j< sql.group[idGroup].CountValueRecFooter;j++)
						{
							//string node = (string) sql.group[idGroup].array[j];
							string node = sql.group[idGroup].ValueRec(j,t);
							string func = sql.group[idGroup].Func(j,t);
							tw.WriteLine("<" + node + "><xsl:value-of select=\""+func+"(current-group()/" + node + ")\"/></" + node + ">");
						}
						
					}
					else if (sql.group[idGroup].CountValueRecFooter > 0 && sql.group[idGroup].type == TypeGroup.HEADER)
					{
						tw.WriteLine("<" + sql.group[idGroup].UseTag(TypeFH.HEADER) + ">");
						//����� ��� ��������.
						for (int j=0;j< sql.group[idGroup].CountValueRecFooter;j++)
						{
							//string node = (string) sql.group[idGroup].array[j];
							string node = sql.group[idGroup].ValueRec(j,t);
							string func = sql.group[idGroup].Func(j,t);
							tw.WriteLine("<" + node + "><xsl:value-of select=\""+func+"(current-group()/" + node + ")\"/></" + node + ">");
							//							tw.WriteLine("<xsl:value-of select=\"sum(current-group()/" + (string) sql.group[idGroup].array[j] + ")\"/>");
						}
						tw.WriteLine("</" + sql.group[idGroup].UseTag(TypeFH.HEADER) + ">");
					}
				}
				else
				{
					All = false;
					for (int i=0;i< sql.group[idGroup].Count; i++)
						tw.WriteLine("<xsl:for-each-group select=\"current-group()\" group-by=\"" + sql.group[idGroup][i] + "\">");
				}
			}

			if (!All)
				tw.WriteLine("<xsl:copy-of select=\"current-group()\"/>");
			else
				tw.WriteLine("<xsl:copy-of select=\"current()\"/>");

			for (int idGroup =sql.group.Count-1; idGroup >=0 ; idGroup--)
			{
				if (sql.group[idGroup].isAll)
				{
					if (sql.group[idGroup].type == TypeGroup.CONTAIN)
					{
						//����� ��� ��������.
						tw.WriteLine("</" + sql.group[idGroup].UseTag(TypeFH.CONTAIN) + ">");
					}
					tw.WriteLine("</xsl:for-each>");
					
					if (sql.group[idGroup].type == TypeGroup.FOOTER)
					{
						tw.WriteLine("<" + sql.group[idGroup].UseTag(TypeFH.FOOTER) + ">");
						//����� ��� ��������.
						for (int j=0;j< sql.group[idGroup].CountValueRecFooter;j++)
						{
							//string node = (string) sql.group[idGroup].array[j];
							string node = sql.group[idGroup].ValueRec(j,t);
							string func = sql.group[idGroup].Func(j,t);

							if (idGroup==0)
								tw.WriteLine("<" + node + "><xsl:value-of select=\""+func+"($group_var_" + id.ToString() +"/*/" + node + ")\"/></" + node + ">");
							else
								tw.WriteLine("<" + node + "><xsl:value-of select=\""+func+"($current-group()/" + node + ")\"/></" + node + ">");

							//							tw.WriteLine("<xsl:value-of select=\"sum(current-group()/" + (string) sql.group[idGroup].array[j] + ")\"/>");
						}
						tw.WriteLine("</" + sql.group[idGroup].UseTag(TypeFH.FOOTER) + ">");
					}
				}
				else if (idGroup == 0 && sql.group[idGroup].Count > 0)
				{
					
					if (sql.group[idGroup].CountValueRecFooter !=0 && sql.group[idGroup].type == TypeGroup.CONTAIN)
					{
						tw.WriteLine("</" + sql.group[idGroup].UseTag(TypeFH.CONTAIN) + ">");
					}
					else if (sql.group[idGroup].type == TypeGroup.FOOTER)
					{
						tw.WriteLine("<" + sql.group[idGroup].UseTag(TypeFH.FOOTER) + ">");
						//����� ��� ��������.
						for (int j=0;j< sql.group[idGroup].CountValueRecFooter;j++)
						{
							//string node = (string) sql.group[idGroup].array[j];
							string node = sql.group[idGroup].ValueRec(j,t);
							string func = sql.group[idGroup].Func(j,t);
							tw.WriteLine("<" + node + "><xsl:value-of select=\""+func+"(current-group()/" + node + ")\"/></" + node + ">");
							//							tw.WriteLine("<xsl:value-of select=\"sum(current-group()/" + (string) sql.group[idGroup].array[j] + ")\"/>");
						}
						tw.WriteLine("</" + sql.group[idGroup].UseTag(TypeFH.FOOTER) + ">");
					}

					for (int i=0;i< sql.group[idGroup].Count; i++)
					{
						tw.WriteLine("</xsl:for-each-group>");
					}
				}
				else
				{
					if (sql.group[idGroup].type == TypeGroup.FOOTER)
					{
						tw.WriteLine("<" + sql.group[idGroup].UseTag(TypeFH.FOOTER) + ">");
						//����� ��� ��������.
						for (int j=0;j< sql.group[idGroup].CountValueRecFooter;j++)
						{
							//string node = (string) sql.group[idGroup].array[j];
							string node = sql.group[idGroup].ValueRec(j,t);
							string func = sql.group[idGroup].Func(j,t);
							tw.WriteLine("<" + node + "><xsl:value-of select=\""+func+"(current-group()/" + node + ")\"/></" + node + ">");
							//							tw.WriteLine("<xsl:value-of select=\"sum(current-group()/" + (string) sql.group[idGroup].array[j] + ")\"/>");
						}
						tw.WriteLine("</" + sql.group[idGroup].UseTag(TypeFH.FOOTER) + ">");
					}
					for (int i=0;i< sql.group[idGroup].Count; i++)
						tw.WriteLine("</xsl:for-each-group>");
				}
			}
		}
        /// <summary>
        /// ����� XSLT
        /// </summary>
		public void WriteXSL()
		{
			WriteHead();

			//tw.WriteLine("<xsl:variable name=\"" + sql.GetSQLUniqueName() + "\">");
			GroupBy(sql);

			WriteFrom(sql, DataSetName);
			if (sql.select.paint.Count > 0)
			{
				tw.WriteLine("<xsl:choose>");
				for (int i=0; i< sql.select.paint.Count; i++)
				{
					string name = "";
					if (sql.select.paint.By.table != null && sql.select.paint.By.table != "")
						name+="var_"+sql.select.paint.By.table + "/";
					name += sql.select.paint.By.name;

					tw.WriteLine("<xsl:when test=\"$" + name + "='" + sql.select.paint.Val(i) + "'\">");
					tw.WriteLine("<" + sql.select.paint[i] + ">");

					WriteSelect(sql.select,sql);

					tw.WriteLine("</" + sql.select.paint[i] + ">");
					tw.WriteLine("</xsl:when>");
				}

				tw.WriteLine("<xsl:otherwise>");
				tw.WriteLine("<" + sql.select.paint.Other +">");
				WriteSelect(sql.select,sql);
				tw.WriteLine("</" + sql.select.paint.Other + ">");
				tw.WriteLine("</xsl:otherwise>");
				
				tw.WriteLine("</xsl:choose>");
			}
			else
			{
				tw.WriteLine("<" + sql.As[0] + ">");
				if (sql.select.KeyCount != 0)
				{
					//tw.Write("<xsl:attribute-set name=\"one\">");
					for (int i=0;i<sql.select.KeyCount;i++)
					{
						string node = sql.select.GetKey(i);
						//tw.Write("<xsl:value-of select=\"$\"/>");
						//string node = select[i];
						string val = "$var_"+ sql.select.GetKeyTable(i) + "/" +  node;

						tw.WriteLine("<xsl:attribute name=\"" + node +"\">");
						tw.WriteLine("<xsl:value-of select=\"" + val + "\"/>");
						tw.WriteLine("</xsl:attribute>");
					}
					//tw.Write("</xsl:attribute-set>");
					//tw.WriteLine(">");
				}

				WriteSelect(sql.select,sql);
				tw.WriteLine("</" + sql.As[0] + ">");
			}
			WriteFromEnd(sql);

			//-1 - �������� ��� ��� ����������� SQL ������� (��� ������)
			GroupByEnd1(sql, -1);

			//tw.WriteLine("</xsl:variable>");
			//tw.WriteLine("<xsl:copy-of select='$a123'/>");
			//tw.WriteLine("<xsl:copy-of select=\"$" + sql.GetSQLUniqueName() + "\"/>");

			WriteEnd();
		}

        /// <summary>
        /// ����� XSLT ��� SQLStruct
        /// </summary>
        /// <param name="sql">����� SQL</param>
        /// <param name="prevGroupCount">����� ���������� �����������</param>
		public void WriteXSL(SQLStruct sql, int prevGroupCount)
		{
			//tw.WriteLine("<xsl:variable name=\"" + sql.GetSQLUniqueName() + "\">");
			if (sql.select.paint.Count > 0)
			{
				if (sql.select.KeyCount == 0)
					tw.WriteLine("<" + sql.As[0] + ">");
				GroupBy(sql);

				WriteFrom(sql, DataSetName);

				if (sql.select.KeyCount != 0)
				{
					tw.Write("<" + sql.As[0] + ">");
					for (int i=0;i<sql.select.KeyCount;i++)
					{
						string node = sql.select.GetKey(i);
						//tw.Write("<xsl:value-of select=\"$\"/>");
						//string node = select[i];
						string val = "$var_"+ sql.select.GetKeyTable(i) + "/" +  node;

						//tw.WriteLine("<xsl:attribute name=\"" + node +"\" select=\"" + val + "\"/>");
						tw.WriteLine("<xsl:attribute name=\"" + node +"\">");
						tw.WriteLine("<xsl:value-of select=\"" + val + "\"/>");
						tw.WriteLine("</xsl:attribute>");
					}
					//tw.WriteLine(">");
				}

				tw.WriteLine("<xsl:choose>");
				for (int i=0;i<sql.select.paint.Count; i++)
				{
					string name = "";
					if (sql.select.paint.By.table != null && sql.select.paint.By.table != "")
						name+="var_"+sql.select.paint.By.table + "/";
					name += sql.select.paint.By.name;

					tw.WriteLine("<xsl:when test=\"$" + name + "='" + sql.select.paint.Val(i) + "'\">");
					if (sql.select.selectType != SELECTTYPE.ONCE && sql.select.KeyCount == 0)
					{
						if (sql.select.UseTag == null)
							tw.WriteLine("<" +sql.select.paint[i] + ">");
						else
							tw.WriteLine("<" + sql.select.UseTag + ">");
					}
			
					WriteSelect(sql.select,sql);

					if (sql.select.selectType != SELECTTYPE.ONCE && sql.select.KeyCount == 0)
					{
						//tw.WriteLine("</tr-" + sql.from[0] + ">");
						if (sql.select.UseTag == null)
							tw.WriteLine("</" + sql.select.paint[i] + ">");
						else
							tw.WriteLine("</" + sql.select.UseTag + ">");
					}
					tw.WriteLine("</xsl:when>");
				}
				//Other
				tw.WriteLine("<xsl:otherwise>");
				tw.WriteLine("<" + sql.select.paint.Other +">");
				WriteSelect(sql.select,sql);
				tw.WriteLine("</" + sql.select.paint.Other +">");
				tw.WriteLine("</xsl:otherwise>");
				tw.WriteLine("</xsl:choose>");
				if (sql.select.KeyCount != 0)
					tw.WriteLine("</" + sql.As[0] +">");

				WriteFromEnd(sql);
				GroupByEnd1(sql, prevGroupCount);

				//������ ����������� group
				
                

				if (sql.select.KeyCount == 0)
					tw.WriteLine("</" + sql.As[0] +">");
			}
			else
			{
				if (sql.select.KeyCount == 0)
					tw.WriteLine("<" + sql.As[0] + ">");
			
		
				GroupBy(sql);
				WriteFrom(sql, DataSetName);

				if (sql.select.KeyCount != 0)
				{
					tw.WriteLine("<" + sql.As[0] + ">");
					for (int i=0;i<sql.select.KeyCount;i++)
					{
						string node = sql.select.GetKey(i);
						//tw.Write("<xsl:value-of select=\"$\"/>");
						//string node = select[i];
						string val = "$var_"+ sql.select.GetKeyTable(i) + "/" +  node;

						//tw.WriteLine("<xsl:attribute name=\"" + node +"\" select=\"" + val + "\"/>");
						tw.WriteLine("<xsl:attribute name=\"" + node +"\">");
						tw.WriteLine("<xsl:value-of select=\"" + val + "\"/>");
						tw.WriteLine("</xsl:attribute>");
					}
				}


				if (sql.select.selectType != SELECTTYPE.ONCE && sql.select.KeyCount == 0)
				{
					if (sql.select.UseTag == null)
					{
						//tw.WriteLine("<tr-" + sql.from[0] + ">");
						string trName = sql.GetTr(-1);
						tw.WriteLine("<" + trName + ">");
					}
					else
						tw.WriteLine("<" + sql.select.UseTag + ">");
				}
			
				WriteSelect(sql.select,sql);

				if (sql.select.selectType != SELECTTYPE.ONCE && sql.select.KeyCount == 0)
				{
					//tw.WriteLine("</tr-" + sql.from[0] + ">");
					if (sql.select.UseTag == null)
					{
						string trName = sql.GetTr(-1);
						//tw.WriteLine("</tr-" + sql.from[0] + ">");
						tw.WriteLine("</" + trName + ">");
					}
					else
						tw.WriteLine("</" + sql.select.UseTag + ">");
				}
				if (sql.select.KeyCount != 0)
					tw.WriteLine("</" + sql.As[0] +">");

				WriteFromEnd(sql);

				GroupByEnd1(sql, prevGroupCount);

				if (sql.select.KeyCount == 0)
					tw.WriteLine("</" + sql.As[0] +">");
			}
		
			//tw.WriteLine("</xsl:variable>");
			//tw.WriteLine("<xsl:copy-of select=\"$" + sql.GetSQLUniqueName() + "\"/>");
		}
		/// <summary>
		/// ����� �����������
		/// </summary>
		/// <param name="where">�����-�����������</param>
		/// <param name="CurentTable">��� ������� �������</param>
		/// <param name="DataSetName">��� ����-����</param>
		/// <param name="Last"></param>
		public void WriteBeginWhere(WhereClass where, string CurentTable, string DataSetName, bool Last)
		{			
			string str = where.ToString(CurentTable);
			string AliasTable = CurentTable;
			if (where.baseSql.alias.GetTableAlias(CurentTable)!=null)
				AliasTable = where.baseSql.alias.GetTableAlias(CurentTable);
			if (str == "" || str == null)
				tw.WriteLine("<xsl:for-each select=\"//" + DataSetName + "//" + AliasTable +"\">");
			else
				tw.WriteLine("<xsl:for-each select=\"//" + DataSetName + "//" + AliasTable +"[" + str +"]\">");
			if (Last)
			{
				tw.WriteLine("<xsl:if test=\"position() &lt; 2\">");
			}
		}
        /// <summary>
        /// ����� ����������� 
        /// </summary>
        /// <param name="where1"></param>
        /// <param name="where2"></param>
        /// <param name="CurentTable"></param>
        /// <param name="DataSetName"></param>
        /// <param name="Last"></param>
		public void WriteBeginWhere(WhereClass where1, WhereClass where2, string CurentTable, string DataSetName, bool Last)
		{
			
			string str1 = where1.ToString(CurentTable);
			string str2 = where2.ToString(CurentTable);
			string str = "";
			//= str1 + " and "+ str2;
			if ((str1 == "" || str1 !=null) && (str2 == "" || str2 !=null))
				tw.WriteLine("<xsl:for-each select=\"//" + DataSetName + "//" + CurentTable +"\">");
			else
			{
				if (str1 == null || str1 == "")
					str = str2;
				else if (str2 == null || str2 == "")
					str = str1;
				else
					str = str1 + " and "+ str2;
				tw.WriteLine("<xsl:for-each select=\"//" + DataSetName + "//" + CurentTable +"[" + str +"]\">");
			}
			if (Last)
			{
				tw.WriteLine("<xsl:if test=\"position() &lt; 2\">");
			}
		}
        /// <summary>
        /// ��������� ���� ��� �����������
        /// </summary>
        /// <param name="where"></param>
        /// <param name="Last"></param>
		public void WriteBeginEnd(WhereClass where,bool Last)
		{
			if (where.isOnlyIf())
			{
				//tw.Write("</xsl:if>");
			}            
			if (Last)
			{
				tw.WriteLine("</xsl:if");
			}
		}
		/// <summary>
		/// ����� SELECT
		/// </summary>
		/// <param name="select">SELECT-�����</param>
		/// <param name="sql">SQL - �����</param>
		private void WriteSelect(SelectClass select, SQLStruct sql)
		{
			//����� �������, ���� � ��� ����� ��� ONCE - ���� ������
			for (int i=0;i< select.Count; i++)
			{
				if (select.isString(i))
				{
					string node = select[i];
					if (select.isKey(node))
						continue;
					string tablename = select.Table(i);//sql.GetTableName(select[i]);
					
					string val = "$var_"+ tablename + "/" +  select[i];
					

					if (sql.alias.GetColumnAlias(node) != null)
					{
						string Col = "$var_"+ tablename + "/" + sql.alias.GetColumnAlias(node).name;
						//string var_new = "$var_"+ tablename + "/" +  select[i];
						tw.WriteLine("<" + node + "><xsl:value-of select=\"" + Col + "\"/></" + node + ">");
					}else
					{
						tw.WriteLine("<" + node + "><xsl:value-of select=\"" + val + "\"/></" + node + ">");
					}

					if (select.IsUsedInChild(i))
					{
						string Col = val;
						if (sql.alias.GetColumnAlias(node) != null)
							Col = "$var_"+ tablename + "/" + sql.alias.GetColumnAlias(node).name;

						tw.WriteLine("<xsl:variable name=\"" + sql.GetSQLUniqueName() +"_select_" + node +"\" select=\"" + Col + "\"/>");
					}
				}
				else
				{
					WriteXSL(select.GetSQL(i), sql.group.Count);
				}
			}
			/*if (select.selectType == SELECTTYPE.ONCE)
				tw.WriteLine("</xsl:if>");*/
		}
        /// <summary>
        /// ����� ������������ ���� � ����
        /// </summary>
		private void WriteHead()
		{
			tw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			if (!isGroup)
			{
				tw.WriteLine("<xsl:transform xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"2.0\">");
			}
			else
				tw.WriteLine("<xsl:transform xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"2.0\">");
			
			tw.WriteLine("<xsl:template match=\"" + DataSetName + "\"><" + DataSetName +"_IntekoAG>");
		}
        /// <summary>
        /// ����� �������� ����� � ����
        /// </summary>
		private void WriteEnd()
		{
			tw.WriteLine("</" + DataSetName +"_IntekoAG></xsl:template>");
			tw.WriteLine("</xsl:transform>");
			tw.Close();
		}
	};
}
